using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC
{
    public class LJOptional<T>
    {
        private T value;
        private bool hasValue;

        public LJOptional()
        {
            hasValue = false;
        }

        public bool HasValue()
        {
            return hasValue;
        }

        public T GetValue()
        {
            return this.value;
        }

        public void SetValue(T val)
        {
            this.hasValue = true;
            this.value = val;
        }

        public void SetEmpty()
        {
            this.hasValue = false;
        }

    }

}
