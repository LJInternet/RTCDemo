//
// Created by Administrator on 2023/4/13.
//

#ifndef LJSDK_WIN2CFEEBACK_H
#define LJSDK_WIN2CFEEBACK_H

#include "FeedbackConstant.h"
#ifdef __cplusplus
extern "C" {
#endif
	FEEDBACK_EXTERN void feedback_init(char* token, char* host, int port, bool isDebug);
	FEEDBACK_EXTERN void feedback_destroy();
	FEEDBACK_EXTERN void send_feedback(char* title, char* context, char* filePath, char* liveId, int pathLen);
	FEEDBACK_EXTERN void set_common_attrs(char* jsonStr, int len);
	FEEDBACK_EXTERN void subscribe_feedback_result(feedback_callback cb);
#ifdef __cplusplus
}
#endif
#endif //LJSDK_WIN2CFEEBACK_H
