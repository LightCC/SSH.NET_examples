using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SshEngine
{
    public interface ISshCommand
    {
        bool IsExecuted { get; }
        string Cmd { get; set; }
        string StdOutText { get; }
        string StdErrText { get; }
    }
}
