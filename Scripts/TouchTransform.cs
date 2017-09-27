using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTransform : MonoBehaviour {

    public OVRInput.Controller controller;

    void Start() {
        OVRManager.display.RecenterPose();
    }

    // Update is called once per frame
    void Update()
    {
            transform.localPosition = OVRInput.GetLocalControllerPosition(controller);
            transform.localRotation = OVRInput.GetLocalControllerRotation(controller);

        if(OVRInput.Get(OVRInput.Button.One, controller))
        {
            Debug.Log("A or X was pressed");
        }
    }
}
