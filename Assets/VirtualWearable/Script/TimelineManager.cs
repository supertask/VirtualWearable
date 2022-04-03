using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace VW
{
    [RequireComponent(typeof(PlayableDirector))]
    public class VWTimelineManager : MonoBehaviour
    {
        public TimelineAsset openingTL;
        public TimelineAsset closingTL;
        private PlayableDirector director;

        protected void Start()
        {
            this.director = this.GetComponent<PlayableDirector>();
        }

        public void OpenVW()
        {
            if (this.director.state == PlayState.Paused) {
                this.director.Play(this.openingTL);
            }
        }

        public void CloseVW()
        {
            if (this.director.state == PlayState.Paused) {
                this.director.Play(this.closingTL);
            }
        }
    }
}
