using System;
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

        private double scaleUpTime = 0.18;
        private double scaleDownTime = 0.08;


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
                    //PlayableDirector director = this.vwOpeningDirector.GetComponent<PlayableDirector>();
                    //director.Play();
                    if (stateOfScaleUpAppIcons != null) { StopCoroutine(stateOfScaleUpAppIcons);  }
                    if (stateOfScaleDownAppIcons != null) { StopCoroutine(stateOfScaleDownAppIcons); }
                    stateOfScaleUpAppIcons = ScaleUpAppIcons();
                    StartCoroutine(stateOfScaleUpAppIcons);

                }
                else if (this.model.handUtilAccess.JustClosedHandOn(hands, HandUtil.RIGHT))
                {
                    //PlayableDirector director = this.vwClosingDirector.GetComponent<PlayableDirector>();
                    //director.Play();
                    if (stateOfScaleUpAppIcons != null) { StopCoroutine(stateOfScaleUpAppIcons);  }
                    if (stateOfScaleDownAppIcons != null) { StopCoroutine(stateOfScaleDownAppIcons); }
                    stateOfScaleDownAppIcons = ScaleDownAppIcons();
                    StartCoroutine(stateOfScaleDownAppIcons);
                }


                //Debug
                //this.debugSphere1.transform.position = HandUtil.ToVector3(hand.Arm.PrevJoint);
                //this.debugSphere2.transform.position = HandUtil.ToVector3(hand.Arm.NextJoint);
            }

            //Debug.Log("handUtilAccess: " + this.model.handUtilAccess);
            //Debug.Log("hands: " + hands);
            this.model.handUtilAccess.SavePreviousHands(hands);
        }

        IEnumerator stateOfScaleDownAppIcons;
        IEnumerator stateOfScaleUpAppIcons;

        private IEnumerator ScaleUpAppIcons()
        {
            return ScaleAppIcons(true, Vector3.zero, Vector3.one, scaleUpTime);
        }
        private IEnumerator ScaleDownAppIcons()
        {
            return ScaleAppIcons(false, Vector3.one, Vector3.zero, scaleDownTime);
        }

        private IEnumerator ScaleAppIcons(bool isScaleUp, Vector3 srcScale, Vector3 targetScale, double animationTime)
        {

            return AnimThread(
                () => {
                    this.model.PalmLookAtCenter.SetActive(true);
                    this.model.PalmLookAtCenter.transform.localScale = srcScale;
                },
                (double progress) => {
                    this.model.PalmLookAtCenter.transform.localScale = Vector3.Lerp(srcScale, targetScale, (float)progress);
                },
                () => {
                    this.model.PalmLookAtCenter.transform.localScale = targetScale;
                    this.model.PalmLookAtCenter.SetActive(isScaleUp ? true : false);
                },
                animationTime
            );
        }

        private IEnumerator AnimThread(Action beforeAnimation, Action<double> duringAnimation, Action afterAnimation, double animationTime)
        {
            beforeAnimation();

            double progress = 0.0;
            double rate = 1.0 / animationTime;
            while (progress < 1.0)
            {
                progress += Time.deltaTime * rate;

                duringAnimation(progress);
                yield return null;
            }

            afterAnimation();
        }
    }
}
