using System;
using UnityEngine;



namespace HeadphonesDetectorPlugin
{
    public class AndroidAudioSateChangeMessageHandler : AndroidJavaProxy
    {
        private Action<string, bool> _handler;
        
        
        public AndroidAudioSateChangeMessageHandler(Action<string, bool> handler) : base("com.sekougi.headphonesdetectorplugin.IMessageHandler")
        {
            _handler = handler;
        }
        
        // This method called from android side when headphones plugged or 
        // when on receiver enabling
        public void Handle(string message, bool state)
        {
            _handler(message, state);
        }
    }
}