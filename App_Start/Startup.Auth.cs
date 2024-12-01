using System.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Web.Helpers;
using Microsoft.Owin.Security;
using Microsoft.Owin.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using System;
using System.Threading.Tasks;

namespace MVCFW48Azure
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetLoggerFactory(new DiagnosticsLoggerFactory()); // Enable detailed logging for OWIN middleware
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            //app.UseKentorOwinCookieSaver();
            //app.UseCookieAuthentication(new CookieAuthenticationOptions());

            //AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            // Cookie Authentication defined first, but it might not actually be default anyway..
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType, // "Cookies"
                LoginPath = new PathString("/Account/SignIn"),
                LogoutPath = new PathString("/Account/SignOut")
            });

            string tenant = ConfigurationManager.AppSettings["Tenant"];
            string redirectUri = ConfigurationManager.AppSettings["RedirectUri"];

            // OpenID Connect Authentication should be second
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["ClientId"],
                Authority = $"https://login.microsoftonline.com/{tenant}/v2.0", // replaced "common" with Tenant.
                RedirectUri = redirectUri,
                ClientSecret = ConfigurationManager.AppSettings["ClientSecret"],
                ResponseType = OpenIdConnectResponseType.CodeIdToken, // "code id_token"

                Notifications = new OpenIdConnectAuthenticationNotifications
                {        
                    // This will fire when the authentication request is being made to Azure AD
                    RedirectToIdentityProvider = context =>
                    {
                        // Reconstruct the full request URL
                        var authorizationUrl = context.ProtocolMessage.CreateAuthenticationRequestUrl();

                        Trace.WriteLine($"Authorization Request URL: {authorizationUrl}");

                        // Manually set the state parameter, if necessary
                        //var state = Guid.NewGuid().ToString();
                        //context.ProtocolMessage.State = state;

                        Trace.WriteLine($"Current-state: {context.ProtocolMessage.State}"); // Log the state parameter
                        return Task.CompletedTask;
                    },
                    AuthorizationCodeReceived = context =>
                    {
                        var state = context.ProtocolMessage.State;
                        Debug.WriteLine($"State Parameter: {state}"); // Log the state parameter
                        return Task.FromResult(0);
                    },
                    AuthenticationFailed = context =>
                    {
                        Debug.WriteLine($"Authentication Failed: {context.Exception.Message}");
                        return Task.FromResult(0);
                    },
                    MessageReceived = context =>
                    {
                        var query = context.Request.QueryString.Value;
                        Trace.WriteLine($"Query-string: {query}");
                        return Task.CompletedTask;
                    }
                },

                Scope = "openid profile email",
                PostLogoutRedirectUri = ConfigurationManager.AppSettings["PostLogoutRedirectUri"],
                TokenValidationParameters = new TokenValidationParameters // Add token validation parameters if needed
                {
                    ValidateIssuer = true
                },
                ProtocolValidator = new OpenIdConnectProtocolValidator
                {
                    RequireNonce = true,
                    RequireState = false, // If RequireState is set to true, an error "IDX21329" occurs (https://github.com/aspnet/AspNetKatana/issues/516)
                    RequireStateValidation = false
                },
                SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType
            });

        }
    }
}
