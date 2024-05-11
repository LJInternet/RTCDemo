//
// Created by Administrator on 2022/11/10.
//

#include "log.h"

#ifndef LJSDK_RUDP_COMMON_H
#define LJSDK_RUDP_COMMON_H

#define RUDP_LOG(level,fmt,...) \
    LJ::LOG("LJ_RUDP", level, fmt, ##__VA_ARGS__)

#define LOG_ERROR LJ::LOG_ERROR
#define LOG_WARN LJ::LOG_WARN
#define LOG_INFO LJ::LOG_INFO
#define LOG_DEBUG LJ::LOG_DEBUG

#endif //LJSDK_RUDP_COMMON_H
