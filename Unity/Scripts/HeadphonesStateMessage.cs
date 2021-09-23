namespace HeadphonesDetectorPlugin
{
    public class HeadphonesStateMessage
    {
        private string _message;
        private bool _state;
        
        public string Message
        {
            get { return _message; }
        }

        public bool State
        {
            get { return _state; }
        }


        public HeadphonesStateMessage(string message, bool state)
        {
            _message = message;
            _state = state;
        }
    }
}