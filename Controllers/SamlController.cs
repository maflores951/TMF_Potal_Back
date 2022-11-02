using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.MvcCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using tmf_group.Helper;
using LoginBase.Services;
using Microsoft.Extensions.Configuration;
using LoginBase.Models.Common;
using tmf_group.Models.Common;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Text.Json;

namespace tmf_group.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamlController : ControllerBase
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private readonly Saml2Configuration config;
        private IUserService _userService;
        public IConfiguration _configuration;
        private readonly IWebHostEnvironment _enviroment;


        public SamlController(IOptions<Saml2Configuration> configAccessor, IUserService userService, IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _userService = userService;
            config = configAccessor.Value;
            _enviroment = env;
        }

        [Route("Login")]
        public IActionResult Login(string returnUrl = null)
        {
            var path = Path.Combine(_enviroment.ContentRootPath, "Log");
            Log oLog = new(path);
            oLog.Add("LoginSAML_Inicia_True");
            try
            {
                var binding = new Saml2RedirectBinding();
                binding.SetRelayStateQuery(new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? Url.Content("~/") } });

                var respuesta =  binding.Bind(new Saml2AuthnRequest(config)).ToActionResult();

                return Ok(respuesta);
            }
            catch (System.Exception es)
            {
                var respuestaMensaje = es.Message;
                var respuesta = 0;
                return Ok(respuestaMensaje);
            }
            
        }

        //Esta api se debe de poner en el redireccinamiento del SAML de amazon para validar que el logeo sea correcto
        [Route("AssertionConsumerService")]
        public async Task<IActionResult> AssertionConsumerService()
        {
            var path = Path.Combine(_enviroment.ContentRootPath, "Log");
            Log oLog = new(path);
            var binding = new Saml2PostBinding();
            var saml2AuthnResponse = new Saml2AuthnResponse(config);
            var appSettingsSection = _configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();

            var fecha = DateTimeOffset.UtcNow;
            saml2AuthnResponse.IssueInstant = saml2AuthnResponse.IssueInstant.LocalDateTime;
            oLog.Add(" _Inicia_True");

            string emailClaim = "";

            try
            {
                binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);

                if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
                {
                    oLog.Add("_Saml2StatusCodes.Success");
                    throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");
                   
                }
                binding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnResponse);
            }
            catch (System.Exception es)
            {
                oLog.Add(es.Message + " _Try_binding.ReadSamlResponse" + " " + DateTimeOffset.UtcNow + " _ " + saml2AuthnResponse.IssueInstant);
            }


            try
            {
                var emailSAML = await saml2AuthnResponse.CreateSession(HttpContext, claimsTransform: (claimsPrincipal) => ClaimsTransform.Transform(claimsPrincipal));

                foreach (var item in emailSAML.Claims)
                {
                    oLog.Add(item.Value + " emailSAML_JsonSerializer1");
                    oLog.Add(item.Type + " emailSAML_JsonSerializer2");
                    if (item.Type == "EMAIL") emailClaim = item.Value; //Amazon
                    //if (item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") emailClaim = item.Value; // OKTA
                }

                if (emailClaim == "")
                    return Redirect(appSettings.UrlFront + "errorLogin");

                var usuarioLogin = _userService.AuthSaml(emailClaim);

                oLog.Add(emailSAML.Identity.Name + " _emailSAML_Name");

                if (usuarioLogin == null)
                    return Redirect(appSettings.UrlFront + "errorLogin");

                oLog.Add("_usuarioLogin_True");
                var usuarioToken = CifrarUsuario.GeneraUsuario(usuarioLogin);

                if (usuarioToken == null)
                    return Redirect(appSettings.UrlFront + "errorLogin");

                oLog.Add("_usuarioToken_True");

                var relayStateQuery = binding.GetRelayStateQuery();
                var returnUrl = relayStateQuery.ContainsKey(relayStateReturnUrl) ? relayStateQuery[relayStateReturnUrl] : Url.Content("~/");

               
                var urlLogin = appSettings.UrlFront + "user/InicioSaml/" + usuarioToken.Trim().Replace("/", "$").Replace("+", "&");

                oLog.Add(urlLogin + " _returnUrl_true");
                return Redirect(appSettings.UrlFront + "user/InicioSaml/" + usuarioToken.Trim().Replace("/", "$").Replace("+", "&"));
            }
            catch (System.Exception es)
            {
                oLog.Add(es.Message + " _saml2AuthnResponse");
                return Redirect(appSettings.UrlFront + "errorLogin");
            }
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(Url.Content("~/"));
            }

            var binding = new Saml2PostBinding();
            var saml2LogoutRequest = await new Saml2LogoutRequest(config, User).DeleteSession(HttpContext);
            return Redirect("~/");
        }
    }
}
