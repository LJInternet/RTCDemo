using LJ.RTC.Common;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace LJ.RTC.Video
{
    internal class RemoteVideoManager : BaseRtcEngineModule
    {
        RawImage mRenders;
        private Material mCacheRenderMaterial = null;
        private ReaderWriterLockSlim rwlock;

        private Texture2D m_YImageTex;
        private Texture2D m_UImageTex;
        private Texture2D m_VImageTex;

        private byte[] mYBuffer;
        private byte[] mUBuffer;
        private byte[] mVBuffer;

        private int mWidth;
        private int mHeight;
        public RemoteVideoManager(IRtcEngineApi rtcEngineApi) : base(rtcEngineApi)
        {
            JLog.Info("RemoteVideoManager");

            this.rwlock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public void SetRemodeRender(RawImage image) {
            mRenders = image;
            mCacheRenderMaterial = mRenders.material;
            mRtcEngineApi.LocalRegisterDecodeVideo(OnDecodedVideoFrame);
        }

        public override void OnCreate()
        {
            JLog.Info("RemoteVideoManager OnCreate");
        }

        public override void OnDestroy()
        {
            JLog.Info("RemoteVideoManager OnDestroy");
            mRtcEngineApi.LocalRegisterDecodeVideo(null);
            if (mRenders != null) {
                mRenders.material = mCacheRenderMaterial;
                mRenders = null;
            }
            if (m_YImageTex != null) {
                GameObject.Destroy(m_YImageTex);
                m_YImageTex = null;
            }
            if (m_UImageTex != null)
            {
                GameObject.Destroy(m_UImageTex);
                m_UImageTex = null;
            }
            if (m_VImageTex != null)
            {
                GameObject.Destroy(m_VImageTex);
                m_VImageTex = null;
            }
            base.OnDestroy();
        }

        void OnDecodedVideoFrame(VideoFrame videoFrame) {
            rwlock.EnterWriteLock();
            int ySize = videoFrame.width * videoFrame.height;
            int uvSize = videoFrame.width * videoFrame.height / 4;
            if (mYBuffer == null || mYBuffer.Length != ySize) {
                mYBuffer = new byte[ySize];
            }
            if (mUBuffer == null || mUBuffer.Length != uvSize)
            {
                mUBuffer = new byte[uvSize];
            }
            if (mVBuffer == null || mVBuffer.Length != uvSize)
            {
                mVBuffer = new byte[uvSize];
            }
            Buffer.BlockCopy(videoFrame.data, 0, mYBuffer, 0, ySize);
            Buffer.BlockCopy(videoFrame.data, ySize, mUBuffer, 0, uvSize);
            Buffer.BlockCopy(videoFrame.data, ySize + uvSize, mVBuffer, 0, uvSize);
            mWidth = videoFrame.width;
            mHeight = videoFrame.height;
            MainThreadHelper.QueueOnMainThread((object obj) => {
                RenderYUVData();
            }, null);
            rwlock.ExitWriteLock();
        }

        private void SetRenderInfo(RawImage render, int width, int height)
        {
            RectTransform rectTransform = render.transform.GetComponent<RectTransform>();
            Vector3 scale = rectTransform.localScale;
            scale.x *= scale.x < 0 ? 1 : -1;
            scale.y = scale.y < 0 ? 1 : -1;
            //scale.x *= scale.x < 0 ? -1 : 1;
            //scale.y *= scale.y < 0 ? 1 : -1;
            int renderWidth = (int)rectTransform.sizeDelta.x;
            int renderHeight = (int)rectTransform.sizeDelta.y;
            rectTransform.localScale = scale;
            rectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
            float retRatio = rectTransform.sizeDelta.x / rectTransform.sizeDelta.y;
            float textureRatio = 1f * width / (1f * height);
            Rect rect = new Rect(0, 0, 1, 1);
            if (textureRatio < retRatio)
            {
                rect.height = textureRatio / retRatio;
                rect.y = (1 - rect.height) * 0.5f;
            }
            else
            {
                rect.width = retRatio / textureRatio;
                rect.x = (1 - rect.width) * 0.5f;
            }
            render.uvRect = rect;
        }
        private void RenderYUVData()
        {
            if (mRenders == null)
            {
                //JLog.Debug("mRender == null");
                return;
            }

            if (mYBuffer == null)
            {
                //JLog.Debug("mYBuffer == null");
                return;
            }
            if (mWidth == 0 || mHeight == 0) {
                return;
            }
            rwlock.EnterReadLock();
            if (m_YImageTex == null)
            {
                m_YImageTex = new Texture2D(mWidth, mHeight, TextureFormat.R8, false);
                m_UImageTex = new Texture2D(mWidth / 2, mHeight / 2, TextureFormat.R8, false);
                m_VImageTex = new Texture2D(mWidth / 2, mHeight / 2, TextureFormat.R8, false);

                mRenders.material = new Material(Shader.Find("UI/RendererShader601"));
                mRenders.material.mainTexture = m_YImageTex;
                mRenders.material.SetTexture("_UTex", m_UImageTex);
                mRenders.material.SetTexture("_VTex", m_VImageTex);
                //mRenders.material.SetFloat("_RotationAngle", 180.0f);
                SetRenderInfo(mRenders, mWidth, mHeight);
            }

            m_YImageTex.LoadRawTextureData(mYBuffer);
            m_UImageTex.LoadRawTextureData(mUBuffer);
            m_VImageTex.LoadRawTextureData(mVBuffer);
            m_YImageTex.Apply();
            m_UImageTex.Apply();
            m_VImageTex.Apply();
            rwlock.ExitReadLock();
        }
    }
}
