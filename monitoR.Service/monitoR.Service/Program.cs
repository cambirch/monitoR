using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;
using Autofac.Core;
using Autofac.Extras.Quartz;
using monitoR.Service.JobListeners;
using monitoR.Service.Jobs;
using monitoR.Service.ObjectInterfaces;
using monitoR.Service.Utilities;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace monitoR.Service
{
    class Program
    {
        static void Main(string[] args) {
            Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter {Level = Common.Logging.LogLevel.Info};

            // Start the DI
            var container = InitContainer();

            // Initialize the schedule
            IScheduler scheduler = container.Resolve<IScheduler>();
            scheduler.Start();


            // Add the job
            IJobDetail job = JobBuilder.Create<CheckStoragePoolHealth>().WithIdentity("checkStoragePoolHealth", "group").Build();
            ITrigger trigger = TriggerBuilder.Create().WithIdentity("trigger1", "group").StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()).Build();
            scheduler.ScheduleJob(job, trigger);


            //HostFactory.Run(x => {
            //                    x.Service<App>();
            //                    x.RunAsLocalSystem();
            //                    x.StartAutomaticallyDelayed();

            //                    x.SetDescription("Monitors a windows server for critical events and sends notifications.");
            //                    x.SetDisplayName("MonitoR");
            //                    x.SetServiceName("MonitoR");

            //                    x.EnableServiceRecovery(r => { r.RestartService(1); });
            //                });

            Application.Run(container.Resolve<Config>());
            
            scheduler.Shutdown();
        }

        #region DI Setup

        private static Autofac.IContainer DiContainer { get; set; }

        private static IContainer InitContainer() {
            var builder = new Autofac.ContainerBuilder();

            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(new QuartzAutofacJobsModule(Assembly.GetExecutingAssembly()));

            builder.RegisterType<Config>().AsSelf().SingleInstance();
            builder.RegisterType<UIJobListener>().AsSelf();

            builder.Register<IEnumerable<StoragePool>>((c, p) => { return Powershell.GetStoragePool(); }).AsSelf();

            return DiContainer = builder.Build();
        }

        #endregion
    }
}
