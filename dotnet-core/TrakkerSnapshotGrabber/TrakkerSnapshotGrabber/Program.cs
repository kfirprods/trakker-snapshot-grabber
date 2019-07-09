using System;
using System.IO;

namespace TrakkerSnapshotGrabber
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rootPath = args[0];
            var snapshotOutputPath = args[1];

            if (File.Exists(snapshotOutputPath))
            {
                File.Delete(snapshotOutputPath);
            }

            Console.WriteLine("On it...");
            SnapshotGenerator.GenerateSnapshot(rootPath, snapshotOutputPath);
            Console.WriteLine("Done");
        }
    }
}
