using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
            var guid = bytes.ToGuid();
            int currentPlayers = bytes.ToI32(16);
            int maxPlayers = bytes.ToI32(20);
            bool hasPassword = bytes.ToBool(24);
            int host = bytes.ToI32(25);
            ushort port = bytes.ToU16(29);
            string name = bytes.ToName(31);
            return new RoomInfo(guid, currentPlayers, maxPlayers, hasPassword, host, port, name);
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
            byte[] currentPlayers = roomInfo.CurrentPlayers.ToBytes();
            byte[] maxPlayers = roomInfo.MaxPlayers.ToBytes();
            byte[] hasPassword = roomInfo.HasPassword.ToBytes();
            byte[] host = roomInfo.Host.ToBytes();
            byte[] port = roomInfo.Port.ToBytes();
            byte[] name = roomInfo.Name.ToBytes();
            return Combine(currentPlayers, maxPlayers, hasPassword, host, port, name);
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

        public static void Write(this NetworkStream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void Write(this NetworkStream stream, int i)
        {
            stream.Write(i.ToBytes());
        }    
        
        public static void Write(this NetworkStream stream, bool b)
        {
            stream.Write(b.ToBytes());
        }

        public static void Write(this NetworkStream stream, Guid guid)
        {
            stream.Write(guid.ToBytes());
        }

        public static void Write(this NetworkStream stream, string str)
        {
            byte[] buffer = str.ToBytes();
            stream.Write(buffer.Length);
            stream.Write(buffer);
        }

        public static void Write(this NetworkStream stream, RoomInfo info)
        {
            byte[] buffer = info.ToBytes();
            stream.Write(buffer.Length);
            stream.Write(buffer);
        }

        public static void Read(this NetworkStream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, bytes.Length);
        }        
        
        public static byte[] Read(this NetworkStream stream, int size)
        {
            byte[] bytes = new byte[size];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
