using UnityEngine;
using System.Collections;

public class InfoClear : MonoBehaviour {
    
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    private OVRInput.Axis1D trigger = OVRInput.Axis1D.PrimaryHandTrigger;

    private GameObject child;
    private GameObject lastTarget;
    private bool check;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {
        if (controller.GetPressUp(gripButton) || OVRInput.Get(trigger) == 0 && check == false)
        {
            for (int i = 3; i < gameObject.transform.childCount; i++)
            {
                child = gameObject.transform.GetChild(i).gameObject;
                child.transform.parent = lastTarget.transform;
                child.transform.position = new Vector3(0, 0, 0);
            }
        }
        else if (controller.GetPressUp(gripButton) || OVRInput.Get(trigger) == 0 && check == true) {
            for (int i = 3; i < gameObject.transform.childCount; i++)
            {
                child = gameObject.transform.GetChild(i).gameObject;
                child.SetActive(false);
                child.transform.position = new Vector3(0, 0, 0);
                child.transform.parent = lastTarget.transform;
            }
        }
	}

    private void OnTriggerEnter (Collider collider)
    {
        lastTarget = collider.gameObject;
        check = true;
    }
    private void OnTriggerExit (Collider collider)
    {
        lastTarget = collider.gameObject;
        check = false;
    }
}
