#import <Foundation/Foundation.h>
#import <AudioToolbox/AudioSession.h>
#import <AVFoundation/AVFoundation.h>
#import "AudioStateChangeReceiver.h"




static AudioStateChangeReceiver* receiver = NULL;

FOUNDATION_EXPORT void RegisterMessageHandler(MonoPMessageDelegate delegate)
{
    if(receiver != NULL)
        return;
    
    receiver = [[AudioStateChangeReceiver alloc]init: delegate];
}

FOUNDATION_EXPORT bool IsHeadphonesPlugged()
{
    AVAudioSessionRouteDescription* route = [[AVAudioSession sharedInstance] currentRoute];
    
    for (AVAudioSessionPortDescription* desc in [route outputs])
    {
        if ([[desc portType] isEqualToString:AVAudioSessionPortHeadphones])
            return true;
        if ([[desc portType] isEqualToString:AVAudioSessionPortBluetoothLE])
            return true;
        if ([[desc portType] isEqualToString:AVAudioSessionPortBluetoothHFP])
            return true;
        if ([[desc portType] isEqualToString:AVAudioSessionPortBluetoothA2DP])
            return true;
    }
    
    return false;
}




@implementation AudioStateChangeReceiver
- (id) init:(MonoPMessageDelegate) delegate
{
    self = [super init];
    _delegate = delegate;
    
    return self;
}


- (void) SendMessageToUnity: (const char*) message headphonesState: (const bool) state {
    dispatch_async(dispatch_get_main_queue(), ^{
            _delegate(message, state);
    });
}


- (void) SendHeadphonesStateMessage: (bool) state {
    if(state)
    {
        [self SendMessageToUnity: "Headphone plugged in"
                 headphonesState: true];
    }
    else
    {
        [self SendMessageToUnity: "Headphone unplugged"
                 headphonesState: false];
    }
}


- (void) AudioRoutingListenerCallback:(NSNotification*)notification
{
    NSDictionary *interuptionDict = notification.userInfo;
    NSInteger routeChangeReason = [[interuptionDict valueForKey:AVAudioSessionRouteChangeReasonKey] integerValue];
    NSLog(@"route change reason: %lid", routeChangeReason);
    
    switch (routeChangeReason) {
        case AVAudioSessionRouteChangeReasonNewDeviceAvailable:
            [self SendHeadphonesStateMessage: IsHeadphonesPlugged()];
            break;
            
        case AVAudioSessionRouteChangeReasonOldDeviceUnavailable:
            [self SendHeadphonesStateMessage: IsHeadphonesPlugged()];
            break;
    }
}


- (void) EnableReceiver:(bool)enable
{
    if(_enabled == enable)
    {
        if(enable)
            [self SendHeadphonesStateMessage: IsHeadphonesPlugged()];
        
        NSLog(@"Receiver enabling duplicate");
        return;
    }
    
    if(enable)
    {
        _enabled = true;
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(AudioRoutingListenerCallback:)
                                                     name:AVAudioSessionRouteChangeNotification
                                                   object:nil];
        
        [self SendHeadphonesStateMessage: IsHeadphonesPlugged()];
    }
    else
    {
        _enabled = false;
        [[NSNotificationCenter defaultCenter] removeObserver:self];
    }
    
    
    NSLog(@"Receiver state changed: %d", enable);
}


- (bool) Enabled
{
    return _enabled;
}
@end




FOUNDATION_EXPORT void EnableReceiver(bool enable)
{
    if(receiver != NULL)
        [receiver EnableReceiver:enable];
}

FOUNDATION_EXPORT bool IsEnabled()
{
    if(receiver != NULL)
        return [receiver Enabled];
    else
        return false;
}

FOUNDATION_EXPORT int GetDeviceSoundVolume()
{
    [[AVAudioSession sharedInstance] setActive:YES error:nil];
    float vol = [[AVAudioSession sharedInstance] outputVolume] * 100;
    
    return (int) vol;
}