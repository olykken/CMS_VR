/*
    EventScroller.cs

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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using VRTK;
using CMSdraw;

public class EventScroller : MonoBehaviour {

    //Particle effect that plays whenever an event is loaded into the detector area
    public GameObject eventSpray;

	public GameObject[] eventArea;//Pieces of the detector made inactive while events are shown
	public GameObject[] events;//List of all events
	public GameObject currentEvent;//The current event being displayed

    //References to the individual pieces of the events
    Transform[] sub_eventsE;
    Transform[] sub_eventsT;
    Transform[] sub_eventsJ;
    Transform[] sub_eventsME;
    Transform[] sub_eventsMU;
    Transform[] sub_eventsHB;
    Transform[] sub_eventsHE;
    Transform[] sub_eventsEB;
    Transform[] sub_eventsEE;

	int lastEvent;//indexed reference to the last event displayed

	int eventNumber = 0;
	int num_events = 0;

    string folderPath;

    //Animation Variables
    private float _currentScale = InitScale;
    private const float TargetScale = 1f;
    private const float InitScale = .01f;
    private const int FramesCount = 100;
    private const float AnimationTimeSeconds = 1;
    private float _deltaTime = AnimationTimeSeconds / FramesCount;
    private float _deltaScale = (TargetScale - InitScale) / FramesCount;
    private bool _upScale = false;

    void Start()
    {

        //Gets the Detector pieces that are where the events happen and puts them in an array
        eventArea = GameObject.FindGameObjectsWithTag("eventArea")
            .Cast<GameObject>()
            .ToArray();

        //Gets the complete events from the readJSON script
        folderPath = Path.GetFullPath("Assets/activeEvents/");//Path to actual event files
        Debug.Log("Using folderpath " + folderPath);
        readJSON CMS_JSON = new readJSON(folderPath);
        events = new GameObject[eventNumber];
        events = CMS_JSON.events.ToArray();
        Debug.Log("Rendered " + CMS_JSON.eventNumber + " events");
        foreach (GameObject e in events)
        {
            e.transform.position = new Vector3(0, -119.9f, -0.1f);
            e.transform.Rotate(0, 90, 0);
            GameObject EventCanvas = new GameObject("EventCanvas");
            EventCanvas.AddComponent<Canvas>();
            EventCanvas.AddComponent<GraphicRaycaster>();
            EventCanvas.AddComponent<CanvasScaler>();
            EventCanvas.tag = "EventCanvas";
            EventCanvas.transform.position = new Vector3(0.1f, -119.9f, -3.4f);
            EventCanvas.transform.Rotate(0.0f, -90.0f, 0.0f);
            EventCanvas.transform.localScale = new Vector3(0.006f, 0.0065f, 0.01f);
            EventCanvas.transform.SetParent(e.transform);

            e.transform.Find("Electrons").tag = "Electrons";
            e.transform.Find("Tracks").tag = "Tracks";
            e.transform.Find("Jets").tag = "Jets";
            e.transform.Find("MissingET").tag = "MET";
            e.transform.Find("Muons").tag = "Muons";
            e.transform.Find("HBRechits").tag = "HBrechits";
            e.transform.Find("HERechits").tag = "HErechits";
            e.transform.Find("EBRechits").tag = "EBrechits";
            e.transform.Find("EERechits").tag = "EErechits";

            sub_eventsE = e.transform.Find("Electrons").GetComponentsInChildren<Transform>();
            sub_eventsT = e.transform.Find("Tracks").GetComponentsInChildren<Transform>();
            sub_eventsJ = e.transform.Find("Jets").GetComponentsInChildren<Transform>();
            sub_eventsME = e.transform.Find("MissingET").GetComponentsInChildren<Transform>();
            sub_eventsMU = e.transform.Find("Muons").GetComponentsInChildren<Transform>();
            sub_eventsHB = e.transform.Find("HBRechits").GetComponentsInChildren<Transform>();
            sub_eventsHE = e.transform.Find("HERechits").GetComponentsInChildren<Transform>();
            sub_eventsEB = e.transform.Find("EBRechits").GetComponentsInChildren<Transform>();
            sub_eventsEE = e.transform.Find("EERechits").GetComponentsInChildren<Transform>();

            List<Transform> all_sub_events = new List<Transform>();
            if (sub_eventsE != null) all_sub_events.AddRange(sub_eventsE);
            if (sub_eventsT != null) all_sub_events.AddRange(sub_eventsT);
            if (sub_eventsJ != null) all_sub_events.AddRange(sub_eventsJ);
            if (sub_eventsME != null) all_sub_events.AddRange(sub_eventsME);
            if (sub_eventsMU != null) all_sub_events.AddRange(sub_eventsMU);
            if (sub_eventsHB != null) all_sub_events.AddRange(sub_eventsHB);
            if (sub_eventsHE != null) all_sub_events.AddRange(sub_eventsHE);
            if (sub_eventsEB != null) all_sub_events.AddRange(sub_eventsEB);
            if (sub_eventsEE != null) all_sub_events.AddRange(sub_eventsEE);

            foreach (Transform t in all_sub_events)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                //t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
                GameObject infocanvas = new GameObject("InfoCanvas");
                infocanvas.AddComponent<VRTK_InteractableObject>();
                infocanvas.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
                infocanvas.GetComponent<VRTK_InteractableObject>().validDrop = VRTK_InteractableObject.ValidDropTypes.Drop_Anywhere;
                infocanvas.AddComponent<Canvas>();
                infocanvas.AddComponent<GraphicRaycaster>();
                infocanvas.AddComponent<CanvasScaler>();
                infocanvas.tag = "InfoCanvas";
                infocanvas.transform.position = new Vector3(0, 0f, 0);
                infocanvas.transform.localScale = new Vector3(0.0018f, 0.0022f, 0.01f);
                infocanvas.transform.SetParent(t.transform);
                GameObject text = new GameObject("Text");
                Text infotext = text.AddComponent<Text>();
                text.transform.localPosition = new Vector3(0, 0f, 0.0f);
                text.transform.localScale = new Vector3(.0018f, .0022f, 1f);
                text.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.GetComponent<Text>().fontSize = 11;
                text.GetComponent<Text>().fontStyle = FontStyle.Bold;
                text.GetComponent<Text>().color = Color.black;
                text.transform.SetParent(infocanvas.transform);
                infocanvas.SetActive(true);
                if (t.gameObject != null && t.gameObject.name.Contains("track"))
                    {
                       infotext.GetComponent<Text>().text = t.gameObject.GetComponent<Track>().track_name + "\n"
                       + " pt = " + t.gameObject.GetComponent<Track>().track_pt + "\n"
                       + " eta = " + t.gameObject.GetComponent<Track>().track_eta + "\n"
                       + " phi = " + t.gameObject.GetComponent<Track>().track_phi + "\n"
                       + " charge = " + t.gameObject.GetComponent<Track>().track_charge + "\n";
                    }
                else if (t.gameObject.name.Contains("jet"))
                    {
                       infotext.GetComponent<Text>().text = t.gameObject.GetComponent<Jet>().jet_name + "\n"
                       + " pt = " + t.gameObject.GetComponent<Jet>().jet_pt + "\n"
                       + " eta = " + t.gameObject.GetComponent<Jet>().jet_eta + "\n"
                       + " phi = " + t.gameObject.GetComponent<Jet>().jet_phi + "\n"
                       + " theta = " + t.gameObject.GetComponent<Jet>().jet_theta + "\n";
                    }
                else if (t.gameObject.name.Contains("muon"))
                    {
                       infotext.GetComponent<Text>().text = t.gameObject.GetComponent<Muon>().muon_name + "\n"
                       + " pt = " + t.gameObject.GetComponent<Muon>().muon_pt + "\n"
                       + " eta = " + t.gameObject.GetComponent<Muon>().muon_eta + "\n"
                       + " phi = " + t.gameObject.GetComponent<Muon>().muon_phi + "\n"
                       + " charge = " + t.gameObject.GetComponent<Muon>().muon_charge + "\n";
                    }
                else if (t.gameObject.name.Contains("electron"))
                     {
                       infotext.GetComponent<Text>().text = t.gameObject.GetComponent<Track>().track_name + "\n"
                       + " pt = " + t.gameObject.GetComponent<Track>().track_pt + "\n"
                       + " eta = " + t.gameObject.GetComponent<Track>().track_eta + "\n"
                       + " phi = " + t.gameObject.GetComponent<Track>().track_phi + "\n"
                       + " charge = " + t.gameObject.GetComponent<Track>().track_charge + "\n";
                    }
                else if (t.gameObject.name.Contains("MET"))
                    {
                       infotext.GetComponent<Text>().text = t.gameObject.GetComponent<MET>().MET_name + "\n"
                       + " pt = " + t.gameObject.GetComponent<MET>().MET_pt + "\n"
                       + " phi = " + t.gameObject.GetComponent<MET>().MET_phi + "\n";
                    }
                else if (t.gameObject.name.Contains("rechit"))
                    {
                       infotext.GetComponent<Text>().text = t.gameObject.GetComponent<Rechit>().rechit_name + "\n"
                       + " energy = " + t.gameObject.GetComponent<Rechit>().rechit_energy + "\n"
                       + " eta = " + t.gameObject.GetComponent<Rechit>().rechit_eta + "\n"
                       + " phi = " + t.gameObject.GetComponent<Rechit>().rechit_phi + "\n"
                       + " time = " + t.gameObject.GetComponent<Rechit>().rechit_time + "\n";
                    }
                else
                       infotext.GetComponent<Text>().text = "No Data Available";
 
               
            }
        }
        Debug.Log("events.Count = " + events.Count());
    }

	//Checks to determine what actions the player wants to happen
	public bool Clicked() {
		if (OVRInput.GetUp (OVRInput.Button.SecondaryShoulder) || Input.GetKey(KeyCode.N) || OVRInput.GetUp(OVRInput.Button.One)) {
			return true;
		} else {
			return false;
		}
	}

	public bool Unclick() {
		if (OVRInput.GetUp (OVRInput.RawButton.Y) || Input.GetKey(KeyCode.Y) || OVRInput.GetUp(OVRInput.Button.Four)) {
			return true;
		} else {
			return false;
		}
	}

	public bool BackClick() {
		if (OVRInput.GetUp (OVRInput.Button.PrimaryShoulder) || Input.GetKey(KeyCode.B) || OVRInput.GetUp(OVRInput.Button.Two)) {
			return true;
		} else {
			return false;
		}
	}

    public void NextOne()
    {
        foreach (GameObject area in eventArea)
        {
            area.SetActive(false);
        }
        if (eventNumber < events.Length - 1)
        {
            eventNumber++;
        }
        else {
            eventNumber = 0;
        }
        Debug.Log("click: eventNumber = " + eventNumber);
        Debug.Log("loaded event is " + events[eventNumber]);
        if (currentEvent != null) currentEvent.SetActive(false);
        currentEvent = events[eventNumber];
        currentEvent.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        currentEvent.SetActive(true);
        eventSpray.GetComponent<ParticleSystem>().Play();
        StartCoroutine(GrowEvent());
    }

    public void LastOne()
    {
        foreach (GameObject area in eventArea)
        {
            area.SetActive(false);
        }
        if (eventNumber == 0)
        {
            lastEvent = events.Count() - 1;
            eventNumber = lastEvent;
        }
        else
        {
            eventNumber--;
        }
        Debug.Log("click: eventNumber = " + eventNumber);
        Debug.Log("loaded event is " + events[eventNumber]);
        if (currentEvent != null) currentEvent.SetActive(false);
        currentEvent = events[eventNumber];
        currentEvent.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        currentEvent.SetActive(true);
        eventSpray.GetComponent<ParticleSystem>().Play();
        StartCoroutine(GrowEvent());
    }

    //Makes the events simulate a collision
    private IEnumerator GrowEvent()
    {
        Vector3 initScale = currentEvent.gameObject.transform.localScale;
        Vector3 finalScale = new Vector3(1.0f,1.0f,1.0f);

        float currentTime = 0.0f;

        do
        {
            currentEvent.gameObject.transform.localScale = Vector3.Lerp(initScale, finalScale, currentTime / 2);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= 2);
    }

    void Update () {

		// moves one event forward in the array's list
		if (Clicked()) {
            NextOne();
		}

		// moves one event back in the array's list
		if (BackClick()) {
            LastOne();

        }

		// removes the events and replaces them with the detector pieces
		if (Unclick ()) {
            currentEvent.SetActive (false);

			foreach (GameObject area in eventArea) {
				area.SetActive (true);
			}
		}

    } // end Update
}
