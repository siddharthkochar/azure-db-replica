using System.Diagnostics;

namespace AzureDatabaseReplica
{
    public static class SqlOperations
    {
        public static void RenameDatabase(string server, string oldName, string newName)
        {
            string alter = $"alter database [{oldName}] modify name=[{newName}];";
            Execute(server, alter);
        }

        public static void DropDatabase(string server, string database)
        {
            string drop = $"drop database if exists [{database}]; ";
            Execute(server, drop);
        }

        public static void Backup(string bak, string server, string database)
        {
            string backup = $"backup database [{database}] to disk = '{bak}' with copy_only";
            Execute(server, backup);
        }

        public static void Restore(string server, string bak, string mdf, string ldf, string database, string newDatabaseName)
        {
            string log = $"{newDatabaseName}_log";
            string restore = $"restore database [{database}] " +
                $"from disk = '{bak}' with move '{newDatabaseName}' " +
                $"to '{mdf}', move '{log}' to '{ldf}', replace, recovery";

            Execute(server, restore);
        }

        private static void Execute(string server, string query)
        {
            string arguments = $"-S {server} -Q \"{query}\"";
            Process.Start("sqlcmd", arguments).WaitForExit();
        }
    }
}
