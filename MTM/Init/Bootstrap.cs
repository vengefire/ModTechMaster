namespace MTM.Init
{
    using Framework.Interfaces.Injection;

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