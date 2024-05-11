#pragma once
#include <cstdlib>
#include "log.h"

#if (defined  _WIN32 && !defined __PRETTY_FUNCTION__)
#define __PRETTY_FUNCTION__ __FUNCTION__
#endif
#define RUNNABLE_DEBUG 0
namespace LJ
{

class Runnable
{
public:
    Runnable():srcId(0){}
    virtual ~Runnable() {
        if(RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC destroy %p, src=%p", this, (void*)srcId);
        }
    }
    virtual void run() = 0;
    virtual void *getObject()
    {
        return NULL;
    }

    virtual const char *getClassInfo() = 0;
    virtual Runnable* clone(){ return NULL;};
    long srcId;
};

typedef void* (*thread_run)(void *);
class CFuncWrapper : public Runnable
{
    public:
    CFuncWrapper(thread_run func, void *obj):
        _obj(obj),
        _func(func)
    {

    }

    virtual void run()
    {
        _func(_obj);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual CFuncWrapper *clone() {
        CFuncWrapper *_cp = new CFuncWrapper(_func, _obj);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }

    private:
        thread_run _func;
        void* _obj;
};

template <typename TClass, typename Func>
class Functor0 : public Runnable
{
public:
    Functor0(TClass *obj, Func func):
        _obj(obj),
        _func(func)
    {

    }

    virtual void run()
    {
        (_obj->*_func)();
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor0 *clone() {
        Functor0<TClass, Func> *_cp = new Functor0<TClass, Func>(_obj, _func);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }

private:
    TClass *_obj;
    Func _func;
};

template <typename TClass, typename Func, typename T1>
class Functor1: public Runnable
{
public:
    Functor1(TClass *obj, Func func, T1 param1):
        _obj(obj),
        _func(func),
        _param1(param1)
    {

    }

