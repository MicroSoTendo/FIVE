using System;

namespace ListServerCore
{
    public enum ListServerHeader : byte
    {
        AliveTick = 1,
        RoomInfos = 2,
        AssignGuid = 3,
        CreateRoom = 4,
        RemoveRoom = 5,
        UpdateName = 6,
        UpdateCurrentPlayer = 7,
        UpdateMaxPlayer = 8,
        UpdatePassword = 9
    }
}
