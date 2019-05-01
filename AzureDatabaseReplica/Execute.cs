using Microsoft.Extensions.Options;

namespace AzureDatabaseReplica
{
    class Execute
    {
        public Execute(IDatabase database, IOptions<AppSettings> options)
        {
            database.Export();
            database.Import();
            database.Rename();
            database.Backup();

            var replicas = options.Value.Destination.Replicas;
            foreach (var replica in replicas)
            {
                database.Restore(replica);
            }
        }
    }
}
