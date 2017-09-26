using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace ssh_ex_console.cs
{
    class Program
    {
        static void Main(string[] args)
        {
            SshExamples sshEx = new SshExamples();
            sshEx.TestBasicSshCommandsWithStream();
        }
    }
}
