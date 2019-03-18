using UnityEngine;



namespace HeadphonesDetectorPlugin
{
    /// <summary>
    /// Receiving messages about headphones plug from android device
    /// Created receiver not enabled
    /// There can be more then one receiver in program but that not recomended
    /// </summary>
    public class AndroidHeadphonesStateReceiver : HeadphonesStateReceiverBase
    {
        private const string HEADPHONES_PLUGGED_FUNC = "HeadphonesPlugged";
        private const string GET_DEVICE_VOLUME = "GetDeviceVolume";
        private const string GET_ENABLED_FUNC = "GetEnabled";
        private const string ENABLE_FUNC = "SetEnabled";
        
        private AndroidJavaObject _audioStateChangeReceiver;
        private AndroidAudioSateChangeMessageHandler _handler;

        public override bool Enabled
        {
            get { return _audioStateChangeReceiver.Call<bool>(GET_ENABLED_FUNC); }
        }

        public override bool HeadphonesPlugged
        {
            get { return _audioStateChangeReceiver.Call<bool>(HEADPHONES_PLUGGED_FUNC); }
        }
        
        
        public AndroidHeadphonesStateReceiver()
        {
            var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var androidContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
         
            _handler = new AndroidAudioSateChangeMessageHandler(Invoke);
            _audioStateChangeReceiver = new AndroidJavaObject("com.sekougi.headphonesdetectorplugin.AudioInOutChangeReceiver", androidContext, _handler);
        }

        public override int GetDeviceVolume()
        {
            return _audioStateChangeReceiver.Call<int>(GET_DEVICE_VOLUME);
        }

        public override void Enable(bool enable)
        {
            _audioStateChangeReceiver.Call(ENABLE_FUNC, enable);
            Debug.LogFormat("<color=red>[headphonesSateReceiver] Android Headphones State receiver enabled: {0}</color>", enable);
        }
    }
}