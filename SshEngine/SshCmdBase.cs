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

        public virtual void ExecuteCmd()
        {
            

            //var cmdobj = ssh.CreateCommand(CmdText);
            //StdOutText = cmdobj.Execute();

            //var reader = new StreamReader(cmdobj.ExtendedOutputStream);
            //StdErrText = reader.ReadToEnd();

            //IsExecuted = true;
        }

    }
}
