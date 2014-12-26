using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;
using Autofac.Core;
using Autofac.Extras.Quartz;
using ConfigR;
using FluentEmail;
using monitoR.Service.DebugObjects;
using monitoR.Service.Interfaces;
using monitoR.Service.JobListeners;
using monitoR.Service.Jobs;
using monitoR.Service.PSObjects;
using monitoR.Service.Utilities;
using Ploeh.AutoFixture;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Topshelf;
using Topshelf.Quartz;
using monitoR.Service.Service;
using Topshelf.Autofac;
using Topshelf.Quartz.Autofac;
using monitoR.Service.Notifier;
using PersistentObjectCachenet45;

namespace monitoR.Service
{
    class Program
    {
        static void Main(string[] args) {
            Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter {Level = Common.Logging.LogLevel.Info};

            // Turn our cache system into a "database"
            //PersistentObjectCache.SetDefaultInvalidationTime(TimeSpan.MaxValue);

            // Start the DI
            var container = InitContainer();

            //// Initialize the schedule
            //IScheduler scheduler = container.Resolve<IScheduler>();
            //scheduler.Start();


            // Add the job
            //IJobDetail job = JobBuilder.Create<CheckStoragePoolHealth>().WithIdentity("checkStoragePoolHealth", "group").Build();
            //ITrigger trigger = TriggerBuilder.Create().WithIdentity("trigger1", "group").StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()).Build();
            //scheduler.ScheduleJob(job, trigger);

            // Add the listener
            //var listener = container.Resolve<EmailNotifyJobListener>();
            //scheduler.ListenerManager.AddJobListener(listener, KeyMatcher<JobKey>.KeyEquals(new JobKey("checkStoragePoolHealth", "group")));

            HostFactory.Run(c =>
            {
                c.UseAutofacContainer(container);

                c.Service<AppService>((Action<Topshelf.ServiceConfigurators.ServiceConfigurator<AppService>>)(s =>
                {
                    s.ConstructUsingAutofacContainer();

                    s.UseQuartzAutofac();

                    s.WhenStarted((sc, control) => sc.Start(control));
                    s.WhenStopped((sc, control) => sc.Stop(control));
                    
                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() => JobBuilder.Create<CheckStoragePoolHealth>().WithIdentity("checkStoragePoolHealth", "group").Build())
                        .AddTrigger(() => TriggerBuilder.Create().WithIdentity("trigger1", "group").StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()).Build())
                        );
                    
                    //s.WhenStarted((service, control) => service.Start());
                }));

                c.RunAsLocalSystem();
                c.StartAutomaticallyDelayed();

                c.SetDescription("Monitors a windows server for critical events and sends notifications.");
                c.SetDisplayName("MonitoR");
                c.SetServiceName("MonitoR");

                c.EnableServiceRecovery(r => { r.RestartService(1); });
            });

            Application.Run(container.Resolve<Config>());
            
            //scheduler.Shutdown();
        }

        #region DI Setup

        internal static Autofac.IContainer DiContainer { get; set; }

        private static IContainer InitContainer() {
            var builder = new Autofac.ContainerBuilder();

            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(new QuartzAutofacJobsModule(Assembly.GetExecutingAssembly()));

            builder.RegisterType<AppService>();

            builder.RegisterType<Config>().AsSelf().SingleInstance();
            builder.RegisterType<UIJobListener>().AsSelf();
            builder.RegisterType<EmailNotifyJobListener>().AsSelf().SingleInstance();

#if DEBUG
            var fixture = new Ploeh.AutoFixture.Fixture();

            builder.Register<IEnumerable<IStoragePool>>((c, p) => { return fixture.CreateMany<DebugStoragePool>(); }).AsSelf();
#else
            builder.Register<IEnumerable<IStoragePool>>((c, p) => { return Powershell.GetStoragePool(); }).AsSelf();
#endif

            builder.RegisterInstance(ConfigR.Config.Global).As<IConfig>().ExternallyOwned();

            builder.RegisterType<EmailNotifier>().As<INotifier>();

            // Setup the e-mail settings
            builder.Register<SmtpClient>((c, p) => {
                var smtpClient = c.Resolve<IConfig>().Get<SmtpClient>("smtp");
                return smtpClient;
            }).SingleInstance().ExternallyOwned();
            builder.Register<FluentEmail.Email>((c, p) => {
                return new Email(c.Resolve<SmtpClient>(), c.Resolve<IConfig>().Get<string>("fromEmail"));
            }).AsSelf().ExternallyOwned();

            return DiContainer = builder.Build();
        }

        #endregion
    }
}

namespace Topshelf.Quartz.Autofac
{
	public static class AutofacScheduleJobHostConfiguratorExtensions
	{
        public static Topshelf.HostConfigurators.HostConfigurator UseQuartzAutofac(this Topshelf.HostConfigurators.HostConfigurator configurator)
		{
            AutofacScheduleJobServiceConfiguratorExtensions.SetupAutofac();

			return configurator;
		}
	}
}
//using System;
//using Ninject;
//using Quartz;
//using Topshelf.Logging;
//using Topshelf.Ninject;
//using Topshelf.ServiceConfigurators;

namespace Topshelf.Quartz.Autofac
{
	public static class AutofacScheduleJobServiceConfiguratorExtensions
	{
		public static Topshelf.ServiceConfigurators.ServiceConfigurator<T> UseQuartzAutofac<T>(this Topshelf.ServiceConfigurators.ServiceConfigurator<T> configurator)
			where T : class
		{
			SetupAutofac();

			return configurator;
		}

		internal static void SetupAutofac()
		{
			var log = Topshelf.Logging.HostLogger.Get(typeof(AutofacScheduleJobServiceConfiguratorExtensions));

            var container = monitoR.Service.Program.DiContainer;

			//IKernel kernel = NinjectBuilderConfigurator.Kernel;

            //if (kernel == null)
            //    throw new Exception("You must call UseNinject() to use the Quartz Topshelf Ninject integration.");

            var schedulerFactory = container.Resolve<Func<IScheduler>>();

			//Func<IScheduler> schedulerFactory = () => kernel.Get<IScheduler>();

			ScheduleJobServiceConfiguratorExtensions.SchedulerFactory = schedulerFactory;

			log.Info("[Topshelf.Quartz.Ninject] Quartz configured to construct jobs with Ninject.");
		}
	}
}

