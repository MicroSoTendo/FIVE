using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace ListServerCore
{
    internal static class Util
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ToI32(this byte[] bytes, int startIndex = 0)
        {
            fixed (byte* pbytes = &bytes[startIndex])
            {
                return *(int*)pbytes;
            }
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
            return new Guid(bytes[startIndex..(startIndex + 16)]);
        }

        public static string ToName(this byte[] bytes, int startIndex = 0)
        {
            return Encoding.Unicode.GetString(bytes[startIndex..]);
        }

        public static unsafe RoomInfo ToRoomInfo(this byte[] bytes, int host)
        {
            Guid guid = Guid.NewGuid();
            fixed (byte* pBuffer = bytes)
            {
                return new RoomInfo(
                    guid, 
                    *(int*)pBuffer, 
                    *(int*)(pBuffer + sizeof(int)), 
                    *(bool*)(pBuffer + sizeof(int) + sizeof(int)), 
                    host,
                    *(ushort*)(pBuffer + sizeof(int) + sizeof(int) + sizeof(bool)), 
                    Encoding.Unicode.GetString(pBuffer + sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(ushort), 
                        bytes.Length - sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(ushort))
                    );
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyFrom(this byte[] dest, byte[] source, int destStartIndex = 0)
        {
            Unsafe.CopyBlock(ref dest[destStartIndex], ref source[0], (uint)source.Length);
        }        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyFrom(this byte[] dest, byte[] source1, byte[] source2, int destStartIndex = 0)
        {
            dest.CopyFrom(source1, destStartIndex);
            dest.CopyFrom(source2, destStartIndex + source1.Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyFrom(this byte[] dest, byte[] source1, byte[] source2, byte[] source3, int destStartIndex = 0)
        {
            dest.CopyFrom(source1, source2, destStartIndex);
            dest.CopyFrom(source3, destStartIndex + source1.Length + source2.Length);
        }

        public static unsafe byte[] ToBytes(this RoomInfo roomInfo)
        {
            byte[] buffer = new byte[31 + Encoding.Unicode.GetByteCount(roomInfo.Name)];
            fixed (byte* pBuffer = buffer)
            {
                *(Guid*) pBuffer = roomInfo.Guid;
                *(int*)(pBuffer + 16) = roomInfo.CurrentPlayers;
                *(int*)(pBuffer + 20) = roomInfo.MaxPlayers;
                *(bool*)(pBuffer + 24) = roomInfo.HasPassword;
                *(int*)(pBuffer + 25) = roomInfo.Host;
                *(ushort*)(pBuffer + 29) = roomInfo.Port;
            }
            Encoding.Unicode.GetBytes(roomInfo.Name, 0, roomInfo.Name.Length, buffer, 31);
            return buffer;
        }

        public static unsafe T Read<T>(this NetworkStream stream) where T : unmanaged
        {
            byte[] buffer = new byte[sizeof(T)];
            stream.Read(buffer, 0, buffer.Length);
            fixed (byte* pBuffer = buffer)
            {
                return *(T*) pBuffer;
            }
        }        
        
        public static unsafe void ReadTo<T>(this NetworkStream stream, byte[] buffer, int offset = 0) where T : unmanaged
        {
            stream.Read(buffer, offset, sizeof(T));
        }
    }
}
