#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Moonstone
{
    sealed class FileRollback
    {
        readonly List<string> createdFiles = new List<string>();
        readonly List<string> createdDirectories = new List<string>();
        readonly List<FileBackup> backups = new List<FileBackup>();
        string backupRoot;

        public int CreatedFileCount { get; private set; }
        public int OverwrittenFileCount { get; private set; }
        public int CreatedDirectoryCount { get; private set; }

        public void EnsureDirectory(string path)
        {
            if (string.IsNullOrEmpty(path) || Directory.Exists(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
            createdDirectories.Add(path);
            CreatedDirectoryCount++;
        }

        public void CopyFile(string sourcePath, string destinationPath)
        {
            TrackFileForWrite(destinationPath);
            File.Copy(sourcePath, destinationPath, true);
        }

        public void Rollback()
        {
            for (int i = createdFiles.Count - 1; i >= 0; i--)
            {
                if (File.Exists(createdFiles[i]))
                {
                    File.Delete(createdFiles[i]);
                }
            }

            for (int i = backups.Count - 1; i >= 0; i--)
            {
                File.Copy(backups[i].BackupPath, backups[i].OriginalPath, true);
            }

            for (int i = createdDirectories.Count - 1; i >= 0; i--)
            {
                string directory = createdDirectories[i];
                if (Directory.Exists(directory) && Directory.GetFileSystemEntries(directory).Length == 0)
                {
                    Directory.Delete(directory);
                }
            }

            CleanupBackups();
        }

        public void CleanupBackups()
        {
            try
            {
                if (!string.IsNullOrEmpty(backupRoot) && Directory.Exists(backupRoot))
                {
                    Directory.Delete(backupRoot, true);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not clean up Moonstone setup rollback backups: {exception.Message}");
            }
        }

        void TrackFileForWrite(string path)
        {
            if (File.Exists(path))
            {
                BackupFile(path);
                OverwrittenFileCount++;
                return;
            }

            if (!createdFiles.Contains(path))
            {
                createdFiles.Add(path);
                CreatedFileCount++;
            }
        }

        void BackupFile(string path)
        {
            if (string.IsNullOrEmpty(backupRoot))
            {
                backupRoot = Path.Combine(Path.GetTempPath(), "MoonstoneSetupRollback", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(backupRoot);
            }

            string backupPath = Path.Combine(backupRoot, backups.Count.ToString());
            File.Copy(path, backupPath, true);
            backups.Add(new FileBackup(path, backupPath));
        }
    }
}
#endif
