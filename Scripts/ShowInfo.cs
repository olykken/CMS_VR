/*
    ShowInfo.cs

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
//Allows the event data to be read by the user

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInfo : MonoBehaviour {

    private GameObject target;
    private GameObject lastTarget;
    private GameObject info;
    private GameObject intoClear;

    public GameObject display;

    private void OnTriggerEnter(Collider other)
    {
        if (info == null)
        {
            target = other.gameObject;
            lastTarget = other.gameObject;
            info = other.gameObject.transform.Find("InfoCanvas").gameObject;
            info.gameObject.transform.SetParent(display.transform);
            Debug.Log("Data Found");
        }
        else
        {
            info.gameObject.transform.SetParent(target.transform);
            info.transform.localPosition = new Vector3(0, 500, 0);
            info = null;
            target = null;
            Debug.Log("Removing Data");

            target = other.gameObject;
            lastTarget = other.gameObject;
            info = other.gameObject.transform.Find("InfoCanvas").gameObject;
            info.gameObject.transform.SetParent(display.transform);
            Debug.Log("Data Found");
        }
    }

    private void OnTriggerStay(Collider other)
    {
            info.transform.localPosition = new Vector3(0, 0, 0);
            info.transform.localRotation = new Quaternion(0, 0, 0, 0);
            Debug.Log("Showing Data");
    }

    private void OnTriggerExit(Collider other)
    {
        if (info != null)
        {
            info.gameObject.transform.SetParent(target.transform);
            info.transform.localPosition = new Vector3(0, 500, 0);
            info = null;
            target = null;
            Debug.Log("Removing Data");
        }
    }
}
