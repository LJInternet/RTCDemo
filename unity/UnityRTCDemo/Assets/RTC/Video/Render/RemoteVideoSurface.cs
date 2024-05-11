

using UnityEngine;
using UnityEngine.UI;

namespace LJ.RTC.Video
{
    public class RemoteVideoSurface : MonoBehaviour
    {
        public void Start()
        {
            RawImage rawImage = GetComponent<RawImage>();
        }

        public void OnDestroy()
        {
            
        }

    }
}
