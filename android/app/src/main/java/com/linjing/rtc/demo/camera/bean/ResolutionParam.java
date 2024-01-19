package com.linjing.rtc.demo.camera.bean;

/**
 * @author: zhengweicheng
 */
public class ResolutionParam implements Cloneable {

    /**
     * 分辨率标识->1流畅，2高清，4超清，8蓝光3M，16蓝光4M，32其他
     */
    private int mResolution;
    private String mResolutionName;
    /**
     * 视频宽高
     */
    private int mVideoWidth;
    private int mVideoHeight;
    /**
     * 编码宽高
     */
    private int mEncodeWidth;
    private int mEncodeHeight;
    /**
     * 视频码率
     */
    private int mVideoBitrate;
    private int mMaxVideoBitrate;
    private int mMinVideoBitrate;
    private int mVideoFrameRate;
    private boolean isLand;
    private String mTips;

    private ResolutionParam(Builder builder) {
        mResolution = builder.mResolution;
        mVideoWidth = builder.mVideoWidth;
        mVideoHeight = builder.mVideoHeight;
        mEncodeWidth = builder.mEncodeWidth == 0 ? builder.mVideoWidth : builder.mEncodeWidth;
        mEncodeHeight = builder.mEncodeHeight == 0 ? builder.mVideoHeight : builder.mEncodeHeight;
        mVideoBitrate = builder.mVideoBitrate;
        mMaxVideoBitrate = builder.mMaxVideoBitrate;
        mMinVideoBitrate = builder.mMinVideoBitrate;
        mVideoFrameRate = builder.mVideoFrameRate;
        mResolutionName = builder.mResolutionName;
        mTips = builder.mTips;
    }

    public int getResolution() {
        return mResolution;
    }

    public String getResolutionName() {
        return mResolutionName;
    }

    public void setLand(boolean land) {
        isLand = land;
    }

    public int getVideoWidth() {
        return mVideoWidth;
    }

    public int getVideoHeight() {
        return mVideoHeight;
    }

    public int getEncodeWidth() {
        return mEncodeWidth;
    }

    public int getEncodeHeight() {
        return mEncodeHeight;
    }

    public int getVideoBitrate() {
        return mVideoBitrate;
    }

    public int getMaxVideoBitrate() {
        return mMaxVideoBitrate;
    }

    public int getMinVideoBitrate() {
        return mMinVideoBitrate;
    }

    public int getVideoFrameRate() {
        return mVideoFrameRate;
    }

    public String getTips() {
        return mTips;
    }

    public void setResolution(int mResolution) {
        this.mResolution = mResolution;
    }

    public void setResolutionName(String mResolutionName) {
        this.mResolutionName = mResolutionName;
    }

    public void setVideoWidth(int mVideoWidth) {
        this.mVideoWidth = mVideoWidth;
    }

    public void setVideoHeight(int mVideoHeight) {
        this.mVideoHeight = mVideoHeight;
    }

    public void setEncodeWidth(int mEncodeWidth) {
        this.mEncodeWidth = mEncodeWidth;
    }

    public void setEncodeHeight(int mEncodeHeight) {
        this.mEncodeHeight = mEncodeHeight;
    }

    public void setVideoBitrate(int mVideoBitrate) {
        this.mVideoBitrate = mVideoBitrate;
    }

    public void setMaxVideoBitrate(int mMaxVideoBitrate) {
        this.mMaxVideoBitrate = mMaxVideoBitrate;
    }

    public void setMinVideoBitrate(int mMinVideoBitrate) {
        this.mMinVideoBitrate = mMinVideoBitrate;
    }

    public void setVideoFrameRate(int mVideoFrameRate) {
        this.mVideoFrameRate = mVideoFrameRate;
    }

    public void setTips(String mTips) {
        this.mTips = mTips;
    }

    public static class Builder {
        private int mResolution;
        private int mVideoWidth;
        private int mVideoHeight;
        private int mVideoBitrate;
        private int mMaxVideoBitrate;
        private int mMinVideoBitrate;
        private int mVideoFrameRate;
        private int mEncodeWidth;
        private int mEncodeHeight;
        private String mResolutionName;
        private String mTips;


        public Builder() {
        }

        public Builder videoWidth(int val) {
            this.mVideoWidth = val;
            return this;
        }

        public Builder videoHeight(int val) {
            this.mVideoHeight = val;
            return this;
        }

        public Builder videoBitrate(int val) {
            this.mVideoBitrate = val;
            return this;
        }

        public Builder maxVideoBitrate(int val) {
            this.mMaxVideoBitrate = val;
            return this;
        }


        public Builder minVideoBitrate(int val) {
            this.mMinVideoBitrate = val;
            return this;
        }

        public Builder videoFrameRate(int val) {
            this.mVideoFrameRate = val;
            return this;
        }

        public Builder resolution(int val) {
            this.mResolution = val;
            return this;
        }


        public Builder name(String name) {
            this.mResolutionName = name;
            return this;
        }

        public Builder tips(String val) {
            this.mTips = val;
            return this;
        }

        public Builder encodeWidth(int val) {
            this.mEncodeWidth = val;
            return this;
        }

        public Builder encodeHeight(int val) {
            this.mEncodeHeight = val;
            return this;
        }

        public ResolutionParam build() {
            return new ResolutionParam(this);
        }

    }

    public int videoWidth() {
        if (isLand) {
            return Math.max(mVideoWidth, mVideoHeight);
        } else {
            return Math.min(mVideoWidth, mVideoHeight);
        }
    }

    public int videoHeight() {
        if (isLand) {
            return Math.min(mVideoWidth, mVideoHeight);
        } else {
            return Math.max(mVideoWidth, mVideoHeight);
        }
    }

    @Override
    public ResolutionParam clone() {
        ResolutionParam lp = null;
        try {
            lp = (ResolutionParam) super.clone();
        } catch (CloneNotSupportedException e) {
            e.printStackTrace();
        }
        return lp;
    }
}
