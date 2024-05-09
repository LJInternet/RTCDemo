//
//  MultiRTCViewController.swift
//  RtcDemo
//
//  Created by fancyjing on 2024/4/29.
//

import Foundation
import UIKit
import RtcSDK
struct Stack<Element> {
    private var elements: [Element] = []

    mutating func push(_ element: Element) {
        elements.append(element)
    }

    mutating func pop() -> Element? {
        return elements.popLast()
    }

    func peek() -> Element? {
        return elements.last
    }

    var isEmpty: Bool {
        return elements.isEmpty
    }

    var count: Int {
        return elements.count
    }
};

class MultiRTCViewController : UIViewController, ILJChannelEventHandler {
    
    @IBOutlet weak var channelTextField: UITextField!
    @IBOutlet var previewView: LJPreviewView!
    @IBOutlet weak var videoSeatViews : UIView!

    //var remoteViewsInfo : Stack<CGRect> = Stack<CGRect>()
    var remoteViewsInfo = [Int: CGRect]()
    var remoteViews = [Int64: LJRemoteView]()
    var rtcEngine : LJRtcEngine?
    var joined : Bool = false
    
    var _channel : LJChannel?
    
    override func viewDidLoad() {
        print("MultiRTC viewDidLoad")
        super.viewDidLoad()
        view.backgroundColor = UIColor.white
        channelTextField.text = "954523222"
        
        initRemoteViewInfo()
        
        let folderName = "rtclog" // 自定义文件夹名称
        let documentsURL = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask).first!
        let folderURL = documentsURL.appendingPathComponent(folderName)
        
        let rtcConfig = RtcEngineConfig()
        rtcConfig.enableNativeLog = true
        rtcConfig.logPath = folderURL.path
        rtcEngine = LJRtcEngine.sharedEngine(c : rtcConfig)
        let engine = rtcEngine!
        engine.setDebug(debug: 1)
        engine.enableVideo()
        engine.disableAudio()
        
