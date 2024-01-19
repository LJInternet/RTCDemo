//
// Created by Administrator on 2023/5/19.
//
#ifndef LJSDK_CONNECTIONRESPONSE_H
#define LJSDK_CONNECTIONRESPONSE_H
#include "ClientConstants.h"
#include <string>
RUDP_EXPORT_API class ResponseBody {
public:
    ResponseBody() {
        jsonObj = nullptr;
    }

    virtual void parse(std::string& jsonStr);

    void clear();

    virtual ~ResponseBody() {
        clear();
    }
protected:
    void* jsonObj = nullptr;
};


RUDP_EXPORT_API class JoinResponse : public ResponseBody {
public:
    JoinResponse() {};
    ~JoinResponse() {};

    void parse(std::string& jsonStr) override;

    long long _channelId;
    long long _peerId;
    std::string _host;
    int _port = 8000;

};
#endif //LJSDK_CONNECTIONRESPONSE_H
