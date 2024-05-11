
using LJ.RTC.Common;

public class JLog
{

    private static ILog mLogerImpl;

    public static void Init(ILog logImpl) {
        mLogerImpl = logImpl;
    }

    public static void Debug(string msg)
    {
        if (mLogerImpl != null)
        {
            mLogerImpl.Debug(msg);
        }
    }

    public static void Debug(string tag, string msg)
    {
        if (mLogerImpl != null)
        {
            mLogerImpl.Debug(tag + ":" + msg);
        }
    }

    public static void Error(string msg)
    {
        if (mLogerImpl != null)
        {
            mLogerImpl.Error(msg);
        }
    }

    public static void Error(string tag, string msg)
    {
        if (mLogerImpl != null)
        {
            mLogerImpl.Error(tag + ":" + msg);
        }
    }

    public static void Info(string msg)
    {
        if (mLogerImpl != null)
        {
            mLogerImpl.Info(msg);
        }
    }

    public static void Info(string tag, string msg)
    {
        if (mLogerImpl != null)
        {
            mLogerImpl.Info(tag + ":" + msg);
        }
    }

    public static void Uninit() {
        mLogerImpl = null;
    }
}
