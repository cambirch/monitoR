using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nortal.Utilities.TaskSchedulerEngine;
using Topshelf;

namespace monitoR.Service
{
    public class App : ServiceControl
    {
        private readonly AllHostTasksSchedulerEngine _engine = new AllHostTasksSchedulerEngine();

        public bool Start(HostControl hostControl) {
            _engine.RunInSeparateThread();
            return true;
        }

        public bool Stop(HostControl hostControl) {
            _engine.RequestStop();
            return true;
        }
    }
}
