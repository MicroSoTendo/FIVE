// ----------------------------------------------------------------------------
// <copyright file="RegionHandler.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   The RegionHandler class provides methods to ping a list of regions,
//   to find the one with best ping.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif

#if UNITY_WEBGL
#define PING_VIA_COROUTINE
#endif

namespace Photon.Realtime
{
    using System;
    using System.Text;
    using System.Net;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ExitGames.Client.Photon;

#if SUPPORTED_UNITY
    using UnityEngine;
    using Debug = UnityEngine.Debug;
#endif
#if SUPPORTED_UNITY || NETFX_CORE
    using SupportClass = ExitGames.Client.Photon.SupportClass;
#endif

    /// <summary>
    /// Provides methods to work with Photon's regions (Photon Cloud) and can be use to find the one with best ping.
    /// </summary>
    /// <remarks>
    /// When a client uses a Name Server to fetch the list of available regions, the LoadBalancingClient will create a RegionHandler
    /// and provide it via the OnRegionListReceived callback.
    ///
    /// Your logic can decide to either connect to one of those regional servers, or it may use PingMinimumOfRegions to test
    /// which region provides the best ping.
    ///
    /// It makes sense to make clients "sticky" to a region when one gets selected.
    /// This can be achieved by storing the SummaryToCache value, once pinging was done.
    /// When the client connects again, the previous SummaryToCache helps limiting the number of regions to ping.
    /// In best case, only the previously selected region gets re-pinged and if the current ping is not much worse, this one region is used again.
    /// </remarks>
    public class RegionHandler
    {
        /// <summary>A list of region names for the Photon Cloud. Set by the result of OpGetRegions().</summary>
        /// <remarks>
        /// Implement ILoadBalancingCallbacks and register for the callbacks to get OnRegionListReceived(RegionHandler regionHandler).
        /// You can also put a "case OperationCode.GetRegions:" into your OnOperationResponse method to notice when the result is available.
        /// </remarks>
        public List<Region> EnabledRegions { get; protected internal set; }

        private string availableRegionCodes;

        private Region bestRegionCache;

        /// <summary>
        /// When PingMinimumOfRegions was called and completed, the BestRegion is identified by best ping.
        /// </summary>
        public Region BestRegion
        {
            get
            {
                if (EnabledRegions == null)
                {
                    return null;
                }
                if (bestRegionCache != null)
                {
                    return bestRegionCache;
                }

                EnabledRegions.Sort((a, b) => { return (a.Ping == b.Ping) ? 0 : (a.Ping < b.Ping) ? -1 : 1; });

                bestRegionCache = EnabledRegions[0];
                return bestRegionCache;
            }
        }

        /// <summary>
        /// This value summarizes the results of pinging the currently available EnabledRegions (after PingMinimumOfRegions finished).
        /// </summary>
        public string SummaryToCache
        {
            get
            {
                if (BestRegion != null)
                {
                    return BestRegion.Code + ";" + BestRegion.Ping + ";" + availableRegionCodes;
                }

                return availableRegionCodes;
            }
        }

        public string GetResults()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Region Pinging Result: {0}\n", BestRegion.ToString());
            if (pingerList != null)
            {
                foreach (RegionPinger region in pingerList)
                {
                    sb.AppendFormat(region.GetResults() + "\n");
                }
            }

            sb.AppendFormat("Previous summary: {0}", previousSummaryProvided);
            return sb.ToString();
        }

        public void SetRegions(OperationResponse opGetRegions)
        {
            if (opGetRegions.OperationCode != OperationCode.GetRegions)
            {
                return;
            }
            if (opGetRegions.ReturnCode != ErrorCode.Ok)
            {
                return;
            }

            string[] regions = opGetRegions[ParameterCode.Region] as string[];
            string[] servers = opGetRegions[ParameterCode.Address] as string[];
            if (regions == null || servers == null || regions.Length != servers.Length)
            {
                //TODO: log error
                //Debug.LogError("The region arrays from Name Server are not ok. Must be non-null and same length. " + (regions == null) + " " + (servers == null) + "\n" + opGetRegions.ToStringFull());
                return;
            }

            bestRegionCache = null;
            EnabledRegions = new List<Region>(regions.Length);

            for (int i = 0; i < regions.Length; i++)
            {
                Region tmp = new Region(regions[i], servers[i]);
                if (string.IsNullOrEmpty(tmp.Code))
                {
                    continue;
                }

                EnabledRegions.Add(tmp);
            }

            Array.Sort(regions);
            availableRegionCodes = string.Join(",", regions);
        }

