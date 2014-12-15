using System;
using System.Linq;
using monitoR.Service.Interfaces;
using Microsoft.Management.Infrastructure;
using Ploeh.AutoFixture;

namespace monitoR.Service.DebugObjects
{

    class DebugStoragePool : IStoragePool
    {
        public DebugStoragePool() {
            //var fixture = new Fixture();
            //UniqueId = fixture.Create<Guid>();
            //FriendlyName = fixture.Create<string>();
            //IsPrimordial = fixture.Create<bool>();
            HealthStatus = RandomEnum<DiskHealthStatus>();
        }

        public Guid UniqueId { get; set; }

        public string FriendlyName { get; set; }

        public bool IsPrimordial { get; set; }

        public DiskHealthStatus HealthStatus { get; private set; }



        private Random rand = new Random();
        private T RandomEnum<T>() {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[rand.Next(0, values.Length)];
        }
    }
}
