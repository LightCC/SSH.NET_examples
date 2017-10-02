using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using SshEngine;

namespace ssh_ex_console.cs
{
    public class SshConsole
    {
        private SshConnection _localSsh;

        public void TestBasicSshCommandsWithStream()
        {
            _localSsh = new SshConnection();
            UpdateInfoFromConsole();

            using (var sshClient = new SshClient(_localSsh.Info))
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

        public void UpdateInfoFromConsole()
        {
            string inputUseHost, input;
            do
            {
                Console.WriteLine("Host: {0}:{1}", _localSsh.HostIp, _localSsh.HostPort);
                Console.Write("Use this Host (Y/n)? ");
                inputUseHost = Console.ReadLine().ToLower().Trim();

                if (inputUseHost == "n")
                {
                    Console.Write("New Host (Name/IP): ");
                    _localSsh.HostIp = Console.ReadLine().Trim();
                    Console.Write("New Host Port [22]: ");
                    input = Console.ReadLine().Trim();
                    try
                    {
                        _localSsh.HostPort = Convert.ToInt32(input);
                    }
                    catch
                    {
                        _localSsh.HostPort = 22;
                    }
                    Console.WriteLine();
                }
            } while (inputUseHost != "y");

            Console.Write("Username [{0}]: ", _localSsh.Username);
            string userInput = Console.ReadLine().Trim();
            if (userInput.Length > 0)
            {
                _localSsh.Username = userInput;
                Console.Write("Password: ");
                _localSsh.Password = Console.ReadLine().Trim();
            } // else do nothing - they just hit "Enter" or entered all whitespace 

            _localSsh.UpdateInfo();
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
