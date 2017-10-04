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
                ExecuteSshCmdWithFullConsoleOutput(sshClient, _localSsh, cmd);
                
                Console.WriteLine("---");
                cmd = "echo $USER";
                ExecuteSshCmdWithFullConsoleOutput(sshClient, _localSsh, cmd);
                
                Console.WriteLine("---");
                cmd = "pwd";
                ExecuteSshCmdWithFullConsoleOutput(sshClient, _localSsh, cmd);
                
                Console.WriteLine("---");
                cmd = "df -h";
                ExecuteSshCmdWithFullConsoleOutput(sshClient, _localSsh, cmd);
                
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


        /// <summary>
        /// Loop of single commands entered at console and sent to remote
        /// Each command is essentially executed as a new login
        /// (e.g. remote will always start at same login directory)
        /// 
        /// Will List output of the command to console,
        /// and if non-empty, a [[StdErr]] heading and error output
        /// 
        /// Use 'exit' command to break the loop
        /// </summary>
        public void ManualSingleCommandLoop()
        {
            using (var sshClient = new SshClient(_localSsh.Info))
            {
                string cmd;

                sshClient.Connect();

                do
                {
                    Console.Write("{0}:{1} > ", _localSsh.HostIp, _localSsh.HostPort);
                    cmd = Console.ReadLine().Trim();
                    if (cmd.Length > 0 && cmd.ToLower() != "exit")
                    {
                        Console.WriteLine();
                        _localSsh.ExecuteSingleCommand(sshClient, cmd);
                        if (_localSsh.StdOut.Length > 0)
                        {
                            Console.WriteLine(_localSsh.StdOut);
                        }
                        if(_localSsh.StdErr.Length > 0)
                        {
                            Console.WriteLine("[[StdErr]]");
                            Console.WriteLine(_localSsh.StdErr);
                        }
                    }

                } while (cmd.ToLower() != "exit");

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
                Console.WriteLine("Username: {0}", _localSsh.Username);
                Console.WriteLine("Password: {0}", _localSsh.Password);
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


        public static void ExecuteSshCmdWithFullConsoleOutput(SshClient sshClient, SshConnection sshConn, string Cmd = null)
        {
            if (Cmd == null) { Cmd = sshConn.Cmd; }
            sshConn.ExecuteSingleCommand(sshClient, Cmd);

            Console.WriteLine("COMMAND: \"{0}\"", sshConn.Cmd);

            if (sshConn.StdOut.Length > 0)
            {
                Console.WriteLine("[STDOUT]");
                Console.WriteLine(sshConn.StdOut);
            }

            if (sshConn.StdErr.Length > 0)
            {
                Console.WriteLine("[STDERR]");
                Console.WriteLine(sshConn.StdErr);
            }

        }


    }
}
