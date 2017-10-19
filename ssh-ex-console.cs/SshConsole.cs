using System;
using SshEngine;

namespace ssh_ex_console.cs
{
    public class SshConsole
    {
        private SshSessionBase _localSsh;

        /// <summary>
        /// Creates an SshSession with methods that will interact with a console
        /// </summary>
        public SshConsole()
        {
            // Restore from either the embedded default settings file, or the last host info that was saved in a prior session to a user settings file
            string host = Properties.Settings.Default.HostName;
            int port = Properties.Settings.Default.HostPort;
            string user = Properties.Settings.Default.Username;
            string pass = Properties.Settings.Default.Password;

            // Create the local Ssh session with basic password authentication
            _localSsh = new SshSessionBase(host, port, user, pass);
        }

        /// <summary>
        /// Tests several basic SSH commands to make sure the connection is functioning. Prints the commands, stdout, and stderr, with headers for each.
        /// </summary>
        public void TestBasicSshCommandsWithStream()
        {
            {
                string cmd;
                
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

        /// <summary>
        /// Displays Host Name/IP and User/Pass information and asks whether
        /// to enter new host/user info.  If yes asks for each piece of info
        /// from the console, then re-asks in a loop
        /// 
        /// After replying not to enter new info, will save out current values
        /// to the user settings file, and update the local ConnectionInfo
        /// </summary>
        public void UpdateInfoFromConsole()
        {
            string inputUseHost, input;
            do
            {
                Console.WriteLine("Host: {0}:{1}", _localSsh.HostName, _localSsh.HostPort);
                Console.WriteLine("Username: {0}", _localSsh.Username);
                Console.WriteLine("Password: {0}", _localSsh.Password);
                Console.WriteLine();
                Console.Write("Enter new Host and User Info (y/n)? ");
                inputUseHost = Console.ReadLine().ToLower().Trim();

                if (inputUseHost == "y")
                {
                    Console.Write("New Host (Name/IP): ");
                    _localSsh.HostName = Console.ReadLine().Trim();

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
            } while (inputUseHost != "n" );

            _localSsh.UpdateInfoWithPasswordAuthentication();

            Properties.Settings.Default.HostName = _localSsh.HostName;
            Properties.Settings.Default.HostPort = _localSsh.HostPort;
            Properties.Settings.Default.Username = _localSsh.Username;
            Properties.Settings.Default.Password = _localSsh.Password;
            Properties.Settings.Default.Save();
        }


        /// <summary>
        /// Will Execute the given command on the given SshSession, and
        /// print out the COMMAND, [STDOUT], and [STDERR] to console
        /// or print an error if connection cannot be made
        /// </summary>
        /// <param name="sshSess">SSH Session to send command to</param>
        /// <param name="cmd">string command to execute</param>
        /// <returns></returns>
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
