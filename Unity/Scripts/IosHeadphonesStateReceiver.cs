using System.Runtime.InteropServices;
using UnityEngine;



#if UNITY_IOS && !UNITY_EDITOR
namespace HeadphonesDetectorPlugin
{
    /// <summary>
    /// Receiving messages about headphones plug from ios device
    /// Created receiver not enabled
    /// There can only be one receiver in program
    /// </summary>
    public class IosHeadphonesStateReceiver : HeadphonesStateReceiverBase
    {
        private delegate void MonoPMessageDelegate(string message, bool state);
        private static IosHeadphonesStateReceiver _instance;
        
        public override bool Enabled
        {
            get { return IsEnabled(); }
        }

        public override bool HeadphonesPlugged
        {
            get { return IsHeadphonesPlugged(); }
        }
        

        public IosHeadphonesStateReceiver()
        {
            _instance = this;
            Initialize();
        }

        public override void Enable(bool enable)
        {
            EnableReceiver(enable);
            Debug.LogFormat("<color=red>[headphonesSateReceiver] iOS Headphones State receiver enabled: {0}</color>", enable);
        }

        public override int GetDeviceVolume()
        {
            return GetDeviceSoundVolume();
        }

        private static void Initialize()
        {
            RegisterMessageHandler(OnMessage);
        }

        
#region iOS
        
        [AOT.MonoPInvokeCallback(typeof(MonoPMessageDelegate))]
        private static void OnMessage(string message, bool state)
        {
            _instance.Invoke(message, state);
        }
        
        [DllImport("__Internal")]
        private static extern void EnableReceiver(bool enable);

        [DllImport("__Internal")]
        private static extern bool IsEnabled(); 
        
        [DllImport("__Internal")]
        private static extern void RegisterMessageHandler(MonoPMessageDelegate messageDelegate);

        [DllImport("__Internal")]
        private static extern bool IsHeadphonesPlugged();

        [DllImport("__Internal")]
        private static extern int GetDeviceSoundVolume();

#endregion
    }
}
#endif
