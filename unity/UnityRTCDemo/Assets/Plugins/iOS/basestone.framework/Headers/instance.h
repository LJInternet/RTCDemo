//定义一个用于线程安全的单例定义宏，防止在使用lazy init单例时导致的多纯种安全问题。
//huangjianxiong 2018.5.21

#include "mutex.h"
#include "scoped_lock.h"

//定义一个单例的实现，单例对象是对象中定义的静态变量，用于构造函数为private的对象。在模块加载的时候自动初始化单例

#define DEFINE_INSTANCE_FROM_OBJ(cls,fun,var) \
cls cls::var; \
cls & cls::fun()\
{\
    return var;\
}

//定义一个单例实现，单例对象构造函数为public的情况。在第一次调用的时候实例化。

#define DEFINE_INSTANCE_PTR(cls,fun) \
static LJ::Mutex _mutex##cls;\
static cls * _instance##cls=NULL; \
cls * cls::fun()\
{\
    if(NULL == _instance##cls)\
    {\
        LJ::ScopedLock autoLock(_mutex##cls);\
        if(NULL == _instance##cls) \
        {\
            _instance##cls = new cls();\
        }\
    }\
    return _instance##cls;\
}

#define DEFINE_INSTANCE_PTR_EX(cls,fun,...) \
static LJ::Mutex _mutex##cls;\
static cls * _instance##cls=NULL; \
cls * cls::fun()\
{\
    if(NULL == _instance##cls)\
    {\
        LJ::ScopedLock autoLock(_mutex##cls);\
        if(NULL == _instance##cls) \
        {\
            _instance##cls = new cls(__VA_ARGS__);\
        }\
    }\
    return _instance##cls;\
}

#define DEFINE_INSTANCE_REF(cls,fun) \
static LJ::Mutex _mutex##cls;\
static cls * _instance##cls=NULL; \
cls & cls::fun()\
{\
    if(NULL == _instance##cls)\
    {\
        LJ::ScopedLock autoLock(_mutex##cls);\
        if(NULL == _instance##cls) \
        {\
            _instance##cls = new cls();\
        }\
    }\
    return *_instance##cls;\
}


#define DEFINE_FREE_INSTANCE(cls,fun)\
void cls::fun()\
{\
    LJ::ScopedLock autoLock(_mutex##cls);\
    if(NULL != _instance##cls)\
    {\
        delete _instance##cls;\
        _instance##cls = NULL;\
    }\
}


