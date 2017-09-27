/*
	CameraMan.cs
	Created by Carl Emil Carlsen.
	Copyright 2014 Sixth Sensor.
	All rights reserved.
	http://sixthsensor.dk
*/

using UnityEngine;
using System.Collections;

namespace TubeRender
{
	public class CameraMan : MonoBehaviour
	{
		public float speed = 4;
		public Vector3 focusPoint = Vector3.zero;
		public bool hover = false;
		public float hoverRange = 0.5f;
		private Vector3 shift_x = new Vector3(0.005f, 0f, 0f);
		private Vector3 shift_z = new Vector3(0f, 0f, 0.005f);
		
		Camera cam;
		
		
		void Awake()
		{
			cam = gameObject.GetComponentInChildren( typeof( Camera ) ) as Camera;
		}
		
		
		void LateUpdate()
		{
			// rotate camera for SUPER dramatic effect //
			transform.Rotate( Vector3.up, Time.deltaTime * speed );
			
			// hover camera for EXTRA sugar //
			if( hover ) transform.position = Vector3.up * Mathf.Sin( Time.time*0.5f ) * hoverRange;
			
			// focus //
			cam.transform.LookAt( focusPoint );

			transform.Translate(shift_z);
		}
	}
}

