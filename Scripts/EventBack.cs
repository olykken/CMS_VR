// Created by Oliver Lykken
// This script allows the player to load in the previous event.

using UnityEngine;
using System.Collections;

public class EventBack : MonoBehaviour {

    public GameObject escroll;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObject.index); } }

    // Get trigger inputs
    void Start()
    {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    // Load up the previous event
    void Update()
    {
        if (controller.GetPressDown(triggerButton))
        {
            controller.TriggerHapticPulse(700);
            escroll.GetComponent<EventScroller>().LastOne();
            Debug.Log("right trigger was pressed");
        }
    }
}
