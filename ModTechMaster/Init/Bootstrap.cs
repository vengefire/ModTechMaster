using Framework.Interfaces.Injection;

namespace ModTechMaster.UI.Init
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