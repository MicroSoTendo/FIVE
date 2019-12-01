namespace FIVE.Network.Lobby
{
    /// <summary>
    /// Used for communicating with list server.
    /// </summary>
    public enum LobbySyncHeader : byte
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
