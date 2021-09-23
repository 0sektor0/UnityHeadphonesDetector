package com.sekougi.headphonesdetectorplugin;

public interface IMessageHandler {
    void Handle(String message, boolean state);
}
