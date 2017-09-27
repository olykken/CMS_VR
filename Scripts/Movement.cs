using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public enum State
    {
        NULL,
        LTOUCH,
        RTOUCH
    };

    public OVRInput.Controller Controller = OVRInput.Controller.None;
    public GameObject cameraRig;

    private State activecontroller;
    private Vector3 moveDirection;
    private Vector3 elevationDirection;

    // Use this for initialization
    void Start () {
        cameraRig.GetComponent<Rigidbody>().useGravity = false;

        if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
        {
            activecontroller = State.LTOUCH;
        }
        else if (OVRInput.GetActiveController() == OVRInput.Controller.RTouch)
        {
            activecontroller = State.RTOUCH;
        }
        else
        {
            activecontroller = State.NULL;
        }

    }

    // Update is called once per frame
    void Update () {
        switch (activecontroller)
        {
            case State.RTOUCH:
                moveDirection = new Vector3(Input.GetAxis("Oculus_GearVR_RThumbstickX"), 0, Input.GetAxis("Oculus_GearVR_RThumbstickY"));
                cameraRig.transform.Translate(moveDirection * Time.deltaTime);
                Debug.Log("Active controller is right touch");
                break;
            case State.LTOUCH:
                elevationDirection = new Vector3(0, Input.GetAxis("Oculus_GearVR_LThumbstickY"), 0);
                cameraRig.transform.Translate(elevationDirection * Time.deltaTime);
                Debug.Log("Active controller is left touch");
                break;
            case State.NULL:
                moveDirection = new Vector3(0,0,0);
                elevationDirection = new Vector3(0, 0, 0);
                break;
        }
	}
}
