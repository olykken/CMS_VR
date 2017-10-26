/*
    InfoGrab.cs

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
// Controls the hand-panel that has the voice commands on it

using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.UI;
using CMSdraw;

public class InfoGrab : MonoBehaviour {

    private SteamVR_Controller.Device SteamController { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    private ulong gripButton = SteamVR_Controller.ButtonMask.ApplicationMenu;

    private OVRInput.RawButton trigger = OVRInput.RawButton.RIndexTrigger;

    private GameObject target;
    private GameObject info;

    private int grabCount = 0;
    private int buttonTime = 3;

    public GameObject display;


    // Use this for initialization
    void Start() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        display.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

        if ((SteamController.GetPressDown(gripButton) && grabCount == 0) 
            || (OVRInput.GetUp(trigger) && grabCount == 0))
        {
            display.SetActive(true);
            grabCount = 1;
            Debug.Log("HandPanel Activated");
        }else if ((OVRInput.GetUp(trigger) && grabCount == 1) 
            || (SteamController.GetPressDown(gripButton) && grabCount==1))
        {
            display.SetActive(false);
            grabCount = 0;
            Debug.Log("HandPanel Deactivated");
        }
    } // end Update
}

