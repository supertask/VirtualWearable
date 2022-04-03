
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

  public class MyToggleToggler : MonoBehaviour {

    public void SetToggle(Toggle toggle) {
        if (toggle.isOn) {
            Debug.Log("Toggle on");
        } else {
            Debug.Log("Toggle off");
        }
    }
  }
