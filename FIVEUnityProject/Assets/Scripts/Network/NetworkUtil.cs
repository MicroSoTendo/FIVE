using System;
using System.Linq;
using System.Text;

namespace FIVE.Network
{
    internal static class NetworkUtil
    {
        public static int ToI32(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToInt32(bytes, startIndex);
        }
        public static ushort ToU16(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToUInt16(bytes, startIndex);
        }
        public static bool ToBool(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToBoolean(bytes, startIndex);
        }

        public static Guid ToGuid(this byte[] bytes, int startIndex = 0)
        {
            byte[] guidBytes = new byte[16];
            Array.Copy(bytes, startIndex, guidBytes, 0, 16);
            return new Guid(guidBytes);
        }

        public static string ToName(this byte[] bytes, int startIndex = 0)
        {
            return Encoding.Unicode.GetString(bytes, startIndex, bytes.Length - startIndex);
        }

        public static RoomInfo ToRoomInfo(this byte[] bytes)
        {
            Guid guid = bytes.ToGuid();
            int currentPlayers = bytes.ToI32(16);
            int maxPlayers = bytes.ToI32(20);
            bool hasPassword = bytes.ToBool(24);
            int host = bytes.ToI32(25);
            ushort port = bytes.ToU16(29);
            string name = bytes.ToName(31);
            return new RoomInfo(guid,currentPlayers,maxPlayers,hasPassword,host,port,name);
        }



        public static byte[] ToBytes(this int i)
        {
            return BitConverter.GetBytes(i);
        }

        public static byte[] ToBytes(this string str)
        {
            return Encoding.Unicode.GetBytes(str);
        }

        public static byte[] ToBytes(this Guid guid)
        {
            return guid.ToByteArray();
        }

        public static byte[] ToBytes(this bool value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] ToBytes(this ushort value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] ToBytes(this RoomInfo roomInfo)
        {
            byte[] guid = roomInfo.Guid.ToBytes();
            byte[] currentPlayers = roomInfo.CurrentPlayers.ToBytes();
            byte[] maxPlayers = roomInfo.MaxPlayers.ToBytes();
            byte[] hasPassword = roomInfo.HasPassword.ToBytes();
            byte[] host = roomInfo.Host.ToBytes();
            byte[] port = roomInfo.Port.ToBytes();
            byte[] name = roomInfo.Name.ToBytes();
            return Combine(guid, currentPlayers, maxPlayers, hasPassword, host, port, name);
        }

        private static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
