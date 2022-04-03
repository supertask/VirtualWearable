using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{

    public static float Remap(float value, float low1, float high1, float low2, float high2) {
            return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }


    public static GameObject FindRecursively(GameObject target, string name)
    {
        foreach (Transform child in target.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.name == name) { return child.gameObject; }
        }
        return null;
    }

    public static void EnableMeshRendererRecursively(GameObject target, bool enabling)
    {
        if (target.GetComponent<MeshRenderer>() != null) {
            target.GetComponent<MeshRenderer>().enabled = enabling;
            return;
        }
        foreach (Transform child in target.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.GetComponent<MeshRenderer>() != null) {
                child.gameObject.GetComponent<MeshRenderer>().enabled = enabling;
            }
        }
        return;
    }

    public static List<GameObject> FindColliderObjectsRecursively(GameObject target)
    {
        List<GameObject> colliderObjs = new List<GameObject>();
        foreach (Transform child in target.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.GetComponent<Collider>() != null) { colliderObjs.Add(child.gameObject); }
        }
        return colliderObjs;
    }

    /// <summary>
    /// 指定された GameObject を複製して返します
    /// </summary>
    public static GameObject Clone(GameObject go)
    {
        var clone = GameObject.Instantiate(go) as GameObject;
        clone.transform.parent = go.transform.parent;
        clone.transform.localPosition = go.transform.localPosition;
        clone.transform.localScale = go.transform.localScale;
        return clone;
    }


    public class Timer
    {
        public float startedTime;
        public float currentTime;
        public float waitingSec;
        public bool isStarted;
        public Timer(float waitingSec)
        {
            this.currentTime = 0.0f;
            this.waitingSec = waitingSec;
            this.isStarted = false;
        }
        public bool OnTime()
        {
            if (this.isStarted)
            {
                if (this.currentTime > this.waitingSec)
                {
                    this.Reset();
                    return true; //設定した時刻が来れば，true
                }
                else { return false; }
            }
            else { return false; }
        }
        public void Start()
        {
            if (this.isStarted) { return; } //既にスタートしている場合
            this.currentTime = 0.0f;
            this.isStarted = true;
            this.startedTime = Time.time;
        }
        public void Reset()
        {
            this.currentTime = 0.0f;
            this.isStarted = false;
        }
        public void Clock()
        {
            this.currentTime += Time.deltaTime;
        }
    }

    //指定の時間が来たら，OnTimeで知らせる
    //ボタンの連打を防ぐ
    public class ButtonMashingStopper
    {
        public float currentTime;
        public float waitingSec;
        public ButtonMashingStopper(float waitingSec)
        {
            this.currentTime = 0.0f;
            this.waitingSec = waitingSec;
        }
        public bool allowNextButton()
        {
            if (currentTime > this.waitingSec)
            {
                this.Reset();
                this.Clock();
                return true;
            }
            else
            {
                this.Clock();
                return false;
            }
        }
        public void Clock() { this.currentTime += Time.deltaTime; }
        public void Reset() { this.currentTime = 0.0f; }
    }
}