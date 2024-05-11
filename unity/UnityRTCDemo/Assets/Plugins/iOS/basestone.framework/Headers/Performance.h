//
// Created by Administrator on 2023/5/8.
//
#include "basestone.h"
#include "report.h"
#include <iostream>
#include <thread>
#include <chrono>
#include <string.h>
#ifndef WIN32
#include <unistd.h>
#endif
#define PERFORMANCE_TAG  "performance"
#ifndef LJSDK_PERFORMANCE_H
#define LJSDK_PERFORMANCE_H
DEFINE_EVENT_SLOT_KEY(memoryusage, LJ::EventLevelInfo, Simple);
DEFINE_EVENT_SLOT_KEY(gpuusage, LJ::EventLevelInfo, Simple);
DEFINE_EVENT_SLOT_KEY(cpuusage, LJ::EventLevelInfo, Simple);

class BASESTONE_EXTERN Performance {
public:
    Performance();
    virtual ~Performance();
    virtual void start();
    virtual void stop();

    inline int GetCurrentPid()
    {
        return getpid();
    }
    static Performance* createPerformance();


    template<typename TClass, typename Func>
    long startLoop(TClass *obj, Func func, unsigned int msecInterval, bool repeat);

    void reportTimeOut();

    virtual void onReportTimeOut() = 0;

private:
    long m_taskId = -1;
protected:
    const int Byte2MB = 1024 * 1024;
    LJ::Bundle _cpuInfo;
    LJ::Bundle _gpuInfo;
    LJ::Bundle _memoryInfo;
};
#endif //LJSDK_PERFORMANCE_H
