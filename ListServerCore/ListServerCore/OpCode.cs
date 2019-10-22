using System;

namespace ListServerCore
{
    [Flags]
    public enum OpCode
    {
        CreateRoom = 1,
        RemoveRoom = 1 << 1,
        UpdateRoom = 1 << 2,
        UpdateName = 1 << 3,
        UpdateCurrentPlayer = 1 << 4,
        UpdateMaxPlayer = 1 << 5,
        UpdatePassword = 1 << 6,
    }
}
