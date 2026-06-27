using Application.Interfaces;
using Application.Models.Requests;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{

    private readonly IAuthenticationService _customAuthenticationService;

    public AuthenticationController(IAuthenticationService autenticacionService)
    {
        _customAuthenticationService = autenticacionService;
    }


    [HttpPost("authenticate")] //Vamos a usar un POST ya que debemos enviar los datos para hacer el login
    public ActionResult<string> Autenticar([FromBody] AuthenticationRequest authenticationRequest) //Enviamos como parámetro la clase que creamos arriba
    {
        var token = _customAuthenticationService.Autenticar(authenticationRequest);
        if (token == null)
        {
            throw new UnauthorizedException("Credenciales inválidas");
        }
        return Ok(token);
    }

}