        private List<RegionPinger> pingerList;
        private Action<RegionHandler> onCompleteCall;
        private int previousPing;
        public bool IsPinging { get; private set; }
        private string previousSummaryProvided;

        public bool PingMinimumOfRegions(Action<RegionHandler> onCompleteCallback, string previousSummary)
        {
            if (EnabledRegions == null || EnabledRegions.Count == 0)
            {
                //TODO: log error
                //Debug.LogError("No regions available. Maybe all got filtered out or the AppId is not correctly configured.");
                return false;
            }

            if (IsPinging)
            {
                //TODO: log warning
                //Debug.LogWarning("PingMinimumOfRegions() skipped, because this RegionHander is already pinging some regions.");
                return false;
            }

            IsPinging = true;
            onCompleteCall = onCompleteCallback;
            previousSummaryProvided = previousSummary;

            if (string.IsNullOrEmpty(previousSummary))
            {
                return PingEnabledRegions();
            }

            string[] values = previousSummary.Split(';');
            if (values.Length < 3)
            {
                return PingEnabledRegions();
            }

            bool secondValueIsInt = int.TryParse(values[1], out int prevBestRegionPing);
            if (!secondValueIsInt)
            {
                return PingEnabledRegions();
            }

            string prevBestRegionCode = values[0];
            string prevAvailableRegionCodes = values[2];


            if (string.IsNullOrEmpty(prevBestRegionCode))
            {
                return PingEnabledRegions();
            }
            if (string.IsNullOrEmpty(prevAvailableRegionCodes))
            {
                return PingEnabledRegions();
            }
            if (!availableRegionCodes.Equals(prevAvailableRegionCodes) || !availableRegionCodes.Contains(prevBestRegionCode))
            {
                return PingEnabledRegions();
            }
            if (prevBestRegionPing >= RegionPinger.PingWhenFailed)
            {
                return PingEnabledRegions();
            }

            // let's check only the preferred region to detect if it's still "good enough"
            previousPing = prevBestRegionPing;

            Region preferred = EnabledRegions.Find(r => r.Code.Equals(prevBestRegionCode));
            RegionPinger singlePinger = new RegionPinger(preferred, OnPreferredRegionPinged);
            singlePinger.Start();

            return true;
        }

        private void OnPreferredRegionPinged(Region preferredRegion)
        {
            if (preferredRegion.Ping > previousPing * 1.50f)
            {
                PingEnabledRegions();
            }
            else
            {
                IsPinging = false;
                onCompleteCall(this);
            }
        }


        private bool PingEnabledRegions()
        {
            if (EnabledRegions == null || EnabledRegions.Count == 0)
            {
                //TODO: log
                //Debug.LogError("No regions available. Maybe all got filtered out or the AppId is not correctly configured.");
                return false;
            }

            pingerList = new List<RegionPinger>();
            foreach (Region region in EnabledRegions)
            {
                RegionPinger rp = new RegionPinger(region, OnRegionDone);
                pingerList.Add(rp);
                rp.Start(); // TODO: check return value
            }

            return true;
        }

