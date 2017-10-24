using System;
using System.Xml.Linq;
using SshEngine;

namespace SshConsole
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
                // Create a basic command object, using our existing SshSessionBase object
                var cmd = new SshCmdBase(_localSsh);

                cmd.CmdText = "echo \"This is StdOut\"; echo \"This is StdErr\" >&2";
                ExecuteSshCmdWithFullConsoleOutput(cmd);

                Console.WriteLine("---");
                cmd.CmdText = "pwd; cd ..; pwd; echo $USER";
                ExecuteSshCmdWithFullConsoleOutput(cmd);

                Console.WriteLine("---");
                cmd.CmdText = "pwd";
                ExecuteSshCmdWithFullConsoleOutput(cmd);

                Console.WriteLine("---");
                cmd.CmdText = "df -h";
                ExecuteSshCmdWithFullConsoleOutput(cmd);

                Console.WriteLine();
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
            string cmdInput;
            var cmd = new SshCmdBase(_localSsh);

            do
            {
                Console.Write("{0}:{1} > ", _localSsh.HostName, _localSsh.HostPort);
                cmdInput = Console.ReadLine().Trim();
                if (cmdInput.Length > 0 && cmdInput.ToLower() != "exit")
                {
                    Console.WriteLine();
                    cmd.CmdText = cmdInput;
                    _localSsh.ExecuteBaseSingleCommand(cmd);
                    if (cmd.StdOutText.Length > 0)
                    {
                        Console.WriteLine(cmd.StdOutText);
                    }
                    if(cmd.StdErrText.Length > 0)
                    {
                        Console.WriteLine("[[StdErr]]");
                        Console.WriteLine(cmd.StdErrText);
                    }
                }
            } while (cmdInput.ToLower() != "exit");

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
                Console.Write("Use this Host and User Info (y/n)? ");
                inputUseHost = Console.ReadLine().ToLower().Trim();

                if (inputUseHost == "n")
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
                    input = Console.ReadLine().Trim();
                    if (input.Length > 0)
                    {
                        _localSsh.Password = input;
                    }
                    Console.WriteLine();
                }
            } while (inputUseHost == "n" );

            _localSsh.UpdateInfoWithPasswordAuthentication();

            // save current values (new or old) into the local user.settings file
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
        public static bool ExecuteSshCmdWithFullConsoleOutput(ISshCmd cmd, string cmdInput = null)
        {
            cmd.ExecuteCmd(cmdInput);
            if (cmd.IsExecuted)
            {
                Console.WriteLine("COMMAND: \"{0}\"", cmd.CmdText);

                if (cmd.StdOutText.Length > 0)
                {
                    Console.WriteLine("[STDOUT]");
                    Console.WriteLine(cmd.StdOutText);
                }

                if (cmd.StdErrText.Length > 0)
                {
                    Console.WriteLine("[STDERR]");
                    Console.WriteLine(cmd.StdErrText);
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
