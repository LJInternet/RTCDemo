
using System;
using System.Collections.Generic;
using System.IO;

namespace LJ.RTC.Common
{

	public class Marshallable : IProtoPacket
	{


		private static string TAG = "Marshallable";

		/**
		 * 
		 * @ClassName: ELenType 
		 * @Description: 长度类型，用于表示String/byte[]等（需要先压入/弹出长度short/int，再压入/弹出实际数据）的长度类型。
		 * 					E_SHORT代表short，E_INT代表int，E_NONE表示不需要使用到长度。
		 *
		 * for example
		 * if (lenType == ELenType.E_SHORT) {
		 *   	pushString16((String)elem);
		 * } else if (lenType == ELenType.E_INT) {
		 *   	pushString32((String)elem);
		 * } else {
		 *   	YLog.warn(TAG, "invalid lenType=%d for pushString", lenType);
		 * }
		 * 
		 */
		public enum ELenType
		{
			E_SHORT,
			E_INT,
			E_NONE
		}

		protected MemoryStream mStream;
		protected BinaryWriter mWriter;
		protected BinaryReader mReader;

		public Marshallable()
		{

			mStream = new MemoryStream();
			mWriter = new BinaryWriter(mStream);
			mReader = new BinaryReader(mStream);
		}

		public Marshallable(bool bAllocBuff)
		{
			if (bAllocBuff)
			{
				mStream = new MemoryStream(ProtoConst.PROTO_PACKET_SIZE);
				mWriter = new BinaryWriter(mStream);
				mReader = new BinaryReader(mStream);
			}
		}

		public Marshallable(int buffSize)
		{
			mStream = new MemoryStream(buffSize);
			mWriter = new BinaryWriter(mStream);
			mReader = new BinaryReader(mStream);
		}


		public virtual byte[] marshall()
		{
			return mStream.ToArray();
		}


		public virtual void unmarshall(byte[] buf)
		{
			mStream = new MemoryStream(buf);
			mWriter = new BinaryWriter(mStream);
			mReader = new BinaryReader(mStream);
		}


		public void pushBool(bool val)
		{
			byte b = 0;
			if (val)
				b = 1;

			check_capactiy(1);
			mWriter.Write(b);
		}

		public bool popBool()
		{
			byte b = mReader.ReadByte();
			return (b == 1);
		}

		public void pushByte(byte val)
		{
			check_capactiy(1);
			mWriter.Write(val);
		}

		public byte popByte()
		{
			return mReader.ReadByte();
		}

		public void pushBytes(byte[] buf)
		{
			if (buf == null)
			{
				check_capactiy(2);
				mWriter.Write((short)0);
			}
			else
			{
				check_capactiy(2 + buf.Length);
				mWriter.Write((short)buf.Length);
				mWriter.Write(buf);
			}
		}

		public byte[] popBytes()
		{
			int len = mReader.ReadInt16();
			byte[] buf = null;
			if (len >= 0)
			{
				buf = new byte[len];
				mReader.Read(buf);
			}
			return buf;
		}

		public void pushBytes32(byte[] buf)
		{
			if (buf == null)
			{
				check_capactiy(4);
				mWriter.Write(0);
			}
			else
			{
				check_capactiy(4 + buf.Length);
				mWriter.Write(buf.Length);
				mWriter.Write(buf);
			}
		}

		public byte[] popBytes32()
		{
			int len = mReader.ReadInt32();
			byte[] buf = null;
			if (len >= 0)
			{
				buf = new byte[len];
				mReader.Read(buf);
			}

			return buf;
		}

		public byte[] popAll()
		{
			long len = mStream.Length;
			byte[] buf = new byte[len];
			mReader.Read(buf);
			return buf;
		}

		public void pushShort(short val)
		{
			check_capactiy(2);
			mWriter.Write(val);
		}

		public short popShort()
		{
			return mReader.ReadInt16();
		}

