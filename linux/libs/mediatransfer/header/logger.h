#pragma once
#ifndef BASE_LOGGER_H
#define BASE_LOGGER_H
#include <cstdio>
#include <string>
#include "common_defs.h"

namespace LJ
{
/**
 * Logger is a class that print log in specific way.
 */
class BASESTONE_EXTERN Logger
{
public:
    /**
     * Default constructor.
     */
    Logger();

    /**
     * Destructor.
     */
    virtual ~Logger() = default;

    virtual /**
     * Print log.
     * @param tag log tag.
     * @param level log level.
     * @param str log string.
     */
    void log(const char *tag, LogLevel level, std::string &log, uint8_t offset, long threadID);

    virtual void enableLogFile(bool enabled);

    virtual void flushToDisk(bool isSync);
};

}
#endif
