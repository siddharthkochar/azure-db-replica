using System.Diagnostics;
using System.IO;

namespace AzureDatabaseReplica
{
    public class BacpacOperations
    {
        public static void Export(string sqlPackage, string server, string database, string username, string password, string targetBacpacFilePath)
        {
            string arguments =
                $"/a:Export " +
                $"/ssn:{server} " +
                $"/sdn:{database} " +
                $"/su:{username} " +
                $"/sp:{password} " +
                $"/tf:{targetBacpacFilePath}";

            string directory = Path.GetDirectoryName(targetBacpacFilePath);
            string path = Path.GetFullPath(directory);
            Directory.CreateDirectory(path);
            Process.Start(sqlPackage, arguments).WaitForExit();
        }

        public static void Import(string sqlPackage, string sourceBacpacFile, string targetServer, string targetDatabase, string username, string password)
        {
            string arguments = $"/a:Import " +
                 $"/sf:{sourceBacpacFile} " +
                 $"/tsn:{targetServer} " +
                 $"/tdn:{targetDatabase} " +
                 $"/tu:{username} " +
                 $"/tp:{password}";

            Process.Start(sqlPackage, arguments).WaitForExit();
        }
    }
}
