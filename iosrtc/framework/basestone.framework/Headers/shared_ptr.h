#pragma once

#include "ptr_disposer.h"
#include "mutex.h"
#include "scoped_lock.h"

#if __cplusplus >= 201103L
#include <memory>
#endif

namespace LJ
{

#if __cplusplus < 201103L
template <class T>
class SharedCount
{
public:
    SharedCount(T *ptr, PtrDisposer<T> &disposer) : _ptr(ptr), _refCount(1), _lock(), _disposer(disposer)
    {
    }

    ~SharedCount()
    {
        if(_ptr != NULL)
        _disposer.dispose(_ptr);
    }

    void incRefCount()
    {
        ScopedLock autoLock(_lock);
        ++_refCount;
    }

    void decRefCount()
    {
        bool shouldDelete = false;

        {
            ScopedLock autoLock(_lock);

            if (_refCount > 0)
            {
                --_refCount;
            }

            if (_refCount == 0)
            {
                shouldDelete = true;
            }
        }

        if (shouldDelete)
        {
            delete this;
        }
    }

private:
    T *_ptr;
    unsigned int _refCount;
    Mutex _lock;
    PtrDisposer<T> &_disposer;
};

/**
 * SharedPtr is a class to encapsulate an object pointer. When a pointer is
 * put into SharedPtr object, the object reference count will increase. When
 * a SharedPtr object is destructed, the object reference count will decrease.
 * If the object reference count is decreased to 0, the object will be deleted
 * automatically.
 */
template <class T>
class SharedPtr
{
private:
    class DefaultPtrDisposer : public LJ::PtrDisposer<T>
    {
    public:
        virtual void dispose(T *ptr)
        {
            if (ptr)
            {
                delete ptr;
            }
        }
    };

public:
    /**
     * Default constructor.
     * @param ptr object pointer.
     */
    SharedPtr() : _ptr(NULL), _sc(NULL)
    {
    }

    /**
     * Constructor.
     * @param ptr object pointer.
     */
    SharedPtr(T *ptr) : _ptr(ptr), _sc(new SharedCount<T>(ptr, _kDefaultPtrDisposer))
    {
    }

    /**
     * Constructor with disposer.
     * @param ptr object pointer.
     * @param disposer object disposer.
     */
    SharedPtr(T *ptr, PtrDisposer<T> &disposer) : _ptr(ptr), _sc(new SharedCount<T>(ptr, disposer))
    {
    }

    /**
     * Copy constructor.
     * @param obj another SharedPtr object.
     */
    SharedPtr(const SharedPtr<T> &obj) : _ptr(obj._ptr), _sc(obj._sc)
    {
        if (obj._sc)
            obj._sc->incRefCount();
    }

    /**
     * Destructor.
     */
    ~SharedPtr()
    {
        if (_sc)
            _sc->decRefCount();
    }

    /**
     * Assign. The original object reference count will be decreased, and
     * the new object reference count will be increased.
     * @param obj another SharedPtr object.
     * @return self object.
     */
    SharedPtr &operator=(const SharedPtr<T> &obj)
    {
        if (this != &obj) {
            if (obj._sc)
                obj._sc->incRefCount();
            if (_sc)
                _sc->decRefCount();
            _sc = obj._sc;
            _ptr = obj._ptr;
        }
        return *this;
    }

    SharedPtr(SharedPtr&& other)
    {
        _ptr = other._ptr;
        other._ptr = nullptr;
        _sc = other._sc;
        other._sc = nullptr;
    }

    SharedPtr &operator=(SharedPtr<T>&& obj)
    {
        if (this != &obj) {
            if (_sc)
                _sc->decRefCount();
            _ptr = obj._ptr;
            obj._ptr = nullptr;
            _sc = obj._sc;
            obj._sc = nullptr;
        }
        return *this;
    }

    /**
     * Return the object pointer.
     * @return object pointer.
     */
    const T *operator->() const
    {
        return _ptr;
    }

    /**
     * Return the object pointer.
     * @return object pointer.
     */
    T *operator->()
    {
        return _ptr;
    }

    /**
     * Return the object pointer.
     * @return object pointer.
     */
    const T *ptr() const
    {
        return _ptr;
    }

    /**
     * Return the object pointer.
     * @return object pointer.
     */
    T *ptr()
    {
        return _ptr;
    }

    /**
     * Return the object reference.
     * @return object reference.
     */
    const T &operator*() const
    {
        return *_ptr;
    }

    /**
     * Return the object reference.
     * @return object reference.
     */
    T &operator*()
    {
        return *_ptr;
    }

    /**
     * Return if the pointer equals to another.
     * @return true if equal.
     */
    bool operator==(T *ptr) const
    {
        return _ptr == ptr;
    }

    /**
     * Return if the pointer equals to another.
     * @return true if equal.
     */
    bool operator==(SharedPtr<T> obj) const
    {
        return _ptr == obj._ptr;
    }

private:
    //static void *operator new(size_t size);

    static DefaultPtrDisposer _kDefaultPtrDisposer;

    T *_ptr;
    SharedCount<T> *_sc;
};

template <class T>
typename SharedPtr<T>::DefaultPtrDisposer SharedPtr<T>::_kDefaultPtrDisposer;
#else
template <class T>
class SharedPtr : private std::shared_ptr<T>
{
public:
    SharedPtr() : std::shared_ptr<T>() {}
    SharedPtr(T *ptr) : std::shared_ptr<T>(ptr) {}
    SharedPtr(const SharedPtr<T>& obj) : std::shared_ptr<T>(obj) {}
    SharedPtr(SharedPtr<T>&& obj) : std::shared_ptr<T>(std::move(obj)) {}

    SharedPtr<T> &operator=(const SharedPtr<T> &obj)
    {
        std::shared_ptr<T>::operator=(obj);
        return *this;
    }
    SharedPtr<T> &operator=(SharedPtr<T>&& obj)
    {
        std::shared_ptr<T>::operator=(std::move(obj));
        return *this;
    }
    const T *operator->() const
    {
        return std::shared_ptr<T>::operator->();
    }
    T *operator->()
    {
        return std::shared_ptr<T>::operator->();
    }
    const T *ptr() const
    {
        return std::shared_ptr<T>::get();
    }
    T *ptr()
    {
        return std::shared_ptr<T>::get();
    }
    bool operator==(T *ptr) const
    {
        return this->ptr() == ptr;
    }
    bool operator==(const SharedPtr<T>& obj) const
    {
        return this->ptr() == obj.ptr();
    }
    const T& operator*() const
    {
        return std::shared_ptr<T>::operator*();
    }
    T& operator*()
    {
        return std::shared_ptr<T>::operator*();
    }
};
#endif

}