		public void pushInt(long val)
		{
			check_capactiy(4);
			mWriter.Write((int)val);
		}

		public int popInt()
		{
			return mReader.ReadInt32();
		}

		public void pushInt64(long val)
		{
			check_capactiy(8);
			mWriter.Write(val);
		}

		public long popInt64()
		{
			return mReader.ReadInt64();
		}

		public void pushString16(string val)
		{
			if (val == null)
			{
				check_capactiy(2);
				mWriter.Write((short)0);
				return;
			}
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(val);
			int size = bytes.Length;
			check_capactiy(2 + size);
			pushShort((short)size);
			if (val.Length > 0)
			{
				mWriter.Write(bytes);
			}
		}

		public string popString16()
		{
			short len = mReader.ReadInt16();
			byte[] buf = null;
			if (len >= 0)
			{
				buf = new byte[len];
				mReader.Read(buf);
				try
				{
					return System.Text.Encoding.UTF8.GetString(buf); // ansi
				}
				catch (Exception e)
				{
				}
			}

			return "";
		}


		public void pushIntArray(long[] vals)
		{
			if (vals == null)
			{
				pushInt(0);
				return;
			}

			int len = vals.Length;
			pushInt(len);
			for (int i = 0; i < len; i++)
			{
				pushInt(vals[i]);
			}
		}


		public int[] popIntArray()
		{
			int len = popInt();
			int[] vals = new int[len];
			for (int i = 0; i < len; i++)
			{
				vals[i] = popInt();
			}

			return vals;
		}

		public void pushShortArray(short[] vals)
		{
			if (vals == null)
			{
				pushInt(0);
				return;
			}

			int len = vals.Length;
			pushInt(len);
			for (int i = 0; i < len; i++)
			{
				pushShort(vals[i]);
			}
		}

		public short[] popShortArray()
		{
			int len = popInt();
			short[] vals = new short[len];
			for (int i = 0; i < len; i++)
			{
				vals[i] = popShort();
			}

			return vals;
		}

		public String popString32()
		{
			int len = mReader.ReadInt32();
			byte[] buf = null;
			if (len >= 0)
			{
				buf = new byte[len];
				mReader.Read(buf);
				try
				{
					string result = System.Text.Encoding.UTF8.GetString(buf);
					return result; // ansi
				}
				catch (Exception e)
				{

				}
			}

			return "";
		}

		public String popString16(String charsetName)
		{
			short len = mReader.ReadInt16();
			byte[] buf = null;
			if (len >= 0)
			{
				buf = new byte[len];
				mReader.Read(buf);
				try
				{
					return System.Text.Encoding.UTF8.GetString(buf);
				}
				catch (Exception e)
				{

				}
			}

			return "";
		}

		public void pushString32(String val)
		{
			if (val == null)
			{
				check_capactiy(4);
				mWriter.Write(0);
				return;
			}
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(val);
			int size = bytes.Length;
			check_capactiy(4 + size);
			mWriter.Write(size);
			if (size > 0)
			{
				mWriter.Write(bytes);
			}
		}


		protected void check_capactiy(int size)
		{
			if (mStream.Capacity - mStream.Position < size)
			{
				increase_capacity(size - (int)(mStream.Capacity - mStream.Position));
			}
		}

		protected void increase_capacity(int minIncrement)
		{
			int capacity = mStream.Capacity;

			if (capacity == 0)
			{
				return;
			}

			int size = 2 * capacity;
			if (minIncrement > capacity)
			{
				size = capacity + minIncrement;
			}
			MemoryStream tempStream = new MemoryStream(size);
			mWriter = new BinaryWriter(tempStream);
			mReader = new BinaryReader(tempStream);
			mWriter.Write(mStream.ToArray());
			mStream = tempStream;

			//Log.i(TAG, "increase_capacity, size="+size);
		}

		public void pushMarshallable(Marshallable val)
		{
			if (val != null)
			{
				val.marshall(mStream, mWriter, mReader);
			}
		}

