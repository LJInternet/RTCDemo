using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    public class HPMarshaller : Marshallable
    {


        public HPMarshaller() : base(false)
        {
        }


        /** 
         * 此基类的子类的marshall操作都使用HPmarshall函数, buffer内存作为参数传进来
         * 如果直接使用marshall参数, 则会 产生崩溃. 
         */
        public byte[] HPmarshall()
        {
            mStream = new MemoryStream(ProtoConst.PROTO_PACKET_SIZE);
            mWriter = new BinaryWriter(mStream);
            mReader = new BinaryReader(mStream);
            return marshall();
        }
    }

}
