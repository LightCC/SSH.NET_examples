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
            output1.Should().Contain("/home/",because:"we are expecting the 'pwd' command to include /home/ in an active SSH server at the home directory. If this failed with an Exception, it is likely either the local SSH Server info is bad or the server is not currently running.  See CheckThatLocalSshServerSettings_AreNotDefault test for more information and a method to update the local settings info.");
        }

        [Fact]
        public void CheckThatLocalSshServerSettings_AreNotDefault()
        {
            // NOTICE:  If this test is failing, you need to uncomment the below and set them to a local login that has an active SSH server with this login info
            // This is because running the unit tests appears to draw from a different user.config file location, one of /home/user/AppData/Local/Microsoft_Corporation/SshEngine.Tests_StrongName_xxxx/y.y.y.y/user.config, where xxxx is a hash of some sort and y.y.y.y appears to be either a Windows or Visual Studio version number (not this application).
            // It is unclear how to share the user.config file that is stored by the SshConsole.exe application.

            // Uncomment the below with local info, then delete it after running the test once to set the local user.config file to a live SSH server without storing the info in the repo.
            // Config.SshHostName = ;
            // Config.SshHostPort = 22;
            // Config.SshUsername = ;
            // Config.SshPassword = ;
            //Config.Save();

            Config.SshHostName.Should().NotContain("DefaultHost", because:"We expect a local user.config settings file to have the location of a local SSH Server for SshHostName, SshHostPort, SshUsername, and SshPassword - please review the comments in this test for more information...");
        }

    }
}
