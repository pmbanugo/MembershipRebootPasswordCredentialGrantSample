using BrockAllen.MembershipReboot;

namespace WebApiResourceOwnerPasswordCredential
{
    public class MembershipRebootConfig
    {
        public static MembershipRebootConfiguration Create()
        {
            var config = new MembershipRebootConfiguration
            {
                MultiTenant = true,
                RequireAccountVerification = true,
                EmailIsUsername = true,
                AllowLoginAfterAccountCreation = true
            };
            config.AddEventHandler(new DebuggerEventHandler());

            return config;
        }
    }
}