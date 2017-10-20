using System;

namespace SshConsole
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

            sshEx.ManualSingleCommandLoop();
        }

    }
}
