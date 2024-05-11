#pragma once

#include <cinttypes>
#include <string>
#include "common_defs.h"
#include "logger.h"

#if defined _WIN32
#define __PRETTY_FUNCTION__ __FUNCTION__
#pragma warning(disable:4005) //disable reporting redefine warnings
#endif

#ifndef LOG
#define LOG(tag,level,fmt,...) \
    Log::log(tag, level, fmt, ##__VA_ARGS__)
#endif // LOG

#define LJ_LOG(level,fmt,...) \
    LOG("LJ_BASE", level, fmt, ##__VA_ARGS__)

#define LJ_LOG_FLUSH(isSync) \
    LJ::Log::flushToDisk(isSync)

namespace LJ
{

class Logger;

/**
 * Log utility class.
 */
class BASESTONE_EXTERN Log
{
public:
    static void enableLogFile(bool enabled);
    /**
     * Print log.
     * @param tag log tag.
     * @param level log level.
     * @param fmt log formatted string.
     */
    static void log(const char *tag, LogLevel level, const char *fmt, ...);

    static void logArgs(const char *tag, LogLevel level, const char *fmt, va_list args);

    static void setLogger(Logger* logger);

    static void flushToDisk(bool isSync);

    /**
     * Print log.
     * @param tag log tag.
     * @param level log level.
     * @param fmt log formatted string.
     * @param vaList variable arguments list.
     */
//    static void log2(LogServer *logServer, const char *tag, LogLevel level, const char *fmt, ...);
    /**
     * Set log level.
     * @param level new log level.
     */
    static void setLogLevel(LogLevel level);
private:
    Log();
    static Logger _kLogger;
    static Logger* _loggerImpl;
    static const unsigned int LOG_BUFFER_LENGTH = 10240;
};

}
