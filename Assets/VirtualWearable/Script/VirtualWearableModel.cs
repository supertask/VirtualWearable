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

        private GameObject arm, armUI, armUIGeneral, armUISystem, armUIClock, armRingUI;
        private GameObject palmUI, firstHandWingUI, secondHandWingUI;
        private GameObject armGeneralIcons, armSystemIcons, armClockIcons, armClock;
        private GameObject palmIcons, firstAppIcons, secondAppIcons, iconOcclusions;

        public float HAND_AJUST__TOWARDS_FINGER = -0.058f;
        public float HAND_AJUST__TOWARDS_THUMB = 0.0045f;
        public const float ARM_WIDTH_METER_IN_BLENDER = 6.35024f * 0.01f; // = 6.35024cm
        public const float ARM_LENGTH_METER_IN_BLENDER = 25.6461f * 0.01f; // = 25.6461cm
        public readonly Vector3 DEFAULT_OCCLUTION_SCALE = new Vector3(1, 0.15f, 1);

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
            this.armUIGeneral = this.armUI.transform.Find("ArmUI_General").gameObject;
            this.armUISystem = this.armUI.transform.Find("ArmUI_System").gameObject;
            this.armUIClock = this.armUI.transform.Find("ArmUI_Clock").gameObject;

            this.armRingUI = this.vwUI.transform.Find("ArmRingUI").gameObject;

            this.palmUI = this.vwUI.transform.Find("PalmUI").gameObject;
            this.firstHandWingUI = this.vwUI.transform.Find("FirstHandWingUI").gameObject;
            this.secondHandWingUI = this.vwUI.transform.Find("SecondHandWingUI").gameObject;
            this.armGeneralIcons = this.icons.transform.Find("ArmUI_GeneralIcons").gameObject;
            this.armSystemIcons = this.icons.transform.Find("ArmUI_SystemIcons").gameObject;
            this.armClockIcons = this.icons.transform.Find("ArmUI_ClockIcons").gameObject;
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
            this.MoveIconsIntoUI(this.palmIcons, this.palmUI, new Vector3(0f, 0.00175f, 0f), Quaternion.Euler(0, 0, 0));
            this.MoveIconsIntoUI(this.armGeneralIcons, this.armUIGeneral, new Vector3(0f, 0f, 0f), Quaternion.Euler(0, 90, 0) );
            this.MoveIconsIntoUI(this.armSystemIcons, this.armUISystem, new Vector3(0f, 0f, 0f), Quaternion.Euler(0, 90, 0) );
            this.MoveIconsIntoUI(this.armClockIcons, this.armUIClock, new Vector3(0f, 0.00575f, 0f), Quaternion.Euler(90, 270, 0) );
            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.palmUI, new Vector3(0f, -0.0002f, 0f), DEFAULT_OCCLUTION_SCALE );
            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.armUIGeneral, new Vector3(0f, -0.002f, 0f), DEFAULT_OCCLUTION_SCALE );
            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.armUISystem, new Vector3(0f, -0.002f, 0f), DEFAULT_OCCLUTION_SCALE );
            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.armUIClock, new Vector3(0f, 0f, 0f), new Vector3(1, 0.15f, 2.5f) );

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

        private void MoveIconsIntoUI(GameObject sourceParent, GameObject targetParent, Vector3 localPosition, Quaternion localRotation)
        {
            //Debug.Log("num of children: " + sourceParent.gameObject.transform.childCount);
            for (int i = sourceParent.gameObject.transform.childCount - 1; i >= 0; i--)
            {
                Transform source = sourceParent.transform.GetChild(i);
                Transform target = targetParent.transform.GetChild(i);
                source.parent = target;
                source.localPosition = localPosition;
                source.localRotation = localRotation;

                //GameObject occulutionGO = InstantiateRandomOcclutionGO(occlutionParent);
                //occulutionGO.transform.parent = target;
                //occulutionGO.transform.localPosition = Vector3.zero;
                //occulutionGO.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void MoveOcculutionsIntoUI(GameObject occlutionParent, GameObject targetParent,
               Vector3 localPosition, Vector3 localScale)
        {
            //Debug.Log("num of children: " + sourceParent.gameObject.transform.childCount);
            for (int i = targetParent.gameObject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject occulutionGO = InstantiateRandomOcclutionGO(occlutionParent);
                Transform target = targetParent.transform.GetChild(i);
                occulutionGO.transform.parent = target;
                occulutionGO.transform.localPosition = localPosition;
                occulutionGO.transform.localRotation = Quaternion.Euler(0, 0, 0);
                occulutionGO.transform.localScale = localScale;
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
            this.arm.transform.rotation = HandUtil.ToQuaternion(hand.Arm.Rotation) * Quaternion.AngleAxis(270, Vector3.left);
            this.armUI.transform.rotation = HandUtil.ToQuaternion(hand.Arm.Rotation) * Quaternion.AngleAxis(270, Vector3.left);
            this.armRingUI.transform.rotation = HandUtil.ToQuaternion(hand.Arm.Rotation) * Quaternion.AngleAxis(270, Vector3.left);
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
