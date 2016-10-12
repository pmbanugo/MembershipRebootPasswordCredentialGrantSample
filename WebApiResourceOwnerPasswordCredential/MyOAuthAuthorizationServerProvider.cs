using BrockAllen.MembershipReboot;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Threading.Tasks;

namespace WebApiResourceOwnerPasswordCredential
{
    public class MyOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override System.Threading.Tasks.Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string cid, csecret;
            if (context.TryGetBasicCredentials(out cid, out csecret))
            {
                var svc = context.OwinContext.Environment.GetUserAccountService<UserAccount>();
                if (svc.Authenticate("clients", cid, csecret))
                {
                    context.Validated();
                }
            }
            return Task.FromResult<object>(null);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var svc = context.OwinContext.Environment.GetUserAccountService<UserAccount>();
            UserAccount user;
            if (svc.Authenticate("users", context.UserName, context.Password, out user))
            {
                var claims = user.GetAllClaims();

                var id = new System.Security.Claims.ClaimsIdentity(claims, "MembershipReboot");
                context.Validated(id);
            }

            return base.GrantResourceOwnerCredentials(context);
        }
    }
}