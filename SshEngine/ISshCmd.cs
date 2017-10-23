using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SshEngine
{
    public interface ISshCmd
    {
        bool IsExecuted { get; set; }
        string CmdText { get; set; }
        string StdOutText { get; set; }
        string StdErrText { get; set; }

        void ExecuteCmd();
    }
}
