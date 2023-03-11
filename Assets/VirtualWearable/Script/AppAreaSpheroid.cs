using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VirtualWearable
{
    public class AppArea
    {
        public enum AppDisplayingMode
        {
            OnHand = 0,
            InLineOfSight = 1
        };
        public struct App
        {
            public Vector3 scale;
        }
        public struct AppAreaLayer
        {
            public int partitionedNum; //app num, ï™äÑêî
            public float displayingAngle;
            public float phi;
        }

        private GameObject spheroidCenterObj;
        private Spheroid spheroid;
        private List<MeshRenderer> appIconRenderers;
        private GameObject appShowcase;
        private GameObject appIconsOnRightHand;
        private AppDisplayingMode appDisplayingMode;
        private Vector3 spheroidScale;

        public GameObject SpheroidCenterObj { get { return spheroidCenterObj; } }
        public List<MeshRenderer> AppIconRenderers { get { return appIconRenderers; } }
        public Spheroid SpheroidData { get { return spheroid; } }
        public Vector3 SpheroidScale { get { return spheroidScale; } }

        private AppAreaLayer[] appAreaLayers;
        private App app;

        public AppArea(GameObject appIconsOnRightHand, GameObject appShowcaseOnStage, GameObject appShowcaseOnHand, AppDisplayingMode appDisplayingMode)
        {
            if (appDisplayingMode == AppArea.AppDisplayingMode.InLineOfSight)
            {
                appShowcase = appShowcaseOnStage;
                appShowcaseOnStage.transform.parent.gameObject.SetActive(true);
                appShowcaseOnHand.transform.parent.gameObject.SetActive(false);
            }
            else if (appDisplayingMode == AppArea.AppDisplayingMode.OnHand)
            {
                appShowcase = appShowcaseOnHand;
                appShowcaseOnStage.transform.parent.gameObject.SetActive(false);
                appShowcaseOnHand.transform.parent.gameObject.SetActive(true);
            }

            this.appIconsOnRightHand = appIconsOnRightHand;
            this.appDisplayingMode = appDisplayingMode;

            this.spheroidCenterObj = this.appShowcase.transform.Find("SpheroidCenter").gameObject;

            this.appIconRenderers = new List<MeshRenderer>();
            foreach (MeshRenderer renderer in this.appIconsOnRightHand.GetComponentsInChildren<MeshRenderer>())
            {
                this.appIconRenderers.Add(renderer);
            }

            Transform t = this.spheroidCenterObj.transform;

            if (appDisplayingMode == AppDisplayingMode.InLineOfSight)
            {
                this.spheroid = new Spheroid(t.position, t.rotation, radius: new Vector3(0.075f, 0.065f, 0.065f) );
                this.spheroidScale = Vector3.one * 2.5f;
                this.app = new App() { scale = Vector3.one * 0.7f };
                this.appAreaLayers = new AppAreaLayer[2]
                {
                    new AppAreaLayer() { partitionedNum = 5, displayingAngle = 120f, phi = Mathf.PI * 0.57f },
                    new AppAreaLayer() { partitionedNum = 6, displayingAngle = 160f, phi = Mathf.PI * 0.7f }
                };
            }
            else if (appDisplayingMode == AppDisplayingMode.OnHand)
            {
                this.spheroid = new Spheroid(t.position, t.rotation, radius: new Vector3(0.085f, 0.065f, 0.065f) );
                this.spheroidScale = Vector3.one;
                this.app = new App() { scale = Vector3.one * 1.0f };
                this.appAreaLayers = new AppAreaLayer[1]
                {
                    new AppAreaLayer() { partitionedNum = 5, displayingAngle = 120f, phi = Mathf.PI * 0.7f }
                };
            }
            this.SetAppPointsIn(spheroidCenterObj);

            if (appDisplayingMode == AppDisplayingMode.InLineOfSight)
            {
                this.spheroidCenterObj.transform.localRotation = Quaternion.Euler(270, 180, 0);
            }
        }

        public void UpdateTransform(Vector3 palmPosition, Quaternion palmQuaternion)
        {
            if (appDisplayingMode == AppDisplayingMode.OnHand)
            {
                this.appShowcase.transform.localPosition = palmPosition;
                this.appShowcase.transform.localRotation = palmQuaternion * Quaternion.AngleAxis(270, Vector3.left);
            }
        }

        private void SetAppPointsIn(GameObject appPointsParent)
        {
            // Set settings
            for (int li = 0; li < appAreaLayers.Length; li++)
            {
                for (int pi = 0; pi < appAreaLayers[li].partitionedNum; pi++)
                {
                    var appPointObj = new GameObject(string.Format("AppPoint_{0}_{1}", li, pi));
                    appPointObj.transform.position = this.spheroid.PositionOnSphere(
                        pi, appAreaLayers[li].phi,
                        appAreaLayers[li].displayingAngle, appAreaLayers[li].partitionedNum
                    );
                    appPointObj.transform.LookAt(appPointsParent.transform, Vector3.forward);
                    appPointObj.transform.parent = appPointsParent.transform;
                    appPointObj.transform.localScale = app.scale;
                }
            }
        }


        [System.Serializable]
        public class Spheroid
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 radius;

            public Spheroid(Vector3 position, Quaternion rotation, Vector3 radius)
            {
                this.position = position;
                this.rotation = rotation;
                this.radius = radius;
            }

            public Vector3 PositionOnSphere(int index, float phi, float angle, int partitionedNum)
            {
                float partitionedAngle = angle / partitionedNum;
                float offsetAngle = 90f - angle / 2f;
                float currentTheta = (offsetAngle + index * partitionedAngle) * Mathf.Deg2Rad;

                //https://www.tutorialspoint.com/plotting-points-on-the-surface-of-a-sphere-in-python-s-matplotlib
                Vector3 point = new Vector3(
                    position.x + radius.x * Mathf.Sin(phi) * Mathf.Cos(currentTheta),
                    position.y + radius.y * Mathf.Sin(phi) * Mathf.Sin(currentTheta),
                    position.z + radius.z * Mathf.Cos(phi)
                );
                //point = RotatePointAroundPivot(transform.position, point, new Vector3(270, 180, 0));

                return point;
            }
            //private Vector3 RotatePointAroundPivot(Vector3 pivot, Vector3 point, Vector3 angle)
            //{
            //    Vector3 dir = point - pivot;
            //    dir = Quaternion.Euler(angle) * dir;
            //    point = dir + pivot;
            //    return point;
            //}
        }
    }
}
