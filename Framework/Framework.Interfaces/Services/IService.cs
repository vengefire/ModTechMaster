namespace Framework.Interfaces.Services
{
    public interface IService
    {
        void OnStart();

        void OnStop();

        string ServiceName();
    }
}