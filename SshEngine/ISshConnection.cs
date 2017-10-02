using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace SshEngine
{
    public interface ISshConnection
    {
        string HostIp { get; set; }
        int HostPort { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        ConnectionInfo Info { get; }
        AuthenticationMethod Auth { get; set; }

    }

}
