#pragma once

#include "system_util.h"
#include "string_util.h"
#include "task_loop.h"
#include "statistics.h"
#include "mutex.h"
#include "scoped_lock.h"
#include "bundle.h"

namespace LJ
{
#define ASSERT_EVENT(cond) if(!(cond)){LJ::ReportCenter::instance().reportAssert(__FILE__, __LINE__);LOG("LJ_BASE", LOG_WARN, "assert at file %s, line %d", __FILE__, __LINE__);assert(false);}

#define EVENT_SLOT_KEY(key) EVENT_SLOT_KEY_##key

#define DEFINE_EVENT_SLOT_KEY(key, level, type) static LJ::EventSlot##type EVENT_SLOT_KEY_##key(#key, level)

#define REPORT_EVENT(key, detail) if(!EVENT_SLOT_KEY_##key.isRegisted())\
    {EVENT_SLOT_KEY_##key.setRegisted();LJ::ReportCenter::instance().registerEventSlot(EVENT_SLOT_KEY_##key);}\
    LJ::ReportCenter::instance().report(EVENT_SLOT_KEY_##key, detail);

#define UNREGISTER_REPORT_EVENT(key) if(EVENT_SLOT_KEY_##key.isRegisted())\
    {EVENT_SLOT_KEY_##key.unRegisted();LJ::ReportCenter::instance().unregisterEventSlot(EVENT_SLOT_KEY_##key);}

class IReportCenterCallback
{
public:
    virtual ~IReportCenterCallback() {};

    virtual void uploadReport(std::string reportStr) = 0;
};

struct ReportCenterParam
{
    TaskLoop *taskLoop;
    IReportCenterCallback *callBack;
    uint32_t collectDuration;
};

class EventSlot;

class BASESTONE_EXTERN ReportCenter
{
public:
    static ReportCenter &instance();
    //set parameters
    void init(ReportCenterParam param);
    void release();

    void setCommonAttrs(Bundle attrs);

    void report(EventSlot &slot, Bundle &data);
    void reportAssert(const char *fileName, int lineNumber);

    void registerEventSlot(EventSlot &slot);
    void unregisterEventSlot(EventSlot &slot);

private:
    friend class EventSlotAssert;
    void uploadLog(const std::string & comments);

private:
    ReportCenter();
    ~ReportCenter();

    void onTimer();

    //param
    ReportCenterParam _param;

    std::list<EventSlot *> _slotList;
    long _timerId;
    Bundle _commonAttr;

#ifdef WIN32
    std::string _osInfo;
    std::string _gpuInfo;
    std::string _cpuInfo;
#endif

    Mutex _lock;

    static ReportCenter s_theObj;
};

enum EventLevel
{
    EventLevelInfo,
    EventLevelAlarm,
    EventLevelError,
};

class BASESTONE_EXTERN EventSlot: private LJ::NoCopyable
{
public:
    EventSlot(std::string name, EventLevel level);

    virtual ~EventSlot() {}

    virtual void doInput(const Bundle &details) = 0;
    virtual void fillData(Bundle &infoDict) = 0;

    std::list<Bundle> generateReport();
    void input(const Bundle &details);

    void setRegisted()
    {
        _registed = true;
    }

    void unRegisted()
    {
        _registed = false;
    }

    bool isRegisted()
    {
        return _registed;
    }

private:

    std::string eventLevelToString(EventLevel level);

    std::string _eventName;
    EventLevel _level;
    bool _registed;
    Mutex _dataLock;
protected:
    uint32_t _count;
};

class EventSlotAssert: public EventSlot
{
public:

    EventSlotAssert(std::string name);

    virtual void doInput(const Bundle &details);
    virtual void fillData(Bundle &infoDict);

private:
    std::map<std::string, uint32_t> _counterMap;
};

class BASESTONE_EXTERN EventSlotSimple: public EventSlot
{
public:
    EventSlotSimple(std::string name, EventLevel level);

    virtual void doInput(const Bundle &details);
    virtual void fillData(Bundle &infoDict);

private:
    std::vector<Bundle> _detailValue;
};

}