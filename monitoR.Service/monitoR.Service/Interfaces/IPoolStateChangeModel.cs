using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitoR.Service.Interfaces
{
    interface IPoolStateChangeModel
    {

        DiskHealthStatus NewHealthStatus { get; set; }

        bool IsFailureState { get; set; }


    }
}
