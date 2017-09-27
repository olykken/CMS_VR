/*
    readJSON.cs

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
using JsonFx.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CMSdraw;

public class readJSON {

	[HideInInspector]
	public string rawText;
	public int trackCount, electronCount, jetCount, METCount, muonCount, rechitCount;
	private CMSdraw.DrawEvents DE;
	private GameObject CurrentEvent;
	public List<GameObject> events;
	private int lastEvent;
	public int eventNumber = 0;
	public GameObject Tracks;
	public GameObject Electrons;
	public GameObject Jets;
	public GameObject MET;
	public GameObject Muons;
	public GameObject HBrechit, HErechit, EBrechit, EErechit;


	public readJSON(string folderpath) {

		events = new List<GameObject>();
        //events = new GameObject[MAXEVENTS];
        //folderpath = "/Users/lykken/Desktop/Unity/active/";
        string[] files = Directory.GetFiles(folderpath, "*.json");
        foreach (string filename in files) 
		{
			CurrentEvent = new GameObject("Event_" + eventNumber);
			Debug.Log ("Reading JSON file " + filename);
            if (File.Exists(filename))
                rawText = File.ReadAllText(@filename);
            else
                Debug.LogError("No file" + filename);

			// You probably don't need the next two lines:
			//byte[] bytes = Encoding.Default.GetBytes(rawText);
			//rawText = Encoding.UTF8.GetString(bytes);

			// Clean up known flaws of the CMS JSON files:
			rawText = rawText.Replace ("(", "[").Replace (")", "]").Replace ("nan", "0").Replace ("\'", "\"").Replace (" ", "");

            // For debugging:
            //string outfile = folderpath + "out.json";
			//File.WriteAllText(outfile, rawText);

			// Call the main JSON reader:
			JSONHandler CMShandler = new JSONHandler (rawText);

			DE = new CMSdraw.DrawEvents();

			Debug.Log ("Rendering electrons:");
			Electrons = new GameObject ("Electrons");
			Electrons.transform.parent = CurrentEvent.transform;
			electronCount = 0;
			electronCount = DE.DrawTracks (CMShandler.GetCollection ("GsfElectrons_V1"), CMShandler.GetCollection ("Extras_V1"), CMShandler.GetAssociation ("GsfElectronExtras_V1"),
				CMSvar.GsfElectrons_V1_pt_index, CMSvar.GsfElectrons_V1_eta_index, CMSvar.GsfElectrons_V1_phi_index, CMSvar.GsfElectrons_V1_charge_index,
				CMSvar.GsfElectrons_V1_pt_min, "electron", electronCount, Electrons);
			Debug.Log ("Found " + electronCount + " electrons");

		
			Debug.Log ("Rendering tracks:");
			trackCount = 0;
			Tracks = new GameObject ("Tracks");
			Tracks.transform.parent = CurrentEvent.transform;
			trackCount = DE.DrawTracks (CMShandler.GetCollection ("Tracks_V2"), CMShandler.GetCollection ("Extras_V1"), CMShandler.GetAssociation ("TrackExtras_V1"),
				CMSvar.Tracks_V2_pt_index, CMSvar.Tracks_V2_eta_index, CMSvar.Tracks_V2_phi_index, CMSvar.Tracks_V2_charge_index,
				CMSvar.Tracks_V2_pt_min, "track", trackCount, Tracks);
			Debug.Log ("Found " + trackCount + " tracks");

			Debug.Log ("Rendering jets:");
			jetCount = 0;
			Jets = new GameObject ("Jets");
			Jets.transform.parent = CurrentEvent.transform;
			jetCount = DE.DrawJets (CMShandler.GetCollectionSingleArray ("PFJets_V1"), CMSvar.PFJets_V1_et_index, CMSvar.PFJets_V1_eta_index, 
				CMSvar.PFJets_V1_theta_index, CMSvar.PFJets_V1_phi_index,
				CMSvar.PFJets_V1_pt_min, "jet", jetCount, Jets);
			Debug.Log ("Found " + jetCount + " jets");

			Debug.Log ("Rendering MET:");
			METCount = 0;
			MET = new GameObject ("MET");
			MET.transform.parent = CurrentEvent.transform;
			METCount = DE.DrawMET (CMShandler.GetCollection ("PFMETs_V1"), CMSvar.PFMETs_V1_pt_index, CMSvar.PFMETs_V1_phi_index,
				CMSvar.PFMETs_V1_pt_min, "MET", METCount, MET);

			Debug.Log ("Rendering muons:");
			muonCount = 0;
			Muons = new GameObject ("Muons");
			Muons.transform.parent = CurrentEvent.transform;
			muonCount = DE.DrawMuons (CMShandler.GetCollectionSingleArray ("GlobalMuons_V1"), CMShandler.GetCollectionSingleArray ("Points_V1"), 
				CMShandler.GetAssociation ("MuonGlobalPoints_V1"), CMSvar.GlobalMuons_V1_pt_index, CMSvar.GlobalMuons_V1_eta_index, CMSvar.GlobalMuons_V1_phi_index, 
				CMSvar.GlobalMuons_V1_charge_index, CMSvar.GlobalMuons_V1_pt_min, "muon", muonCount, Muons);
			Debug.Log ("Found " + muonCount + " muons");

			Debug.Log ("Rendering HBrechits:");
			rechitCount = 0;
			HBrechit = new GameObject ("HBrechits");
			HBrechit.transform.parent = CurrentEvent.transform;
			rechitCount = DE.DrawRechits (CMShandler.GetCollection ("HBRecHits_V2"), CMSvar.HBrecHits_V2_energy_index,
				CMSvar.HBrecHits_V2_eta_index, CMSvar.HBrecHits_V2_phi_index, CMSvar.HBrecHits_V2_time_index,
				CMSvar.HBrecHits_V2_detid_index, 0.5f, 0.5f, "HBrechit", rechitCount, HBrechit);
			Debug.Log ("Found " + rechitCount + " HBrechits");

			Debug.Log ("Rendering HErechits:");
			rechitCount = 0;
			HErechit = new GameObject ("HErechits");
			HErechit.transform.parent = CurrentEvent.transform;
			rechitCount = DE.DrawRechits (CMShandler.GetCollection ("HERecHits_V2"), CMSvar.HErecHits_V2_energy_index,
				CMSvar.HErecHits_V2_eta_index, CMSvar.HErecHits_V2_phi_index, CMSvar.HErecHits_V2_time_index,
				CMSvar.HErecHits_V2_detid_index, 0.5f, 0.5f, "HErechit", rechitCount, HErechit);
			Debug.Log ("Found " + rechitCount + " HErechits");

			Debug.Log ("Rendering EBrechits:");
			rechitCount = 0;
			EBrechit = new GameObject ("EBrechits");
			EBrechit.transform.parent = CurrentEvent.transform;
			rechitCount = DE.DrawRechits (CMShandler.GetCollection ("EBRecHits_V2"), CMSvar.EBrecHits_V2_energy_index,
				CMSvar.EBrecHits_V2_eta_index, CMSvar.EBrecHits_V2_phi_index, CMSvar.EBrecHits_V2_time_index,
				CMSvar.EBrecHits_V2_detid_index, 0.25f, 0.5f, "EBrechit", rechitCount, EBrechit);
			Debug.Log ("Found " + rechitCount + " EBrechits");

			Debug.Log ("Rendering EErechits:");
			rechitCount = 0;
			EErechit = new GameObject ("EErechits");
			EErechit.transform.parent = CurrentEvent.transform;
			rechitCount = DE.DrawRechits (CMShandler.GetCollection ("EERecHits_V2"), CMSvar.EErecHits_V2_energy_index,
				CMSvar.EErecHits_V2_eta_index, CMSvar.EErecHits_V2_phi_index, CMSvar.EErecHits_V2_time_index,
				CMSvar.EErecHits_V2_detid_index, 0.5f, 0.5f, "EErechit", rechitCount, EErechit);
			Debug.Log ("Found " + rechitCount + " EErechits");

			Debug.Log ("Finished rendering " + CurrentEvent);
			CurrentEvent.SetActive(false);
			events.Add(CurrentEvent);
			eventNumber++;

		} // foreach filename

	} // readJSON
		

}