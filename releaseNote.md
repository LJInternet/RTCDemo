### 20240930 发布：
1.调整帧率算法（编码端有效）
PC/Linux：media_engine_register_event_listener //type== RUDP_CB_VIDEO_FRAME_RATE_CONTROL
android：RTCEngine// addHandler(IRtcEngineEventHandler handler) //IRtcEngineEventHandler onEncodeFrameControl
ios:IRTCDelegate // RTCEventHandler //onFrameControl
2.解码增加丢包策略，增加解码拥塞回调(回调该方法表示过去一分钟出现多次拥塞)（拉流解码端有效）
PC/Linux：media_engine_register_event_listener //type== CB_DECODE_VIDEO_LAG
android：RTCEngine// registerDecodeObserverIDecodeObserver obs)//IDecodeObserver// onDecodeLag
ios:IRTCDelegate // RTCEventHandler //onVideoDecodeLag
### 20240509 发布：
anroid、ios、windows解码增加丢包策略以及编码增加帧率控制

### 20240509 发布：
anroid、ios、windows、linux增加RTM使用demo以及使用说明

### 20240419 发布：
ios: 修改相机权限和麦克风权限申请以及添加IOSH264硬解码

### 20240417 发布：
ios: 替换log命名空间

### 20240402 发布：
windows: win_20240402 3592c695eb8d781039896a0761523c421b751a89 (C层接口增加延时统计信息回调)

### 20240319 发布：
android aar：livewrapper-unity_2024_03_19_17_14_30.aar 061826ba2fbb31036a61f900bd8e9f5841f9c6fc

android obj：livewrapper-unity_2024_03_19_17_14_30_obj.zip 061826ba2fbb31036a61f900bd8e9f5841f9c6fc

windows: win_202402291811 4588cc0c94dd480ec64564e05402f71367059de7

linux: out20240319 061826ba2fbb31036a61f900bd8e9f5841f9c6fc

ios: b4de0bc898c59b1efc39ed07c6796ea95b6f5eeb
