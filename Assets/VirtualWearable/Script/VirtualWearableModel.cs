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
    public class Ellipsoid
    {
        public Vector3 center;
        public Vector3 radius;
        public float angle; //0 to 360 degree
        public float partitionedNum;

        public Ellipsoid(Vector3 center, Vector3 radius, float angle, int partitionedNum)
        {
            this.center = center;
            this.radius = radius;
            this.angle = angle;
            this.partitionedNum = partitionedNum;
        }

        //partitionedNum: 分割数
        public Vector3 PositionOnSphere(int index, float height)
        {
            // 角度を計算する
            float partitionedAngle = angle / partitionedNum;
            float offsetAngle = 90f - angle / 2f;
            float radian = (offsetAngle + index * partitionedAngle) * Mathf.Deg2Rad;

            // 楕円球上の点を計算する
            Vector3 point = new Vector3(
                center.x + radius.x * Mathf.Cos(radian),
                center.y + radius.y * Mathf.Sin(radian),
                center.z + height + radius.z * Mathf.Sin(radian)
            );
            return point;
        }
    }

    public class VirtualWearableModel: MonoBehaviour
    {
        public GameObject vwUI; //Virtual Wearable UI
        public GameObject icons;
        public GameObject rightHand;
        //public GameObject particleExplosionVFX;

        private GameObject arm, armUI, armUIGeneral, armUISystem, armUIClock, armRingUI;
        private GameObject palmUI, firstHandWingUI, secondHandWingUI;
        private GameObject armGeneralIcons, armSystemIcons, armClockIcons, armClock;
        private GameObject palmIcons, firstAppIcons, secondAppIcons, appIconsOnRightHand, iconOcclusions;
        private GameObject[] rightFingers;
        private HandUtil handUtil;

        public float HAND_AJUST__TOWARDS_FINGER = -0.058f;
        public float HAND_AJUST__TOWARDS_THUMB = 0.0045f;
        public const float ARM_WIDTH_METER_IN_BLENDER = 6.35024f * 0.01f; // = 6.35024cm
        public const float ARM_LENGTH_METER_IN_BLENDER = 25.6461f * 0.01f; // = 25.6461cm
        public readonly Vector3 DEFAULT_OCCLUTION_SCALE = new Vector3(1, 0.15f, 1);
        public readonly Vector3 APP_SCALE_ON_FINGERS = Vector3.one * 4;
        public readonly string[] fingerNames = new string[5] { "L_index_end", "L_middle_end", "L_pinky_end", "L_ring_end", "L_thumb_end" }; 

        public Transform playerHeadTransform;
        public HandUtil handUtilAccess {
            get { return handUtil; }
        }
        public GameObject PalmLookAtCenter
        {
            get { return palmLookAtCenter; }
        }

        private bool isVisibleVirtualWearable;
        public bool IsVisibleVirtualWearable { get { return isVisibleVirtualWearable; } }
        private GameObject palmCenter;
        private GameObject palmLookAtCenter;

        void Awake()
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

            this.rightFingers = GameObject.FindGameObjectsWithTag("RightFingers");
            //Debug.Log("finger: " + fingers);
            //Debug.Log("finger: " + fingers.Length);
            //Debug.Log("finger: " + fingers[0]);

            this.armGeneralIcons = this.icons.transform.Find("ArmUI_GeneralIcons").gameObject;
            this.armSystemIcons = this.icons.transform.Find("ArmUI_SystemIcons").gameObject;
            this.armClockIcons = this.icons.transform.Find("ArmUI_ClockIcons").gameObject;
            this.palmIcons = this.icons.transform.Find("PalmIcons").gameObject;
            this.firstAppIcons = this.icons.transform.Find("FirstAppIcons").gameObject;
            this.secondAppIcons = this.icons.transform.Find("SecondAppIcons").gameObject;
            this.appIconsOnRightHand = this.icons.transform.Find("AppIconsOnRightHand").gameObject;
            this.iconOcclusions = this.icons.transform.Find("IconOcclusions").gameObject;

            //this.palmCenter = new GameObject("PalmCenter");
            this.palmCenter = this.vwUI.transform.parent.Find("PalmCenter").gameObject;
            this.palmCenter.transform.parent = this.vwUI.transform.parent;
            this.palmLookAtCenter = new GameObject("palmLookAtCenter");
            this.palmLookAtCenter.transform.parent = this.palmCenter.transform;
            this.palmLookAtCenter.transform.position = new Vector3(0, 0.06f, 0);
            //this.palmLookAtCenter.transform.localScale = Vector3.zero;
            //this.palmLookAtCenter.SetActive(false);

            float ellipsoidSize = 0.06f; //meter
            float ellipsoidHeight = 0.09f; //meter
            int PARTITIONED_NUM = 5; //app num
            Ellipsoid ellipsoid = new Ellipsoid(this.palmLookAtCenter.transform.position,
                new Vector3(ellipsoidSize, ellipsoidSize, ellipsoidHeight),
                120f, PARTITIONED_NUM);
            for (int i = 0; i < PARTITIONED_NUM; i++)
            {
                var appCenter = new GameObject("appCenter" + i);
                //TODO: Change position to sphere position later
                appCenter.transform.position = ellipsoid.PositionOnSphere(i, 0);
                appCenter.transform.LookAt(this.palmLookAtCenter.transform, Vector3.forward);
                appCenter.transform.parent = this.palmLookAtCenter.transform;
            }


            //Disable mesh
            Util.EnableMeshRendererRecursively(this.firstAppIcons, false);
            Util.EnableMeshRendererRecursively(this.secondAppIcons, false);

            //this.MoveChildren(this.firstAppIcons, this.firstHandWingUI, this.iconOcclusions);
            //this.MoveChildren(this.secondAppIcons, this.secondHandWingUI, this.iconOcclusions);

            //this.MoveIconsIntoUI(this.palmIcons, this.palmUI, new Vector3(0f, 0.00175f, 0f), Quaternion.Euler(0, 0, 0));
            this.MoveIconsIntoUI(this.armGeneralIcons, this.armUIGeneral, new Vector3(0f, 0f, 0f), Quaternion.Euler(0, 90, 0), null );
            this.MoveIconsIntoUI(this.armSystemIcons, this.armUISystem, new Vector3(0f, 0f, 0f), Quaternion.Euler(0, 90, 0), null );
            this.MoveIconsIntoUI(this.armClockIcons, this.armUIClock, new Vector3(0f, 0.00575f, 0f), Quaternion.Euler(90, 270, 0), null );
            //this.MoveOcculutionsIntoUI(this.iconOcclusions, this.palmUI, new Vector3(0f, -0.0002f, 0f), DEFAULT_OCCLUTION_SCALE );
            this.MoveIconsIntoUI(this.appIconsOnRightHand, this.palmLookAtCenter, new Vector3(0f, 0f, 0f), Quaternion.Euler(0, 0, 0), APP_SCALE_ON_FINGERS);

            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.armUIGeneral, new Vector3(0f, -0.002f, 0f), DEFAULT_OCCLUTION_SCALE );
            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.armUISystem, new Vector3(0f, -0.002f, 0f), DEFAULT_OCCLUTION_SCALE );
            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.armUIClock, new Vector3(0f, 0f, 0f), new Vector3(1, 0.15f, 2.5f) );

            this.handUtil = new HandUtil(playerHeadTransform);
            //Debug.Log("handUtil: " + handUtil);

            this.isVisibleVirtualWearable = false;
        }

        private void Start()
        {
            this.palmLookAtCenter.transform.localScale = Vector3.zero;
            this.palmLookAtCenter.SetActive(false);
        }


        private void MoveIconsIntoUI(GameObject sourceParent, GameObject targetParent,
            Vector3? localPosition, Quaternion? localRotation, Vector3? localScale)
        {
            //Debug.Log("num of children: " + sourceParent.gameObject.transform.childCount);
            for (int i = sourceParent.gameObject.transform.childCount - 1; i >= 0; i--)
            {
                Transform source = sourceParent.transform.GetChild(i);
                Transform target = targetParent.transform.GetChild(i);
                source.parent = target;
                if (localScale.HasValue) { source.localScale = localScale.Value; }
                if (localPosition.HasValue) { source.localPosition = localPosition.Value; }
                if (localRotation.HasValue) { source.localRotation = localRotation.Value; }
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
            Quaternion palmQuaternion = HandUtil.ToQuaternion(hand.Rotation);
            Vector3 directionTowardsIndexFinger = HandUtil.ToVector3(hand.Direction);
            Vector3 handNormal = HandUtil.ToVector3(hand.PalmNormal);

            // IMPORTANT: A CENTER POSITION OF VIRTURAL WEARABLE UI is A POINT BETWEEN A HAND AND AN ARM.
            // Hand position & rotation
            //https://docs.unity3d.com/ScriptReference/Vector3.Cross.html
            Vector3 directionTowardsThumb = Vector3.Cross(handNormal, directionTowardsIndexFinger).normalized;
            this.vwUI.transform.position = palmPosition + directionTowardsIndexFinger * HAND_AJUST__TOWARDS_FINGER;
            this.vwUI.transform.position += directionTowardsThumb * HAND_AJUST__TOWARDS_THUMB;
            this.vwUI.transform.rotation = palmQuaternion *
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

            this.palmCenter.transform.position = palmPosition;
            this.palmCenter.transform.rotation = palmQuaternion * Quaternion.AngleAxis(270, Vector3.left);

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
