using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitoR.Service.Interfaces;
using Quartz;
using monitoR.Service.Notifier;
using PersistentObjectCachenet45;

namespace monitoR.Service.Jobs
{
    class CheckStoragePoolHealth : IJob
    {
        private readonly Func<IEnumerable<IStoragePool>> _getStoragePool;
        private readonly INotifier _notification;
 
        public CheckStoragePoolHealth(Func<IEnumerable<IStoragePool>> getStoragePool, INotifier notification) {
            _getStoragePool = getStoragePool;
            _notification = notification;
        }

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler"/> when a <see cref="T:Quartz.ITrigger"/>
        ///             fires that is associated with the <see cref="T:Quartz.IJob"/>.
        /// </summary>
        /// <remarks>
        /// The implementation may wish to set a  result object on the 
        ///             JobExecutionContext before this method exits.  The result itself
        ///             is meaningless to Quartz, but may be informative to 
        ///             <see cref="T:Quartz.IJobListener"/>s or 
        ///             <see cref="T:Quartz.ITriggerListener"/>s that are watching the job's 
        ///             execution.
        /// </remarks>
        /// <param name="context">The execution context.</param>
        public void Execute(IJobExecutionContext context) {
            Console.WriteLine("Execute");

            var storagePools = _getStoragePool();

            var isAnyUnhealthy = storagePools.Any(f => {
                Console.WriteLine("Val: " + f.HealthStatus.ToString());
                                                      return !f.IsPrimordial && f.HealthStatus != DiskHealthStatus.Healthy;
                                                  });

            context.Put("anyUnhealthy", isAnyUnhealthy);
            context.Result = isAnyUnhealthy;


            var previousStatus = PersistentObjectCache.ContainsAsync<StoragePoolHealthStatus>("StoragePoolHealthStatus").Result;
            bool hasStatusChanged = false;
            if (previousStatus) {
                Console.WriteLine("Previous State");
                var status = PersistentObjectCache.GetObjectAsync<StoragePoolHealthStatus>("StoragePoolHealthStatus", true).Result;
                if (status.IsFailureState != isAnyUnhealthy) hasStatusChanged = true;
            } else {
                hasStatusChanged = true;
            }

            // Respond to a change
            if (hasStatusChanged) {
                Console.WriteLine("Status Changed");
                var newStatus = new StoragePoolHealthStatus { IsFailureState = isAnyUnhealthy };
                PersistentObjectCache.SetObjectAsync("StoragePoolHealthStatus", newStatus);

                _notification.SendNotification("stateChange", new PoolStateChangeModel { IsFailureState = isAnyUnhealthy });
            }
        }
    }

    class StoragePoolHealthStatus
    {
        public bool IsFailureState { get; set; }

    }

    public class PoolStateChangeModel : IPoolStateChangeModel
    {

        public DiskHealthStatus NewHealthStatus { get; set; }

        public bool IsFailureState { get; set; }
    }

}
