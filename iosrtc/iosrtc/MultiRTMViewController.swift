//
//  MultiRTMViewController.swift
//  RtcDemo
//
//  Created by fancyjing on 2024/4/11.
//

import UIKit
import RtmSdk

class MultiRTMViewController : UIViewController, IRtmMsgDelegate,IRtmExEventDelegate {
    public func onJoinChannelFail(result: Int32, msg: String) {
        showStr(tempStr: "RTMEx onJoinChannelFail result \(result) msg \(msg)")
    }
    
    public func onLeveChannelFail(result: Int32, msg: String) {
        showStr(tempStr: "RTMEx onLeveChannelFail result \(result) msg \(msg)")
    }
    
    public func onUserJoined(userId: UInt64) {
        showStr(tempStr: "RTMEx onUserJoined userId \(userId)")
    }
    
    public func onUserOffLine(userId: UInt64) {
        showStr(tempStr: "RTMEx onUserOffLine userId \(userId)")
    }
    
    public func onJoinChannelSuccess() {
        showStr(tempStr: "RTMEx onJoinChannelSuccess")
    }
    
    public func onLeveChannelSuccess() {
        showStr(tempStr: "RTMEx onLeveChannelSuccess")
    }
    

    public func onLinkStatus(status : Int32) {
        showStr(tempStr: "RTMEx onLinkStatus status \(status)")
    }
    
    
    private var rtmEngine : RTMEngineEx? = nil
    
    private var push : Bool = false
    
    private var joined : Bool = false
    
    private var showCount : Int? = 0
    
    @IBOutlet weak var channelTextField: UITextField!
    @IBOutlet weak var pushBtn: UIButton!
    @IBOutlet weak var textView: UITextView!
    var timer: DispatchSourceTimer?
    
    override func viewDidLoad() {
        print("MultiRTM viewDidLoad")
        super.viewDidLoad()
        view.backgroundColor = UIColor.white
        channelTextField.text = GlobalConstants.defaultRTMMultiChannel

        // 创建DispatchQueue定时器，每隔1秒执行一次
        timer = DispatchSource.makeTimerSource(queue: DispatchQueue.global())
        timer?.schedule(deadline: .now(), repeating: .seconds(2))
        timer?.setEventHandler { [weak self] in
            self?.rtmEngine?.sendMsg(msg: "test msg")
        }
       timer?.resume()
        
    }
    
    deinit {
        // 在视图控制器被销毁时停止并取消定时器
        rtmEngine?.leveChannel()
        rtmEngine = nil
        print("MultiRTM deinit")
    }
    
    override func viewDidAppear(_ animated: Bool) {
        print("MultiRTM viewDidAppear")
        super.viewDidAppear(animated)

    }
    
    override func viewWillDisappear(_ animated: Bool) {
        print("MultiRTM viewWillDisappear")
        super.viewWillDisappear(animated)
    }
    
    @IBAction func handleBack(_ sender: UIButton) {
        timer?.cancel()
        rtmEngine?.leveChannel()
        rtmEngine = nil
        self.dismiss(animated: true)
    }
    
    @IBAction func handlePushBtn(_ sender: UIButton) {
        push = !push
        if (push) {
            pushBtn.titleLabel?.text = "推流"
        } else {
            pushBtn.titleLabel?.text = "拉流"
        }
    }
    
    @IBAction func handleJoin(_ sender: UIButton) {
        print("MultiRTM handleJoin")
        if (joined) {
            return
        }
        let config = RTMExConfig()
        config.appId = GlobalConstants.appId
        config.localIp = 0
        config.dataWorkMode = 0
        config.isDebug = 1
        config.token = GlobalConstants.token
        rtmEngine = RTMEngineEx(config: config)
        rtmEngine?.setDebugEvn(isTestEvn: true)
        joined = true

        rtmEngine?.eventDelegate = self
        rtmEngine?.subcribeMsgCallback(msgcallback : self)
        rtmEngine?.joinChannel(uid: GlobalConstants.uid, channelId: channelTextField.text!)
    }
    
    @IBAction func handleLeave(_ sender: UIButton) {
        rtmEngine?.leveChannel()
        joined = false
    }
    
    public func onMsgcallbck(buf: UnsafePointer<CChar>?, size: UInt32, uid : UInt64) {
        let data = Data(bytes: buf!, count: Int(size))
        let str = String(data: data, encoding: .utf8)!
        let tempStr = "RTMEx onMsgcallbck uid \(uid) msg \(str)"
        showStr(tempStr: tempStr)
    }
    
    private func showStr(tempStr : String) {
        DispatchQueue.main.async {[weak self, tempStr] in
            self?.showCount = (self?.showCount ?? 0) + 1;
            if (self?.showCount == 20) {
                self?.textView?.text = ""
                self?.showCount = 0
            }
            self?.textView?.text += tempStr
            self?.textView?.text += "\n"
            
        }
    }
}
