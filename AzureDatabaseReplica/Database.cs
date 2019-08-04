using Microsoft.Extensions.Options;
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
            BacpacOperations.Export(
                options.SqlPackage,
                options.Source.Server,
                options.Source.Database,
                options.Source.Username,
                options.Source.Password,
                options.Source.Bacpac);
        }

        public void Import()
        {
            BacpacOperations.Import(
                options.SqlPackage,
                options.Source.Bacpac,
                options.Destination.Server,
                temporaryDatabase,
                options.Destination.Username,
                options.Destination.Password);
        }

        public void Rename()
        {
            SqlOperations.DropDatabase(options.Destination.Server, options.Destination.Database);
            SqlOperations.RenameDatabase(options.Destination.Server, temporaryDatabase, options.Destination.Database);
        }

        public void Backup()
        {
            string path = $"{localPath}\\{temporaryDatabase}.bak";
            SqlOperations.Backup(path, options.Destination.Server, options.Destination.Database);
        }

        public void Restore(string databaseName)
        {
            string bakPath = $"{localPath}\\{temporaryDatabase}.bak";
            string mdfPath = $"{localPath}\\{databaseName}.mdf";
            string ldfPath = $"{localPath}\\{databaseName}.ldf";
            SqlOperations.Restore(options.Destination.Server, bakPath, mdfPath, ldfPath, databaseName, temporaryDatabase);
        }

        private string GetLocalPath(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            return Path.GetFullPath(directory);
        }
    }
}
