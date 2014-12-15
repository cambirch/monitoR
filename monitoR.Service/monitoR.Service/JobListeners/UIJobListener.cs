using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace monitoR.Service.JobListeners
{
    class UIJobListener : IJobListener
    {
        private readonly Lazy<Config> _configForm;

        public UIJobListener(Lazy<Config> config) {
            _configForm = config;
        }

        #region IJobListener Members

        public void JobExecutionVetoed(IJobExecutionContext context) {
            
        }

        public void JobToBeExecuted(IJobExecutionContext context) {
            
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException) {
            Console.WriteLine("Listen");

            var result = (bool)context.Result;

            var configForm = _configForm.Value;

            configForm.label1.BeginInvoke((Action)(() => {
                                                       configForm.label1.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                                       configForm.label2.Text = "Is Any Unhealthy: " + context.Get("anyUnhealthy").ToString();
                                                   }));

        }

        public string Name {
            get { return "UIJobListener"; }
        }

        #endregion
    }
}
