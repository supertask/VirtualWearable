using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Experimental.VFX;
using Leap;
using Leap.Unity;
using static Leap.Finger;

namespace VW
{
    public class VirtualWearableController : MonoBehaviour
    {
        public GameObject leapProviderObj;
        private LeapServiceProvider m_Provider;

        private VirtualWearableModel model;

        public GameObject vwOpeningDirector;
        public GameObject vwClosingDirector;

        void Start()
        {
            this.m_Provider = this.leapProviderObj.GetComponent<LeapServiceProvider>();
            this.model = this.GetComponent<VirtualWearableModel>();
            this.model.VisibleVirtualWearable(false);
        }

        void Update()
        {
            Frame frame = this.m_Provider.CurrentFrame;
            Hand[] hands = HandUtil.GetCorrectHands(frame); //0=LEFT, 1=RIGHT

            if (hands[HandUtil.RIGHT] == null) {
                 if (this.model.IsVisibleVirtualWearable) { this.model.VisibleVirtualWearable(false); }
            }
            else {
                if (!this.model.IsVisibleVirtualWearable) { this.model.VisibleVirtualWearable(true); }
                this.model.AdjustVirtualWearable(hands[HandUtil.RIGHT]);

                if (this.model.handUtilAccess.JustOpenedHandOn(hands, HandUtil.RIGHT))
                {
                    PlayableDirector director = this.vwOpeningDirector.GetComponent<PlayableDirector>();
                    director.Play();
                }
                else if (this.model.handUtilAccess.JustClosedHandOn(hands, HandUtil.RIGHT))
                {
                    PlayableDirector director = this.vwClosingDirector.GetComponent<PlayableDirector>();
                    director.Play();
                    //this.model.particleExplosionVFXObj.GetComponent<VisualEffect>().Play();
                }


                //Debug
                //this.debugSphere1.transform.position = HandUtil.ToVector3(hand.Arm.PrevJoint);
                //this.debugSphere2.transform.position = HandUtil.ToVector3(hand.Arm.NextJoint);
            }

            //Debug.Log("handUtilAccess: " + this.model.handUtilAccess);
            //Debug.Log("hands: " + hands);
            this.model.handUtilAccess.SavePreviousHands(hands);
        }
    }
}