        private void OnRegionDone(Region region)
        {
            bestRegionCache = null;
            foreach (RegionPinger pinger in pingerList)
            {
                if (!pinger.Done)
                {
                    return;
                }
            }

            IsPinging = false;
            onCompleteCall(this);
        }
    }

    public class RegionPinger
    {
        public static int Attempts = 5;
        public static bool IgnoreInitialAttempt = true;
        public static int MaxMilliseconsPerPing = 800; // enter a value you're sure some server can beat (have a lower rtt)
        public static int PingWhenFailed = Attempts * MaxMilliseconsPerPing;

        private readonly Region region;
        private string regionAddress;
        public int CurrentAttempt = 0;

        public bool Done { get; private set; }
        private readonly Action<Region> onDoneCall;

        private PhotonPing ping;

        private List<int> rttResults;

#if PING_VIA_COROUTINE
        // for WebGL exports, a coroutine is used to run pings. this is done on a temporary game object/monobehaviour
        private MonoBehaviour coroutineMonoBehaviour;
#endif


        public RegionPinger(Region region, Action<Region> onDoneCallback)
        {
            this.region = region;
            this.region.Ping = PingWhenFailed;
            Done = false;
            onDoneCall = onDoneCallback;
        }

        private PhotonPing GetPingImplementation()
        {
            PhotonPing ping = null;

#if !NETFX_CORE
            if (LoadBalancingPeer.PingImplementation == typeof(PingMono))
            {
                ping = new PingMono(); // using this type explicitly saves it from IL2CPP bytecode stripping
            }
#endif
#if NATIVE_SOCKETS
            if (LoadBalancingPeer.PingImplementation == typeof(PingNativeDynamic))
            {
                ping = new PingNativeDynamic();
            }
#endif
#if UNITY_WEBGL
            if (LoadBalancingPeer.PingImplementation == typeof(PingHttp))
            {
                ping = new PingHttp();
            }
#endif

            if (ping == null)
            {
                ping = (PhotonPing)Activator.CreateInstance(LoadBalancingPeer.PingImplementation);
            }

            return ping;
        }


        public bool Start()
        {
            // all addresses for Photon region servers will contain a :port ending. this needs to be removed first.
            // PhotonPing.StartPing() requires a plain (IP) address without port or protocol-prefix (on all but Windows 8.1 and WebGL platforms).
            string address = region.HostAndPort;
            int indexOfColon = address.LastIndexOf(':');
            if (indexOfColon > 1)
            {
                address = address.Substring(0, indexOfColon);
            }
            regionAddress = ResolveHost(address);


            ping = GetPingImplementation();


            Done = false;
            CurrentAttempt = 0;
            rttResults = new List<int>(Attempts);

#if PING_VIA_COROUTINE
            GameObject go = new GameObject();
            go.name = "RegionPing_" + this.region.Code + "_" + this.region.Cluster;
            this.coroutineMonoBehaviour = go.AddComponent<MonoBehaviourEmpty>();        // is defined below, as special case for Unity WegGL
            this.coroutineMonoBehaviour.StartCoroutine(this.RegionPingCoroutine());
#else
#if UNITY_SWITCH
            SupportClass.StartBackgroundCalls(this.RegionPingThreaded, 0);
#else
            SupportClass.StartBackgroundCalls(RegionPingThreaded, 0, "RegionPing_" + region.Code + "_" + region.Cluster);
#endif
#endif

            return true;
        }

        protected internal bool RegionPingThreaded()
        {
            region.Ping = PingWhenFailed;

            float rttSum = 0.0f;
            int replyCount = 0;


            Stopwatch sw = new Stopwatch();
            for (CurrentAttempt = 0; CurrentAttempt < Attempts; CurrentAttempt++)
            {
                bool overtime = false;
                sw.Reset();
                sw.Start();

                try
                {
                    ping.StartPing(regionAddress);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("RegionPinger.RegionPingThreaded() catched an exception for ping.StartPing(). Exception: " + e + " Source: " + e.Source + " Message: " + e.Message);
                    break;
                }


                while (!ping.Done())
                {
                    if (sw.ElapsedMilliseconds >= MaxMilliseconsPerPing)
                    {
                        overtime = true;
                        break;
                    }
#if !NETFX_CORE
                    System.Threading.Thread.Sleep(0);
#endif
                }


                sw.Stop();
                int rtt = (int)sw.ElapsedMilliseconds;
                rttResults.Add(rtt);

                if (IgnoreInitialAttempt && CurrentAttempt == 0)
                {
                    // do nothing.
                }
                else if (ping.Successful && !overtime)
                {
                    rttSum += rtt;
                    replyCount++;
                    region.Ping = (int)((rttSum) / replyCount);
                }

#if !NETFX_CORE
                System.Threading.Thread.Sleep(10);
#endif
            }

            //Debug.Log("Done: "+ this.region.Code);
            Done = true;
            ping.Dispose();

            onDoneCall(region);

            return false;
        }


#if SUPPORTED_UNITY
        /// <remarks>
        /// Affected by frame-rate of app, as this Coroutine checks the socket for a result once per frame.
        /// </remarks>
        protected internal IEnumerator RegionPingCoroutine()
        {
            region.Ping = PingWhenFailed;

            float rttSum = 0.0f;
            int replyCount = 0;


            Stopwatch sw = new Stopwatch();
            for (CurrentAttempt = 0; CurrentAttempt < Attempts; CurrentAttempt++)
            {
                bool overtime = false;
                sw.Reset();
                sw.Start();

                try
                {
                    ping.StartPing(regionAddress);
                }
                catch (Exception e)
                {
                    Debug.Log("catched: " + e);
                    break;
                }


                while (!ping.Done())
                {
                    if (sw.ElapsedMilliseconds >= MaxMilliseconsPerPing)
                    {
                        overtime = true;
                        break;
                    }
                    yield return 0; // keep this loop tight, to avoid adding local lag to rtt.
                }


                sw.Stop();
                int rtt = (int)sw.ElapsedMilliseconds;
                rttResults.Add(rtt);


                if (IgnoreInitialAttempt && CurrentAttempt == 0)
                {
                    // do nothing.
                }
                else if (ping.Successful && !overtime)
                {
                    rttSum += rtt;
                    replyCount++;
                    region.Ping = (int)((rttSum) / replyCount);
                }

                yield return new WaitForSeconds(0.1f);
            }


#if PING_VIA_COROUTINE
            GameObject.Destroy(this.coroutineMonoBehaviour.gameObject);   // this method runs as coroutine on a temp object, which gets destroyed now.
#endif

            //Debug.Log("Done: "+ this.region.Code);
            Done = true;
            onDoneCall(region);
            yield return null;
        }
#endif

        public string GetResults()
        {
            return string.Format("{0}: {1} ({2})", region.Code, region.Ping, rttResults.ToStringFull());
        }

        /// <summary>
        /// Attempts to resolve a hostname into an IP string or returns empty string if that fails.
        /// </summary>
        /// <remarks>
        /// To be compatible with most platforms, the address family is checked like this:<br/>
        /// if (ipAddress.AddressFamily.ToString().Contains("6")) // ipv6...
        /// </remarks>
        /// <param name="hostName">Hostname to resolve.</param>
        /// <returns>IP string or empty string if resolution fails</returns>
        public static string ResolveHost(string hostName)
        {

            if (hostName.StartsWith("wss://"))
            {
                hostName = hostName.Substring(6);
            }
            if (hostName.StartsWith("ws://"))
            {
                hostName = hostName.Substring(5);
            }

            string ipv4Address = string.Empty;

            try
            {
#if UNITY_WSA || NETFX_CORE || UNITY_WEBGL
                return hostName;
#else
                IPAddress[] address = Dns.GetHostAddresses(hostName);
                if (address.Length == 1)
                {
                    return address[0].ToString();
                }

                // if we got more addresses, try to pick a IPv6 one
                // checking ipAddress.ToString() means we don't have to import System.Net.Sockets, which is not available on some platforms (Metro)
                for (int index = 0; index < address.Length; index++)
                {
                    IPAddress ipAddress = address[index];
                    if (ipAddress != null)
                    {
                        if (ipAddress.ToString().Contains(":"))
                        {
                            return ipAddress.ToString();
                        }
                        if (string.IsNullOrEmpty(ipv4Address))
                        {
                            ipv4Address = address.ToString();
                        }
                    }
                }
#endif
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("RegionPinger.ResolveHost() catched an exception for Dns.GetHostAddresses(). Exception: " + e + " Source: " + e.Source + " Message: " + e.Message);
            }

            return ipv4Address;
        }
    }

#if PING_VIA_COROUTINE
    internal class MonoBehaviourEmpty : MonoBehaviour { }
#endif
}