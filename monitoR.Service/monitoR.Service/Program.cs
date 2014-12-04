using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Topshelf;

namespace monitoR.Service
{
    class Program
    {
        static void Main(string[] args) {

            //HostFactory.Run(x => {
            //                    x.Service<App>();
            //                    x.RunAsLocalSystem();
            //                    x.StartAutomaticallyDelayed();

            //                    x.SetDescription("Monitors a windows server for critical events and sends notifications.");
            //                    x.SetDisplayName("MonitoR");
            //                    x.SetServiceName("MonitoR");

            //                    x.EnableServiceRecovery(r => { r.RestartService(1); });
            //                });

            Application.Run(new Config());

        }
    }
}
