using Renci.SshNet;
using System;
using System.IO;

namespace SshEngine
{
    public class SshSessionBase : ISshConnection, IDisposable
    {
        // ISshConnection Props
        private string _hostName;
        private int _hostPort;
        private string _username;
        private string _password;
        private ConnectionInfo _info;
        private AuthenticationMethod _auth;

        // ISshCmd Props
        // None for now - need to pass in ISshCmd

        // Other Fields
        private SshClient _ssh;

        // ISshConnection Public Properties
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        public int HostPort
        {
            get { return _hostPort; }
            set { _hostPort = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public ConnectionInfo Info
        {
            get { return _info; }
            private set { _info = value; }
        }

        // TODO: Should some or all of this be private?
        public AuthenticationMethod Auth
        {
            get { return _auth; }
            set { _auth = value; }
        }

        public SshSessionBase(string hostName, int hostPort, string username, string password)
        {
            _hostName = hostName;
            _hostPort = hostPort;
            _username = username;
            _password = password;

            UpdateInfoWithPasswordAuthentication();
        }

        public void UpdateInfoWithPasswordAuthentication()
        {
            // todo Update to accept an authentication type rather than hardcode it

            if (_username == null || _password == null || _hostName == null || _hostPort == 0)
            {
                _info = null;
            }
            else
            {
                // var privateKeyFile = new PrivateKeyFile("c:\\privatekeyfilename", "passPhrase");

                // Setup all the possible authentication methods
                // AuthenticationMethod authNone = new NoneAuthenticationMethod(username);
                AuthenticationMethod authPassword = new PasswordAuthenticationMethod(_username, _password);
                // AuthenticationMethod authKeyboard = new KeyboardInteractiveAuthenticationMethod(username);
                //AuthenticationMethod authPrivateKey = new PrivateKeyAuthenticationMethod(username, privateKeyFile);

                // Pick which authentication method to use for this test
                _auth = authPassword;

                _info = new ConnectionInfo(_hostName, _hostPort, _username, _auth);

            }
            
        }

        public bool ExecuteCommandInShell(string cmd)
        {
            //this.CmdText = cmd;
            //return ExecuteCommandInShell();
            return false;
        }

        public bool ExecuteCommandInShell()
        {
            //if (Connect())
            //{
            //    var shellIn = new StreamWriter(_in);
            //    var shellOut = new StreamReader(_out);
            //    var shellErr = new StreamReader(_err);

                //shellIn.WriteLine(this.CmdText);
                //shellIn.Flush();

                //_stdoutText = shellOut.ReadToEnd();
                //_stderrText = shellErr.ReadToEnd();

            //    IsExecuted = true;
            //}
            //else
            //{
            //    _stdoutText = null;
            //    _stderrText = null;
            //    IsExecuted = false;
            //}
            //IsExecuted = false;
            //return IsExecuted;
            return false;
        }

        /// <summary>
        /// Execute a single command on the SSH Connection
        /// The text command comes from the cmd.CmdText property
        /// cmd.StdOutText is the command output from StdOut (empty string if none)
        /// cmd.StdErrText is the command output from StdErr (empty string if none)
        /// </summary>
        /// <param name="cmd">an object of ISshCmd type</param>
        public void ExecuteBaseSingleCommand(ISshCmd cmd)
        {
            if (Connect())
            {
                var cmdobj = _ssh.CreateCommand(cmd.CmdText);
                cmd.StdOutText = cmdobj.Execute();

                var reader = new StreamReader(cmdobj.ExtendedOutputStream);
                cmd.StdErrText = reader.ReadToEnd();

                cmd.IsExecuted = true;
            }
            else
            {
                cmd.StdOutText = null;
                cmd.StdErrText = null;
                cmd.IsExecuted = false;
            }
        }

        private bool Connect()
        {
            if (_ssh == null)
            {
                if (Info == null)
                {
                    throw new ApplicationException("Connection Info not set!");
                }
                else
                {
                    _ssh = new SshClient(Info);
                }
            }
            if (_ssh.IsConnected)
            {
                return true;
            }
            else
            {
                try
                {
                    _ssh.Connect();
                    //_ssh.CreateShell(_in, _out, _err, "TEST", 80, 50, 800, 500, null, 1024);
                    return true;
                }
                catch (Exception e)
                {
                    // TODO Connection Failure Exception - need to tighten this up
                    // One argument thrown is about the IP address being 0.0.0.0 in Dns.GetHostAddresses
                    if (e is ArgumentException)
                    {
                        return false;
                    }
                    throw;
                }
            }
        }

        private void DisconnectSshClientIfConnected()
        {
            if (_ssh.IsConnected)
            {
                _ssh.Disconnect();
            }
        }

        private void DisposeOfSshClient()
        {
            if (_ssh != null)
            {
                DisconnectSshClientIfConnected();
                _ssh.Dispose();
            }
        }

        public void Dispose()
        {
            //_in = null;
            //_out = null;
            //_err = null;
            DisposeOfSshClient();
        }

        public override string ToString()
        {
            string InfoString = String.Empty;
            InfoString = String.Format("Host: {0}:{1}", _hostName, _hostPort) + Environment.NewLine;
            InfoString += String.Format("Username: ", _username) + Environment.NewLine;
            InfoString += String.Format("Password: ", _password) + Environment.NewLine;

            return InfoString;
        }
    }
}
