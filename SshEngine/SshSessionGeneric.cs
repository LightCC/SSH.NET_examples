using System;

namespace SshEngine
{
    public class SshSessionGeneric : SshSessionBase
    {
        public SshSessionGeneric(string hostName, int hostPort, string username, string password) : base(hostName, hostPort, username, password)
        {
            
        }
    }
}