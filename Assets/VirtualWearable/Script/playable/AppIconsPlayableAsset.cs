using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace VW
{
    [System.Serializable]
    public class AppIconsPlayableAsset : PlayableAsset
    {
        //public ExposedReference<GameObject> icons;
        public ExposedReference<GameObject> vw;

        //Timelineでいじるパラメータ
        public int appIndex;
        public bool isScalingUp;
        public bool applyAllIcons;

        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            //behaviourに対してパラメータを入れ込んでいく
            AppIconsPlayableBehaviour behaviour = new AppIconsPlayableBehaviour();
            behaviour.vw = this.vw.Resolve(graph.GetResolver()); //ExposedReferenceからとる時のおまじない
            behaviour.appIndex = this.appIndex;
            behaviour.isScalingUp = this.isScalingUp;
            behaviour.applyAllIcons = this.applyAllIcons;
            return ScriptPlayable<AppIconsPlayableBehaviour>.Create(graph, behaviour);
        }
    }
}
