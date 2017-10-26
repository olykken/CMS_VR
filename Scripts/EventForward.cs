/*
    EventForward.cs

	This is a NovaVR script 
	created for the CMS Collaboration CMS.VR project
	http://novavrllc.com

	Version 0.5

	Copyright (c) 2017 NovaVR LLC.
	Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby
	granted, provided that the above copyright notice and this permission notice appear in all copies.

	THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL
	IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
	INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN
	AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
	PERFORMANCE OF THIS SOFTWARE.
	
*/
// This script allows the player to load in the next event.

using UnityEngine;
using System.Collections;

public class EventForward : MonoBehaviour {

    public GameObject escroll;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device Controller { get { return SteamVR_Controller.Input((int)trackedObject.index); } }

    // moves one event forward in the array's list
    public void OnNewEvent()
    {
        escroll.GetComponent<EventScroller>().NextOne();
    }

    // moves one event back in the array's list
    public void OnOldEvent()
    {
        escroll.GetComponent<EventScroller>().LastOne();

    }

    // removes the events and replaces them with the detector pieces
    public void OnNoEvent()
    {
        escroll.GetComponent<EventScroller>().currentEvent.SetActive(false);

        foreach (GameObject area in escroll.GetComponent<EventScroller>().eventArea)
        {
            area.SetActive(true);
        }
    }

    // Get trigger inputs
    void Start()
    {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.GetPressDown(triggerButton))
        {
            Controller.TriggerHapticPulse(500);
            escroll.GetComponent<EventScroller>().NextOne();
            Debug.Log("right trigger was pressed");
        }
    }
}
