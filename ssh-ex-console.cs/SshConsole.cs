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

        public SshConsole()
        {
            _localSsh = new SshConnection();
        }

        public void TestBasicSshCommandsWithStream()
        {
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
                Console.WriteLine();
                Console.Write("Press <Enter> To Continue");
                Console.ReadLine();
                Console.WriteLine();

                sshClient.Disconnect();

                //Used with unit test framework
                //Assert.Inconclusive();
            }
        }

        public void ManualCommandLoop()
        {
            using (var sshClient = new SshClient(_localSsh.Info))
            {
                string cmd, cmdlower;
                string cmdout;

                sshClient.Connect();

                do
                {
                    Console.Write("{0}:{1} > ", _localSsh.HostIp, _localSsh.HostPort);
                    cmd = Console.ReadLine().Trim();
                    cmdlower = cmd.ToLower();
                    if (cmd.Length > 0 && cmdlower != "exit")
                    {
                        Console.WriteLine();
                        cmdout = ExecuteSshCmd_ReturnCmdOutErr(sshClient, cmd);
                        Console.WriteLine(cmdout);
                    }

                } while (cmdlower != "exit");

                Console.WriteLine();
                Console.Write("Press <Enter> To Continue");
                Console.ReadLine();
                Console.WriteLine();

                sshClient.Disconnect();
            }

        }

        public void UpdateInfoFromConsole()
        {
            string inputUseHost, input;
            do
            {
                Console.WriteLine("Host: {0}:{1}", _localSsh.HostIp, _localSsh.HostPort);
                Console.WriteLine("Username: {0}; Password: {1}", _localSsh.Username, _localSsh.Password);
                Console.WriteLine();
                Console.Write("Use this Host and User Info (Y/n)? ");
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

                    Console.Write("Username [{0}]: ", _localSsh.Username);
                    string userInput = Console.ReadLine().Trim();
                    if (userInput.Length > 0)
                    {
                        _localSsh.Username = userInput;
                    } // else do nothing - they just hit "Enter" or entered all whitespac

                    Console.Write("Password: ");
                    _localSsh.Password = Console.ReadLine().Trim();
                    Console.WriteLine();
                }
            } while (inputUseHost != "y");

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
