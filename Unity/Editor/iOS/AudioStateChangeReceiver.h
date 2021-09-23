#ifndef AudioStateChangeReceiver_h
#define AudioStateChangeReceiver_h


typedef void (*MonoPMessageDelegate)(const char* message, const bool state);

@interface AudioStateChangeReceiver : NSObject
{
@private bool _enabled;
@private MonoPMessageDelegate _delegate;
}
- (void) SendMessageToUnity:(const char*) message headphonesState: (const bool) state;
- (void) AudioRoutingListenerCallback:(NSNotification*) notification;
- (void) SendHeadphonesStateMessage:(bool) state;
- (id) init:(MonoPMessageDelegate) delegate;
- (void) EnableReceiver:(bool) enable;
@end


#endif
