/*
    miniEventScroller.cs

	This is a NovaVR script 
	created for the CMS Collaboration CMS.VR project
	http://novavrllc.com

	Version 0.5

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class miniEventScroller : MonoBehaviour {

	public GameObject[] eventArea;
	public GameObject area;
	public GameObject[] events;
	public GameObject currentEvent;
	string folderPath;
	int lastEvent;
	int eventNumber = 0;

	void Start () {

		folderPath = "/Users/lykken/Desktop/Unity/active/";
		readJSON CMS_JSON = new readJSON(folderPath);
		events = new GameObject[eventNumber];
		events = CMS_JSON.events.ToArray();
		Debug.Log ("Rendered " + CMS_JSON.eventNumber + " events");
		foreach (GameObject e in events) 
			Debug.Log(e);
		Debug.Log ("events.Count = " + events.Count());
		/*
		eventArea = GameObject.FindGameObjectsWithTag ("eventArea");
		
		events = Resources.LoadAll("events", typeof(GameObject))
			.Cast<GameObject>()
			.ToArray();

		jsons = Resources.LoadAll ("JSON");

		for (int i = 0; i < events.Length; i++) {
			events[i].name = i.ToString ();
		}
		foreach(GameObject ev in events) {
			Debug.Log("found event " + ev.name); 
		}
		Debug.Log ("loaded event is " + events [eventNumber].name);
		*/
	}

	public bool clicked() {
		return Input.GetButtonDown ("Forward");
	}

	public bool Unclick() {
		return Input.GetButtonDown ("Y");
	}

	public bool backClick() {
		return Input.GetButtonDown ("Back");
	}
	

	void Update () {

		if (clicked()) {
			foreach(GameObject area in eventArea) {
				area.SetActive (false);	
			}
			//Destroy (currentEvent);
			if (eventNumber < events.Length - 1) {
				eventNumber++;
			} else {
				eventNumber = 0;
			}
			Debug.Log ("click: eventNumber = " + eventNumber);
			Debug.Log ("loaded event is " + events[eventNumber]);
			//currentEvent = Instantiate(events[eventNumber]) as GameObject;
			if (currentEvent != null) currentEvent.SetActive(false);
			currentEvent = events[eventNumber];
			//currentEvent.transform.position = new Vector3 (0, -119.9f, 0);
			//currentEvent.transform.localScale = new Vector3 (0.01f, 0.01f, 0.01f); 
			currentEvent.SetActive(true);
		}

		if (backClick()) {
			foreach(GameObject area in eventArea) {
				area.SetActive (false);	
			}
			//Destroy (currentEvent);
			if (eventNumber == 0) {
				lastEvent = events.Count() - 1;
				eventNumber = lastEvent;
			}else {
				eventNumber--;
			}
			Debug.Log ("backClick: eventNumber = " + eventNumber);
			Debug.Log ("loaded event is " + events[eventNumber]);
			//currentEvent = Instantiate(events[eventNumber]) as GameObject;
			if (currentEvent != null) currentEvent.SetActive(false);
			currentEvent = events[eventNumber];
			//currentEvent.transform.position = new Vector3 (0, -119.9f, 0);
			//currentEvent.transform.localScale = new Vector3 (0.01f, 0.01f, 0.01f); 
			currentEvent.SetActive(true);
		}

		if (Unclick ()) {
			currentEvent.SetActive (false);

			foreach (GameObject area in eventArea) {
				area.SetActive (true);
			}
		}
	}
}
