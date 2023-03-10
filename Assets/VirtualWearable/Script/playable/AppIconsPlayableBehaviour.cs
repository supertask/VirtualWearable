using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace VirtualWearable
{
    // A behaviour that is attached to a playable
    public class AppIconsPlayableBehaviour : PlayableBehaviour
    {
        public GameObject vw;
        //public GameObject icons;

        //public float maxWingLeftVerticalClipPercent, minWingLeftVerticalClipPercent;
        public int appIndex;

        public bool isScalingUp;
        public bool applyAllIcons;

        private List<GameObject> handWingUIs;
        //private List<List<Quaternion>> localRotationsOfAppIcons;


        // Called when the owning graph starts playing
        public override void OnGraphStart(Playable playable) {
            //Debug.Log(this.handWingUIs.Count);
            if (isScalingUp) {
                foreach (GameObject handWingUI in this.handWingUIs) { Util.EnableMeshRendererRecursively(handWingUI, true); }
            }
        }

        // Called when the owning graph stops playing
        public override void OnGraphStop(Playable playable) {
            if (! isScalingUp) {
                foreach (GameObject handWingUI in this.handWingUIs) { Util.EnableMeshRendererRecursively(handWingUI, false); }
            }
        }

        public override void OnPlayableCreate(Playable playable)
        {
            Debug.Log("here");
            this.initHandWingUIs(); //Startでよびたい
        }

        private void initHandWingUIs()
        {
            this.handWingUIs = new List<GameObject>();
            this.handWingUIs.Add(this.vw.transform.Find("FirstHandWingUI").gameObject);
            this.handWingUIs.Add(this.vw.transform.Find("SecondHandWingUI").gameObject);
            //this.localRotationsOfAppIcons = new List<List<Quaternion>>();

            //TODO: ここの処理を最初だけにする -> Modelで作ってそれをここに渡す
            for (int hi = 0; hi < this.handWingUIs.Count; hi++)
            {
                foreach (Transform child in this.handWingUIs[hi].transform) {
                    if (child.childCount > 0) {
                        //this.localRotationsOfAppIcons[hi].Add(child.GetChild(0).localRotation);
                        child.GetChild(0).localScale = new Vector3(Constants.App.Size, Constants.App.Size, Constants.App.Size);
                    }
                }
            }

            this.PopupIcons(0.0f);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
        }

        // Called when the state of the playable is set to Play
        public override void OnBehaviourPlay(Playable playable, FrameData info) {
        }

        // Called when the state of the playable is set to Paused
        public override void OnBehaviourPause(Playable playable, FrameData info) {
            //Util.EnableMeshRendererRecursively(this.firstAppIconsObj, false);
            //Util.EnableMeshRendererRecursively(this.secondAppIconsObj, false);
        }

        // Called each frame while the state is set to Play
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            float progress = Mathf.Clamp01((float)(playable.GetTime() / playable.GetDuration())); //0.0 - 1.0
            //Debug.Log("progress: " + progress);
            this.PopupIcons(progress);
        }

        private void PopupIcons(float progress)
        {
            //Vector3 beginScale, endScale;
            float beginPosY, endPosY; //, beginRotAngle, endRotAngle;
            if (isScalingUp) {
                //beginScale = Vector3.zero;
                beginPosY = 0.0f;
                endPosY = Constants.App.LocalPositionY;
                //beginRotAngle = Constants.App.LocalRotationBeginAngle;
                //endScale = new Vector3(Constants.App.Size, Constants.App.Size, Constants.App.Size);
                //endRotAngle = Constants.App.LocalRotationEndAngle;
            } else {
                beginPosY = Constants.App.LocalPositionY;
                endPosY = 0.0f;
                //beginScale = new Vector3(Constants.App.Size, Constants.App.Size, Constants.App.Size);
                //beginRotAngle = Constants.App.LocalRotationEndAngle;
                //endScale = Vector3.zero;
                //endRotAngle = Constants.App.LocalRotationBeginAngle;
            }
            //Vector3 scale = Vector3.Lerp(beginScale, endScale, progress);
            //float rotAngle = Mathf.Lerp(beginRotAngle, endRotAngle, progress);
            float posY = Mathf.Lerp(beginPosY, endPosY, progress);
            //Vector3 scale = new Vector3(Constants.App.Size, Constants.App.Size, Constants.App.Size);
            //Debug.Log(posY);

            Debug.Log("Count: " + this.handWingUIs.Count);

            for (int hi = 0; hi < this.handWingUIs.Count; hi++)
            {
                Debug.Log("childCount: " + this.handWingUIs[hi].transform.childCount);

                if (this.applyAllIcons) {
                    for (int ai = 0; ai < this.handWingUIs[hi].transform.childCount; ai++) {
                        Transform child = this.handWingUIs[hi].transform.GetChild(ai);
                        if (child.childCount > 0) {
                            Transform grandChild = child.GetChild(0);
                            //grandChild.localScale = scale;
                            grandChild.localPosition = Vector3.forward * posY;
                            Debug.Log("grandChildNameAll: " + grandChild.name);
                            /*
                            grandChild.rotation = this.localRotationsOfAppIcons[hi][ai] *
                                Quaternion.AngleAxis(90, Vector3.up);
                            */

                            //grandChild.localRotation = Quaternion.Lerp(this.localRotationsOfAppIcons, progress);
                            //grandChild.localRotation = child.localRotation * Quaternion.AngleAxis(270, Vector3.left) *
                            //    Quaternion.AngleAxis( Mathf.Lerp(Constants.App.LocalRotationEndAngle, 0, progress), Vector3.up);
                        }
                    }
                } else {
                    Transform child = this.handWingUIs[hi].transform.GetChild(this.appIndex);
                    if (child.childCount > 0) {
                        Transform grandChild = child.GetChild(0);
                        //Debug.Log(child.localPosition.y);
                        Debug.Log("grandChildName on each: " + grandChild.name);

                        //grandChild.localScale = scale;
                        grandChild.localPosition = Vector3.forward * posY;

                        //Debug.Log(appIndex);
                        //grandChild.localRotation = this.localRotationsOfAppIcons[hi][this.appIndex] *
                        //    Quaternion.AngleAxis(Mathf.Lerp(180.0f, 360.0f, progress), Vector3.up);

                        //grandChild.localRotation = child.localRotation * Quaternion.AngleAxis(270, Vector3.left) *
                        //   Quaternion.AngleAxis( Mathf.Lerp(0, Constants.App.LocalRotationEndAngle, progress), Vector3.up);
                    }
                }
            }
        }

    }
}
