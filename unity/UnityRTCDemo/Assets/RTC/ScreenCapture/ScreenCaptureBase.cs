using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC
{
    public class ScreenCaptureParam
    {
        public int enableAudio = -1; // 1 表示使用IOS系统界面上的麦克风采集 2表示使用主进程的麦克风采集
        public int enableVideo = -1;
        public int videoWidth = 720;
        public int videoHeight = 1280;
        public int videoFps = 15;
        public int videoBitrate = 12000;

        public ScreenCaptureParam() {
        }
    };
}
