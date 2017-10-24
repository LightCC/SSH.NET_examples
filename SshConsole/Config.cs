using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using SshConsole.Properties;

namespace SshConsole
{
    public static class Config
    {
        public static string SshHostName
        {
            get { return Settings.Default.HostName; }
            set { Settings.Default.HostName = value; }
        }

        public static int SshHostPort
        {
            get { return Settings.Default.HostPort; }
            set { Settings.Default.HostPort = value; }
        }

        public static string SshUsername
        {
            get { return Settings.Default.Username; }
            set { Settings.Default.Username = value; }
        }

        public static string SshPassword
        {
            get { return Settings.Default.Password; }
            set { Settings.Default.Password = value; }
        }

        public static void Save()
        {
            Settings.Default.Save();
        }

        //public static string ToString()
        //{
        //    return Settings.Default.ToString();
        //}

    }
}
