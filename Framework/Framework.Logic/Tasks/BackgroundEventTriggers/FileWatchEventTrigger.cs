using System;
using System.IO;
using System.Threading;
using Framework.Interfaces.Tasks;
using Framework.Utils.Directory;
using Framework.Utils.Extensions.File;

namespace Framework.Logic.Tasks.BackgroundEventTriggers
{
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

            fileSystemWatcher = new FileSystemWatcher
            {
                Path = watchPath,
                Filter = fileFilter,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            fileSystemWatcher.Created += OnFileCreated;
            fileSystemWatcher.Deleted += OnFileDeleted;
            fileSystemWatcher.EnableRaisingEvents = false;
        }

        public event Action<EventArgs> TriggerEventHandler;

        public void StartMonitoring()
        {
            ProcessExistingFiles();
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void StopMonitoring()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void ProcessExistingFiles()
        {
            foreach (var file in Directory.GetFiles(fileSystemWatcher.Path))
            {
                var fi = new FileInfo(file);
                TriggerEventHandler(new EventArgs(fi.Name, fi.FullName, FileEventTrigger.Created));
            }
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            TriggerEventHandler(new EventArgs(Path.GetFileName(e.FullPath), e.FullPath, FileEventTrigger.Deleted));
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            EnsureFileUnlocked(e);
            TriggerEventHandler(new EventArgs(Path.GetFileName(e.FullPath), e.FullPath, FileEventTrigger.Changed));
        }

        private void EnsureFileUnlocked(FileSystemEventArgs e)
        {
            var fi = new FileInfo(e.FullPath);
            while (fi.IsFileLocked())
            {
                Thread.Sleep(1);
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            EnsureFileUnlocked(e);
            TriggerEventHandler(new EventArgs(Path.GetFileName(e.FullPath), e.FullPath, FileEventTrigger.Created));
        }

        public class EventArgs
        {
            public EventArgs(string fileName, string fullPath, FileEventTrigger fileEventTrigger)
            {
                FileName = fileName;
                FullPath = fullPath;
                FileEventTrigger = fileEventTrigger;
            }

            public string FileName { get; private set; }

            public string FullPath { get; private set; }

            public FileEventTrigger FileEventTrigger { get; private set; }
        }
    }
}