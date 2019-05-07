using System.Threading.Tasks;

namespace Framework.Interfaces.Tasks
{
    public interface IServiceTaskRunner
    {
        string Name { get; }

        Task Task { get; }

        Task StartProcessing();
    }
}