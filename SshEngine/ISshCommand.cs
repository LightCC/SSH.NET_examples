using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SshEngine
{
    public interface ISshCommand
    {
        string Cmd { get; set; }
        string StdOut { get; }
        string StdErr { get; }
    }
}
