using System;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace monitoR.Service.Interfaces
{
    interface IStoragePool
    {
        Guid UniqueId { get; } 
        string FriendlyName { get; }
        bool IsPrimordial { get; }
        DiskHealthStatus HealthStatus { get; }
    }

}
