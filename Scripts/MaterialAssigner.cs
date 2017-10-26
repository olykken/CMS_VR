/*
    MaterialAssigner.cs

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
// This script puts the materials on the event pieces

using UnityEngine;
using System.Collections;

public class MaterialAssigner : MonoBehaviour {


	private MeshRenderer [] cmsRender;
	private Material cmsColor;


	void Start () {
		cmsColor = Resources.Load ("cms_skpt", typeof(Material)) as Material;
		cmsRender = FindObjectsOfType (typeof(MeshRenderer)) as MeshRenderer[];
			//GetComponentInChildren<MeshRenderer> ();
		foreach (MeshRenderer mr in cmsRender) {
			mr.material = cmsColor;
		}
	}
	

	void Update () {

	}
}