    virtual void run()
    {
        (_obj->*_func)(_param1);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor1 *clone() {
        Functor1<TClass, Func, T1> *_cp = new Functor1<TClass, Func, T1>(_obj, _func, _param1);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    TClass *_obj;
    Func _func;
    T1 _param1;
};

template <typename TClass, typename Func, typename T1, typename T2>
class Functor2: public Runnable
{
public:
    Functor2(TClass *obj, Func func, T1 param1, T2 param2):
        _obj(obj),
        _func(func),
        _param1(param1),
        _param2(param2)
    {

    }

    virtual void run()
    {
        (_obj->*_func)(_param1, _param2);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor2 *clone() {
        Functor2<TClass, Func, T1, T2> *_cp = new Functor2<TClass, Func, T1, T2>(_obj, _func, _param1, _param2);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    TClass *_obj;
    Func _func;
    T1 _param1;
    T2 _param2;
};

template <typename TClass, typename Func, typename T1, typename T2, typename T3>
class Functor3: public Runnable
{
public:
    Functor3(TClass *obj, Func func, T1 param1, T2 param2, T3 param3):
        _obj(obj),
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3)
    {

    }

    virtual void run()
    {
        (_obj->*_func)(_param1, _param2, _param3);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor3 *clone() {
        Functor3<TClass, Func, T1, T2, T3> *_cp = new Functor3<TClass, Func, T1, T2, T3>(_obj, _func, _param1, _param2, _param3);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    TClass *_obj;
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
};

template <typename TClass, typename Func, typename T1, typename T2, typename T3, typename T4>
class Functor4: public Runnable
{
public:
    Functor4(TClass *obj, Func func, T1 param1, T2 param2, T3 param3, T4 param4):
        _obj(obj),
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3),
        _param4(param4)
    {

    }

    virtual void run()
    {
        (_obj->*_func)(_param1, _param2, _param3, _param4);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor4 *clone() {
        Functor4<TClass, Func, T1, T2, T3, T4> *_cp = new Functor4<TClass, Func, T1, T2, T3, T4>(_obj, _func, _param1, _param2, _param3, _param4);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    TClass *_obj;
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
    T4 _param4;
};

template <typename TClass, typename Func, typename T1, typename T2, typename T3, typename T4, typename T5>
class Functor5: public Runnable
{
public:
    Functor5(TClass *obj, Func func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5):
        _obj(obj),
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3),
        _param4(param4),
        _param5(param5)
    {

    }

    virtual void run()
    {
        (_obj->*_func)(_param1, _param2, _param3, _param4, _param5);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor5 *clone() {
        Functor5<TClass, Func, T1, T2, T3, T4, T5> *_cp = new Functor5<TClass, Func, T1, T2, T3, T4, T5>(_obj, _func, _param1, _param2, _param3, _param4, _param5);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    TClass *_obj;
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
    T4 _param4;
    T5 _param5;
};

template <typename TClass, typename Func, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6>
class Functor6: public Runnable
{
public:
    Functor6(TClass *obj, Func func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6):
        _obj(obj),
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3),
        _param4(param4),
        _param5(param5),
        _param6(param6)
    {

    }

    virtual void run()
    {
        (_obj->*_func)(_param1, _param2, _param3, _param4, _param5, _param6);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor6 *clone() {
        Functor6<TClass, Func, T1, T2, T3, T4, T5, T6> *_cp = new Functor6<TClass, Func, T1, T2, T3, T4, T5, T6>(_obj, _func, _param1, _param2, _param3, _param4, _param5, _param6);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    TClass *_obj;
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
    T4 _param4;
    T5 _param5;
    T6 _param6;
};

template <typename TClass, typename Func, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6, typename T7>
class Functor7: public Runnable
{
public:
    Functor7(TClass *obj, Func func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7):
        _obj(obj),
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3),
        _param4(param4),
        _param5(param5),
        _param6(param6),
        _param7(param7)
    {

    }

    virtual void run()
    {
        (_obj->*_func)(_param1, _param2, _param3, _param4, _param5, _param6, _param7);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor7 *clone() {
        Functor7<TClass, Func, T1, T2, T3, T4, T5, T6, T7> *_cp = new Functor7<TClass, Func, T1, T2, T3, T4, T5, T6, T7>(_obj, _func, _param1, _param2, _param3, _param4, _param5, _param6, _param7);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    TClass *_obj;
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
    T4 _param4;
    T5 _param5;
    T6 _param6;
    T7 _param7;
};

template <typename TClass, typename Func, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6, typename T7, typename T8>
class Functor8: public Runnable
{
public:
    Functor8(TClass *obj, Func func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8):
        _obj(obj),
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3),
        _param4(param4),
        _param5(param5),
        _param6(param6),
        _param7(param7),
        _param8(param8)
    {

    }

    virtual void run()
    {
        (_obj->*_func)(_param1, _param2, _param3, _param4, _param5, _param6, _param7, _param8);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor8 *clone() {
        Functor8<TClass, Func, T1, T2, T3, T4, T5, T6, T7, T8> *_cp = new Functor8<TClass, Func, T1, T2, T3, T4, T5, T6, T7, T8>(_obj, _func, _param1, _param2, _param3, _param4, _param5, _param6, _param7, _param8);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    TClass *_obj;
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
    T4 _param4;
    T5 _param5;
    T6 _param6;
    T7 _param7;
    T8 _param8;
};


template <typename TClass, typename Func, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6, typename T7, typename T8, typename T9>
class Functor9: public Runnable
{
public:
    Functor9(TClass *obj, Func func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9):
        _obj(obj),
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3),
        _param4(param4),
        _param5(param5),
        _param6(param6),
        _param7(param7),
        _param8(param8),
        _param9(param9)
    {

    }

    virtual void run()
    {
        (_obj->*_func)(_param1, _param2, _param3, _param4, _param5, _param6, _param7, _param8, _param9);
    }

    virtual void *getObject()
    {
        return _obj;
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual Functor9 *clone() {
        Functor9<TClass, Func, T1, T2, T3, T4, T5, T6, T7, T8, T9> *_cp = new Functor9<TClass, Func, T1, T2, T3, T4, T5, T6, T7, T8, T9>(_obj, _func, _param1, _param2, _param3, _param4, _param5, _param6, _param7, _param8, _param9);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    TClass *_obj;
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
    T4 _param4;
    T5 _param5;
    T6 _param6;
    T7 _param7;
    T8 _param8;
    T9 _param9;
};


template <typename Func>
class StaticFunctor0 : public Runnable
{
public:
    StaticFunctor0(Func func):
        _func(func)
    {

    }

    virtual void run()
    {
        (*_func)();
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual StaticFunctor0 *clone() {
        StaticFunctor0<Func> *_cp = new StaticFunctor0<Func>(_func);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    Func _func;
};

template <typename Func, typename T1>
class StaticFunctor1: public Runnable
{
public:
    StaticFunctor1(Func func, T1 param1):
        _func(func),
        _param1(param1)
    {

    }

    virtual void run()
    {
        (*_func)(_param1);
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual StaticFunctor1 *clone() {
        StaticFunctor1<Func, T1> *_cp = new StaticFunctor1<Func, T1>(_func, _param1);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    Func _func;
    T1 _param1;
};

template <typename Func, typename T1, typename T2>
class StaticFunctor2: public Runnable
{
public:
    StaticFunctor2(Func func, T1 param1, T2 param2):
        _func(func),
        _param1(param1),
        _param2(param2)
    {

    }

    virtual void run()
    {
        (*_func)(_param1, _param2);
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual StaticFunctor2 *clone() {
        StaticFunctor2<Func, T1, T2> *_cp = new StaticFunctor2<Func, T1, T2>(_func, _param1, _param2);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    Func _func;
    T1 _param1;
    T2 _param2;
};

template <typename Func, typename T1, typename T2, typename T3>
class StaticFunctor3: public Runnable
{
public:
    StaticFunctor3(Func func, T1 param1, T2 param2, T3 param3):
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3)
    {

    }

    virtual void run()
    {
        (*_func)(_param1, _param2, _param3);
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual StaticFunctor3 *clone() {
        StaticFunctor3<Func, T1, T2, T3> *_cp = new StaticFunctor3<Func, T1, T2, T3>(_func, _param1, _param2, _param3);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
};

template <typename Func, typename T1, typename T2, typename T3, typename T4>
class StaticFunctor4: public Runnable
{
public:
    StaticFunctor4(Func func, T1 param1, T2 param2, T3 param3, T4 param4):
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3),
        _param4(param4)
    {

    }

    virtual void run()
    {
        (*_func)(_param1, _param2, _param3, _param4);
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual StaticFunctor4 *clone() {
        StaticFunctor4<Func, T1, T2, T3, T4> *_cp = new StaticFunctor4<Func, T1, T2, T3, T4>(_func, _param1, _param2, _param3, _param4);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
    T4 _param4;
};

template <typename Func, typename T1, typename T2, typename T3, typename T4, typename T5>
class StaticFunctor5: public Runnable
{
public:
    StaticFunctor5(Func func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5):
        _func(func),
        _param1(param1),
        _param2(param2),
        _param3(param3),
        _param4(param4),
        _param5(param5)
    {

    }

    virtual void run()
    {
        (*_func)(_param1, _param2, _param3, _param4, _param5);
    }

    virtual  const char *getClassInfo()
    {
        return __PRETTY_FUNCTION__;
    }

    virtual StaticFunctor5 *clone() {
        StaticFunctor5<Func, T1, T2, T3, T4, T5> *_cp = new StaticFunctor5<Func, T1, T2, T3, T4, T5>(_func, _param1, _param2, _param3, _param4, _param5);
        _cp->srcId = srcId;
        if (RUNNABLE_DEBUG) {
            LJ_LOG(LOG_INFO, "FUNC clone %p, src=%p",_cp, (void*)srcId);
        }
        return _cp;
    }
private:
    Func _func;
    T1 _param1;
    T2 _param2;
    T3 _param3;
    T4 _param4;
    T5 _param5;
};

}
