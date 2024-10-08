//
//  ViewController.swift
//  RtcDemo
//
//  Created by fancyjing on 2023/3/22.
//

import UIKit
import AVKit
import MediaPlayer
import RtcSDK

class VideoPreviewViewController: UIViewController {
    
    var url: URL?
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        if let url = self.url {
            let player = AVPlayerViewController()
            player.player = AVPlayer(url: url)
            player.view.frame = self.view.bounds
            
            self.view.addSubview(player.view)
            self.addChild(player)
            
            player.player?.play()
        }
    }
    
    override var prefersStatusBarHidden: Bool {
        return true
    }
    
    @IBAction func handleCancel(_ sender: Any) {
        self.dismiss(animated: true, completion: nil)
    }
    
    @IBAction func handleSave(_ sender: Any) {
        if let url = self.url {
            UISaveVideoAtPathToSavedPhotosAlbum(url.path, self, #selector(handleDidCompleteSavingToLibrary(path:error:contextInfo:)), nil)
        }
    }
    
    @objc func handleDidCompleteSavingToLibrary(path: String?, error: Error?, contextInfo: Any?) {
        self.dismiss(animated: true, completion: nil)
    }
}

class VideoSettingsViewController: UITableViewController {
    
//    var previewView: CKFPreviewView!
    
    @IBOutlet weak var cameraSegmentControl: UISegmentedControl!
    @IBOutlet weak var flashSegmentControl: UISegmentedControl!
    @IBOutlet weak var gridSegmentControl: UISegmentedControl!
    
    @IBAction func handleCamera(_ sender: UISegmentedControl) {
//        if let session = self.previewView.session as? LJCaptureSession {
//            session.cameraPosition = sender.selectedSegmentIndex == 0 ? .back : .front
//        }
    }
    
    @IBAction func handleFlash(_ sender: UISegmentedControl) {
//        if let session = self.previewView.session as? CKFVideoSession {
//            let values: [CKFVideoSession.FlashMode] = [.auto, .on, .off]
//            session.flashMode = values[sender.selectedSegmentIndex]
//        }
    }
    
    @IBAction func handleGrid(_ sender: UISegmentedControl) {
//        self.previewView.showGrid = sender.selectedSegmentIndex == 1
    }
    
    @IBAction func handleMode(_ sender: UISegmentedControl) {
//        if let session = self.previewView.session as? CKFVideoSession {
//            let modes = [(1920, 1080, 30), (1920, 1080, 60), (3840, 2160, 30)]
//            let mode = modes[sender.selectedSegmentIndex]
//            session.setWidth(mode.0, height: mode.1, frameRate: mode.2)
//        }
    }
}

class ViewController: UIViewController {
    
    @IBOutlet weak var zoomLabel: UILabel!
    
    var rtcEngine : LJRtcEngine?
    var joined : Bool = false
    
    func didChangeValue(session: LJCaptureSessionBase, value: Any, key: String) {
        if key == "zoom" {
            self.zoomLabel.text = String(format: "%.1fx", value as! Double)
        }
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
//        if let vc = segue.destination as? VideoSettingsViewController {
//            vc.previewView = self.previewView
//        } else if let nvc = segue.destination as? UINavigationController, let vc = nvc.children.first as? VideoPreviewViewController {
//            vc.url = sender as? URL
//        }
    }
    
    @IBOutlet weak var previewView: LJPreviewView!
    @IBOutlet weak var remoteView : UIView!

    
//    var volumeView : MPVolumeView!
    
    override func viewDidLoad() {
        print("ViewController viewDidLoad")
        super.viewDidLoad()
        
//        volumeView = MPVolumeView(frame: CGRect(x: 50, y: 50, width: 60, height: 60))
//        self.view.addSubview(volumeView)
        
        let folderName = "rtclog" // 自定义文件夹名称
        let documentsURL = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask).first!
        let folderURL = documentsURL.appendingPathComponent(folderName)
        
        let rtcConfig = RtcEngineConfig()
        rtcConfig.enableNativeLog = true
        rtcConfig.logPath = folderURL.path
        rtcEngine = LJRtcEngine.sharedEngine(c : rtcConfig)
        let engine = rtcEngine!
//        join(mode: .push)
        engine.setDebug(debug: 1)
        //engine.enableVideo()
        engine.disableAudio()
        
        previewView.autorotate = true;
        engine.setupLocalVideo(view: previewView)
        engine.setVideoDecodeType(decodeType : VideoDecodeType.HARD.rawValue, isLowLatency : true);
                
//        remoteView.backgroundColor = UIColor(white: 1, alpha: 0.2)
        self.view.insertSubview(remoteView, belowSubview: previewView)
    }
    
    deinit {
        print("ViewController deinit")
        rtcEngine?.leaveChannel()
        LJRtcEngine.destroy()
        rtcEngine = nil
    }
    
    func join(mode: RTCWorkMode){
        if joined {
            print("----already joined, ignore action")
            return
        }

        rtcEngine?.setWorkMode(mode: mode)
        let engine = rtcEngine!
        engine.setupRemoteVideo(view: remoteView)
        let config = ChannelConfig()
        config.userID = 111
        config.token = "linjing@2023"
        config.appID = 1
        config.channelID = "954523133"
        let udpConfig = UdpInitConfig()
        config.configs.append(udpConfig)
        _ = engine.joinChannel(channelConfig: config)
        //engine.startP2P()
        joined = true
    }
    
    override func viewDidAppear(_ animated: Bool) {
        print("ViewController viewDidAppear")
        super.viewDidAppear(animated)
//        rtcEngine?.startPreview()
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        print("ViewController viewDidAppear")
//        rtcEngine?.stopPreview()
    }
    
    @IBAction func handlePush(_ sender: UIButton) {
        join(mode: .push)
    }
    
    @IBAction func handlePull(_ sender: UIButton) {
        join(mode: .pull)
    }
    
    @IBAction func handleLeave(_ sender: UIButton) {
        rtcEngine?.leaveChannel()
        joined = false
    }
    
    var localMuted = false
    @IBAction func muteLocalAudio(_ sender: UIButton) {
//        localMuted = localMuted == true ? false : true
//        rtcEngine?.muteLocalAudioStream(mute: localMuted)
        rtcEngine?.stopPreview()
    }
    
    var remoteMuted = false
    @IBAction func muteRemoteAudio(_ sender: UIButton) {
//        remoteMuted = remoteMuted == true ? false : true
//        rtcEngine?.muteRemoteAudioStream(mute: remoteMuted)
        rtcEngine?.startPreview()
    }
    
    @IBAction func handleCapture(_ sender: UIButton) {
        _ = self.rtcEngine!.switchCamera()
    }
    
    @IBAction func handlePhoto(_ sender: Any) {
//        guard let window = UIApplication.shared.keyWindow else {
//            return
//        }
//
//        let vc = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "Photo")
//        UIView.transition(with: window, duration: 0.5, options: .transitionFlipFromLeft, animations: {
//            window.rootViewController = vc
//        }, completion: nil)
    }
    
    override var prefersStatusBarHidden: Bool {
        return true
    }
    
    @IBAction func handleBack(_ sender: UIButton) {
        rtcEngine?.leaveChannel()
        LJRtcEngine.destroy()
        rtcEngine = nil
        self.dismiss(animated: true)
    }
}

