// ----------------------------------------------------------------------------
// <copyright file="SupportLogger.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Implements callbacks of the Realtime API to logs selected information
//   for support cases.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------



#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System.Text;
    using System.Collections.Generic;

    using Stopwatch = System.Diagnostics.Stopwatch;

#if SUPPORTED_UNITY
    using UnityEngine;
#endif

#if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

    /// <summary>
    /// Helper class to debug log basic information about Photon client and vital traffic statistics.
    /// </summary>
    /// <remarks>
    /// Set SupportLogger.Client for this to work.
    /// </remarks>
#if SUPPORTED_UNITY
    [AddComponentMenu("")] // hide from Unity Menus and searches
    public class SupportLogger : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ILobbyCallbacks
#else
	public class SupportLogger : IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks , ILobbyCallbacks
#endif
    {
        /// <summary>
        /// Toggle to enable or disable traffic statistics logging.
        /// </summary>
        public bool LogTrafficStats = true;
        private readonly bool loggedStillOfflineMessage;

        private LoadBalancingClient client;

        private Stopwatch startStopwatch;

        private int pingMax;
        private int pingMin;

        /// <summary>
        /// Photon client to log information and statistics from.
        /// </summary>
        public LoadBalancingClient Client
        {
            get => client;
            set
            {
                if (client != value)
                {
                    if (client != null)
                    {
                        client.RemoveCallbackTarget(this);
                    }
                    client = value;
                    client.AddCallbackTarget(this);
                }
            }
        }


#if SUPPORTED_UNITY
        protected void Start()
        {
            if (startStopwatch == null)
            {
                startStopwatch = new Stopwatch();
                startStopwatch.Start();
            }

            InvokeRepeating("TrackValues", 0.5f, 0.5f);
        }

        protected void OnApplicationPause(bool pause)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnApplicationPause: " + pause + " connected: " + client.IsConnected);
        }

        protected void OnApplicationQuit()
        {
            CancelInvoke();
        }
#endif

        public void StartLogStats()
        {
#if SUPPORTED_UNITY
            InvokeRepeating("LogStats", 10, 10);
#else
            Debug.Log("Not implemented for non-Unity projects.");
#endif
        }

        public void StopLogStats()
        {
#if SUPPORTED_UNITY
            CancelInvoke("LogStats");
#else
            Debug.Log("Not implemented for non-Unity projects.");
#endif
        }


        private string GetFormattedTimestamp()
        {
            if (startStopwatch == null)
            {
                startStopwatch = new Stopwatch();
                startStopwatch.Start();
            }
            return string.Format("[{0}.{1}]", startStopwatch.Elapsed.Seconds, startStopwatch.Elapsed.Milliseconds);
        }


        // called via InvokeRepeatedly
        private void TrackValues()
        {
            int currentRtt = client.LoadBalancingPeer.RoundTripTime;
            if (currentRtt > pingMax)
            {
                pingMax = currentRtt;
            }
            if (currentRtt < pingMin)
            {
                pingMin = currentRtt;
            }
        }


        /// <summary>
        /// Debug logs vital traffic statistics about the attached Photon Client.
        /// </summary>
        public void LogStats()
        {
            if (client.State == ClientState.PeerCreated)
            {
                return;
            }

            if (LogTrafficStats)
            {
                Debug.Log(GetFormattedTimestamp() + " SupportLogger " + client.LoadBalancingPeer.VitalStatsToString(false) + " Ping min/max: " + pingMin + "/" + pingMax);
            }
        }

        /// <summary>
        /// Debug logs basic information (AppId, AppVersion, PeerID, Server address, Region) about the attached Photon Client.
        /// </summary>
        private void LogBasics()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0} SupportLogger Info: ", GetFormattedTimestamp());
            sb.AppendFormat("AppID: \"{0}\" AppVersion: \"{1}\" PeerID: {2} ",
                string.IsNullOrEmpty(client.AppId) || client.AppId.Length < 8
                    ? client.AppId
                    : string.Concat(client.AppId.Substring(0, 8), "***"), client.AppVersion, client.LoadBalancingPeer.PeerID);
            //NOTE: this.client.LoadBalancingPeer.ServerIpAddress requires Photon3Unity3d.dll v4.1.2.5 and up
            sb.AppendFormat("NameServer: {0} Server: {1} IP: {2} Region: {3}", client.NameServerHost, client.CurrentServerAddress, client.LoadBalancingPeer.ServerIpAddress, client.CloudRegion);

            Debug.Log(sb.ToString());
        }


        public void OnConnected()
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnConnected().");
            pingMax = 0;
            pingMin = client.LoadBalancingPeer.RoundTripTime;
            LogBasics();

            if (LogTrafficStats)
            {
                client.LoadBalancingPeer.TrafficStatsEnabled = false;
                client.LoadBalancingPeer.TrafficStatsEnabled = true;
                StartLogStats();
            }
        }

        public void OnConnectedToMaster()
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnConnectedToMaster().");
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnFriendListUpdate(friendList).");
        }

        public void OnJoinedLobby()
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnJoinedLobby(" + client.CurrentLobby + ").");
        }

        public void OnLeftLobby()
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnLeftLobby().");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnCreateRoomFailed(" + returnCode + "," + message + ").");
        }

        public void OnJoinedRoom()
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnJoinedRoom(" + client.CurrentRoom + "). " + client.CurrentLobby + " GameServer:" + client.GameServerAddress);
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnJoinRandomFailed(" + returnCode + "," + message + ").");
        }

        public void OnCreatedRoom()
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnCreatedRoom(" + client.CurrentRoom + "). " + client.CurrentLobby + " GameServer:" + client.GameServerAddress);
        }

        public void OnLeftRoom()
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnLeftRoom().");
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            StopLogStats();

            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnDisconnected(" + cause + ").");
            LogBasics();
            LogStats();
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnRegionListReceived(regionHandler).");
            LogBasics();
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnRoomListUpdate(roomList). roomList.Count: " + roomList.Count);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnPlayerEnteredRoom(" + newPlayer + ").");
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnPlayerLeftRoom(" + otherPlayer + ").");
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnRoomPropertiesUpdate(propertiesThatChanged).");
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnPlayerPropertiesUpdate(targetPlayer,changedProps).");
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnMasterClientSwitched(" + newMasterClient + ").");
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationResponse(" + data.ToStringFull() + ").");
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationFailed(" + debugMessage + ").");
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            Debug.Log(GetFormattedTimestamp() + " SupportLogger OnLobbyStatisticsUpdate(lobbyStatistics).");
        }


#if !SUPPORTED_UNITY
        private static class Debug
        {
            public static void Log(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
            public static void LogWarning(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
            public static void LogError(string msg)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
        }
#endif
    }
}