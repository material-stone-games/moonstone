#if UNITY_EDITOR
namespace Moonstone
{
    readonly struct FileBackup
    {
        public readonly string OriginalPath;
        public readonly string BackupPath;

        public FileBackup(string originalPath, string backupPath)
        {
            OriginalPath = originalPath;
            BackupPath = backupPath;
        }
    }
}
#endif
