package com.linjing.rtc.demo.camera.helper;


import com.linjing.rtc.demo.camera.bean.ResolutionParam;

/**
 * Created by cbw on 2021/4/21
 */
public class ResolutionOptions {

    public static ResolutionParam LowParam = new ResolutionParam.Builder()
            .resolution(1)
            .videoWidth(544)
            .videoHeight(960)
            .videoFrameRate(30)
            .videoBitrate(750 * 1000)
            .maxVideoBitrate(750 * 1000)
            .minVideoBitrate(200 * 1000)
            .name("标清")
            .build();

    public static ResolutionParam HighParam = new ResolutionParam.Builder()
            .resolution(2)
            .videoWidth(544)
            .videoHeight(960)
            .videoFrameRate(30)
            .videoBitrate(1200 * 1000)
            .maxVideoBitrate(1200 * 1000)
            .minVideoBitrate(450 * 1000)
            .name("高清")
            .build();

    public static ResolutionParam SuperParam = new ResolutionParam.Builder()
            .resolution(4)
            .videoWidth(720)
            .videoHeight(1280)
            .videoFrameRate(30)
            .videoBitrate(2000 * 1000)
            .maxVideoBitrate(2000 * 1000)
            .minVideoBitrate(450 * 1000)
            .name("超清")
            .build();

    public static ResolutionParam Blue3mParam = new ResolutionParam.Builder()
            .resolution(8)
            .videoWidth(1080)
            .videoHeight(1920)
            .videoFrameRate(30)
            .videoBitrate(3000 * 1000)
            .maxVideoBitrate(3000 * 1000)
            .minVideoBitrate(450 * 1000)
            .name("1080p 蓝光3M")
            .tips("上传速度需大于4Mbps")
            .build();

    public static ResolutionParam Blue4mParam = new ResolutionParam.Builder()
            .resolution(16)
            .videoWidth(1080)
            .videoHeight(1920)
            .videoFrameRate(30)
            .videoBitrate(4000 * 1000)
            .maxVideoBitrate(4000 * 1000)
            .minVideoBitrate(3000 * 1000)
            .name("1080p 蓝光4M")
            .tips("上传速度需大于6Mbps")
            .build();

    public static ResolutionParam screenLow = new ResolutionParam.Builder()
            .resolution(2)
            .videoWidth(360)
            .videoHeight(640)
            .videoFrameRate(30)
            .videoBitrate(1000 * 1000)
            .maxVideoBitrate(1000 * 1000)
            .minVideoBitrate(450 * 1000)
            .name("360P")
            .tips("")
            .build();

    public static ResolutionParam screenMid = new ResolutionParam.Builder()
            .resolution(2)
            .videoWidth(720)
            .videoHeight(1280)
            .videoFrameRate(30)
            .videoBitrate(2000 * 1000)
            .maxVideoBitrate(2000 * 1000)
            .minVideoBitrate(450 * 1000)
            .name("360P")
            .tips("")
            .build();


}
