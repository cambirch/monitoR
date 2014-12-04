using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nortal.Utilities.TaskSchedulerEngine;

namespace monitoR.Service
{
    public class CheckHardDriveStatusTask : ISchedulerTask
    {
        public void Run(StringBuilder messagesToMainLog) {
            throw new NotImplementedException();
        }

        public bool IsEnabled { get { return true; } }
        public DateTime ExecutionTime { get { return DateTime.Today; } }
        public TimeSpan Interval { get { return TimeSpan.FromMinutes(5); } }
    }
}
