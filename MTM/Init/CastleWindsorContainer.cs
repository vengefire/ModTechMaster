using Castle.Facilities.Logging;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Services.Logging.Log4netIntegration;
using Castle.Windsor;
using Framework.Logic.IOC;

namespace MTM.Init
{
    public class CastleWindsorContainer : BaseCastleWindsorContainer
    {
        public CastleWindsorContainer() : base(Init)
        {
        }

        private static void Init(WindsorContainer windsorContainer)
        {
            windsorContainer.AddFacility<LoggingFacility>(f =>
                f.LogUsing<Log4netFactory>().WithAppConfig());
            windsorContainer.AddFacility<TypedFactoryFacility>();
            windsorContainer.Kernel.Resolver.AddSubResolver(new CollectionResolver(windsorContainer.Kernel));

            windsorContainer.Install(new Installer());
        }
    }
}
