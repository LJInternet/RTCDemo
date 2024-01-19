#pragma once

#include <vector>

namespace LJ
{
template <typename T>
class RingQueue
{
public:
    RingQueue(int capacity):
        _size(0),
        _rIndex(0),
        _wIndex(0),
        _capacity(capacity),
        _data(capacity)
    {

    }

    bool full() const
    {
        return _size == _capacity;
    }

    bool empty() const
    {
        return _size == 0;
    }

    int size() const
    {
        return _size;
    }

    int capacity() const
    {
        return _capacity;
    }

    void clear()
    {
        _size = 0;
        _rIndex = 0;
        _wIndex = 0;
    }

    const T &front() const
    {
        if (_size == 0)
        {
            assert(false);
            static T dummy;
            return dummy;
        }

        return _data[_rIndex];
    }

    T &front()
    {
        if (_size == 0)
        {
            assert(false);
            static T dummy;
            return dummy;
        }

        return _data[_rIndex];
    }

    const T &back() const
    {
        if (_size == 0)
        {
            assert(false);
            static T dummy;
            return dummy;
        }

        return _data[(_wIndex + _capacity - 1) % _capacity];
    }

    T &back()
    {
        if (_size == 0)
        {
            assert(false);
            static T dummy;
            return dummy;
        }

        return _data[(_wIndex + _capacity - 1) % _capacity];
    }

    bool pushBack(T data)
    {

        if (_size >= _capacity)
        {
            assert(false);
            return false;
        }

        _data[_wIndex++] = data;

        if (_wIndex == _capacity)
        {
            _wIndex = 0;
        }

        ++_size;

        return true;
    }

    T popFront()
    {
        if (_size == 0)
        {
            assert(false);
            return T();
        }

        T ret = _data[_rIndex++];

        if (_rIndex == _capacity)
        {
            _rIndex = 0;
        }

        --_size;
        return ret;
    }


    T popBack()
    {
        if (_size == 0)
        {
            assert(false);
            return T();
        }

        T ret = _data[--_wIndex];

        if (_wIndex == -1)
        {
            _wIndex = _capacity - 1;
        }

        --_size;
        return ret;
    }

    T &operator[](int index)
    {
        return _data[(index + _rIndex) % _capacity];
    }

    const T &operator[](int index) const
    {
        return _data[(index + _rIndex) % _capacity];
    }

    bool insertAt(int index, T value)
    {
        if (full())
        {
            return false;
        }

        if (index < 0)
        {
            return false;
        }

        if (index > _size)
        {
            return false;
        }

        int num = _size - index;

        for (int i = 0; i < num; i++)
        {
            operator[](_size - i) = operator[](_size - i - 1);
        }

        operator[](index) = value;

        ++_size;
        ++_wIndex;

        return true;
    }


private:
    int _size;
    int _rIndex;
    int _wIndex;
    int _capacity;
    std::vector<T> _data;

};
}
