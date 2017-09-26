using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace ssh_ex_console.cs
{
    public class SshExamples
    {
        private ConnectionInfo _localConnectionInfo = null;

        public void TestBasicSshCommandsWithStream()
        {
            GetLocalConnectionInfo();

            using (var sshClient = new SshClient(_localConnectionInfo))
            {
                string cmd = String.Empty;
                string cmdOut = String.Empty;

                sshClient.Connect();

                cmd = "echo \"This is StdOut\"; echo \"This is StdErr\" >&2";
                cmdOut = ExecuteSshCmd_ReturnCmdOutErr(sshClient, cmd);
                Console.Write(cmdOut);

                Console.WriteLine("---");
                cmd = "echo $USER";
                cmdOut = ExecuteSshCmd_ReturnCmdOutErr(sshClient, cmd);
                Console.Write(cmdOut);

                Console.WriteLine("---");
                cmd = "pwd";
                cmdOut = ExecuteSshCmd_ReturnCmdOutErr(sshClient, cmd);
                Console.Write(cmdOut);

                Console.WriteLine("---");
                cmd = "df -h";
                cmdOut = ExecuteSshCmd_ReturnCmdOutErr(sshClient, cmd);
                Console.Write(cmdOut);

                // Read an input <Enter> so the window doesn't go away before we can see/read it
                Console.ReadLine();

                sshClient.Disconnect();

                //Used with unit test framework
                //Assert.Inconclusive();
            }
        }

        private void GetLocalConnectionInfo()
        {
            if (_localConnectionInfo == null)
            {
                // hardcoded test vars
                string hostIp = "192.168.42.153";
                int hostPort = 22;
                Console.WriteLine("Host: {0}:{1}{2}", hostIp, hostPort, Environment.NewLine);

                Console.Write("User: ");
                string username = Console.ReadLine().Trim();
                Console.Write("Pass: ");
                string password = Console.ReadLine().Trim();
                // var privateKeyFile = new PrivateKeyFile("c:\\privatekeyfilename", "passPhrase");

                // Setup all the possible authentication methods
                AuthenticationMethod authNone = new NoneAuthenticationMethod(username);
                AuthenticationMethod authPassword = new PasswordAuthenticationMethod(username, password);
                AuthenticationMethod authKeyboard = new KeyboardInteractiveAuthenticationMethod(username);
                //AuthenticationMethod authPrivateKey = new PrivateKeyAuthenticationMethod(username, privateKeyFile);

                // Pick which authentication method to use for this test
                AuthenticationMethod auth = authPassword;

                _localConnectionInfo = new ConnectionInfo(hostIp, hostPort, username, auth);
            }
        }

        public static string ExecuteSshCmd_ReturnCmdOutErr(SshClient sshClient, string commandToExecute)
        {
            using (var sshCmd = sshClient.CreateCommand(commandToExecute))
            {
                string cmdOut = String.Empty;
                cmdOut += String.Format("COMMAND: \"{0}\"", commandToExecute) + Environment.NewLine;

                var result = sshCmd.Execute();
                if (result != String.Empty)
                {
                    cmdOut += "[STDOUT]" + Environment.NewLine;
                    cmdOut += result;
                }

                var reader = new StreamReader(sshCmd.ExtendedOutputStream);
                string stdErrorOutput = reader.ReadToEnd();
                if (stdErrorOutput != String.Empty)
                {
                    cmdOut += "[STDERR]" + Environment.NewLine;
                    cmdOut += stdErrorOutput;
                }

                return cmdOut;
            }
        }
    }
}
