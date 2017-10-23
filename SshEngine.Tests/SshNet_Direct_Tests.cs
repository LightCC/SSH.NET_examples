using FluentAssertions;
using Renci.SshNet;
using SshConsole;
using System;
using System.Security.Principal;
using Xunit;
using System.Windows.Forms;

namespace SshEngine.Tests
{
    public class SshNet_Direct_Tests
    {
        [Fact]
        public void MultiCommandTest()
        {
            string output1;

            var host = Config.SshHostName;
            var port = Config.SshHostPort;
            var user = Config.SshUsername;
            var pass = Config.SshPassword;
            var auth = new PasswordAuthenticationMethod(user, pass);
            var info = new ConnectionInfo(host,port,user,auth);

            using (var ssh = new SshClient(info))
            {
                ssh.Connect();
                var cmd1 = ssh.CreateCommand("pwd");
                output1 = cmd1.Execute();
                ssh.Disconnect();
            }

            //System.Windows.Forms.MessageBox.Show(output1);

            //Assert
            output1.Should().Contain("/home/");
        }

        [Fact]
        public void SaveUserValues()
        {
            //Console.WriteLine(Config.ToString());
            Config.Save();

            Config.SshHostName.Should().Contain("192");
        }

    }
}
