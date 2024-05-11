#pragma once

#include <map>
#include <set>
#include <vector>
#include <list>
#include <string>
#include "shared_ptr.h"
#include "ring_queue.h"
#include "common_defs.h"

namespace Json{
    class Value;
}

namespace LJ
{

/**
 * Bundle is a class for date encapsulation using key-value pair.
 */
class BASESTONE_EXTERN Bundle
{
public:
    /**
     * Bundle value type.
     */
    enum ValueType
    {
        VALUE_NONE=0,
        VALUE_INTEGER,
        VALUE_UINTEGER,
        VALUE_DECIMAL,
        VALUE_STRING,
        VALUE_BOOLEAN,
        VALUE_ARRAY,
        VALUE_OBJECT,
    };

    /**
     * Default constructor.
     */
    Bundle();

    /**
     * Copy constructor.
     */
    Bundle(const Bundle &dictionary);

    Bundle(const std::string & json);

    /**
     * Destructor.
     */
    virtual ~Bundle();

    void * getData(){return _jsonObj;}
    const void * getData() const{return _jsonObj;}
    /**
     * Get the count of values in dictionary.
     * @return the count of values.
     */
    unsigned int size() const;

    /**
     * Get all keys in dictionary.
     * @return all keys in dictionary.
     */
    std::vector<std::string> allKeys() const;

    /**
     * Check if has key in dictionary.
     * @param key key to be checked.
     * @return true if hasing this key.
     */
    bool hasKey(const std::string &key) const;

    /**
     * Get the value type of a key.
     * @param key key to be checked.
     * @return the value type of a key. VALUE_NONE if the key is not in the
     * dictionary.
     */
    ValueType getType(const std::string &key) const;

    /**
     * Set an integer value into dictionary.
     * @param key key of value.
     * @param value integer value.
     */
    void setInteger(const std::string &key, long long value);

    /**
     * Get an integer value from dictionary.
     * @param key key of value.
     * @return integer value.
     */
    long long getInteger(const std::string &key) const;

    /**
     * Set a string value into dictionary.
     * @param key key of value.
     * @param value string value.
     */
    void setString(const std::string &key, const std::string &value);

    /**
     * Get a string value from dictionary.
     * @param key key of value.
     * @return string value.
     */
    std::string getString(const std::string &key) const;

    /**
     * Set a decimal value into dictionary.
     * @param key key of value.
     * @param value decimal value.
     */
    void setDecimal(const std::string &key, double value);

    /**
     * Get a decimal value from dictionary.
     * @param key key of value.
     * @return decimal value.
     */
    double getDecimal(const std::string &key) const;

    /**
     * Set a boolean value into dictionary.
     * @param key key of value.
     * @param value boolean value.
     */
    void setBoolean(const std::string &key, bool value);

    /**
     * Get a boolean value from dictionary.
     * @param key key of value.
     * @return boolean value.
     */
    bool getBoolean(const std::string &key) const;

    /**
     * Set a Bundle object into dictionary.
     * @param key key of value.
     * @param value Bundle object.
     */
    void setObject(const std::string &key, const Bundle &value);

    /**
     * Get a Bundle object from dictionary.
     * @param key key of value.
     * @return Bundle object.
     */
    Bundle getObject(const std::string &key) const;

    /**
     * Set an integer array into dictionary.
     * @param key key of value.
     * @param value integer array.
     */
    void setIntegerArray(const std::string &key, const std::vector<long long> &value);

    void setIntegerList(const std::string &key, const std::list<long long> &value);

    void setIntegerSet(const std::string &key, const std::set<long long> &value);

    /**
     * Set an integer array into dictionary.
     * @param key key of value.
     * @param value integer array.
     */
    void setIntegerArray(const std::string &key, const std::vector<int32_t > &value);
    
    void setIntegerArray(const std::string &key, const std::vector<int16_t > &value);

    void setFloatArray(const std::string &key, const std::vector<float > &value);

    void setIntegerList(const std::string &key, const std::list<int32_t > &value);

    void setIntegerSet(const std::string &key, const std::set<int32_t> &value);

    void setIntergerRingQueue(const std::string &key, const RingQueue<long long> &value);
    /**
     * Get an integer array from dictionary.
     * @param key key of value.
     * @return integer array.
     */
    std::vector<long long> getIntegerArray(const std::string &key) const;

    /**
     * Set a string array into dictionary.
     * @param key key of value.
     * @param value string array.
     */
    void setStringArray(const std::string &key, const std::vector<std::string> &value);

    /**
     * Get a string array from dictionary.
     * @param key key of value.
     * @return string array.
     */
    std::vector<std::string> getStringArray(const std::string &key) const;

    /**
     * Get a string array from dictionary.
     * @param key key of value.
     * @return string array.
     */
    std::vector<std::string> getStringArray() const;

    /**
     * Set a decimal array into dictionary.
     * @param key key of value.
     * @param value decimal array.
     */
    void setDecimalArray(const std::string &key, const std::vector<double> &value);

    /**
     * Get a decimal array from dictionary.
     * @param key key of value.
     * @return decimal array.
     */
    std::vector<double> getDecimalArray(const std::string &key) const;

    /**
     * Set a boolean array into dictionary.
     * @param key key of value.
     * @param value boolean array.
     */
    void setBooleanArray(const std::string &key, const std::vector<bool> &value);

    /**
     * Get a boolean array from dictionary.
     * @param key key of value.
     * @return boolean array.
     */
    std::vector<bool> getBooleanArray(const std::string &key) const;

    /**
     * Set a Bundle object array into dictionary.
     * @param key key of value.
     * @param value Bundle object array.
     */
    void setObjectArray(const std::string &key, const std::vector<Bundle> &value);

    /**
     * Set a Bundle object array into dictionary.
     * @param key key of value.
     * @param value Bundle object array.
     */
    void setObjectList(const std::string &key, const std::list<Bundle> &value);

    /**
     * Set a Bundle object array into dictionary.
     * @param key key of value.
     * @param value Bundle object array.
     */
    void setObjectRingQueue(const std::string &key, const RingQueue<Bundle> &value);

    /**
     * Get a Bundle object array from dictionary.
     * @param key key of value.
     * @return Bundle object array.
     */
    std::vector<Bundle> getObjectArray(const std::string &key) const;

    /**
     * Assign to another Bundle object.
     * @param dictionary another Bundle object.
     * @return the new Bundle object.
     */
    Bundle &operator=(const Bundle &dictionary);

    /**
     * Clear dictionary.
     */
    void clear();

    bool fromJson(const std::string & str);

    std::string toString() const;

    void remove(const std::string &key);

    bool fromFile(const std::string & strFilename);

    bool toFile(const std::string & strFilename) const;
private:
    Bundle(const Json::Value & jsonValue);

    const Json::Value & data(const std::string & key) const;
    Json::Value & data(const std::string & key);
    const Json::Value & self() const;
    Json::Value & self();

    Json::Value *_jsonObj;

};

}
