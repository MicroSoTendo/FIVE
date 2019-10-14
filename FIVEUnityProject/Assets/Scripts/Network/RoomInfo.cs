using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIVE.Network
{
    public struct RoomInfo
    {
        public RoomInfo(string name, int size, string password)
        {
            Password = password;
            Name = name;
            Size = size;
        }

        public string Name { get; }
        public int Size { get; }
        public string Password { get; }
        
    }
}
