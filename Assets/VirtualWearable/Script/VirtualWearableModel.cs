using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Experimental.VFX;
using Leap;
using Leap.Unity;
using static Leap.Finger;

namespace VirtualWearable
{
    //mesh, color
    [System.Serializable]
    public struct AppColor
    {
        [SerializeField] public MeshRenderer renderer;
        [SerializeField] public Color color;
    }

    public class VirtualWearableModel : MonoBehaviour
    {
        #region Serialized fields
        [SerializeField] public GameObject vwUI; //Virtual Wearable UI
        [SerializeField] public GameObject icons;
        [SerializeField] public GameObject rightHand;
        [SerializeField] public GameObject stage;
        [SerializeField] public Transform playerHeadTransform;
        [SerializeField] public GameObject appShowcaseOnStage;
        [SerializeField] public GameObject appShowcaseOnHand;
        [SerializeField] public AppArea.AppDisplayingMode appDisplayingMode;
        [SerializeField] public float HAND_AJUST__TOWARDS_FINGER = -0.058f;
        [SerializeField] public float HAND_AJUST__TOWARDS_THUMB = 0.0045f;
        [SerializeField] public bool isShowGizmo = true;
        [SerializeField] public List<AppColor> appColorMap;
        #endregion

        #region public
        public HandUtil handUtilAccess { get { return handUtil; } }
        public bool IsVisibleVirtualWearable { get { return isVisibleVirtualWearable; } }
        public AppArea AppArea { get { return appArea; } }

        public const float ARM_WIDTH_METER_IN_BLENDER = 6.35024f * 0.01f; // = 6.35024cm
        public const float ARM_LENGTH_METER_IN_BLENDER = 25.6461f * 0.01f; // = 25.6461cm
        public readonly Vector3 ARM_UI_OCCLUTION_SCALE = new Vector3(0.65f, 0.15f, 0.65f);
        public readonly Vector3 APP_SCALE_ON_FINGERS = Vector3.one * 3;
        public readonly string[] fingerNames = new string[5] { "L_index_end", "L_middle_end", "L_pinky_end", "L_ring_end", "L_thumb_end" };
        #endregion


        #region private
        private GameObject arm, armUI, armUIGeneral, armUISystem, armUIClock, armRingUI, thumbUI;
        private GameObject palmUI, firstHandWingUI, secondHandWingUI;
        private GameObject armGeneralIcons, armSystemIcons, armClockIcons, armClock;
        private GameObject palmIcons, firstAppIcons, secondAppIcons, appIconsOnRightHand, iconOcclusions;
        private HandUtil handUtil;
        private AppArea appArea;

        private bool isVisibleVirtualWearable;

        private float appCyberCircuitTimeSpeed = 0.5f;
        private float cyberCircuitTimeSpeed = 0.1f;
        private MaterialPropertyBlock appMatBlock;

        #endregion

        void Awake()
        {
            this.arm = this.vwUI.transform.Find("Arm").gameObject;
            this.armUI = this.vwUI.transform.Find("ArmUI").gameObject;
            this.armUIGeneral = this.armUI.transform.Find("ArmUI_General").gameObject;
            this.armUISystem = this.armUI.transform.Find("ArmUI_System").gameObject;
            this.armUIClock = this.armUI.transform.Find("ArmUI_Clock").gameObject;

            this.armRingUI = this.vwUI.transform.Find("ArmRingUI").gameObject;
            this.palmUI = this.vwUI.transform.Find("PalmUI").gameObject;
            this.thumbUI = this.vwUI.transform.Find("ThumbUI").gameObject;
            //this.firstHandWingUI = this.vwUI.transform.Find("FirstHandWingUI").gameObject;
            //this.secondHandWingUI = this.vwUI.transform.Find("SecondHandWingUI").gameObject;

            this.armGeneralIcons = this.icons.transform.Find("ArmUI_GeneralIcons").gameObject;
            this.armSystemIcons = this.icons.transform.Find("ArmUI_SystemIcons").gameObject;
            this.armClockIcons = this.icons.transform.Find("ArmUI_ClockIcons").gameObject;
            this.palmIcons = this.icons.transform.Find("PalmIcons").gameObject;
            this.firstAppIcons = this.icons.transform.Find("FirstAppIcons").gameObject;
            this.secondAppIcons = this.icons.transform.Find("SecondAppIcons").gameObject;
            this.appIconsOnRightHand = this.icons.transform.Find("AppIconsOnRightHand").gameObject;
            this.iconOcclusions = this.icons.transform.Find("IconOcclusions").gameObject;

            this.appArea = new AppArea(this.appIconsOnRightHand, appShowcaseOnStage, appShowcaseOnHand, appDisplayingMode);
            this.appMatBlock = new MaterialPropertyBlock();
            foreach (AppColor appColor in appColorMap)
            {
                appMatBlock.SetColor("_MainColor", appColor.color);
                appColor.renderer.SetPropertyBlock(appMatBlock);
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
            //this.MoveOcculutionsIntoUI(this.iconOcclusions, this.palmUI, new Vector3(0f, -0.0002f, 0f), ARM_UI_OCCLUTION_SCALE );
            this.MoveIconsIntoUI(this.appIconsOnRightHand, this.appArea.SpheroidCenterObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0, 0, 0), APP_SCALE_ON_FINGERS);

            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.armUIGeneral, new Vector3(0f, -0.002f, 0f), ARM_UI_OCCLUTION_SCALE );
            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.armUISystem, new Vector3(0f, -0.002f, 0f), ARM_UI_OCCLUTION_SCALE );
            this.MoveOcculutionsIntoUI(this.iconOcclusions, this.armUIClock, new Vector3(0f, 0f, 0f), new Vector3(0.7f, 0.15f, 2.5f) );

