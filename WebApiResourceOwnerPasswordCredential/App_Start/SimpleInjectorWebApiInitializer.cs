using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using Owin;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Integration.WebApi;
using System.Web.Http;

namespace WebApiResourceOwnerPasswordCredential
{
    public static class SimpleInjectorWebApiInitializer
    {
        /// <summary>Initialize the container and register it as Web API Dependency Resolver.</summary>
        public static void Initialize(IAppBuilder app)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();

            InitializeContainer(container);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);

            app.Use(async (context, next) =>
            {
                using (container.BeginExecutionContextScope())
                {
                    context.Environment.SetUserAccountService(() => container.GetInstance<UserAccountService>());
                    await next();
                }
            });
        }

        private static void InitializeContainer(Container container)
        {
            System.Data.Entity.Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<DefaultMembershipRebootDatabase, BrockAllen.MembershipReboot.Ef.Migrations.Configuration>());

            container.RegisterSingleton<MembershipRebootConfiguration>(MembershipRebootConfig.Create);
            container.Register<DefaultMembershipRebootDatabase>(() => new DefaultMembershipRebootDatabase(), Lifestyle.Scoped);
            var defaultAccountRepositoryRegistration =
                Lifestyle.Scoped.CreateRegistration<DefaultUserAccountRepository>(container);
            container.AddRegistration(typeof(IUserAccountQuery), defaultAccountRepositoryRegistration);
            container.AddRegistration(typeof(IUserAccountRepository), defaultAccountRepositoryRegistration);
            container.Register<UserAccountService>(() => new UserAccountService(container.GetInstance<MembershipRebootConfiguration>(), container.GetInstance<IUserAccountRepository>()), Lifestyle.Scoped);
        }
    }
}