        initPreviewView()
        engine.setupLocalVideo(view: previewView)
        //engine.setVideoDecodeType(decodeType : VideoDecodeType.HARD.rawValue, isLowLatency : true);
    }
    
    deinit {
        print("MultiRTC deinit")
        for (_, value) in remoteViews {
            value.removeFromSuperview()
        }
        remoteViews.removeAll()
        if (_channel != nil) {
            _channel!.leaveChannel();
            _channel!.releaseChannel();
            _channel = nil;
        }
        rtcEngine?.leaveChannel()
        LJRtcEngine.destroy()
        rtcEngine = nil
    }
    
    override func viewDidAppear(_ animated: Bool) {
        print("ViewController viewDidAppear")
        super.viewDidAppear(animated)
        rtcEngine?.startPreview()
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        print("ViewController viewDidAppear")
        rtcEngine?.stopPreview()
    }
    
    func initRemoteViewInfo() {
        let screenWidth = UIScreen.main.bounds.width
        //这里创建三个远端的视频显示视图的宽高信息，加上preivew是四方格
        // 因此第一个是preview 0～screenwidth/2， 0～160
        // 因此第二个是preview screenwidth/2～screenwidth， 0～160
        // 因此第三个是preview 0～screenwidth/2， 160～320
        // 因此第三个是preview screenwidth/2～screenwidth， 160～320
        remoteViewsInfo[0] = CGRect(x: screenWidth/2, y: 0, width: screenWidth / 2, height: 160)
        remoteViewsInfo[1] = CGRect(x: 0, y: 160, width: screenWidth / 2, height: 160)
        remoteViewsInfo[2] = CGRect(x: screenWidth/2, y: 160, width: screenWidth / 2, height: 160)

        
    }
    
    func initPreviewView() {
        previewView = LJPreviewView()
        
        // 禁用AutoresizingMask，以便使用Auto Layout
        previewView.translatesAutoresizingMaskIntoConstraints = false

        // 将新视图添加到父视图中
        videoSeatViews.addSubview(previewView)
        // 添加约束
        NSLayoutConstraint.activate([
            previewView.widthAnchor.constraint(equalTo: self.view.widthAnchor, multiplier: 0.5), // 宽度为父视图宽度的一半
            previewView.heightAnchor.constraint(equalToConstant: 160)
        ])
        previewView.autorotate = true;
    }
    
    func join(){
        if joined {
            print("----already joined, ignore action")
            return
        }

        _channel = rtcEngine?.createChannel(channelId: channelTextField.text!, uid: Int64(GlobalConstants.uid))
        let channelMediaOptions : ChannelMediaOptions = ChannelMediaOptions()
        channelMediaOptions.publishMicrophoneTrack = true;
        channelMediaOptions.publishCameraTrack = true;
        _channel?.joinChannel(token: GlobalConstants.token, appId: Int64(GlobalConstants.appId),
                              uid: Int64(GlobalConstants.uid), options: channelMediaOptions);
        _channel?.setRtcChannelEventHandler(eventHandler: self);
        joined = true
    }
    
                
    @IBAction func handleJoinChannel(_ sender: UIButton) {
        FLog.info("handleJoinChannel")
        join()
    }
    
    @IBAction func handleLeaveChannel(_ sender: UIButton) {
        FLog.info("handleLeaveChannel")
        if (_channel != nil) {
            _channel!.leaveChannel();
            _channel!.releaseChannel();
            _channel = nil;
        }
        for (_, value) in remoteViews {
            value.removeFromSuperview()
        }
        remoteViews.removeAll()
        joined = false;
    }
    
    func onNetQuality(_ ljChannel:LJChannel, uid: Int64, mLocalQuality: Int32, mRemoteQuality: Int32) {
        //FLog.info("MultiRtc onNetQuality uid=\(uid) mLocalQuality=\(mLocalQuality) mRemoteQuality=\(mRemoteQuality)")
    }
    
    func onJoinChannelSuccess(_ channelId: String, uid: Int64, elapsed: Int64) {
        FLog.info("MultiRtc onJoinChannelSuccess channelId=\(channelId) uid=\(uid) elapsed=\(elapsed)")
    }
    
    func onLeaveChannelSuccess(_ ljChannel: LJChannel) {
        FLog.info("MultiRtc onLeaveChannelSuccess ljChannel=\(ljChannel.getChannelId())")
    }
    
    func onLinkStatus(_ ljChannel: LJChannel, result: Int32) {
        FLog.info("MultiRtc onLinkStatus ljChannel=\(ljChannel.getChannelId()) result=\(result)")
    }
    
    func onUserJoined(_ ljChannel: LJChannel, uid: Int64, elapsed: Int64) {
        FLog.info("MultiRtc onUserJoined ljChannel=\(ljChannel.getChannelId()) uid=\(uid)")
        DispatchQueue.main.async {
            // 在主线程中更新UI
            if (self.remoteViews.keys.contains(uid)) {
                return
            }
            let counts = self.remoteViews.count
            if (counts >= 3) {
                return
            }
            let rect = self.remoteViewsInfo[counts]
            let remoteView = LJRemoteView()
            // 禁用AutoresizingMask，以便使用Auto Layout
            remoteView.translatesAutoresizingMaskIntoConstraints = false

            // 将新视图添加到父视图中
            self.videoSeatViews.addSubview(remoteView)
            // 添加约束
            NSLayoutConstraint.activate([
                remoteView.topAnchor.constraint(equalTo: self.videoSeatViews.topAnchor, constant: rect!.minY),
                remoteView.leadingAnchor.constraint(equalTo: self.videoSeatViews.leadingAnchor, constant: rect!.minX),
                remoteView.widthAnchor.constraint(equalTo: self.view.widthAnchor, multiplier: 0.5), // 宽度为父视图宽度的一半
                remoteView.heightAnchor.constraint(equalToConstant: 160)
            ])
            self.remoteViews[uid] = remoteView
            ljChannel.setForMultiChannelUser(videoView: remoteView, uid: uid, fps: 60)
        }
    }
    
    func onUserOffLine(_ ljChannel: LJChannel, uid: Int64) {
        FLog.info("MultiRtc onUserOffLine ljChannel=\(ljChannel.getChannelId()) uid=\(uid)")
        DispatchQueue.main.async {
            // 在主线程中更新UI
            if (!self.remoteViews.keys.contains(uid)) {
                return
            }
            let remoteView = self.remoteViews.removeValue(forKey: uid)
            remoteView?.removeFromSuperview()
        }
    }
    
    func onFirstRemoteVideoFrameDecode(_ ljChannel: LJChannel, uid: Int64, width: Int32, height: Int32, joinTime: Int64) {
        FLog.info("MultiRtc onFirstRemoteVideoFrameDecode ljChannel=\(ljChannel.getChannelId()) uid=\(uid) w=\(width) h=\(height)")
    }
    
    func onVideoSizeChange(_ ljChannel: LJChannel, uid: Int64, width: Int32, height: Int32) {
        FLog.info("MultiRtc onVideoSizeChange ljChannel=\(ljChannel.getChannelId()) uid=\(uid) w=\(width) h=\(height)")
    }
    
    @IBAction func handleBack(_ sender: UIButton) {
        print("MultiRTC handleBack")
        rtcEngine?.stopPreview()
        for (_, value) in remoteViews {
            value.removeFromSuperview()
        }
        remoteViews.removeAll()
        if (_channel != nil) {
            _channel!.leaveChannel();
            _channel!.releaseChannel();
            _channel = nil;
        }
        rtcEngine?.leaveChannel()
        LJRtcEngine.destroy()
        rtcEngine = nil
        self.dismiss(animated: true)
    }

}
