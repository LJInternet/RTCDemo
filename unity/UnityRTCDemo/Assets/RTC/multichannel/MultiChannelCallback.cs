using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC
{

    /** Occurs when a user joins a channel.
    *
    * This callback notifies the application that a user joins a specified channel when the application calls the {@link LJ.RTC.LJChannel.JoinChannel JoinChannel} method.
    *
    * The channel name assignment is based on `channelId` specified in the `JoinChannel` method.
    *
    * If the `uid` is not specified in the `JoinChannel` method, the server automatically assigns a `uid`.
    *
    * @param channelId The name of the channel that you join.
    * @param uid The user ID of the user joining the channel.
    * @param elapsed Time elapsed (ms) from the user calling the `JoinChannel` method until the SDK triggers this callback.
    */
    public delegate void ChannelOnJoinChannelSuccessHandler(string channelId, long uid, int elapsed);


    /** Reports an error code of `LJChannel`.
    *
    * In most cases, the SDK cannot fix the issue and resume running. The SDK requires the application to take action or informs the user about the issue.
    *
    * For example, the SDK reports an `ERR_START_CALL(1002)` error when failing to initialize a call. The application informs the user that the call initialization failed and invokes the {@link LJ.RTC.LJChannel.LeaveChannel LeaveChannel} method to leave the channel.
    *
    * @param channelId The name of the channel that you join.
    * @param err The error code, see [Error Code](./index.html#error).
    * @param message The error message.
    */
    public delegate void ChannelOnErrorHandler(string channelId, int err, string message);

    /** Occurs when a user leaves the channel.
    *
    * This callback notifies the application that a user leaves the channel when the application calls the {@link LJ.RTC.LJChannel.LeaveChannel LeaveChannel} method.
    *
    * The application retrieves information, such as the call duration and statistics.
    *
    * @param channelId The name of the channel that you join.
    */
    public delegate void ChannelOnLeaveChannelHandler(string channelId);

    public delegate void ChannelOnLeaveChannelErrorHandler(string channelId, int err, string message);

    /** Occurs when a remote user (Communication) or host (Live Broadcast) joins the channel.
    *
    * - Communication profile: This callback notifies the application that another user joins the channel. If other users are already in the channel, the SDK also reports to the application on the existing users.
    * - Live-broadcast profile: This callback notifies the application that the host joins the channel. If other hosts are already in the channel, the SDK also reports to the application on the existing hosts. We recommend limiting the number of hosts to 17.
    *
    * The SDK triggers this callback under one of the following circumstances:
    * - A remote user or host joins the channel by calling the {@link LJ.RTC.LJChannel.JoinChannel JoinChannel} method.
    * - A remote user switches the user role to the host by calling the {@link LJ.RTC.LJChannel.SetClientRole SetClientRole} method after joining the channel.
    * - A remote user or host rejoins the channel after a network interruption.
    *
    * @note
    * In the Live-broadcast profile:
    * - The host receives this callback when another host joins the channel.
    * - The audience in the channel receives this callback when a new host joins the channel.
    * - When a web application joins the channel, the SDK triggers this callback as long as the web application publishes streams.
    *
    * @param channelId The name of the channel that you join.
    * @param uid The user ID of the user or host joining the channel.
    * @param elapsed Time delay (ms) from the local user calling the `JoinChannel` method until the SDK triggers this callback.
    */
    public delegate void ChannelOnUserJoinedHandler(string channelId, UInt64 uid, int elapsed);

    /** Occurs when a remote user (Communication) or host (Live Broadcast) leaves the channel.
    *
    * Reasons why the user is offline:
    *
    * - Leave the channel: When the user or host leaves the channel, the user or host sends a goodbye message. When the message is received, the SDK assumes that the user or host leaves the channel.
    * - Drop offline: When no data packet of the user or host is received for a certain period of time (20 seconds for the Communication profile, and more for the Live-broadcast profile), the SDK assumes that the user or host drops offline. Unreliable network connections may lead to false detections, so we recommend using the LJ RTM SDK for more reliable offline detection.
    *
    * @param channelId The name of the channel that you join.
    * @param uid The user ID of the user leaving the channel or going offline.
    */
    public delegate void ChannelOnUserOffLineHandler(string channelId, UInt64 uid);

    /** Occurs when the SDK cannot reconnect to LJ's edge server 10 seconds after its connection to the server is interrupted.
    *
    * The SDK triggers this callback when it cannot connect to the server 10 seconds after calling the {@link LJ.RTC.LJChannel.JoinChannel JoinChannel} method, whether or not it is in the channel.
    *
    * This callback is different from {@link LJ.RTC.OnConnectionInterruptedHandler OnConnectionInterruptedHandler}:
    * - The SDK triggers the `OnConnectionInterruptedHandler` callback when it loses connection with the server for more than four seconds after it successfully joins the channel.
    * - The SDK triggers the `ChannelOnConnectionLostHandler` callback when it loses connection with the server for more than 10 seconds, whether or not it joins the channel.
    *
    * If the SDK fails to rejoin the channel 20 minutes after being disconnected from LJ's edge server, the SDK stops rejoining the channel.
    *
    * @param channelId The name of the channel that you join.
    */
    public delegate void ChannelOnConnectionLostHandler(string channelId);

    /** Reports the last mile network quality of each user in the channel once every two seconds.
    *
    * Last mile refers to the connection between the local device and the LJ edge server. This callback reports once every two seconds the last mile network conditions of each user in the channel. If a channel includes multiple users, the SDK triggers this callback as many times.
    *
    * @param channelId The name of the channel that you join.
    * @param uid User ID. The network quality of the user with this `uid` is reported. If `uid` is 0, the local network quality is reported.
    * @param txQuality Uplink transmission quality rating of the user in terms of the transmission bitrate, packet loss rate, average RTT (Round-Trip Time), and jitter of the uplink network. `txQuality` is a quality rating helping you understand how well the current uplink network conditions can support the selected VideoEncoderConfiguration. For example, a 1000 Kbps uplink network may be adequate for video frames with a resolution of 640 × 480 and a frame rate of 15 fps in the Live-broadcast profile, but may be inadequate for resolutions higher than 1280 × 720. See #QUALITY_TYPE.
    * @param rxQuality Downlink network quality rating of the user in terms of the packet loss rate, average RTT, and jitter of the downlink network. See #QUALITY_TYPE.
    */
    public delegate void ChannelOnNetworkQualityHandler(string channelId, UInt64 uid, int txQuality, int rxQuality);

    /** Occurs when the video size or rotation of a specified user changes.
    *
    * @param channelId The name of the channel that you join.
    * @param uid The user ID of the remote user or local user (0) whose video size or rotation changes.
    * @param width The new width (pixels) of the video.
    * @param height The new height (pixels) of the video.
    * @param rotation The new rotation of the video [0 to 360).
    */
    public delegate void ChannelOnVideoSizeChangedHandler(string channelId, UInt64 uid, int width, int height);

    /** Occurs when the connection state between the SDK and the server changes.
    *
    * @param channelId The name of the channel that you join.
    * @param status SeeSTATUS_CONNECTED 1, STATUS_DISCONNECTED 2, STATUS_LOST 3
    */

    public delegate void ChannelOnConnectionStateChangedHandler(string channelId, int status);

    /**
    * Occurs when the first remote video frame is rendered.
    * 
    * The SDK triggers this callback when the first frame of the remote video is displayed in the user's video window.
    * The application can get the time elapsed from a user joining the channel until the first video frame is displayed.
    * @param channelId The name of the current channel.
    * @param uid User ID of the remote user sending the video stream.
    * @param width Width (px) of the video frame.
    * @param height Height (px) of the video stream.
    * @param elapsed Time elapsed (ms) from the local user calling `joinChannel` until the SDK triggers this callback.
    */
    public delegate void ChannelOnFirstRemoteVideoFrameHandler(string channelId, UInt64 uid, int width, int height, int elapsed);
}
