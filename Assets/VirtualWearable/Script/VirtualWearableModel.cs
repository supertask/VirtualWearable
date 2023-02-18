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
    public class VirtualWearableModel: MonoBehaviour
    {
        public GameObject vwUI; //Virtual Wearable UI
        public GameObject icons;
        //public GameObject particleExplosionVFX;

        private GameObject arm, armUI, armRingUI;
        private GameObject palmUI, firstHandWingUI, secondHandWingUI;
        private GameObject armIcons, palmIcons, firstAppIcons, secondAppIcons, iconOcclusions;

        public float HAND_AJUST__TOWARDS_FINGER = -0.058f;
        public float HAND_AJUST__TOWARDS_THUMB = 0.0045f;
        public const float ARM_WIDTH_METER_IN_BLENDER = 6.35024f * 0.01f; // = 6.35024cm
        public const float ARM_LENGTH_METER_IN_BLENDER = 25.6461f * 0.01f; // = 25.6461cm

        private HandUtil handUtil;
        public HandUtil handUtilAccess {
            get { return handUtil; }
        }
        public Transform playerHeadTransform;

        private bool isVisibleVirtualWearable;
        public bool IsVisibleVirtualWearable { get { return isVisibleVirtualWearable; } }

        public void Start()
        {
            this.arm = this.vwUI.transform.Find("Arm").gameObject;
            this.armUI = this.vwUI.transform.Find("ArmUI").gameObject;
            this.armRingUI = this.vwUI.transform.Find("ArmRingUI").gameObject;

            this.palmUI = this.vwUI.transform.Find("PalmUI").gameObject;
            this.firstHandWingUI = this.vwUI.transform.Find("FirstHandWingUI").gameObject;
            this.secondHandWingUI = this.vwUI.transform.Find("SecondHandWingUI").gameObject;
            this.armIcons = this.icons.transform.Find("ArmIcons").gameObject;
            this.palmIcons = this.icons.transform.Find("PalmIcons").gameObject;
            this.firstAppIcons = this.icons.transform.Find("FirstAppIcons").gameObject;
            this.secondAppIcons = this.icons.transform.Find("SecondAppIcons").gameObject;
            this.iconOcclusions = this.icons.transform.Find("IconOcclusions").gameObject;

            //Disable mesh
            Util.EnableMeshRendererRecursively(this.firstAppIcons, false);
            Util.EnableMeshRendererRecursively(this.secondAppIcons, false);

            /*
            Debug.Log("d1");
            foreach (Transform child in this.firstHandWingUI.transform) {
                if (child.GetChildCount() > 0) {
                    Transform grandChild = child.GetChild(0);
                    Debug.Log(grandChild.localRotation);
                }
            }
            */

            //this.MoveChildren(this.firstAppIcons, this.firstHandWingUI, this.iconOcclusions);
            //this.MoveChildren(this.secondAppIcons, this.secondHandWingUI, this.iconOcclusions);
            this.MoveChildren(this.palmIcons, this.palmUI, this.iconOcclusions);
            this.MoveChildren(this.armIcons, this.armUI, this.iconOcclusions);

            /*
            Debug.Log("d2");
            foreach (Transform child in this.firstHandWingUI.transform) {
                if (child.GetChildCount() > 0) {
                    Transform grandChild = child.GetChild(0);
                    Debug.Log(grandChild.localRotation);
                }
            }*/

            this.handUtil = new HandUtil(playerHeadTransform);
            Debug.Log("handUtil: " + handUtil);

            this.isVisibleVirtualWearable = false;
        }

        private void MoveChildren(GameObject sourceParent, GameObject targetParent, GameObject occlutionParent)
        {
            //Debug.Log("num of children: " + sourceParent.gameObject.transform.childCount);
            for (int i = sourceParent.gameObject.transform.childCount - 1; i >= 0; i--)
            {
                Transform source = sourceParent.transform.GetChild(i);
                Transform target = targetParent.transform.GetChild(i);
                source.parent = target;
                source.localPosition = Vector3.zero;
                source.localRotation = Quaternion.Euler(0, 0, 0);

                GameObject occulutionGO = InstantiateRandomOcclutionGO(occlutionParent);
                occulutionGO.transform.parent = target;
                occulutionGO.transform.localPosition = Vector3.zero;
                occulutionGO.transform.localRotation = Quaternion.Euler(0, 0, 0);

                /*
                Debug.Log("Name: " + sourceParent.gameObject.name);
                Debug.Log("Child Name: " + s.gameObject.name);
                Debug.Log("localPosition: " + s.localPosition);
                Debug.Log("localRotation: " + s.localRotation);
                */
            }
        }

        public GameObject InstantiateRandomOcclutionGO(GameObject occlutionParent)
        {
            int index = Random.Range(0, occlutionParent.transform.childCount);
            GameObject occlutionObj = Instantiate(occlutionParent.transform.GetChild(index).gameObject);
            return occlutionObj;
        }

        public void AdjustVirtualWearable(Hand hand)
        {
            Vector3 palmPosition = HandUtil.ToVector3(hand.PalmPosition);
            Vector3 directionTowardsIndexFinger = HandUtil.ToVector3(hand.Direction);
            Vector3 handNormal = HandUtil.ToVector3(hand.PalmNormal);

            // IMPORTANT: A CENTER POSITION OF VIRTURAL WEARABLE UI is A POINT BETWEEN A HAND AND AN ARM.
            // Hand position & rotation
            //https://docs.unity3d.com/ScriptReference/Vector3.Cross.html
            Vector3 directionTowardsThumb = Vector3.Cross(handNormal, directionTowardsIndexFinger).normalized;
            this.vwUI.transform.position = palmPosition + directionTowardsIndexFinger * HAND_AJUST__TOWARDS_FINGER;
            this.vwUI.transform.position += directionTowardsThumb * HAND_AJUST__TOWARDS_THUMB;
            this.vwUI.transform.rotation = HandUtil.ToQuaternion(hand.Rotation) *
                Quaternion.AngleAxis(180, Vector3.forward) *
                Quaternion.AngleAxis(180, Vector3.up);

            // Arm position, rotation, and scale
            this.arm.transform.rotation = HandUtil.ToQuaternion(hand.Arm.Rotation) * Quaternion.AngleAxis(180, Vector3.left);
            this.armUI.transform.rotation = HandUtil.ToQuaternion(hand.Arm.Rotation) * Quaternion.AngleAxis(180, Vector3.left);
            this.armRingUI.transform.rotation = HandUtil.ToQuaternion(hand.Arm.Rotation) * Quaternion.AngleAxis(180, Vector3.left);
            /*
            this.arm.transform.rotation = HandUtil.ToQuaternion(hand.Arm.Rotation) * Quaternion.AngleAxis(270, Vector3.left);
            this.armUI.transform.rotation = HandUtil.ToQuaternion(hand.Arm.Rotation) * Quaternion.AngleAxis(270, Vector3.left);
            this.armRingUI.transform.rotation = HandUtil.ToQuaternion(hand.Arm.Rotation) * Quaternion.AngleAxis(270, Vector3.left);
            */

            this.arm.transform.localScale = new Vector3(
                hand.Arm.Width / ARM_WIDTH_METER_IN_BLENDER,
                hand.Arm.Length / ARM_LENGTH_METER_IN_BLENDER,
                1);
            //Debug.Log("width: " + hand.Arm.Width);
            //Debug.Log("length: " + hand.Arm.Length);

            /*
            // Icons
            for (int i = 0; i < this.palmIcons.gameObject.transform.childCount; i++)
            {
                this.palmIcons.gameObject.transform.GetChild(i).position = this.palmUI.gameObject.transform.GetChild(i).position;
                this.palmIcons.gameObject.transform.GetChild(i).rotation = this.palmUI.gameObject.transform.GetChild(i).rotation * Quaternion.AngleAxis(270, Vector3.left); // * this.vwUI.transform.rotation;
            }
            for (int i = 0; i < this.firstAppIcons.gameObject.transform.childCount; i++)
            {
                this.firstAppIcons.gameObject.transform.GetChild(i).position = this.firstHandWingUI.gameObject.transform.GetChild(i).position;
                this.firstAppIcons.gameObject.transform.GetChild(i).rotation = this.firstHandWingUI.gameObject.transform.GetChild(i).rotation * Quaternion.AngleAxis(270, Vector3.left); // * this.vwUI.transform.rotation;
            }
            for (int i = 0; i < this.secondAppIcons.gameObject.transform.childCount; i++)
            {
                this.secondAppIcons.gameObject.transform.GetChild(i).position = this.secondHandWingUI.gameObject.transform.GetChild(i).position;
                this.secondAppIcons.gameObject.transform.GetChild(i).rotation = this.secondHandWingUI.gameObject.transform.GetChild(i).rotation * Quaternion.AngleAxis(270, Vector3.left); // * this.vwUI.transform.rotation;
            }
            */

            //VFX
            //this.particleExplosionVFX.transform.position = this.vwUI.transform.position;
            //this.particleExplosionVFX.transform.rotation = this.vwUI.transform.rotation * Quaternion.AngleAxis(90, Vector3.left);
        }

        public void VisibleVirtualWearable(bool isVisible) {
            Util.EnableMeshRendererRecursively(this.vwUI, isVisible);
            Util.EnableMeshRendererRecursively(this.icons.gameObject, isVisible);
            this.isVisibleVirtualWearable = isVisible;
        }

    }
}
