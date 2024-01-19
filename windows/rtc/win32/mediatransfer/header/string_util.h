#pragma once

#include <string>
#include <sstream>
#include <vector>
#include <stdint.h>
#include "common_defs.h"

namespace LJ
{

//namespace patch {
//    template <typename T> 
//    std::string to_string(const T &n) {
//        std::ostringstream& operator<<(std::ostringstream& os, const T &n);
//        std::ostringstream stm ;
//        stm << n ;
//        return stm.str() ;
//    }
//}

/**
 * String utility.
 */
class BASESTONE_EXTERN StringUtil
{
public:
    /**
     * Encode a memory block into a base64 string.
     * @param src source address of memory block.
     * @param size size of memory block.
     * @return base64 string.
     */
    // static std::string base64Encode(const void *src, unsigned int size);

    /**
     * Decode a base64 string into memory.
     * @param str base64 string.
     * @param dst destination address of memory.
     * @param size of memory.
     * @return size of decoded memory.
     */
    // static unsigned int base64Decode(const std::string &str, void *dst, unsigned int size);

    /**
     * Convert int to string
     * @parm number the number need to convert
     * @return the convert string
     */
    static std::string int2String(long number);

    /**
     * Convert long to string
     * @parm number the number need to convert
     * @return the convert string
     */
    static std::string long2String(long long number);

    /**
     * Convert string to integer.
     * @param str string.
     * @return integer.
     */
    static long string2Int(const std::string &str);

    /**
     * Convert bytes to hex string
     * @param ptr pointer to the bytes array
     * @param size the bytes array's size
     * @return the hex string represents the bytes array
     */
    static std::string bytes2HexString(const uint8_t *ptr, size_t size);

    /**
     * Convert bytes to hex string
     * @param ptr pointer to the bytes array
     * @param size the bytes array's size
     * @return the hex string represents the bytes array
     */
    static std::string bytes2SimpleHexString(const uint8_t *ptr, size_t size);
    /**
     * Convert string to byte array
     * @param str input string
     * @return a byte array
     */
    static std::vector<uint8_t> string2Bytes(const std::string &str);

    template<typename T>
    static std::string num2String(T num)
    {
        std::stringstream ss;
        ss << num;
        return ss.str();
    }

    static std::string getUUIDString();

    // /转换为-，+转换为_，并移除\n \r \t等base64非法字符
    // static std::string urlSafeBase64Encode(const std::string &str);

    /**
     * 将版本号字符串拆分成整形数组
     * @param verStr 待拆分版本字符串 --输入
     * @param arr 拆分结果存放容器-- 输出
     * @param maxLen arr容器最大容纳元素个数
     * @return 向arr容器实际写入的元素个数
     */
    static uint8_t versionStr2Arr(const std::string &verStr, uint8_t *arr, uint8_t maxLen);

private:
    StringUtil();
};

}
