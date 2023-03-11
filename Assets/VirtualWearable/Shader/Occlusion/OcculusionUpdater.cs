using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vuforia;

public class OcculusionUpdater : MonoBehaviour
{
    public GameObject arCameraObj;
    
    public List<Material> occlusionMaterials;

    private Texture2D mainTex;
    private Texture2D uvTex;

    private Material videoBackgroundMaterial;

    
    void Start()
    {   
        //this.occulusionMaterial = this.GetComponent<MeshRenderer>().material;
    }

    private void InitARBackgroundMaterial()
    {
        if (this.videoBackgroundMaterial == null && this.arCameraObj.transform.childCount > 0)
        {
            GameObject videoBackgroundObj = this.arCameraObj.transform.GetChild(0).gameObject;
            MeshRenderer videoBackgroundRenderer = videoBackgroundObj.GetComponent<MeshRenderer>();
            Debug.Log("videoObj: " + videoBackgroundObj);

            if (videoBackgroundRenderer != null)
            {
                this.videoBackgroundMaterial = videoBackgroundRenderer.material;
                Debug.Log("videoMat: " + videoBackgroundMaterial);
            }
        }
    }

    void Update()
    {
        this.InitARBackgroundMaterial();
        if (this.videoBackgroundMaterial != null)
        {
            this.mainTex = this.videoBackgroundMaterial.GetTexture("_MainTex") as Texture2D;
            //this.mainTex = this.videoBackgroundMaterial.mainTexture;
            foreach(Material occlusionMat in occlusionMaterials)
            {
                occlusionMat.EnableKeyword("_MAIN_LIGHT_SHADOWS");
                occlusionMat.SetVector("_ARBackgroundTextureSize", new Vector2(this.mainTex.width, this.mainTex.height));
                occlusionMat.SetTexture("_ARBackgroundTexture", this.mainTex);
            }
        }



        /*
        Vuforia.Image image = CameraDevice.Instance.GetCameraImage(PixelFormat.RGBA8888);
        if (image != null)
        {
            image.CopyToTexture(this.mainTex);
            Debug.Log("my mainTex!!!!!!!: " + this.mainTex);
            
            this.occulusionMaterial.SetTexture("ARBackgroundTexture", this.mainTex);
        } 
        */
    }
    
    void OnDestory()
    {
        DestroyImmediate(videoBackgroundMaterial);
        foreach (Material occlusionMat in occlusionMaterials)
        {
            DestroyImmediate(occlusionMat);
        }
    }
}
