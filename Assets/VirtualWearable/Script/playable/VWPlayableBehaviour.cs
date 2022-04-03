using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class VWPlayableBehaviour : PlayableBehaviour
{
    public GameObject vw;

    public float targetVWAlpha;
    private float srcVWAlpha;

    public float maxWingLeftVerticalClipPercent, minWingLeftVerticalClipPercent;
    public float maxPalmRightVerticalClipPercent, minPalmRightVerticalClipPercent;
    public float maxThumbTopHorizontalClipPercent, minThumbTopHorizontalClipPercent;

    private GameObject handWingObj;
    private GameObject handWingUpperObj;
    private GameObject thumbButtonObj;
    private GameObject handPalmObj;

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable) {
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable) {
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //this.handWingObj.GetComponent<MeshRenderer>().enabled = true;
        //this.handPalmObj.GetComponent<MeshRenderer>().enabled = true;

        this.handWingObj = this.vw.gameObject.transform.Find("FirstHandWingUI").gameObject;
        this.handWingUpperObj = this.vw.gameObject.transform.Find("SecondHandWingUI").gameObject;
        this.thumbButtonObj = this.vw.gameObject.transform.Find("ThumbUI").gameObject;
        this.handPalmObj = this.vw.gameObject.transform.Find("PalmUI").gameObject;

        this.srcVWAlpha = this.handWingObj.GetComponent<MeshRenderer>().sharedMaterial.GetFloat("_Transparent");
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        Material mat;
        float progress = Mathf.Clamp01((float)(playable.GetTime() / playable.GetDuration())); //0.0 - 1.0
        float alpha = Mathf.Lerp(this.srcVWAlpha, this.targetVWAlpha, progress);

        float wingPercent = Mathf.Lerp(this.maxWingLeftVerticalClipPercent, this.minWingLeftVerticalClipPercent, progress);
        mat = this.handWingObj.GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetFloat("_LeftVerticalClipPercent", wingPercent);
        mat.SetFloat("_Transparent", alpha);

        mat = this.handWingUpperObj.GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetFloat("_LeftVerticalClipPercent", wingPercent);
        mat.SetFloat("_Transparent", alpha);

        float palmPercent = Mathf.Lerp(this.maxPalmRightVerticalClipPercent, this.minPalmRightVerticalClipPercent, progress);
        mat = this.handPalmObj.GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetFloat("_RightVerticalClipPercent", palmPercent);
        mat.SetFloat("_Transparent", alpha);

        float thumbPercent = Mathf.Lerp(this.maxThumbTopHorizontalClipPercent, this.minThumbTopHorizontalClipPercent, progress);
        mat = this.thumbButtonObj.GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetFloat("_TopHorizontalClipPercent", thumbPercent);
        mat.SetFloat("_Transparent", alpha);

        this.thumbButtonObj.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_TopHorizontalClipPercent", thumbPercent);
    }
}
