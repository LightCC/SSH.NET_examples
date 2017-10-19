using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Xunit;

namespace SshEngine.Tests.SshSessionBaseTests
{
    public class ExecuteSingleCommand
    {
        [Theory]
        [InlineData("0.0.0.0", 22, "user", "pass")]
        public void ExecuteSingleCommand_ReturnsFalseWithBadIpAddress(string host, int port, string user, string pass)
        {
            var sut = new SshSessionBase(host, port, user, pass);
            bool result = sut.ExecuteSingleCommand("echo test");

            result.Should().BeFalse();
        }

        [Fact]
        public void ExecuteSingleCommand_DoesNotThrowExceptionWithBadIpAddress()
        {
            var sut = new SshSessionBase("0.0.0.0", 22, "user", "pass");
            Action act = () => sut.ExecuteSingleCommand("echo test");

            act.ShouldNotThrow();
            sut.Cmd.Should().Be("echo test");
            sut.StdOutText.Should().BeNull();
            sut.StdErrText.Should().BeNull();
        }

        [Theory]
        [InlineData(null, 22, "user", "pass")]
        [InlineData("0.0.0.0", 0, "user", "pass")]
        [InlineData("0.0.0.0", 22, null, "pass")]
        [InlineData("0.0.0.0", 22, "user", null)]
        public void ExecuteSingleCommand_ThrowsApplicationExceptionWithDefaultData(string host, int port, string user, string pass)
        {
            var sut = new SshSessionBase(host, port, user, pass);
            Action act = () => sut.ExecuteSingleCommand("echo test");

            act.ShouldThrow<ApplicationException>()
                .WithMessage("Connection Info not set!");
        }

    }
}
