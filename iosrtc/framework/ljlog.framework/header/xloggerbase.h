// Tencent is pleased to support the open source community by making Mars available.
// Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.

// Licensed under the MIT License (the "License"); you may not use this file except in 
// compliance with the License. You may obtain a copy of the License at
// http://opensource.org/licenses/MIT

// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing permissions and
// limitations under the License.


/*
 ============================================================================
 ============================================================================
 */

#ifndef XLOGGERBASE_H_
#define XLOGGERBASE_H_

#include <sys/time.h>
#include <time.h>
#include <stdarg.h>
#include <stdint.h>
#include <compiler_util.h>

#ifdef __cplusplus
extern "C" {
#endif

XLOG_EXTERN typedef enum {
    kLevelAll = 0,
    kLevelVerbose = 0,
    kLevelDebug,    // Detailed information on the flow through the system.
    kLevelInfo,     // Interesting runtime events (startup/shutdown), should be conservative and keep to a minimum.
    kLevelWarn,     // Other runtime situations that are undesirable or unexpected, but not necessarily "wrong".
    kLevelError,    // Other runtime errors or unexpected conditions.
    kLevelFatal,    // Severe errors that cause premature termination.
    kLevelNone,     // Special level used to disable all log messages.
} TLogLevel;

XLOG_EXTERN typedef struct XLoggerInfo_t {
    TLogLevel level;
    const char* tag;
    const char* filename;
    const char* func_name;
    int line;

    struct timeval timeval;
    intmax_t pid;
    intmax_t tid;
    intmax_t maintid;
} XLoggerInfo;

extern intmax_t ljlogger_pid();
extern intmax_t ljlogger_tid();
extern intmax_t ljlogger_amintid();
typedef void (*ljlogger_appender_t)(const XLoggerInfo* _info, const char* _log);
extern const char* ljlogger_dump(const void* _dumpbuffer, size_t _len);

TLogLevel XLOG_EXTERN  xlogger_Level();
XLOG_EXTERN void ljlogger_SetLevel(TLogLevel _level);
XLOG_EXTERN int  ljlogger_IsEnabledFor(TLogLevel _level);
XLOG_EXTERN ljlogger_appender_t ljlogger_SetAppender(ljlogger_appender_t _appender);

// no level filter
#ifdef __GNUC__
__attribute__((__format__(printf, 3, 4)))
#endif
XLOG_EXTERN void  ljlogger_AssertP(const XLoggerInfo* _info, const char* _expression, const char* _format, ...);
XLOG_EXTERN void  ljlogger_Assert(const XLoggerInfo* _info, const char* _expression, const char* _log);
#ifdef __GNUC__
__attribute__((__format__(printf, 2, 0)))
#endif
XLOG_EXTERN void        ljlogger_VPrint(const XLoggerInfo* _info, const char* _format, va_list _list);
#ifdef __GNUC__
__attribute__((__format__(printf, 2, 3)))
#endif
XLOG_EXTERN void ljlogger_Print(const XLoggerInfo* _info, const char* _format, ...);
XLOG_EXTERN void ljlogger_Write(const XLoggerInfo* _info, const char* _log);

#ifdef __cplusplus
}
#endif

#endif
