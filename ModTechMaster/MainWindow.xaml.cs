namespace ModTechMaster
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using Core.Interfaces.Services;
    using Data.Models.Mods;
    using Logic.Factories;
    using Logic.Services;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.ModService = new ModService(new MessageService(), new ManifestEntryProcessorFactory());
            this.ReferenceFinderService = new ReferenceFinderService();

            this.InitData();
            this.InitTreeView();
        }

        private ReferenceFinderService ReferenceFinderService { get; }

        private string TargetModCollectionPath { get; set; }

        private ModService ModService { get; }
        private ModCollection ModCollection { get; set; }

        private void InitData()
        {
            this.TargetModCollectionPath = @"C:\dev\repos\ModTechMaster\TestData\In\Mods";
            IModService modService = new ModService(new MessageService(), new ManifestEntryProcessorFactory());
            this.ModCollection = new ModCollection("MTM Mod Collection");

            var di = new DirectoryInfo(this.TargetModCollectionPath);
            if (!di.Exists)
            {
                Console.WriteLine($"The target directory [{di.FullName}] foes not exist.");
                return;
            }

            Console.WriteLine($"Processing mods from [{di.FullName}]");
            var stopwatch = new Stopwatch();
            di.GetDirectories().ToList().ForEach(
                                                 sub =>
                                                 {
                                                     stopwatch.Start();
                                                     Console.Write($"Processing [{sub.Name}]...");
                                                     this.ModCollection.AddModToCollection(modService.TryLoadFromPath(sub.FullName));
                                                     stopwatch.Stop();
                                                     Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
                                                     stopwatch.Reset();
                                                 });
        }

        private void InitTreeView()
        {
            this.tvObjects.ItemsSource = this.ModCollection.Mods;
        }
    }
}