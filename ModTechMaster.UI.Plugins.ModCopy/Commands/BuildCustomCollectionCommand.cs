namespace ModTechMaster.UI.Plugins.ModCopy.Commands
{
    using System;
    using System.Threading.Tasks;

    using ModTechMaster.UI.Core.Async;
    using ModTechMaster.UI.Plugins.Core.Interfaces;
    using ModTechMaster.UI.Plugins.ModCopy.Model;

    public class BuildCustomCollectionCommand : AwaitableDelegateCommand<ModCopyModel>, IPluginCommand
    {
        private readonly ModCopyModel model;

        public BuildCustomCollectionCommand(ModCopyModel model)
            : base(Execute, CanExecute)
        {
            this.model = model;
        }

        public IPluginCommandCategory Category { get; }

        public object CommandParameter => this.model;

        public string Name => "Build Custom Collection";

        private static bool CanExecute(ModCopyModel model)
        {
            return model?.ModCollectionNode?.SelectedModObjectCount > 0;
        }

        private static async Task Execute(ModCopyModel model)
        {
            await Task.Run((Action)model.BuildCustomCollection).ConfigureAwait(false);
        }
    }
}