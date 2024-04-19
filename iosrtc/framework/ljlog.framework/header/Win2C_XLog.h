//
// Created by Administrator on 2023/4/12.
//

#ifndef LJSDK_WIN2C_XLOG_H
#define LJSDK_WIN2C_XLOG_H
#include "compiler_util.h"
#ifdef __cplusplus
extern "C" {
#endif
	XLOG_EXTERN void FLogInit(int level, int mode, char* logDir, char* nameprefix, int cacheDays, int dirLen, int tagLen, int enableLog2File);
	XLOG_EXTERN void FLogDestroy();
	XLOG_EXTERN void FLogFlush(int isSync);
	XLOG_EXTERN void FLogWritLog(int level, char* logStr, int logLen, char* tag, int tagLen, long tid, int pid);
	XLOG_EXTERN int GetLogLevel();
	XLOG_EXTERN void SetLogMode(int mode);
	XLOG_EXTERN void SetConsoleLogOpen(int enable);
	XLOG_EXTERN void SetMaxFileSize(long fileSize);
	XLOG_EXTERN void SetMaxAliveTime(long time);
#ifdef __cplusplus
}
#endif
#endif //LJSDK_WIN2C_XLOG_H
