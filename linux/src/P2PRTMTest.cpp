#include <iostream>
#include <cstdlib> // 用于 atoi 函数
#include <cstring> // 用于 strcmp 函数
#include <sstream>
#include <string>
#include "ClientConstants.h"
#include <RudpApi.h>
#include "system_util.h"
#include "date_time.h"

static void onMsgCallback(const char* msg, uint32_t len, uint64_t uid, void* content) {
    printf("onMsgCallback %s \n", std::string(msg, len));
}

static void onEvnCallback(int type, const char* msg, uint32_t len, int result, void* content) {
    printf("onEvnCallback %d %d \n", type, result);
}

int main(int argc, char** argv) {

    int role = 0;
    std::string channelId = "121212";
    uint64_t appId = 1;
    std::string token = "token";
    uint64_t uid = LJ::DateTime::currentTimeMillis();

    for (int i = 1; i < argc; ++i) {
        const char* arg = argv[i]; // 将 std::string 转换为 const char*
        std::istringstream iss(arg); // 使用 const char* 初始化 istringstream
        std::string key;
        std::string value;

        // 以等号分割参数名和值
        std::getline(iss, key, '=');

        // 从等号后的字符开始读取参数值
        std::getline(iss, value);

        // 去除值前的空格
        value.erase(0, value.find_first_not_of(' '));

        // 根据参数名进行赋值
        if (key == "channelId") {
            channelId = value;
        }
        else if (key == "uid") {
            uid = std::stoul(value);
        }
        else if (key == "appId") {
            appId = std::stoul(value);
        }
        else if (key == "token") {
            token = value;
        }
        else if (key == "role") {
            role = std::stoi(value);
        }
    }

    /**
     * @param token
     * @param isDebug 是否是测试环境
     * @param dataWorkMode RTM 的工作模式 DataWorkMode
     * @param uid 用户ID
     * @param appId 用户Appid
     * @param channelId 频道ID
     */

    set_xmtp_debug(true); // 设置为测试环境，必须在joinchannel前调用，否则会崩溃

    RUDPConfig config;
    config.dataWorkMode = SEND_AND_RECV;
    config.token = token.c_str();
    config.role = role;
    config.isDebug = true;
    config.appId = (int)appId;
    config.mode = 0;
    RUDPEngine* m_rudp = rudp_engine_create(&config);
    rudp_engine_join_channel(m_rudp, (uint64_t)uid, channelId.c_str());

    rudp_engine_register_msg_callback(m_rudp, onMsgCallback, nullptr);
    rudp_engine_register_event_callback(m_rudp, onEvnCallback, nullptr);
    int index = 0;
    while (index < 1000) {
        LJ::SystemUtil::sleep(500);
        std::string msgStr = "test";
        rudp_engine_send(m_rudp, msgStr.c_str(), msgStr.size());
    }

    rudp_engine_leave_channel(m_rudp);
    rudp_engine_destroy(m_rudp);

}