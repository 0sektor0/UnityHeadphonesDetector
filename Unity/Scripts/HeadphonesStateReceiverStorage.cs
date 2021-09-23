using System;
using UnityEngine;



namespace HeadphonesDetectorPlugin
{
    public class HeadphonesStateReceiverStorage
    {
        private static HeadphonesStateReceiverBase _receiver;
        private static bool _isMock;

        public static bool IsMock
        {
            get { return _isMock; }
        }


        public static HeadphonesStateReceiverBase GetReceiver()
        {
            if (_receiver == null)
            {
                try
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    _receiver = new AndroidHeadphonesStateReceiver();
#elif UNITY_IOS && !UNITY_EDITOR
                    _receiver = new IosHeadphonesStateReceiver();
#else
                    _receiver = new HeadphonesStateReceiverMock();
#endif
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("[headphonesSateReceiver] Cannot initialize plugin\n{0}\n{1}", e.Message, e.StackTrace);
                    _receiver = new HeadphonesStateReceiverMock();
                }
            }

            return _receiver;
        }

        public static void ForceToUseMockReceiver()
        {
            if (!_isMock)
            {
                Debug.Log("[headphonesSateReceiver] switching to mock");
                _receiver = new HeadphonesStateReceiverMock();
                
                _isMock = true;
            }
        }

        public static void ForceToUseNativeReceiver()
        {
            if (_isMock)
            {
                Debug.Log("[headphonesSateReceiver] switching to native");
                _receiver = null;
                GetReceiver();

                _isMock = false;
            }
        }
    }
}