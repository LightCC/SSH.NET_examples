using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace ssh_ex_console.cs
{
    class Program
    {
        static void Main(string[] args)
        {
            var localConnectionInfo = GetLocalConnectionInfo();
            TestBasicSshCommands(localConnectionInfo);
        }

        private static void TestBasicSshCommands(ConnectionInfo localConnectionInfo)
        {
            using (var sshClient = new SshClient(localConnectionInfo))
            {
                sshClient.Connect();

                ExecuteCmdAndPrintExtendedStream(sshClient, "echo \"This is StdOut\"; echo \"This is StdErr\" >&2");
                Console.WriteLine("---");
                ExecuteCmdAndPrintExtendedStream(sshClient, "echo $USER");
                Console.WriteLine("---");
                ExecuteCmdAndPrintExtendedStream(sshClient, "pwd");
                Console.WriteLine("---");
                ExecuteCmdAndPrintExtendedStream(sshClient, "df -h");

                // Read an input <Enter> so the window doesn't go away before we can see/read it
                Console.ReadLine();

                sshClient.Disconnect();

                //Used with unit test framework
                //Assert.Inconclusive();
            }
        }

        private static ConnectionInfo GetLocalConnectionInfo()
        {
            // hardcoded test vars
            string ip = "192.168.42.153";
            int port = 22;
            Console.Write("Username: ");
            string username = Console.ReadLine().Trim();
            Console.Write("Password: ");
            string password = Console.ReadLine().Trim();
            // var privateKeyFile = new PrivateKeyFile("c:\\privatekeyfilename", "passPhrase");

            // Setup all the possible authentication methods
            AuthenticationMethod authNone = new NoneAuthenticationMethod(username);
            AuthenticationMethod authPassword = new PasswordAuthenticationMethod(username, password);
            AuthenticationMethod authKeyboard = new KeyboardInteractiveAuthenticationMethod(username);
            //AuthenticationMethod authPrivateKey = new PrivateKeyAuthenticationMethod(username, privateKeyFile);

            // Pick which authentication method to use for this test
            AuthenticationMethod auth = authPassword;

            ConnectionInfo localConnectionInfo = new ConnectionInfo(ip, port, username, auth);
            return localConnectionInfo;
        }

        private static void ExecuteCmdAndPrintExtendedStream(SshClient sshClient, string commandToExecute)
        {
            using (var sshCmd = sshClient.CreateCommand(commandToExecute))
            {
                Console.WriteLine("COMMAND: \"{0}\"", commandToExecute);

                var result = sshCmd.Execute();
                if (result == String.Empty)
                {
                    // If empty, print nothing
                    //Console.WriteLine("StdOut: [n/a]");
                }
                else
                {
                    Console.WriteLine("StdOut:");
                    // Output already has a linefeed, just use write
                    Console.Write(result);
                }

                var reader = new StreamReader(sshCmd.ExtendedOutputStream);
                string stdErrorOutput = reader.ReadToEnd();
                if (stdErrorOutput == String.Empty)
                {
                    // If result is empty, print nothing
                    //Console.WriteLine("StdErr: [n/a]");
                }
                else
                {

                    Console.WriteLine("StdErr:");
                    // Output already has a linefeed, just use write
                    Console.Write(stdErrorOutput);
                }

            }
        }
    }
}
