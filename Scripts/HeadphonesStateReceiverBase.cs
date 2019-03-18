using System;



namespace HeadphonesDetectorPlugin
{
    public abstract class HeadphonesStateReceiverBase
    {
        public event Action<HeadphonesStateMessage> OnHeadphonesStateChanged; 
        
        public virtual bool Enabled
        {
            get { return false; }
        }

        public virtual bool HeadphonesPlugged
        {
            get { return false; }
        }


        public abstract void Enable(bool enable);

        public abstract int GetDeviceVolume();

        protected void Invoke(string message, bool headphonesState)
        {
            var stateMessage = new HeadphonesStateMessage(message, headphonesState);

            if (OnHeadphonesStateChanged != null)
                OnHeadphonesStateChanged(stateMessage);
        }
    }
}