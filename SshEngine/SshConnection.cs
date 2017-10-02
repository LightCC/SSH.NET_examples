using System;
using Renci.SshNet;

namespace SshEngine
{
    public class SshConnection : ISshConnection
    {
        private string _hostIp;
        private int _hostPort;
        private string _username;
        private string _password;
        private ConnectionInfo _info;
        private AuthenticationMethod _auth;

        public string HostIp
        {
            get { return _hostIp; }
            set { _hostIp = value; }
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

        public SshConnection(ConnectionInfo connectionInfo = null)
        {
            if (connectionInfo == null)
            {
                DefaultInfo();
            }
            else
            {
                Info = connectionInfo;
            }
        }

        private void DefaultInfo()
        {
            // hardcoded test vars
            // string hostIp = "192.168.42.153";
            _hostIp = "128.0.0.1";
            _hostPort = 22;
            _username = "user";
            _password = "password";
            _auth = new PasswordAuthenticationMethod(_username, _password);

            Info = new ConnectionInfo(_hostIp, _hostPort, _username, _auth);
        }

        public void UpdateInfo()
        {
            // todo Update to accept an authentication type rather than hardcode it

            // var privateKeyFile = new PrivateKeyFile("c:\\privatekeyfilename", "passPhrase");

            // Setup all the possible authentication methods
            // AuthenticationMethod authNone = new NoneAuthenticationMethod(username);
            AuthenticationMethod authPassword = new PasswordAuthenticationMethod(_username, _password);
            // AuthenticationMethod authKeyboard = new KeyboardInteractiveAuthenticationMethod(username);
            //AuthenticationMethod authPrivateKey = new PrivateKeyAuthenticationMethod(username, privateKeyFile);

            // Pick which authentication method to use for this test
            _auth = authPassword;

            _info = new ConnectionInfo(_hostIp, _hostPort, _username, _auth);
        }

        public string ToString()
        {
            string InfoString = String.Empty;
            InfoString = String.Format("Host: {0}:{1}", _hostIp, _hostPort) + Environment.NewLine;
            InfoString += String.Format("Username: ", _username) + Environment.NewLine;
            InfoString += String.Format("Password: ", _password) + Environment.NewLine;

            return InfoString;
        }
    }
}
