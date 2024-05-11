using System.IO;

namespace LJ.RTC.Common
{
	public interface IProtoPacket
	{
		public byte[] marshall();
		public void unmarshall(byte[] buf);

		public void  marshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader);
		public void unmarshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader);
	}

	public class ProtoConst
	{
		public static int PROTO_TCP_BUFFER_SIZE = 256 * 1024;
		public static int PROTO_UDP_BUFFER_SIZE = 32 * 1024;
		public static int PROTO_PACKET_SIZE = 4 * 1024;
		public static int PROTO_VERSION = -1;
					  
		public static int INVALID_UID = -1;
		public static int INVALID_SID = -1;
					  
		public static int INVALID_EVENT = -1;
		public static int INVALID_REQ = -1;
					  
					  
		public static byte PLATFORM_ANDROID = 0;
		public static byte PLATFORM_IOS = 1;
		public static byte PLATFORM_WINPHONE = 2;
		public static byte PLATFORM_UNKNOWN = 127;
					  
		public static byte SYSNET_WIFI = 0;
		public static byte SYSNET_MOBILE = 1;
		public static byte SYSNET_DISCONNECT = 2;
		public static byte SYSNET_UNKNOWN = 127;
	}

}
