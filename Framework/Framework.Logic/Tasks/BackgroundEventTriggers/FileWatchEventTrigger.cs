namespace Framework.Logic.Tasks.BackgroundEventTriggers
{
    using System;
    using System.IO;
    using System.Threading;
    using Interfaces.Tasks;
    using Utils.Directory;
    using Utils.Extensions.File;

    public class FileWatchEventTrigger : ITaskEventTrigger<FileWatchEventTrigger.EventArgs>
    {
        [Flags]
        public enum FileEventTrigger
        {
            Created = 1,

            Changed = 2,

            Deleted = 4
        }

        private readonly FileSystemWatcher fileSystemWatcher;

        public FileWatchEventTrigger(
            string watchPath,
            string fileFilter)
        {
            DirectoryUtils.EnsureExists(watchPath);

            this.fileSystemWatcher = new FileSystemWatcher {Path = watchPath, Filter = fileFilter, NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite};

            this.fileSystemWatcher.Created += this.OnFileCreated;
            this.fileSystemWatcher.Deleted += this.OnFileDeleted;
            this.fileSystemWatcher.EnableRaisingEvents = false;
        }

        public event Action<EventArgs> TriggerEventHandler;

        public void StartMonitoring()
        {
            this.ProcessExistingFiles();
            this.fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void StopMonitoring()
        {
            this.fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void ProcessExistingFiles()
        {
            foreach (var file in Directory.GetFiles(this.fileSystemWatcher.Path))
            {
                var fi = new FileInfo(file);
                this.TriggerEventHandler(new EventArgs(fi.Name, fi.FullName, FileEventTrigger.Created));
            }
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            this.TriggerEventHandler(new EventArgs(Path.GetFileName(e.FullPath), e.FullPath, FileEventTrigger.Deleted));
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            this.EnsureFileUnlocked(e);
            this.TriggerEventHandler(new EventArgs(Path.GetFileName(e.FullPath), e.FullPath, FileEventTrigger.Changed));
        }

        private void EnsureFileUnlocked(FileSystemEventArgs e)
        {
            var fi = new FileInfo(e.FullPath);
            while (fi.IsFileLocked()) Thread.Sleep(1);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            this.EnsureFileUnlocked(e);
            this.TriggerEventHandler(new EventArgs(Path.GetFileName(e.FullPath), e.FullPath, FileEventTrigger.Created));
        }

        public class EventArgs
        {
            public EventArgs(string fileName, string fullPath, FileEventTrigger fileEventTrigger)
            {
                this.FileName = fileName;
                this.FullPath = fullPath;
                this.FileEventTrigger = fileEventTrigger;
            }

            public string FileName { get; }

            public string FullPath { get; }

            public FileEventTrigger FileEventTrigger { get; }
        }
    }
}