using Amazon;
using Amazon.S3;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application;

using Web.Middlewares;

// Cognito no usa el mapeo de claims por defecto de .NET: conservamos los nombres
// originales del token ("sub", "email", "cognito:username", "client_id").
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var builder = WebApplication.CreateBuilder(args);




if (builder.Environment.IsProduction())
//if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}


builder.Services.AddControllers();
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

builder.Services.AddScoped<IViajeRepository, ViajeRepository>();
builder.Services.AddScoped<IViajeService, ViajeService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IInvitacionRepository, InvitacionRepository>();
builder.Services.AddScoped<IInvitacionService, InvitacionService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IParticipanteViajeRepository, ParticipanteViajeRepository>();
builder.Services.AddScoped<IParticipanteViajeService, ParticipanteViajeService>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IGastoRepository, GastoRepository>();
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IDetalleGastoRepository, DetalleGastoRepository>();
builder.Services.AddScoped<IDetalleGastoService, DetalleGastoService>();

// Las credenciales salen de la cadena por defecto del SDK: en local, el perfil de AWS
// configurado en la máquina; en Elastic Beanstalk, el rol de la instancia. Así no hay
// claves de AWS en la configuración ni en el repositorio.
builder.Services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(
    RegionEndpoint.GetBySystemName(builder.Configuration["S3:Region"] ?? "us-east-1")));
builder.Services.AddScoped<IFileStorageService, S3FileStorageService>();



// journal mode
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' no encontrada.");

builder.Services.AddDbContext<ApplicationContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {

        var schemeName = "ApiBearerAuth";

        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Acá pegar el token generado al loguearse."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[schemeName] = securityScheme;

        var schemeReference = new OpenApiSecuritySchemeReference(schemeName, document);

        var requirement = new OpenApiSecurityRequirement
        {
            [schemeReference] = []
        };

        document.Security = new List<OpenApiSecurityRequirement> { requirement };

        return Task.CompletedTask;
    });
});



var cognitoRegion = builder.Configuration["Cognito:Region"]
    ?? throw new InvalidOperationException("Cognito:Region no está configurado.");
var cognitoUserPoolId = builder.Configuration["Cognito:UserPoolId"]
    ?? throw new InvalidOperationException("Cognito:UserPoolId no está configurado.");
var cognitoClientId = builder.Configuration["Cognito:ClientId"]
    ?? throw new InvalidOperationException("Cognito:ClientId no está configurado.");

var cognitoAuthority = $"https://cognito-idp.{cognitoRegion}.amazonaws.com/{cognitoUserPoolId}";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // Cognito publica su JWKS en {Authority}/.well-known/jwks.json y .NET lo resuelve
    // solo vía OIDC discovery: no hay que configurar ni rotar claves a mano.
    options.Authority = cognitoAuthority;
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidIssuer = cognitoAuthority,

        // Esta configuración espera el ID TOKEN, que sí trae el claim "aud" con el ClientId.
        // Si en algún momento el front pasa a mandar el ACCESS TOKEN, hay que poner
        // ValidateAudience = false y validar a mano el claim "client_id", porque el
        // access token de Cognito no tiene "aud". Tener en cuenta que el access token
        // tampoco trae "email" ni "name", que son los que usa el alta automática de usuarios.
        ValidateAudience = true,
        ValidAudience = cognitoClientId,

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        NameClaimType = "cognito:username",
        // El rol NO sale de "cognito:groups": es nuestro, vive en la tabla Usuarios y se
        // inyecta abajo en OnTokenValidated. Así el panel de administración puede cambiarlo
        // con efecto inmediato, sin depender de que el usuario renueve su token.
        RoleClaimType = ClaimTypes.Role,
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var principal = context.Principal!;

            // El id token y el access token se firman igual, así que la validación de firma
            // no alcanza para distinguirlos: sin este chequeo alguien podría mandar un access
            // token y pasar la validación sin traer los claims que damos por sentados.
            var tokenUse = principal.FindFirst("token_use")?.Value;
            if (tokenUse != "id")
            {
                context.Fail("Se esperaba un id token de Cognito.");
                return Task.CompletedTask;
            }

            if (string.IsNullOrEmpty(principal.FindFirst("sub")?.Value))
            {
                context.Fail("El token no contiene el claim 'sub'.");
                return Task.CompletedTask;
            }

            // Alta automática (JIT provisioning): la primera vez que llega un token válido
            // de alguien que todavía no está en nuestra base, se le crea el registro local.
            // Se hace acá y no sólo en GET /me porque los controllers necesitan el Id local
            // en CADA request (ver el claim NameIdentifier que agregamos más abajo).
            var usuarioService = context.HttpContext.RequestServices.GetRequiredService<IUsuarioService>();
            var usuario = usuarioService.GetOrCreateFromToken(principal);

            var identity = (ClaimsIdentity)principal.Identity!;

            // Los controllers resuelven el usuario con int.Parse(NameIdentifier) porque antes
            // el JWT propio emitía ahí el Id de la tabla Usuarios. El token de Cognito no trae
            // ese claim (su identificador es "sub", un GUID), así que lo reponemos con el Id
            // local para no tener que tocar los seis controllers que dependen de él.
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));

            // Idem con el rol, para que los [Authorize(Roles = "Admin")] sigan funcionando.
            identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Rol.ToString()));

            return Task.CompletedTask;
        }
    };
});

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options => options.AddPolicy("Front", policy => policy
    .WithOrigins(corsOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.Services.AddHttpClient("brevoClient", client =>
            {
                client.BaseAddress =
                    new Uri("https://api.brevo.com/v3/");
            });

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
    app.MapOpenApi();
    app.UseHttpsRedirection();
}
else
{

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");

    });
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseCors("Front");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
