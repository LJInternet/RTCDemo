#pragma once

#include <stdint.h>
#include <limits>
#include <cmath>
#include <string>
#include <vector>

namespace LJ
{

class StatisticsUtil
{
public:
    template<typename T>
    static T calcIncrement(T newVal, T oldVal)
    {
        if (newVal >= oldVal)
        {
            return newVal - oldVal;
        }
        else
        {
            return std::numeric_limits<T>::max() - oldVal + newVal + 1;
        }
    }
};

template <typename T>
class SummaryTypeGeter
{
public:
    typedef long long SummaryType;
};

template<>
class SummaryTypeGeter<float>
{
public:
    typedef double SummaryType;
};

template<>
class SummaryTypeGeter<double>
{
public:
    typedef double SummaryType;
};


template <typename T>
class StatisticsVector
{
public:
    StatisticsVector(uint32_t size = 1):
        _size(size),
        _average(0),
        _stdVar(0),
        _var(0),
        _summary(0),
        _max(0),
        _min(0),
        _needStd(false)
    {
    }

    ~StatisticsVector()
    {
        clear();
    }

    void clear()
    {
        _dataList.clear();
        _average = 0;
        _stdVar = 0;
        _var = 0;
        _summary = 0;
        _max = 0;
        _min = 0;
    }

    void put(T data)
    {
        typename std::vector<T>::const_iterator iter = _dataList.begin();
        size_t dataListSize = _dataList.size();

        while (dataListSize >= _size)
        {
            _summary -= *(_dataList.begin());
            _dataList.erase(_dataList.begin());
            dataListSize--;
        }

        _dataList.push_back(data);
        dataListSize++;

        //assert(std::numeric_limits<T>::max() - _summary > data);

        _summary += data;
        _average = 1.0 * _summary / dataListSize;

        double var = 0;
        iter = _dataList.begin();

        if (iter != _dataList.end())
        {
            _max = *iter;
            _min = *iter;
        }

        while (iter != _dataList.end())
        {
            if (*iter > _max)
            {
                _max = *iter;
            }

            if (*iter < _min)
            {
                _min = *iter;
            }

            if (_needStd)
            {
                var += (*iter - _average) * (*iter - _average);
            }

            ++iter;
        }

        if (_needStd)
        {
            _var = var / dataListSize;
            _stdVar = sqrt(_var);
        }
    }
    void setNeedStd(bool needStd)
    {
        _needStd = needStd;
    }
    void setSize(uint32_t size)
    {
        size_t dataListSize = _dataList.size();

        while (dataListSize >= size)
        {
            _summary -= *(_dataList.begin());
            _dataList.erase(_dataList.begin());
            dataListSize--;
        }

        _size = size;
    }

    uint32_t getDataSize() const
    {
        return _dataList.size();
    }

    uint32_t getSize()
    {
        return _size;
    }
    double getAverage()
    {
        return _average;
    }
    double getAverage() const
    {
        return _average;
    }
    double getStdVar()
    {
        return _stdVar;
    }
    double getVar()
    {
        return _var;
    }
    typename SummaryTypeGeter<T>::SummaryType getSummary()
    {
        return _summary;
    }
    T getMax()
    {
        return _max;
    }
    T getMax() const
    {
        return _max;
    }
    T getMin()
    {
        return _min;
    }
    T getMin() const
    {
        return _min;
    }

    bool isFull()
    {
        return _dataList.size() == _size ? true : false;
    };

    std::vector<T> getData() const
    {
        return _dataList;
    }

private:

    std::vector<T>  _dataList;
    uint32_t _size;
    double _average;
    double _stdVar;
    double _var;
    typename SummaryTypeGeter<T>::SummaryType _summary;
    T _max;
    T _min;
    bool _needStd;
};

}
