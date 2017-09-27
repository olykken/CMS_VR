using UnityEngine;
using System.Collections;

public class EventForward : MonoBehaviour {

    public GameObject escroll;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObject.index); } }

    // Get trigger inputs
    void Start()
    {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.GetPressDown(triggerButton))
        {
            controller.TriggerHapticPulse(700);
            escroll.GetComponent<EventScroller>().NextOne();
            Debug.Log("right trigger was pressed");
        }
    }
}
