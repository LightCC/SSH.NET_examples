using System;
using Renci.SshNet;
using System.IO;
using System.Runtime.Remoting.Channels;

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
        private string _cmd;
        private string _stdout;
        private string _stderr;

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
        public string Cmd
        {
            get { return _cmd; }
            set { _cmd = value; }
        }

        public string StdOut
        {
            get { return _stdout; }
            private set { _stdout = value; }
        }

        public string StdErr
        {
            get { return _stderr; }
            private set { _stderr = value; }
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

        /// <summary>
        /// Execute a single command on the SSH Connection
        /// This version explicitly gives the command, rather than using the property
        /// StdOut and StdErr are put into the local properties
        /// </summary>
        /// <param name="sshClient"></param>
        /// <param name="cmd"></param>
        public bool ExecuteSingleCommand(string cmd)
        {
            // Command is explicit, so write it into local SshSessionBase _cmd first
            _cmd = cmd;
            return ExecuteSingleCommand();
        }

        public bool ExecuteSingleCommand()
        {
            if (Connect())
            {
                var cmd = _ssh.CreateCommand(_cmd);
                _stdout = cmd.Execute();

                var reader = new StreamReader(cmd.ExtendedOutputStream);
                _stderr = reader.ReadToEnd();

                return true;
            }
            else
            {
                _stdout = null;
                _stderr = null;
                return false;
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
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("");
                    Console.WriteLine(e);
                    Console.WriteLine("");
                    return false;
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

        public void DisposeOfSshClient()
        {
            if (_ssh != null)
            {
                DisconnectSshClientIfConnected();
                _ssh.Dispose();
            }
        }

        public void Dispose()
        {
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
