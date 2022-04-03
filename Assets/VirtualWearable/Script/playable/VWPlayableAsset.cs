using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class VWPlayableAsset : PlayableAsset
{
    public ExposedReference<GameObject> vw;

    //Timelineでいじるパラメータ
    public float targetVWAlpha;
    public float maxWingLeftVerticalClipPercent, minWingLeftVerticalClipPercent;
    public float maxPalmRightVerticalClipPercent, minPalmRightVerticalClipPercent;
    public float maxThumbTopHorizontalClipPercent, minThumbTopHorizontalClipPercent;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        //behaviourに対してパラメータを入れ込んでいく
        VWPlayableBehaviour behaviour = new VWPlayableBehaviour();
        behaviour.vw =  this.vw.Resolve(graph.GetResolver()); //ExposedReferenceからとる時のおまじない

        behaviour.targetVWAlpha = this.targetVWAlpha;
        behaviour.maxWingLeftVerticalClipPercent = this.maxWingLeftVerticalClipPercent;
        behaviour.minWingLeftVerticalClipPercent = this.minWingLeftVerticalClipPercent;
        behaviour.maxPalmRightVerticalClipPercent = this.maxPalmRightVerticalClipPercent;
        behaviour.minPalmRightVerticalClipPercent = this.minPalmRightVerticalClipPercent;
        behaviour.maxThumbTopHorizontalClipPercent = this.maxThumbTopHorizontalClipPercent;
        behaviour.minThumbTopHorizontalClipPercent = this.minThumbTopHorizontalClipPercent;

        return ScriptPlayable<VWPlayableBehaviour>.Create(graph, behaviour);
    }
}
