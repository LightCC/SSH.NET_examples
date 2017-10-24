using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Renci.SshNet;

namespace SshEngine
{
    public class SshCmdBase : ISshCmd
    {
        private SshSessionBase _sshSession;

        public bool IsExecuted { get; set; }
        public string CmdText { get; set; }
        public string StdOutText { get; set; }
        public string StdErrText { get; set; }

        public SshCmdBase(SshSessionBase sshSession)
        {
            _sshSession = sshSession;

            SetCmdToNotExecuted();
        }

        public void SetCmdToNotExecuted()
        {
            IsExecuted = false;
            CmdText = null;
            StdOutText = null;
            StdErrText = null;
        }

        public virtual void ExecuteCmd(string cmdText = null)
        {
            if (cmdText != null) { this.CmdText = cmdText; }

            _sshSession.ExecuteBaseSingleCommand(this);
        }

    }
}
