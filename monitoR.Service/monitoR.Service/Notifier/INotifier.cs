using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitoR.Service.Notifier
{
    interface INotifier
    {

        /// <summary>
        /// Used to send a notification.
        /// </summary>
        /// <param name="notificationTemplate">The template used to generate the actual notifcation.</param>
        /// <param name="notificationData">The data model to pass to the template.</param>
        void SendNotification(string notificationTemplate, object notificationData);

    }
}
