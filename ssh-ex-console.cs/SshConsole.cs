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
        private SshSessionBase _localSsh;

        public SshConsole()
        {
            string host = Properties.Settings.Default.HostName;
            int port = Properties.Settings.Default.HostPort;
            string user = Properties.Settings.Default.Username;
            string pass = Properties.Settings.Default.Password;

            _localSsh = new SshSessionBase(host, port, user, pass);
        }

        public void TestBasicSshCommandsWithStream()
        {
            {
            string cmd = String.Empty;
                string cmdOut = String.Empty;

                cmd = "echo \"This is StdOut\"; echo \"This is StdErr\" >&2";
                ExecuteSshCmdWithFullConsoleOutput(_localSsh, cmd);
                
                Console.WriteLine("---");
                cmd = "echo $USER";
                ExecuteSshCmdWithFullConsoleOutput(_localSsh, cmd);
                
                Console.WriteLine("---");
                cmd = "pwd";
                ExecuteSshCmdWithFullConsoleOutput(_localSsh, cmd);
                
                Console.WriteLine("---");
                cmd = "df -h";
                ExecuteSshCmdWithFullConsoleOutput(_localSsh, cmd);
                
                // Read an input <Enter> so the window doesn't go away before we can see/read it
                Console.WriteLine();
                Console.Write("Press <Enter> To Continue");
                Console.ReadLine();
                Console.WriteLine();

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
            string cmd;

            do
            {
                Console.Write("{0}:{1} > ", _localSsh.HostName, _localSsh.HostPort);
                cmd = Console.ReadLine().Trim();
                if (cmd.Length > 0 && cmd.ToLower() != "exit")
                {
                    Console.WriteLine();
                    _localSsh.ExecuteSingleCommand(cmd);
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

        }

        public void UpdateInfoFromConsole()
        {
            string inputUseHost, input;
            do
            {
                Console.WriteLine("Host: {0}:{1}", _localSsh.HostName, _localSsh.HostPort);
                Console.WriteLine("Username: {0}", _localSsh.Username);
                Console.WriteLine("Password: {0}", _localSsh.Password);
                Console.WriteLine();
                Console.Write("Use this Host and User Info (Y/n)? ");
                inputUseHost = Console.ReadLine().ToLower().Trim();

                if (inputUseHost == "n")
                {
                    Console.Write("New Host (Name/IP): ");
                    _localSsh.HostName = Console.ReadLine().Trim();
                    Properties.Settings.Default.HostName = _localSsh.HostName;

                    Console.Write("New Host Port [22]: ");
                    input = Console.ReadLine().Trim();
                    try
                    {
                        _localSsh.HostPort = Convert.ToInt32(input);
                        Properties.Settings.Default.HostPort = _localSsh.HostPort;
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
                        Properties.Settings.Default.Username = _localSsh.Username;
                    } // else do nothing - they just hit "Enter" or entered all whitespac

                    Console.Write("Password: ");
                    _localSsh.Password = Console.ReadLine().Trim();
                    Properties.Settings.Default.Password = _localSsh.Password;
                    Console.WriteLine();
                }
            } while (inputUseHost != "y");

            _localSsh.UpdateInfoWithPasswordAuthentication();
            Properties.Settings.Default.Save();
        }


        public static bool ExecuteSshCmdWithFullConsoleOutput(SshSessionBase sshSess, string cmd = null)
        {
            if (cmd == null) { cmd = sshSess.Cmd; }

            if (sshSess.ExecuteSingleCommand(cmd))
            {
                Console.WriteLine("COMMAND: \"{0}\"", sshSess.Cmd);

                if (sshSess.StdOut.Length > 0)
                {
                    Console.WriteLine("[STDOUT]");
                    Console.WriteLine(sshSess.StdOut);
                }

                if (sshSess.StdErr.Length > 0)
                {
                    Console.WriteLine("[STDERR]");
                    Console.WriteLine(sshSess.StdErr);
                }
                return true;
            }
            else
            {
                Console.WriteLine("Unable to Execute Command:");
                Console.WriteLine(cmd);
                return false;
            }
        }


    }
}
