using ConfigR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine;
using Dynamitey;

namespace monitoR.Service.Notifier
{
    class EmailNotifier : INotifier
    {
        private readonly IConfig _config;
        private readonly Func<FluentEmail.Email> GetEmailCreator;

        public EmailNotifier(IConfig config, Func<FluentEmail.Email> getEmailCreator)
        {
            _config = config;
            GetEmailCreator = getEmailCreator;
        }

        public void SendNotification(string notificationTemplate, object notificationData)
        {
            // Get the template string
            var template = _config.Get<string>("template-" + notificationTemplate);

            Razor.Compile(template, notificationData.GetType(), notificationTemplate);

            var viewBag = new RazorEngine.Templating.DynamicViewBag();

            var emailBody = Razor.Run(notificationTemplate, notificationData, viewBag);

            var subject = viewBag.GetMaybeProperty("Subject", "Unknown Subject sent from monitoR.Service");
            var isHighPriority = viewBag.GetMaybeProperty("IsHighPriority", false);

            var email = GetEmailCreator().To(_config.Get<string>("toEmail"))
                                            .Subject(subject)
                                            .Body(emailBody);

            if (isHighPriority) email = email.HighPriority();

            email.Send();
        }



    }

    static class DynamicExtensions
    {
        public static T GetMaybeProperty<T>(this RazorEngine.Templating.DynamicViewBag obj, string propertyName)
        {
            return GetMaybeProperty<T>(obj, propertyName, default(T));
        }

        public static T GetMaybeProperty<T>(this RazorEngine.Templating.DynamicViewBag obj, string propertyName, T defaultValue)
        {
            if (Dynamic.GetMemberNames(obj,true).Any(f=>f == propertyName)) {
                return Dynamic.InvokeGet(obj, propertyName);
            } else {
                return defaultValue;
            }
        }
    }
}