		public T popMarshallable<T>() where T : Marshallable
		{
			Marshallable val = null;
			try
			{
				Type type = typeof(T);
				val = (T)Activator.CreateInstance(type);
			}
			catch (Exception e)
			{

			}
			val.unmarshall(mStream, mWriter, mReader);
			return (T)val;
		}

		public virtual void marshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
		{
			mStream = stream;
			mReader = reader;
			mWriter = writer;
		}

		public virtual void unmarshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
		{
			mStream = stream;
			mReader = reader;
			mWriter = writer;
		}

		public  void pushCollection<T>(List<T> data,  ELenType lenType)
		{
			if (data == null || data.Count == 0)
			{
				pushInt(0);
			}
			else
			{
				pushInt(data.Count);
				foreach (T elem in data)
				{
					pushElem<T>(elem, lenType);
				}
			}
		}

		private  void pushElem<T>(object elem, ELenType lenType)
		{
			if (elem is int)
			{
					pushInt((int)elem);
			}
			else if (elem is short)
			{
				pushShort((short) elem);
			} 
			else if (elem is long) 
			{
				pushInt64((long)elem);
			} 
			else if (elem is byte)
			{
				pushByte((byte)elem);
			}
			else if (elem is string) {
				if (lenType == ELenType.E_SHORT)
				{
					pushString16((String)elem);
				}
				else if (lenType == ELenType.E_INT)
				{
					pushString32((String)elem);
				}
				else
				{
					JLog.Info(TAG, "invalid for pushString  lenType=" + lenType);
				}
			}
			else if (elem is byte[]) 
			{
				if (lenType == ELenType.E_SHORT)
				{
					pushBytes((byte[])elem);
				}
				else if (lenType == ELenType.E_INT)
				{
					pushBytes32((byte[])elem);
				}
				else
				{
					JLog.Info(TAG, "invalid lenType= for pushBytes" + lenType);
				}
			} 
			else if (elem is Marshallable)	
			{
				((Marshallable)elem).marshall(mStream, mWriter, mReader);
				//	byte[] tmp = ((Marshallable)elem).marshall();
				//	mBuffer.put(tmp);	//TODO check if right
			}
			else
			{
				throw new Exception("unable to marshal element of class " + elem.GetType().Name);
			}
		}

		private T popElem<T>(ELenType lenType)
		{
			Type type = typeof(T);
			object elem = null;
			if (type == typeof(int))
			{
				elem = popInt();
			}
			else if (type == typeof(long))
			{
				elem = popInt64();
			}
			else if (type == typeof(short))
			{
				elem = popShort();
			}
			else if (type == typeof(byte))
			{
				elem = popByte();
			}
			else if (type == typeof(string))
			{
				if (lenType == ELenType.E_SHORT)
				{
					elem = popString16();
				}
				else if (lenType == ELenType.E_INT)
				{
					elem = popString32();
				}
			}
			else if (type == typeof(byte[]))
			{
				elem = popBytes();
			}
			else
			{
				try
				{
					elem = Activator.CreateInstance(type);
				}
				catch (Exception e)
				{

				}
				if (elem is Marshallable) {
					((Marshallable)elem).unmarshall(mStream, mWriter, mReader);
				} else
				{
					JLog.Error("TAG", "unmarshall invalid elemClass type= " +  elem.ToString());
				}
			}
			return (T)elem;
		}

		public List<T> popCollection<T>(ELenType lenType)
		{
			int size = popInt();
			//		if (size > InvalidProtocolData.MAX_PROTO_ELEMENT_COUNT) {
			//			throw new InvalidProtocolData(InvalidProtocolData.ERROR_LEN);
			//		}
			List<T> list = new List<T>();
			if (list == null)
			{
				return null;
			}
			for (int i = 0; i < size; i++)
			{
				object elem = null;
				elem = popElem<T>(lenType);
				list.Add((T)elem);
			}
			return list;
		}

	}
}
