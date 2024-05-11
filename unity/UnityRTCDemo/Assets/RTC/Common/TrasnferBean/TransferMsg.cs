using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    internal class TransferMsg : HPMarshaller
    {

        private int value;

        public TransferMsg(int value)
        {
            this.value = value;
        }


        public override byte[] marshall()
        {
            pushInt(value);

            return base.marshall();
        }
    }
}
