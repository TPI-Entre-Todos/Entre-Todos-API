using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application;

var builder = WebApplication.CreateBuilder(args);


 
if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}


builder.Services.AddControllers();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
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
builder.Services.AddScoped<GastoService>();
builder.Services.AddScoped<IGastoService, GastoService>();


var connection = new SqliteConnection("DataSource = EntreTodos.db");
connection.Open();

// journal mode
using (var command = connection.CreateCommand())
{
    command.CommandText = "PRAGMA journal_mode = DELETE;";
    command.ExecuteNonQuery();
}
builder.Services.AddDbContext<ApplicationContext>(dbContextOptions => dbContextOptions.UseSqlite(connection));

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



builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
    };
});

builder.Services.Configure<AutenticacionServiceOptions>(builder.Configuration.GetSection("Authentication"));

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
