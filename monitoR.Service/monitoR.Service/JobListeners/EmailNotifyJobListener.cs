using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using FluentEmail;

namespace monitoR.Service.JobListeners
{
    class EmailNotifyJobListener : IJobListener
    {
        private bool? LastResult = null;
        private readonly Func<FluentEmail.Email> GetEmailCreator;

        public EmailNotifyJobListener(Func<FluentEmail.Email> getEmailCreator) {
            GetEmailCreator = getEmailCreator;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler"/> after a <see cref="T:Quartz.IJobDetail"/>
        ///             has been executed, and be for the associated <see cref="T:Quartz.Spi.IOperableTrigger"/>'s
        ///             <see cref="M:Quartz.Spi.IOperableTrigger.Triggered(Quartz.ICalendar)"/> method has been called.
        /// </summary>
        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException) {

            var result = (bool)context.Result;

            if (result != LastResult) {
                Console.WriteLine("Notify");

                var statusToString = new Func<bool, string>(b => {
                                                                if (b) {
                                                                    return "Warning / Failure State";
                                                                } else {
                                                                    return "Healthy";
                                                                }
                                                            });

                var email = GetEmailCreator().To("cam.birch@gmail.com", "Cam Birch")
                                             .Subject("Status Change on Storage Pool Monitoring: " + statusToString(result))
                                             .Body("test")
                                             .HighPriority()
                                             .Send();

                LastResult = result;
            }
        }

        /// <summary>
        /// Get the name of the <see cref="T:Quartz.IJobListener"/>.
        /// </summary>
        public string Name {
            get { return "EmailNotifyJobListener"; }
        }
        public void JobToBeExecuted(IJobExecutionContext context) {
            
        }

        public void JobExecutionVetoed(IJobExecutionContext context) {
            
        }

    }
}
