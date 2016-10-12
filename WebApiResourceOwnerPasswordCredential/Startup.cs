using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApiResourceOwnerPasswordCredential.Startup))]

namespace WebApiResourceOwnerPasswordCredential
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            SimpleInjectorWebApiInitializer.Initialize(app);

            app.UseOAuthAuthorizationServer(new Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                Provider = new MyOAuthAuthorizationServerProvider(),
                TokenEndpointPath = new PathString("/token")
            });

            // token consumption
            var oauthConfig = new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                AuthenticationType = "Bearer"
            };
            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerAuthentication(oauthConfig);
        }
    }
}
