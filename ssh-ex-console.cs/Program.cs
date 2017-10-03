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
            SshConsole sshEx = new SshConsole();

            sshEx.UpdateInfoFromConsole();

            Console.Write("Test Basic Commands [Y/n]? ");
            string input = Console.ReadLine().ToLower().Trim();
            if (input.Length == 0 || input == "y")
            {
                sshEx.TestBasicSshCommandsWithStream();
            }

            sshEx.ManualCommandLoop();
        }
    }
}
