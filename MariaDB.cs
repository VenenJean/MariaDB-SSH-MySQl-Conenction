using MySqlConnector;
using Renci.SshNet;
using System.Collections.Specialized;
using System.Configuration;

namespace MariaDB {
    internal class MariaDB {
        private static readonly NameValueCollection AS = ConfigurationManager.AppSettings;

        // Connect to SSH Server using ED25519 Private Key with Passphrase
        private static readonly PrivateKeyFile privateKeyFile = new(AS["sshKeyPath"], AS["sshKeyPassPhrase"]);
        private static readonly PrivateKeyConnectionInfo connectionInfo = new(AS["sshHost"], AS["sshUser"], privateKeyFile);

        private static (SshClient, uint) ConnectSSH() {
            using SshClient sshClient = new(connectionInfo);
            sshClient.Connect();

            // Port forwarding to MariaDB server
            ForwardedPortLocal portForwarding = new("127.0.0.1", AS["dbHost"], Convert.ToUInt32(AS["dbPort"]));
            sshClient.AddForwardedPort(portForwarding);
            portForwarding.Start();

            return (sshClient, portForwarding.Port);
        }

        public static MySqlConnection ConnectDB() {
            var (sshClient, localPort) = ConnectSSH();
            using(sshClient) {
                MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder {
                    Server = "127.0.0.1",
                    Port = localPort,
                    UserID = AS["dbUser"],
                    Password = AS["dbPass"],
                    Database = AS["dbName"],
                };

                MySqlConnection connection = new(csb.ConnectionString);
                connection.Open();

                return connection;
            }
        }
    }
}