using System.Collections.Generic;
using System.IO;
using TrakkerCore;

namespace TrakkerSnapshotGrabber
{
    public static class FileSystemCrawler
    {
        public static IEnumerable<FileSystemEntry> EnumerateEntries(string rootPath)
        {
            var identity = 0;
            var pendingNodes = new Stack<(string path, int id)>();
            pendingNodes.Push((rootPath, identity++));

            while (pendingNodes.Count != 0)
            {
                var poppedNode = pendingNodes.Pop();
                var currentNode = new FileSystemEntry(poppedNode.id, poppedNode.path, 0);

                var files = new string[0];
                var subDirectories = new string[0];

                try
                {
                    files = Directory.GetFiles(currentNode.FullPath);
                }
                // Ignore dir-listing errors
                catch
                {
                }

                try
                {
                    subDirectories = Directory.GetDirectories(currentNode.FullPath);
                }
                // Ignore dir-listing errors
                catch
                {
                }

                var firstId = identity;
                var lastId = firstId + files.Length + subDirectories.Length;
                currentNode.ChildrenFirstId = firstId;
                currentNode.ChildrenLastId = lastId;

                // TODO: Get file sizes here and set the directory size to be their sum

                yield return currentNode;


                foreach (var filePath in files)
                {
                    var fileSize = GetFileSize(filePath);
                    yield return new FileSystemEntry(identity, filePath, fileSize);
                    identity++;
                }

                foreach (var subDirectory in subDirectories)
                {
                    pendingNodes.Push((subDirectory, identity));
                    identity++;
                }
            }
        }

        private static long GetFileSize(string fullPath)
        {
            return new FileInfo(fullPath).Length;
        }
    }
}
