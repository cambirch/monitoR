using System;
using System.Linq;
using monitoR.Service.Interfaces;
using Microsoft.Management.Infrastructure;

namespace monitoR.Service.PSObjects
{

    class StoragePool : IStoragePool
    {
        private readonly CimInstance _storagePoolInstance;
        public StoragePool(CimInstance storagePoolInstance) {
            _storagePoolInstance = storagePoolInstance;
        
        }

        public Guid UniqueId {
            get { return (Guid)_storagePoolInstance.CimInstanceProperties.First(f => f.Name == "UniqueId").Value; }
        }

        public string FriendlyName {
            get { return (string)_storagePoolInstance.CimInstanceProperties.First(f => f.Name == "FriendlyName").Value; }
        }

        public bool IsPrimordial {
            get { return (bool)_storagePoolInstance.CimInstanceProperties.First(f => f.Name == "IsPrimordial").Value; }
        }

        public DiskHealthStatus HealthStatus {
            get { return (DiskHealthStatus)Convert.ToInt32(_storagePoolInstance.CimInstanceProperties.First(f => f.Name == "HealthStatus").Value); }
        }

    }
}
