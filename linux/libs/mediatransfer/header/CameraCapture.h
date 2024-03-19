//
// Created by Administrator on 2023/2/21.
//

#ifndef LJSDK_CAMERACAPTURE_H
#define LJSDK_CAMERACAPTURE_H
#include <vector>

class IVideoCaptureCallback {
public:
    virtual ~IVideoCaptureCallback() {};
    virtual void onNewFrame(unsigned char* data, int len, int width, int height, int rotation, uint64_t timeStamp, int pixel_fmt) = 0;
};

class CameraDeviceManager{
public:
    static std::vector <std::string> getDeviceList();
    static bool isDeviceBusy(char* videoName);
    static std::string GetDeviceOccupyProcessName();
};

class CameraCapture {

public:
    static CameraCapture* CreateCameraCapture();

    virtual ~CameraCapture(){}

    virtual int setupDevice(IVideoCaptureCallback* callback, const char* deviceCode, int oriMode, int width, int height, int fps) = 0;

    virtual int setupDevice(IVideoCaptureCallback* callback,const char* deviceCode, const char* config, int length) {
        return 0;
    };
    virtual void stop() = 0;
};

#endif //LJSDK_CAMERACAPTURE_H
