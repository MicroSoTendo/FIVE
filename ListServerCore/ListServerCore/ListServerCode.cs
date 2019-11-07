using System;

namespace ListServerCore
{
    [Flags]
    public enum ListServerCode : ushort
    {
        AliveTick = 1,
        GetRoomInfos = 1 << 1,
        CreateRoom = 1 << 2,
        RemoveRoom = 1 << 3,
        UpdateRoom = 1 << 4,
        UpdateName = 1 << 5,
        UpdateCurrentPlayer = 1 << 6,
        UpdateMaxPlayer = 1 << 7,
        UpdatePassword = 1 << 8
    }
}
