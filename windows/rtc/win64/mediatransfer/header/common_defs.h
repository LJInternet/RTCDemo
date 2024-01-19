//#pragma once
#ifndef COMMON_DEFS
#define COMMON_DEFS


#include "basestone.h"

namespace LJ
{

/**
 * Logger type.
 */
enum LogType
{
    LOG_TYPE_DEFAULT,
    LOG_TYPE_UDP
};

/**
 * Log level.
 */
enum LogLevel
{
    LOG_CLOSE = -1,
    LOG_ERROR,
    LOG_WARN,
    LOG_INFO,
    LOG_DEBUG
};

static const int RETURN_OK = 0;
static const int RETURN_TIMEOUT = 1;
static const int RETURN_ERROR = -1;


const unsigned int LOG_FILE_SIZE = 3000000;
const unsigned int LOG_FILE_COUNT = 4;
}

#endif // COMMON_DEFS
