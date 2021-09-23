package com.sekougi.headphonesdetectorplugin;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothHeadset;
import android.bluetooth.BluetoothProfile;
import android.content.BroadcastReceiver;
import android.content.IntentFilter;
import android.content.Context;
import android.content.Intent;
import android.media.AudioManager;
import android.os.Handler;
import android.util.Log;



public class AudioInOutChangeReceiver extends BroadcastReceiver {
    private final String TAG = "UnityAudioStateReceiver";

    private IMessageHandler _unityMessageHandler;
    private Handler _unityMainThreadHandler;
    private boolean _enabled;
    private Context _context;
    private boolean _wiredHeadphonesPlugged;
    private boolean _bluetoothHeadphonesPlugged;


    public AudioInOutChangeReceiver(Context context, IMessageHandler handler) {
        _unityMessageHandler = handler;
        _unityMainThreadHandler = new Handler();

        _context = context;
    }

    public boolean GetEnabled()
    {
        return _enabled;
    }

    public void SetEnabled(boolean enabled) {
        if(enabled && !_enabled) {
            IntentFilter filter = new IntentFilter();
            filter.addAction(Intent.ACTION_HEADSET_PLUG);
            filter.addAction(BluetoothHeadset.ACTION_CONNECTION_STATE_CHANGED);
            filter.addAction(BluetoothAdapter.ACTION_STATE_CHANGED);

            Init();

            _context.registerReceiver(this, filter);
            Log.d(TAG,"audio state receiver registered");
        }
        else if(!enabled && _enabled) {
            _context.unregisterReceiver(this);
            Log.d(TAG,"audio state receiver unregistered");
        }
        else
            return;

        _enabled = enabled;
    }

    @Override
    public void onReceive(Context context, Intent intent) {
        String action = intent.getAction();
        boolean isWiredHeadphones = false;
        boolean isPlugged;
        int state;

        Log.d(TAG, action);

        if (action.equals(Intent.ACTION_HEADSET_PLUG)) {
            state = intent.getIntExtra("state", -1);
            isWiredHeadphones = true;
        }
        else if (action.equals(BluetoothHeadset.ACTION_CONNECTION_STATE_CHANGED)) {
            state = intent.getIntExtra(BluetoothProfile.EXTRA_STATE, -1);
            Log.d(TAG, String.format("state: %s", state));

            switch (state) {
                case BluetoothProfile.STATE_CONNECTED:
                    state = 1;
                    break;
                case BluetoothProfile.STATE_DISCONNECTED:
                    state = 0;
                    break;
                default:
                    state = -1;
                    break;
            }

            isWiredHeadphones = false;
        }
        else if(action.equals(BluetoothAdapter.ACTION_STATE_CHANGED)) {
            state = intent.getIntExtra(BluetoothAdapter.EXTRA_STATE, -1);
            if(state == BluetoothAdapter.STATE_OFF) {
                isWiredHeadphones = false;
                state = 0;
            }
        }
        else {
            Log.d(TAG, "unknown action");
            return;
        }

        switch (state) {
            case 0:
                isPlugged = false;
                break;

            case 1:
                isPlugged = true;
                break;

            default:
                Log.d(TAG, "Unknown headset state: " + state);
                return;
        }

        UpdateState(isWiredHeadphones, isPlugged);

        String message = String.format("Headphones State: wired: %s bluetooth: %s", _wiredHeadphonesPlugged, _bluetoothHeadphonesPlugged);
        Log.d(TAG, message);
    }

    public int GetDeviceVolume() {
        AudioManager am = (AudioManager)_context.getSystemService(Context.AUDIO_SERVICE);

        int maxVolume = am.getStreamMaxVolume(3);
        int scaledVolume = am.getStreamVolume(3);
        float volume = (float)scaledVolume / (float)maxVolume * 100.0F;

        return (int)volume;
    }

    private void Init() {
        AudioManager am = (AudioManager)_context.getSystemService(Context.AUDIO_SERVICE);
        boolean wiredHeadphonesPlugged = am.isWiredHeadsetOn();
        _wiredHeadphonesPlugged = !_wiredHeadphonesPlugged;

        boolean bluetoothHeadphonesPlugged = am.isBluetoothA2dpOn() || am.isBluetoothScoOn();
        _bluetoothHeadphonesPlugged = !_bluetoothHeadphonesPlugged;

        UpdateState(false, bluetoothHeadphonesPlugged);
        UpdateState(true, wiredHeadphonesPlugged);
    }

    private void UpdateState(boolean wired, boolean state) {
        boolean duplicateState;

        if(wired)
            duplicateState = UpdateWiredState(state);
        else
            duplicateState = UpdateBluetoothState(state);

        if(duplicateState) {
            Log.d(TAG, "connection event duplicate");
            return;
        }

        if(!_wiredHeadphonesPlugged && !_bluetoothHeadphonesPlugged)
            SendMessageToUnity("Headphones is unplugged", false);
    }

    private boolean UpdateWiredState(boolean state) {
        if(_wiredHeadphonesPlugged == state)
            return true;

        _wiredHeadphonesPlugged = state;
        if(_wiredHeadphonesPlugged && !_bluetoothHeadphonesPlugged)
            SendMessageToUnity("Headphones is plugged", true);

        return false;
    }

    private boolean UpdateBluetoothState(boolean state) {
        if (_bluetoothHeadphonesPlugged == state)
            return true;

        _bluetoothHeadphonesPlugged = state;
        if(_bluetoothHeadphonesPlugged && !_wiredHeadphonesPlugged)
            SendMessageToUnity("Headphones is plugged", true);

        return false;
    }

    public boolean HeadphonesPlugged() {
        return _wiredHeadphonesPlugged || _bluetoothHeadphonesPlugged;
    }

    private void RunOnUnityThread(Runnable runnable) {
        if(_unityMainThreadHandler != null && runnable != null) {
            _unityMainThreadHandler.post(runnable);
        }
    }

    private void SendMessageToUnity(final String message, final boolean state) {
        RunOnUnityThread(new Runnable() {
            @Override
            public void run() {
                if(_unityMessageHandler != null) {
                    _unityMessageHandler.Handle(message, state);
                }
            }
        });
    }
}