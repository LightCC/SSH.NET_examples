using Renci.SshNet;
using System;
using System.IO;

namespace SshEngine
{
    public class SshSessionBase : ISshConnection, ISshCommand, IDisposable
    {
        // ISshConnection Props
        private string _hostName;
        private int _hostPort;
        private string _username;
        private string _password;
        private ConnectionInfo _info;
        private AuthenticationMethod _auth;

        // ISshCommand Props
        private bool _isExecuted;
        private string _stdoutText;
        private string _stderrText;

        private MemoryStream _in = new MemoryStream();
        private MemoryStream _out = new MemoryStream();
        private MemoryStream _err = new MemoryStream();

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

        // ISshCommand Public Properties
        public bool IsExecuted
        {
            get { return _isExecuted; }
            private set { _isExecuted = value; }
        }

        public string Cmd { get; set; }

        public string StdOutText
        {
            get { return _stdoutText; }
            private set { _stdoutText = value; }
        }

        public string StdErrText
        {
            get { return _stderrText; }
            private set { _stderrText = value; }
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
            this.Cmd = cmd;
            return ExecuteCommandInShell();
        }

        public bool ExecuteCommandInShell()
        {
            //if (Connect())
            //{
            //    var shellIn = new StreamWriter(_in);
            //    var shellOut = new StreamReader(_out);
            //    var shellErr = new StreamReader(_err);

                //shellIn.WriteLine(this.Cmd);
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
            IsExecuted = false;
            return IsExecuted;

        }

        /// <summary>
        /// Execute a single command on the SSH Connection
        /// This version explicitly gives the command, rather than using the property
        /// StdOut and StdErrText are put into the local properties
        /// </summary>
        /// <param name="sshClient"></param>
        /// <param name="cmd"></param>
        public bool ExecuteSingleCommand(string cmd)
        {
            // Command is explicit, so write it into local SshSessionBase _cmd first
            this.Cmd = cmd;
            return ExecuteSingleCommand();
        }

        public bool ExecuteSingleCommand()
        {
            if (Connect())
            {
                var cmd = _ssh.CreateCommand(this.Cmd);
                _stdoutText = cmd.Execute();

                var reader = new StreamReader(cmd.ExtendedOutputStream);
                _stderrText = reader.ReadToEnd();

                IsExecuted = true;
            }
            else
            {
                _stdoutText = null;
                _stderrText = null;
                IsExecuted = false;
            }
            return IsExecuted;

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
                    _ssh.CreateShell(_in, _out, _err, "TEST", 80, 50, 800, 500, null, 1024);
                    return true;
                }
                catch (Exception e)
                {
                    // TODO probably need to tighten this down
                    // The exact argument thrown is about the IP address being 0.0.0.0 in Dns.GetHostAddresses
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
            _in = null;
            _out = null;
            _err = null;
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
