//
// Created by Administrator on 2023/2/24.
//

#ifndef LJSDK_APPPROXY_H
#define LJSDK_APPPROXY_H
#include <stdint.h>

enum RtcSDKClient {
    DefaultClient = 0, // 默认平台
    UnityClient = 1, // unity 跨平台构建出来的
};

class AppProxy {
public:
    static AppProxy *getInstance();

    void SetMediaBaseTime(uint64_t time);
    uint64_t getMediaBaseTime();
    bool isUnityClient();
    void setClientMode(RtcSDKClient client);
private:
    AppProxy();
    ~AppProxy();

    static AppProxy sInstance;
    uint64_t MEDIA_BASE_TIME = 0;
    RtcSDKClient m_Client = DefaultClient;

};


#endif //LJSDK_APPPROXY_H
