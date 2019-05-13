using Framework.Interfaces.Injection;

namespace MTM.Init
{
    internal class Bootstrap : IBootstrap
    {
        public IContainer RegisterContainer()
        {
            IContainer container = new CastleWindsorContainer();
            Container.RegisterContainer(container);
            return container;
        }
    }
}