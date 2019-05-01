using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;

namespace AzureDatabaseReplica
{
    public interface IDatabase
    {
        void Export();
        void Import();
        void Rename();
        void Backup();
        void Restore(string databaseName);
    }

    class Database : IDatabase
    {
        private readonly AppSettings options;
        private readonly string temporaryDatabase;
        private readonly string localPath;

        public Database(IOptions<AppSettings> options)
        {
            this.options = options.Value;
            temporaryDatabase = $"{this.options.Destination.Database}-common";
            localPath = GetLocalPath(this.options.Source.Bacpac);
        }
        public void Export()
        {
            string arguments = 
                $"/a:Export " +
                $"/ssn:{options.Source.Server} " +
                $"/sdn:{options.Source.Database} " +
                $"/su:{options.Source.Username} " +
                $"/sp:{options.Source.Password} " +
                $"/tf:{options.Source.Bacpac}";

            Directory.CreateDirectory(localPath);
            Process.Start(options.SqlPackage, arguments).WaitForExit();
        }

        public void Import()
        {
            string arguments = $"/a:Import " +
                 $"/sf:{options.Source.Bacpac} " +
                 $"/tsn:{options.Destination.Server} " +
                 $"/tdn:{temporaryDatabase} " +
                 $"/tu:{options.Destination.Username} " +
                 $"/tp:{options.Destination.Password}";

            Process.Start(options.SqlPackage, arguments).WaitForExit();
        }

        public void Rename()
        {
            string drop = $"drop database if exists [{options.Destination.Database}]; ";
            string alter = $"alter database [{temporaryDatabase}] modify name=[{options.Destination.Database}];";
            string arguments = $"-S {options.Destination.Server} -Q \"{drop} {alter}\"";
            Process.Start("sqlcmd", arguments).WaitForExit();
        }

        public void Backup()
        {
            string path = $"{localPath}\\{temporaryDatabase}.bak";
            string backup = $"backup database [{options.Destination.Database}] to disk = '{path}' with copy_only";
            string arguments = $"-S {options.Destination.Server} -Q \"{backup}\"";
            Process.Start("sqlcmd", arguments).WaitForExit();
        }

        public void Restore(string databaseName)
        {
            string bakPath = $"{localPath}\\{temporaryDatabase}.bak";
            string mdfPath = $"{localPath}\\{databaseName}.mdf";
            string ldfPath = $"{localPath}\\{databaseName}.ldf";
            string log = $"{temporaryDatabase}_log";

            string restore = $"restore database [{databaseName}] " +
                $"from disk = '{bakPath}' with move '{temporaryDatabase}' " +
                $"to '{mdfPath}', move '{log}' to '{ldfPath}', replace, recovery";

            string arguments = $"-S {options.Destination.Server} -Q \"{restore}\"";
            Process.Start("sqlcmd", arguments).WaitForExit();
        }

        private string GetLocalPath(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            return Path.GetFullPath(directory);
        }
    }
}
