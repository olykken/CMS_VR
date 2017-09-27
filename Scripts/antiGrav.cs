using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class antiGrav : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
