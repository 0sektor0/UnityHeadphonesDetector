namespace HeadphonesDetectorPlugin
{
    public class HeadphonesStateReceiverMock : HeadphonesStateReceiverBase
    {
        private bool _isHeadphonesPlugged = true;
        
        public override bool HeadphonesPlugged
        {
            get { return _isHeadphonesPlugged; }
        }

        
        public override void Enable(bool enable)
        {
            var message = string.Format("mock enabled: {0}", enable);
            Invoke(message, _isHeadphonesPlugged);
        }

        public void UseSpeaker()
        {
            _isHeadphonesPlugged = false;
            Invoke("mock using speaker", _isHeadphonesPlugged);
        }

        public void UseHeadphones()
        {
            _isHeadphonesPlugged = true;
            Invoke("mock using headphones", _isHeadphonesPlugged);
        }
        
        public override int GetDeviceVolume()
        {
            return 100;
        }
    }
}