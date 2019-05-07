namespace Framework.Logic.IOC
{
    using System.Security.Principal;
    using Interfaces.Injection;

    public static class IocExtension
    {
        public static IIdentity Identity => Container.Instance.GetInstance<IIdentity>();
    }
}