            this.handUtil = new HandUtil(playerHeadTransform);
            //Debug.Log("handUtil: " + handUtil);

            this.isVisibleVirtualWearable = false;
        }

        private void Start()
        {
            this.appArea.SpheroidCenterObj.transform.localScale = Vector3.zero;
            this.appArea.SpheroidCenterObj.SetActive(false);
        }


        private void FixedUpdate()
        {
            float cyberCircuitTime = Time.time * appCyberCircuitTimeSpeed;

            foreach (AppColor appColor in appColorMap)
            {
                appMatBlock.SetColor("_MainColor", appColor.color);
                appMatBlock.SetFloat("_CyberCircuitTime", cyberCircuitTime);
                appColor.renderer.SetPropertyBlock(appMatBlock);
            }

            cyberCircuitTime = Time.time * cyberCircuitTimeSpeed;
            this.palmUI.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_CyberCircuitTime", cyberCircuitTime);
            this.armUI.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_CyberCircuitTime", cyberCircuitTime);
            this.thumbUI.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_CyberCircuitTime", cyberCircuitTime);
        }

        private void OnDrawGizmos()
        {
            if (!isShowGizmo) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.appArea.SpheroidCenterObj.transform.position, this.appArea.SpheroidData.radius.x);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.appArea.SpheroidCenterObj.transform.position, this.appArea.SpheroidData.radius.z);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(this.appArea.SpheroidCenterObj.transform.position, 0.03f * Vector3.one);
        }

        private void MoveIconsIntoUI(GameObject sourceParent, GameObject targetParent,
            Vector3? localPosition, Quaternion? localRotation, Vector3? localScale)
        {
            int targetChildCount = targetParent.transform.childCount;
            for (int i = sourceParent.transform.childCount - 1; i >= 0; i--)
            {
                if (i >= targetChildCount)
                {
                    continue;
                }
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

            appArea.UpdateTransform(palmPosition, palmQuaternion);



            // Icons
            //for (int i = 0; i < this.palmIcons.gameObject.transform.childCount; i++)
            //{
            //    this.palmIcons.gameObject.transform.GetChild(i).position = this.palmUI.gameObject.transform.GetChild(i).position;
            //    this.palmIcons.gameObject.transform.GetChild(i).rotation = this.palmUI.gameObject.transform.GetChild(i).rotation * Quaternion.AngleAxis(270, Vector3.left); // * this.vwUI.transform.rotation;
            //}
            //for (int i = 0; i < this.firstAppIcons.gameObject.transform.childCount; i++)
            //{
            //    this.firstAppIcons.gameObject.transform.GetChild(i).position = this.firstHandWingUI.gameObject.transform.GetChild(i).position;
            //    this.firstAppIcons.gameObject.transform.GetChild(i).rotation = this.firstHandWingUI.gameObject.transform.GetChild(i).rotation * Quaternion.AngleAxis(270, Vector3.left); // * this.vwUI.transform.rotation;
            //}
            //for (int i = 0; i < this.secondAppIcons.gameObject.transform.childCount; i++)
            //{
            //    this.secondAppIcons.gameObject.transform.GetChild(i).position = this.secondHandWingUI.gameObject.transform.GetChild(i).position;
            //    this.secondAppIcons.gameObject.transform.GetChild(i).rotation = this.secondHandWingUI.gameObject.transform.GetChild(i).rotation * Quaternion.AngleAxis(270, Vector3.left); // * this.vwUI.transform.rotation;
            //}

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
