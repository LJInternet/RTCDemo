using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    public interface ILifecylce
    {
        void OnCreate();

        void OnDestroy();
    }
}
