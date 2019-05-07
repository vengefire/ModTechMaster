namespace Framework.Interfaces.Tasks
{
    using System.Threading.Tasks;

    public interface IServiceTaskRunner
    {
        string Name { get; }

        Task Task { get; }

        Task StartProcessing();
    }
}