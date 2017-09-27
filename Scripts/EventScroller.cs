/*
    miniEventScroller.cs

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
using System.Xml.Serialization;
using UnityEngine.UI;
using VRTK;
using VRTK.GrabAttachMechanics;

public class EventScroller : MonoBehaviour {

	public GameObject[] eventArea;
	public GameObject[] events;
	public GameObject[] event_collection;
    public GameObject area;
	public GameObject currentEvent;
    Transform[] sub_eventsE;
    Transform[] sub_eventsT;
    Transform[] sub_eventsJ;
    Transform[] sub_eventsME;
    Transform[] sub_eventsMU;
    Transform[] sub_eventsHB;
    Transform[] sub_eventsHE;
    Transform[] sub_eventsEB;
    Transform[] sub_eventsEE;
    Transform this_event;
	int lastEvent;
	int eventNumber = 0;
	int num_events = 0;
    string folderPath;

    /*(Depreciated)
        public Object[] jsons;
        public Object[] event_folders;
        public string event_name;
        public string event_path;
        public string xml_path;
        GameObject this_sub_event;
        */

    void Start()
    {

        //Gets the Detector pieces that are where the events happen and puts them in an array
        eventArea = GameObject.FindGameObjectsWithTag("eventArea")
            .Cast<GameObject>()
            .ToArray();

        /*(Depreciated)
                //Gets the events and the related data from the xml files
                event_folders = Resources.LoadAll ("xml");
                num_events = event_folders.Length;
                events = new GameObject[num_events];
        */

        //Gets the complete events from the readJSON script
        folderPath = "/Users/olykken/Desktop/CMSFermi/CMS tester/Assets/active/"; //Path to actual event files
        Debug.Log("Using folderpath " + folderPath);
        readJSON CMS_JSON = new readJSON(folderPath);
        events = new GameObject[eventNumber];
        events = CMS_JSON.events.ToArray();
        Debug.Log("Rendered " + CMS_JSON.eventNumber + " events");
        foreach (GameObject e in events)
        {
            e.transform.position = new Vector3(0, -119.9f, 0.01f);
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
            e.transform.Find("MET").tag = "MET";
            e.transform.Find("Muons").tag = "Muons";
            e.transform.Find("HBrechits").tag = "HBrechits";
            e.transform.Find("HErechits").tag = "HErechits";
            e.transform.Find("EBrechits").tag = "EBrechits";
            e.transform.Find("EErechits").tag = "EErechits";

            sub_eventsE = e.transform.Find("Electrons").GetComponentsInChildren<Transform>();
            sub_eventsT = e.transform.Find("Tracks").GetComponentsInChildren<Transform>();
            sub_eventsJ = e.transform.Find("Jets").GetComponentsInChildren<Transform>();
            sub_eventsME = e.transform.Find("MET").GetComponentsInChildren<Transform>();
            sub_eventsMU = e.transform.Find("Muons").GetComponentsInChildren<Transform>();
            sub_eventsHB = e.transform.Find("HBrechits").GetComponentsInChildren<Transform>();
            sub_eventsHE = e.transform.Find("HErechits").GetComponentsInChildren<Transform>();
            sub_eventsEB = e.transform.Find("EBrechits").GetComponentsInChildren<Transform>();
            sub_eventsEE = e.transform.Find("EErechits").GetComponentsInChildren<Transform>();

            foreach (Transform t in sub_eventsE)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
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
                infocanvas.SetActive(false);
            }
            foreach (Transform t in sub_eventsT)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
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
                infocanvas.SetActive(false);
            }
            foreach (Transform t in sub_eventsJ)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
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
                infocanvas.SetActive(false);
            }
            foreach (Transform t in sub_eventsME)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
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
                infocanvas.SetActive(false);
            }
            foreach (Transform t in sub_eventsMU)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
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
                infocanvas.SetActive(false);
            }
            foreach (Transform t in sub_eventsHB)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
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
                infocanvas.SetActive(false);
            }
            foreach (Transform t in sub_eventsHE)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
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
                infocanvas.SetActive(false);
            }
            foreach (Transform t in sub_eventsEB)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
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
                infocanvas.SetActive(false);
            }
            foreach (Transform t in sub_eventsEE)
            {
                t.gameObject.AddComponent<MeshCollider>();
                t.GetComponent<MeshCollider>().convex = true;
                t.GetComponent<MeshCollider>().isTrigger = true;
                t.gameObject.AddComponent<VRTK_InteractableObject>();
                t.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;
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
                infocanvas.SetActive(false);
            }
        }
        Debug.Log("events.Count = " + events.Count());
    }

