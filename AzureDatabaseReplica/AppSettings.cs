using System.Collections.Generic;

namespace AzureDatabaseReplica
{
    class AppSettings
    {
        public string SqlPackage { get; set; }
        public Source Source { get; set; }
        public Destination Destination { get; set; }
    }

    class ServerCredentials
    {
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    class Source : ServerCredentials
    {
        public string Database { get; set; }
        public string Bacpac { get; set; }
    }

    class Destination : ServerCredentials
    {
        public string Database { get; set; }
        public IReadOnlyCollection<string> Replicas { get; set; }
    }
}
