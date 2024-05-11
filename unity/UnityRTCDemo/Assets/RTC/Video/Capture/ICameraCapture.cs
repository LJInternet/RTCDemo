using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using LJ.RTC.Common;
using LJ.RTC;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.Rendering;
using System.IO;
using Unity.Collections;

namespace LJ.RTC.Video
{
    public abstract class ICameraCapture : ILifecylce
    {
        //WebCamDevice frontCameraDevice;
        //WebCamDevice backCameraDevice;
        //WebCamDevice activeCameraDevice;

        //WebCamTexture activeCameraTexture;

        //// Image rotation
        //Vector3 rotationVector = new Vector3(0f, 0f, 0f);

        //// Image uvRect
        //Rect defaultRect = new Rect(0.25f, 0f, 1f, 1f);
        //Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

        //// Image Parent's scale
        //Vector3 defaultScale = new Vector3(1f, 1f, 1f);
        //Vector3 fixedScale = new Vector3(-1f, 1f, 1f);

        public delegate void OnCameraParamCallback(int width, int height, int facing, int rotation);

        public event OnCaptureVideoFrameInternel mVideoFrameEvent;

        public VideoCaptureTracker mVideoCaptureTracker = new VideoCaptureTracker();

        //VideoConfig mVideoConfig;

        //private RawImage mRender;

        //private Texture2D mResizeTexture;

        //private Rect mPixelRect;

        //private byte[] mCachePixel;
        //private CaptureVideoFrame mCacheVideoFrame;

        //private OnCameraParamCallback mCameraParamCallback;

        //private RenderTexture mRenderTexture;

        //private Rect mResizeRect;
        //private Rect mCorpRect;

        public abstract void OnCreate();
        public abstract void OnDestroy();

        public abstract CAMERA_CAPTURE_ERROR StartPreview();

        public abstract CAMERA_CAPTURE_ERROR StartCameraDevice(string cameraDeviceName);

        public abstract void StopPreview();

        public abstract void SetRender(Component component, OnCameraParamCallback callback);

        public abstract Texture ReadCameraPixel(bool encode);

        protected void InvokeFrameEvent(CaptureVideoFrame frame, bool push) {
            if (mVideoFrameEvent != null)
            {
                mVideoFrameEvent.Invoke(frame, push);
            }
        }

        protected bool HasCallback() {
            return mVideoFrameEvent != null;
        }
       


       

    }
}
