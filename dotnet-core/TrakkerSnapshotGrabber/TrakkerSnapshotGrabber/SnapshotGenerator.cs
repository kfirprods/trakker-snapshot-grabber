using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using TrakkerCore;

namespace TrakkerSnapshotGrabber
{
    public static class SnapshotGenerator
    {
        private const int MaxEntriesPerCluster = 1000;

        public static void GenerateSnapshot(string rootPath, string outputPath)
        {
            var temporaryClusterDirectoryPath = GetTempFolder();

            var clusterManager = new ClusterManager(MaxEntriesPerCluster, temporaryClusterDirectoryPath);
            var totalRootSize = 0L;

            foreach (var entry in FileSystemCrawler.EnumerateEntries(rootPath))
            {
                clusterManager.AddEntry(entry);

                // TODO: Ignore directory entries
                totalRootSize += entry.Size;
            }

            var clusterCount = clusterManager.Flush();

            var metadata = new SnapshotMetadata()
                { ClustersCount = clusterCount, DriveSize = totalRootSize, RootPath = rootPath, EntriesPerCluster = MaxEntriesPerCluster };

            File.WriteAllText(Path.Combine(temporaryClusterDirectoryPath, "metadata.json"),
                JsonConvert.SerializeObject(metadata));

            ZipFile.CreateFromDirectory(temporaryClusterDirectoryPath, outputPath);
            Directory.Delete(temporaryClusterDirectoryPath, true);
        }

        private static string GetTempFolder()
        {
            var trakkerTempDirectory = Path.Combine(Path.GetTempPath(), "trakker");
            if (!Directory.Exists(trakkerTempDirectory))
            {
                Directory.CreateDirectory(trakkerTempDirectory);
            }

            var temporaryClusterDirectoryPath = Path.Combine(trakkerTempDirectory,
                Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            Directory.CreateDirectory(temporaryClusterDirectoryPath);

            return temporaryClusterDirectoryPath;
        }
    }
}
