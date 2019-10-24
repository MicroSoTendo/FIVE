using System;
namespace FIVE.Network
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
        JoinRequest = 1 << 7,
        AcceptJoin = 1 << 8,
        RefuseJoin = 1 << 9,
    }
}
