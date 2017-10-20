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
            string output1, output2;

            var host = Config.SshHostName;
            var port = Config.SshHostPort;
            var user = Config.SshUsername;
            var pass = Config.SshPassword;
            var auth = new PasswordAuthenticationMethod(user, pass);
            var info = new ConnectionInfo(host,port,user,auth);

            using (var ssh = new SshClient(info))
            {
                ssh.Connect();
                var cmd1 = ssh.CreateCommand("pwd; cd ..; pwd");
                output1 = cmd1.Execute();
                var cmd2 = ssh.CreateCommand("pwd; cd ..; pwd");
                output2 = cmd2.Execute();
                ssh.Disconnect();

            }

            string output = output1 + Environment.NewLine + output2;
            System.Windows.Forms.Clipboard.SetText(output);
            System.Windows.Forms.MessageBox.Show(output);

            //Assert
            output2.Should().NotContain("/home/");
        }

        [Fact]
        public void SaveUserValues()
        {
            //Console.WriteLine(Config.ToString());
            Config.Save();
        }

    }
}
