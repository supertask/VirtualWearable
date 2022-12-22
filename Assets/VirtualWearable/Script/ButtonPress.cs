using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class ButtonPress : MonoBehaviour
{
    public float pressingSpeed = 0.05f;
    public float returnSpeed = 6.0f;

    private GameObject buttonTop;
    private GameObject buttonBottom;
    private Vector3 buttonBottomScale;
    private float buttonPressingInflation;
    private float buttonPressedInflation;

    //public Color inactiveColor;
    //public Color activeColor;

    private Vector3 startLocalPosition;
    private float activationDistance;

    //Trigger fingers
    public GameObject handsRig;
    private Vector3 previousFingerPos;
    private bool isFingersInCollider;
    private List<GameObject> fingerTips;

    [Serializable] private class ButtonPressingEvent : UnityEvent<float> { }
    [SerializeField] private UnityEvent onButtonPressed;
    [SerializeField] private ButtonPressingEvent onButtonPressing;

    private bool isPressed;

    void Start()
    {
        // Remember start position of button
        this.buttonTop = this.transform.Find(this.name + "Top").gameObject;
        this.buttonBottom = this.transform.Find(this.name + "Bottom").gameObject;
        this.startLocalPosition = this.buttonTop.transform.localPosition;
        Vector3 diffWithBottom = this.buttonTop.transform.localPosition - this.buttonBottom.transform.localPosition;
        this.activationDistance = Math.Abs(diffWithBottom.y);
        Debug.Log("activationDistance: " + this.activationDistance);
        Debug.Log("startLocalPosition: " + this.startLocalPosition.y);

        this.previousFingerPos = Vector3.zero;
        this.isFingersInCollider = false;
        this.fingerTips = Util.FindColliderObjectsRecursively(handsRig.gameObject);
        /*
        foreach(GameObject fingerTip in this.fingerTips) {
            Debug.Log(fingerTip.name);
        }*/
        this.isPressed = false;
        this.buttonPressingInflation = 1.05f;
        this.buttonPressedInflation = 1.15f;
        this.buttonBottomScale = this.buttonBottom.transform.localScale;
    }

    void Update()
    {
        if (this.isFingersInCollider) { return; }
        this.buttonTop.transform.localPosition = Vector3.Lerp(this.buttonTop.transform.localPosition, startLocalPosition, Time.deltaTime * returnSpeed);

        //float buttonPercent = Util.Remap(distanceWithStartPos, 0, this.activationDistance, 0, 1); //0~this.activationDistance -> 0~1
        //if (buttonBottom) buttonBottom.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, activeColor, buttonPercent);
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enter");
        this.isFingersInCollider = false;
        foreach (GameObject tip in fingerTips) {
            if (tip == other.gameObject) { this.isFingersInCollider = true; }
        }
        this.isPressed = false;
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("Stay");
        if (! this.isFingersInCollider || this.isPressed) { return; }

        //Is pressed?
        Vector3 diffWithStartPos = this.buttonTop.transform.localPosition - this.startLocalPosition;
        float distanceWithStartPos =  Math.Abs(diffWithStartPos.y);
        if (distanceWithStartPos < this.activationDistance) {
            this.isPressed = false;
            float pressingProgress = distanceWithStartPos / this.activationDistance;
            this.buttonBottom.transform.localScale = Vector3.Lerp(
                this.buttonBottomScale,
                this.buttonBottomScale * this.buttonPressingInflation,
                pressingProgress
            );
        } else {
            this.isPressed = true;
            this.onButtonPressed.Invoke();
            this.buttonBottom.transform.localScale = this.buttonBottomScale * buttonPressedInflation;
        }

        Vector3 diffPos = other.gameObject.transform.position - this.previousFingerPos;
        Vector3 localDiffPos = this.transform.InverseTransformPoint(diffPos) * this.pressingSpeed;

        Vector3 newLocalPosition = this.buttonTop.transform.localPosition;
        newLocalPosition.y -= localDiffPos.y;
        newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, this.startLocalPosition.y - this.activationDistance, this.startLocalPosition.y);
        this.buttonTop.transform.localPosition = newLocalPosition;

        this.previousFingerPos = other.gameObject.transform.position;
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("Exit");
        this.isFingersInCollider = false;
        this.previousFingerPos = Vector3.zero;
        this.isPressed = false;
        this.buttonBottom.transform.localScale = this.buttonBottomScale;
    }

}