using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using monitoR.Service.ObjectInterfaces;
using Microsoft.Management.Infrastructure;
using Newtonsoft.Json;

namespace monitoR.Service.Utilities
{
    static class Powershell
    {

        public static IEnumerable<StoragePool> GetStoragePool() {
            return ExecutePS("get-storagepool")
                .Where(f => f != null && (f.BaseObject is CimInstance))
                .Select(f => new StoragePool((CimInstance)f.BaseObject))
                ;
        }

        public static IEnumerable<PSObject> ExecutePS(string script) {
            // TODO: some kind of logging here

            using (PowerShell powerShell = PowerShell.Create()) {
                // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
                //powerShell.AddScript("param($param1) $d = get-date; $s = 'test string value'; $d; $s; $param1; get-service");
                powerShell.AddScript(script);

                // use "AddParameter" to add a single parameter to the last command/script on the pipeline.
                //powerShell.AddParameter("param1", "parameter 1 value!");

                // invoke execution on the pipeline (collecting output)
                Collection<PSObject> psOutput = powerShell.Invoke();

                // check the other output streams (for example, the error stream)
                if (powerShell.Streams.Error.Count > 0) {
                    // error records were written to the error stream.
                    // do something with the items found.
                }

                return psOutput;

                //// loop through each output object item
                //foreach (PSObject outputItem in psOutput) {
                //    // if null object was dumped to the pipeline during the script then a null
                //    // object may be present here. check for null to prevent potential NRE.
                //    if (outputItem != null) {

                //        var cimObject = outputItem.BaseObject as CimInstance;
                //        if (cimObject != null) {
                //            WriteLine("CimInstance");
                //            WriteLine("Properties:");
                //            cimObject.CimInstanceProperties.All(f => {
                //                WriteLine(JsonConvert.SerializeObject(f));
                //                return true;
                //            });
                //            //WriteLine("Properties:");
                //            //cimObject.CimClass.CimClassProperties.All(f => {
                //            //                                              WriteLine(JsonConvert.SerializeObject(f));
                //            //                                              return true;
                //            //                                          });

                //        } else {
                //            //TODO: do something with the output item 
                //            WriteLine(outputItem.BaseObject.GetType().FullName);
                //            WriteLine(JsonConvert.SerializeObject(outputItem.BaseObject) + "\n");

                //        }

                //        // outputItem.BaseOBject
                //    }
                //}
            }
        }


    }
}
