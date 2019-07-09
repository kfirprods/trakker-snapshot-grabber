using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TrakkerCore;

namespace TrakkerSnapshotGrabber
{
    public class ClusterManager
    {
        private List<FileSystemEntry> _currentClusterEntries;
        private readonly int _capacity;
        private readonly string _outputDirectory;
        private int _currentClusterIndex;

        public ClusterManager(int capacity, string outputDirectory)
        {
            this._currentClusterEntries = new List<FileSystemEntry>(capacity);
            this._capacity = capacity;
            this._outputDirectory = outputDirectory;
        }

        public void AddEntry(FileSystemEntry entry)
        {
            this._currentClusterEntries.Add(entry);

            if (this._currentClusterEntries.Count == this._capacity)
            {
                this.Flush();
            }
        }

        public int Flush()
        {
            if (this._currentClusterEntries.Count == 0) return this._currentClusterIndex;

            File.WriteAllText(this.GenerateClusterFilePath(this._currentClusterIndex), JsonConvert.SerializeObject(this._currentClusterEntries, Formatting.None));
            this._currentClusterEntries = new List<FileSystemEntry>(this._capacity);

            return this._currentClusterIndex++;
        }

        private string GenerateClusterFilePath(int clusterIndex)
        {
            return Path.Combine(this._outputDirectory, SnapshotMetadata.GenerateClusterName(clusterIndex));
        }
    }
}