//Depreciated
/*
        //Gets the relevent data for each event
        foreach (Object obj in events) {
			event_name = obj.name;
			Debug.Log (event_name);

			//Creates the larger event GameObject
			GameObject this_event = new GameObject ();
			this_event.transform.position = new Vector3 (0, -119.9f, 0);
			ItemContainer ic = new ItemContainer ();
			ic = ItemContainer.Load(xml_path);
			event_collection = Resources.LoadAll(event_path, typeof(GameObject))
				.Cast<GameObject>()
				.ToArray();

            //writes event information out to a UI
            foreach (Item item in ic.items)
            {
                if (item.title == "CMS Event")
                {
                    GameObject EventCanvas = new GameObject("EventCanvas");
                    EventCanvas.AddComponent<Canvas>();
                    EventCanvas.AddComponent<GraphicRaycaster>();
                    EventCanvas.AddComponent<CanvasScaler>();
                    EventCanvas.tag = "EventCanvas";
                    EventCanvas.transform.position = new Vector3(0.1f, -119.9f, -3.4f);
                    EventCanvas.transform.Rotate(0.0f, -90.0f, 0.0f);
                    EventCanvas.transform.localScale = new Vector3(0.006f, 0.0065f, 0.01f);
                    EventCanvas.transform.SetParent(this_event.transform);
                    GameObject text = new GameObject("Text");
                    Text infotext = text.AddComponent<Text>();
                    infotext.text = item.title + "\n"
                    + " run = " + item.run + "\n"
                    + " event = " + System.Convert.ToInt64(item.ev) + "\n"
                    + " ls = " + item.ls + "\n"
                    + " orbit = " + System.Convert.ToInt64(item.orbit) + "\n"
                    + " time = " + item.time + "\n";
                    text.transform.localPosition = new Vector3(0.1f, -119.9f, -3.4f);
                    text.transform.Rotate(0.0f, -90.0f, 0.0f);
                    text.transform.localScale = new Vector3(.005f, .006f, 1f);
                    text.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    text.GetComponent<Text>().fontSize =10;
                    text.GetComponent<Text>().fontStyle = FontStyle.Bold;
                    text.GetComponent<Text>().color = Color.black;
                    text.transform.SetParent(EventCanvas.transform);
                }
            }

            //Createst the parts of the events that the data is attached to
            foreach (GameObject sub_event in event_collection) {
				GameObject this_sub_event = Instantiate (sub_event) as GameObject;
				this_sub_event.transform.position = new Vector3 (0, -119.9f, 0);
				this_sub_event.transform.localScale = new Vector3 (50f, 50f, 50f);
				this_sub_event.transform.parent = this_event.transform;
				this_sub_event.tag = "Track"; // default
                Mesh m = this_sub_event.GetComponentInChildren<MeshFilter> ().sharedMesh;
				this_sub_event.AddComponent<MeshCollider> ();
				this_sub_event.GetComponent<MeshCollider> ().sharedMesh = m;
                if (m.vertexCount > 3)
                {
                    Debug.Log(this_sub_event.ToString());
                    Debug.Log("Found mesh with " + m.vertexCount + " vertices and " + m.triangles.Length + "triangles");
                    this_sub_event.GetComponent<MeshCollider>().convex = true;
                }
                else
                {
                    this_sub_event.GetComponent<MeshCollider>().convex = false;
                }
                this_sub_event.GetComponent<MeshCollider> ().isTrigger = true;
                this_sub_event.AddComponent<VRTK_InteractableObject>();
                this_sub_event.GetComponent<VRTK_InteractableObject>().touchHighlightColor = Color.green;

                //Creates the canvases where the data for each sub part of the event will be attached
                foreach (Item item in ic.items) {
					if (this_sub_event.name.Contains(item.name +"(Clone)")) {
						GameObject infocanvas = new GameObject ("InfoCanvas");
                        infocanvas.AddComponent<VRTK_InteractableObject>();
                        infocanvas.GetComponent<VRTK_InteractableObject>().isGrabbable =true;
                        infocanvas.GetComponent<VRTK_InteractableObject>().validDrop = VRTK_InteractableObject.ValidDropTypes.Drop_Anywhere;
                        // infocanvas.GetComponent<VRTK_InteractableObject>().grabAttachMechanic = VRTK_InteractableObject.GrabAttachType.Child_Of_Controller;
                        //gameObject.AddComponent<VRTK_ChildOfControllerGrabAttach>();
                        infocanvas.AddComponent<Canvas> ();
						infocanvas.AddComponent<GraphicRaycaster> ();
						infocanvas.AddComponent<CanvasScaler> ();
						infocanvas.tag = "InfoCanvas";
                        infocanvas.transform.position = new Vector3(0, 0f, 0);
                        infocanvas.transform.localScale = new Vector3(0.0018f, 0.0022f, 0.01f);
                        infocanvas.transform.SetParent(this_sub_event.transform);
                        this_sub_event.tag = item.title;
                        GameObject text = new GameObject ("Text");
						Text infotext = text.AddComponent<Text> ();
                        switch (item.title)
                        {
                            case "CMS Event":
                                infotext.text = item.title + "\n"
                                              + " run = " + item.run + "\n"
                                              + " event = " + item.ev + "\n"
                                              + " ls = " + item.ls + "\n"
                                              + " orbit = " + item.orbit + "\n"
                                              + " time = " + item.time + "\n";
                                break;
                            case "HB rechit":
                            case "HE rechit":
                            case "EB rechit":
                            case "EE rechit":
                                infotext.text = item.title + "\n"
                                              + " energy = " + item.energy + "\n"
                                              + " eta = " + item.eta + "\n"
                                              + " phi = " + item.phi + "\n"
                                              + " time = " + item.time + "\n";
                                break;
                            case "Track":
                            case "Global muon":
                            case "Electron":
                                infotext.text = item.title + "\n"
                                              + " pt = " + item.pt + "\n"
                                              + " eta = " + item.eta + "\n"
                                              + " phi = " + item.phi + "\n"
                                              + " charge = " + item.charge + "\n";
                                break;
                            case "PF jet":
                                infotext.text = item.title + "\n"
                                              + " pt = " + item.pt + "\n"
                                              + " eta = " + item.eta + "\n"
                                              + " phi = " + item.phi + "\n"
                                              + " theta = " + item.theta + "\n";
                                break;
                            case "PF MET":
                                infotext.text = item.title + "\n"
                                              + " pt = " + item.pt + "\n"
                                              + " phi = " + item.phi + "\n";
                                break;
                            default:
                                break;
                        }
                        text.transform.localPosition = new Vector3 (0, 0f, 0.0f);
                        text.transform.localScale = new Vector3 (.0018f, .0022f, 1f);
						text.GetComponent<Text> ().font = Resources.GetBuiltinResource<Font> ("Arial.ttf");
                        text.GetComponent<Text>().fontSize = 11;
                        text.GetComponent<Text>().fontStyle = FontStyle.Bold;
						text.GetComponent<Text>().color = Color.black;
						text.transform.SetParent(infocanvas.transform);

                        infocanvas.SetActive(false);
					}
				}
			}
			this_event.name = event_name;
			this_event.SetActive (false);
		    events[eventNumber] = this_event;	
			eventNumber++;
		}

		foreach(GameObject ev in events) {
			Debug.Log("found event " + ev.name); 
		}
		eventNumber = 0;
*/

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
        currentEvent.SetActive(true);
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
        currentEvent.SetActive(true);
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
	}
}
