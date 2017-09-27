/*
    DrawEvents.cs

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
using System.Collections.Generic;
using TubeRender;

namespace CMSdraw {

	public static class CMSvar {

		// scales

		public const float min_track_radius = 0.002f; // for rendering as tapering tubes
		public const float max_track_radius = 0.02f;  // for rendering as tapering tubes
		public const float min_electron_radius = 0.003f; // for rendering as tapering tubes
		public const float max_electron_radius = 0.024f;  // for rendering as tapering tubes
		public const float min_jet_radius = 0.03f; // for rendering as tapering tubes
		public const float max_jet_radius = 0.03f;  // for rendering as tapering tubes
		public const float min_MET_radius = 0.03f; // for rendering as tapering tubes
		public const float max_MET_radius = 0.03f;  // for rendering as tapering tubes
		public const float min_muon_radius = 0.03f; // for rendering as tapering tubes
		public const float max_muon_radius = 0.24f;  // for rendering as tapering tubes
		public const float jetRescale = 0.1f; // rescale length of rendered jets

		// Define a rotation by 90 degrees around the y-axis:

		public static Quaternion rt = Quaternion.Euler(0, 0, 90f);

		// Number of segments in your Bezier curves:
		public const int nsegments = 24;

		// transparency
		public const float track_transparency = 0.5f;
		public const float electron_transparency = 1.0f;
		public const float muon_transparency = 0.5f;
		public const float jet_transparency = 0.5f;
		public const float MET_transparency = 0.5f;

		// color
		public static Color track_color = Color.green;
		public static Color electron_color = Color.black;
		public static Color muon_color = Color.yellow;
		public static Color jet_color = new Color(0.548f,0.7f,0.95f);
		public static Color MET_color = Color.magenta;

		// You might want to apply some cuts:
		public const float Tracks_V2_pt_min = 0.0f; 
		public const float GsfElectrons_V1_pt_min = 5.0f;
		public const float PFJets_V1_pt_min = 20.0f;
		public const float PFMETs_V1_pt_min = 5.0f;
		public const float GlobalMuons_V1_pt_min = 5.0f;

		// Where various info is stored in the CMS JSON format:

		public const int Event_V2_run_index = 0;
		public const int Event_V2_event_index = 1;
		public const int Event_V2_ls_index = 2;
		public const int Event_V2_orbit_index = 3;
		public const int Event_V2_bx_index = 4;
		public const int Event_V2_time_index = 5;
		public const int Event_V2_localtime_index = 6;

		public const int PFJets_V1_et_index = 0;
		public const int PFJets_V1_eta_index = 1;
		public const int PFJets_V1_theta_index = 2;
		public const int PFJets_V1_phi_index = 3;

		public const int PFMETs_V1_phi_index = 0;
		public const int PFMETs_V1_pt_index = 1;
		public const int PFMETs_V1_px_index = 2;
		public const int PFMETs_V1_py_index = 3;
		public const int PFMETs_V1_pz_index = 4;

		public const int Vertices_V1_isValid_index = 0;
		public const int Vertices_V1_isFake_index = 1;
		public const int Vertices_V1_pos_index = 2;
		public const int Vertices_V1_xError_index = 3;
		public const int Vertices_V1_yError_index = 4;
		public const int Vertices_V1_zError_index = 5;
		public const int Vertices_V1_chi2_index = 6;
		public const int Vertices_V1_ndof_index = 7;

		public const int TriggerPaths_V1_Name_index = 0;
		public const int TriggerPaths_V1_Index_index = 1;
		public const int TriggerPaths_V1_WasRun_index = 2;
		public const int TriggerPaths_V1_Accept_index = 3;
		public const int TriggerPaths_V1_Error_index = 4;
		public const int TriggerPaths_V1_Objects_index = 0;

		public const int Photons_V1_energy_index = 0;
		public const int Photons_V1_et_index = 1;
		public const int Photons_V1_eta_index = 2;
		public const int Photons_V1_phi_index = 3;
		public const int Photons_V1_pos_index = 4;
		public const int Photons_V1_hadronicOverEm_index = 5;
		public const int Photons_V1_hadronicDepth1OverEcal_index = 6;
		public const int Photons_V1_hadronicOverDepth2Ecal_index = 7;

		public const int Tracks_V2_pos_index = 0; 
		public const int Tracks_V2_dir_index = 1; 
		public const int Tracks_V2_pt_index = 2; 
		public const int Tracks_V2_phi_index = 3;
		public const int Tracks_V2_eta_index = 4;
		public const int Tracks_V2_charge_index = 5; 

		public const int GsfElectrons_V1_pos_index = 4;
		public const int GsfElectrons_V1_dir_index = 5;
		public const int GsfElectrons_V1_pt_index = 0;
		public const int GsfElectrons_V1_eta_index = 1;
		public const int GsfElectrons_V1_phi_index = 2;
		public const int GsfElectrons_V1_charge_index = 3; 

		public const int GlobalMuons_V1_pt_index = 0;
		public const int GlobalMuons_V1_charge_index = 1;
		public const int GlobalMuons_V1_rp_index = 2;
		public const int GlobalMuons_V1_phi_index = 3;
		public const int GlobalMuons_V1_eta_index = 4;
		public const int GlobalMuons_V1_calo_energy_index = 5;

		public const int HBrecHits_V2_energy_index = 0;
		public const int HBrecHits_V2_eta_index = 1;
		public const int HBrecHits_V2_phi_index = 2;
		public const int HBrecHits_V2_time_index = 3;
		public const int HBrecHits_V2_detid_index = 4;
		public const int HBrecHits_V2_front_1_index = 5;
		public const int HBrecHits_V2_front_2_index = 6;
		public const int HBrecHits_V2_front_3_index = 7;
		public const int HBrecHits_V2_front_4_index = 8;
		public const int HBrecHits_V2_back_1_index = 9;
		public const int HBrecHits_V2_back_2_index = 10;
		public const int HBrecHits_V2_back_3_index = 11;
		public const int HBrecHits_V2_back_4_index = 12;

		public const int HErecHits_V2_energy_index = 0;
		public const int HErecHits_V2_eta_index = 1;
		public const int HErecHits_V2_phi_index = 2;
		public const int HErecHits_V2_time_index = 3;
		public const int HErecHits_V2_detid_index = 4;
		public const int HErecHits_V2_front_1_index = 5;
		public const int HErecHits_V2_front_2_index = 6;
		public const int HErecHits_V2_front_3_index = 7;
		public const int HErecHits_V2_front_4_index = 8;
		public const int HErecHits_V2_back_1_index = 9;
		public const int HErecHits_V2_back_2_index = 10;
		public const int HErecHits_V2_back_3_index = 11;
		public const int HErecHits_V2_back_4_index = 12;

		public const int EBrecHits_V2_energy_index = 0;
		public const int EBrecHits_V2_eta_index = 1;
		public const int EBrecHits_V2_phi_index = 2;
		public const int EBrecHits_V2_time_index = 3;
		public const int EBrecHits_V2_detid_index = 4;
		public const int EBrecHits_V2_front_1_index = 5;
		public const int EBrecHits_V2_front_2_index = 6;
		public const int EBrecHits_V2_front_3_index = 7;
		public const int EBrecHits_V2_front_4_index = 8;
		public const int EBrecHits_V2_back_1_index = 9;
		public const int EBrecHits_V2_back_2_index = 10;
		public const int EBrecHits_V2_back_3_index = 11;
		public const int EBrecHits_V2_back_4_index = 12;

		public const int EErecHits_V2_energy_index = 0;
		public const int EErecHits_V2_eta_index = 1;
		public const int EErecHits_V2_phi_index = 2;
		public const int EErecHits_V2_time_index = 3;
		public const int EErecHits_V2_detid_index = 4;
		public const int EErecHits_V2_front_1_index = 5;
		public const int EErecHits_V2_front_2_index = 6;
		public const int EErecHits_V2_front_3_index = 7;
		public const int EErecHits_V2_front_4_index = 8;
		public const int EErecHits_V2_back_1_index = 9;
		public const int EErecHits_V2_back_2_index = 10;
		public const int EErecHits_V2_back_3_index = 11;
		public const int EErecHits_V2_back_4_index = 12;

	}

	public class Track : MonoBehaviour {

		public float track_pt, track_eta, track_phi, track_charge;
		public string track_name;

		public Track(string name, float pt, float eta, float phi, float charge) {
			this.track_name = name;
			this.track_pt = pt;
			this.track_eta = eta;
			this.track_phi = phi;
			this.track_charge = charge;
		}
	}

	public class Jet : MonoBehaviour {

		public float jet_pt, jet_eta, jet_theta, jet_phi;
		public string jet_name;

		public Jet(string name, float pt, float eta, float theta, float phi) {
			this.jet_name = name;
			this.jet_pt = pt;
			this.jet_eta = eta;
			this.jet_theta = theta;
			this.jet_phi = phi;
		}
	}

	public class MET : MonoBehaviour {

		public float MET_pt, MET_phi;
		public string MET_name;

		public MET(string name, float pt, float phi) {
			this.MET_name = name;
			this.MET_pt = pt;
			this.MET_phi = phi;
		}
	}

	public class Muon : MonoBehaviour {

		public float muon_pt, muon_eta, muon_phi, muon_charge;
		public string muon_name;

		public Muon(string name, float pt, float eta, float phi, float charge) {
			this.muon_name = name;
			this.muon_pt = pt;
			this.muon_eta = eta;
			this.muon_phi = phi;
			this.muon_charge = charge;
		}
	}

	public class Rechit : MonoBehaviour {

		public float rechit_energy, rechit_eta, rechit_phi, rechit_time;
		public long rechit_detid;
		public string rechit_name;

		public Rechit(string name, float pt, float eta, float phi, float time, long detid) {
			this.rechit_name = name;
			this.rechit_energy = pt;
			this.rechit_eta = eta;
			this.rechit_phi = phi;
			this.rechit_time = time;
			this.rechit_detid = detid;
		}
	}


	public class DrawEvents {

		private int ti, ei, mi, pi;
		private float track_pt, track_eta, track_phi, track_charge;
		private float electron_pt, electron_eta, electron_phi, electron_charge;
		private float jet_pt, jet_eta, jet_theta, jet_phi;
		private Vector3 jet_ptvec;
		private float MET_pt, MET_phi;
		private Vector3 MET_ptvec;
		private Vector3 p1, p2, p3, p4;
		private double[] p1v, p2v;
		private Vector3 v1, v2;
		private double[] v1v, v2v;
		private float dp1p2;
		private float scale0, scale1;
		private float t;
		private float tube_min_radius, tube_max_radius;
		private Color tube_base_color;
		private float tube_alpha;
		private Material tube_material;
		private int front_1_index, front_2_index, front_3_index, front_4_index;
		private int back_1_index, back_2_index, back_3_index, back_4_index;

		private int POINT_COUNT;
		private Vector3[] controlPoints, bezierPoints, muonPoints;
		private CubicBezierCurve curve;


		private void RenderTubes(int POINT_COUNT, Vector3[] points, TubeRender.TubeRenderer tube) {
			
			// optimise for realtime manipulation
			//tube.MarkDynamic();

			// define uv mapping for the end caps
			//tube.uvRectCap = new Rect( 0, 0, 4/12f, 4/12f );

			tube.edgeCount = 24;

			tube.caps = TubeRenderer.CapMode.Both;

			// define points and radiuses
			tube.points = new Vector3[ POINT_COUNT ];
			tube.radiuses = new float[ POINT_COUNT ]; // for tapered tubes
			//tube.radius = 0.02f; // for tubes of fixed radius
			for( int p = 0; p < POINT_COUNT; p++ ){
				t = p / (float)POINT_COUNT;
				tube.points[p] = points[p];
				tube.radiuses[p] = tube_min_radius + t * tube_max_radius;
			}

			// make the tube look nice:
			Renderer r = tube.GetComponent<Renderer>();
			if (tube_material != null)
				r.material = tube_material;
			else {
				r.material = new Material (Shader.Find ("Transparent/Diffuse"));
				r.material.EnableKeyword ("_EMISSION");
				r.material.EnableKeyword ("_ALPHAPREMULTIPLY_ON");
				r.material.SetColor ("_Color", tube_base_color);
				Color col = r.material.color;
				col.a = tube_alpha;
				r.material.color = col;
				r.material.SetColor ("_EmissionColor", Color.yellow);
			}

		} // RenderTubes

		public int DrawTracks(object[][] tracks, object[][] extras, object[] assocs, int pt_index, int eta_index, int phi_index, 
			int charge_index, float min_pt, string label, int trackcount, GameObject GB) {

			trackcount = 0;
			if (tracks == null) return 0; // if JSON collection was empty, return immediately

			// assign rendering attributes according to the label
			if (label == "track") {
				tube_min_radius = CMSvar.min_track_radius;
				tube_max_radius = CMSvar.max_track_radius;
				tube_base_color = CMSvar.track_color;
				tube_alpha = CMSvar.track_transparency;
			} else if (label == "electron") {
				tube_min_radius = CMSvar.min_electron_radius;
				tube_max_radius = CMSvar.max_electron_radius;
				tube_base_color = CMSvar.electron_color;
				tube_alpha = CMSvar.electron_transparency;
			} else
				Debug.Log ("Bad label: " + label);

			// extract the data for each track in the JSON event
			foreach (int[][] asc in assocs) {

				ti = asc [0][1];
				ei = asc [1][1];

				track_pt = (float)(double)tracks[ti][pt_index];
				track_eta = (float)(double)tracks[ti][eta_index];
				track_phi = (float)(double)tracks[ti][phi_index];
				track_charge = (float)(int)tracks[ti][charge_index];

				if (track_pt > min_pt) { // render the track
					trackcount++;

					p1v = (double[])extras[ei][0];
					p1.x = (float)p1v[0];
					p1.y = (float)p1v[1];
					p1.z = (float)p1v[2];
					v1v = (double[])extras[ei][1];
					v1.x = (float)v1v[0];
					v1.y = (float)v1v[1];
					v1.z = (float)v1v[2];
					p2v = (double[])extras[ei][2];
					p2.x = (float)p2v[0];
					p2.y = (float)p2v[1];
					p2.z = (float)p2v[2];
					v2v = (double[])extras[ei][3];
					v2.x = (float)v2v[0];
					v2.y = (float)v2v[1];
					v2.z = (float)v2v[2];

					p1 = CMSvar.rt * p1;
					v1 = CMSvar.rt * v1;
					p2 = CMSvar.rt * p2;
					v2 = CMSvar.rt * v2;

					v1.Normalize();
					v2.Normalize();

					dp1p2 = Vector3.Distance(p1, p2);
					scale0 = 0.25f * dp1p2;
					scale1 = 0.25f * dp1p2;

					p3.x = p1.x + scale0 * v1.x;
					p3.y = p1.y + scale0 * v1.y;
					p3.z = p1.z + scale0 * v1.z;
					p4.x = p2.x + scale1 * v2.x;
					p4.y = p2.y + scale1 * v2.y;
					p4.z = p2.z + scale1 * v2.z;

					// now we have the 4 Vector3's that define the Bezier curve
					controlPoints = new Vector3[4] {p1,p2,p3,p4}; 
					POINT_COUNT = CMSvar.nsegments;
					curve = new CubicBezierCurve(controlPoints); // this is the curve
					// the curve will be rendered as <POINT_COUNT> straight segments
					bezierPoints = new Vector3[POINT_COUNT]; 
					for (int i = 0; i < POINT_COUNT; i++) {
						t = i / (float)POINT_COUNT;
						bezierPoints[i] = curve.GetPoint(t);
					}


					// create game object and add TubeRenderer component
					TubeRender.TubeRenderer tube = new GameObject( label + trackcount ).AddComponent<TubeRenderer>();
					// add Track component
					Track track = tube.gameObject.AddComponent<Track>() as Track;
					track.track_pt = track_pt;
					track.track_eta = track_eta;
					track.track_phi = track_phi;
					track.track_charge = track_charge;
					track.track_name = label + trackcount;
					// make tube a child of the game object
					tube.transform.parent = GB.transform;
					RenderTubes(POINT_COUNT, bezierPoints, tube);

				} // if (track_pt > 0)
			} // foreach

			return trackcount;

		} // DrawTracks

		public int DrawJets(object[] jets, int pt_index, int eta_index, int theta_index, int phi_index, 
			float min_pt, string label, int jetcount, GameObject GC) {

			jetcount = 0;
			if (jets == null) return 0; // if JSON collection was empty, return immediately

			// assign rendering attributes according to the label
			if (label == "jet") {
				tube_min_radius = CMSvar.min_jet_radius;
				tube_max_radius = CMSvar.max_jet_radius;
				tube_base_color = CMSvar.jet_color;
				tube_alpha = CMSvar.jet_transparency;
			} else if (label == "MET") {
				tube_min_radius = CMSvar.min_MET_radius;
				tube_max_radius = CMSvar.max_MET_radius;
				tube_base_color = CMSvar.MET_color;
				tube_alpha = CMSvar.MET_transparency;
			} else
				Debug.LogError ("Bad label: " + label);

			// extract the data for each track in the JSON event
			foreach (object jet2 in jets) {

				if (label == "jet") {
					// not sure why we have to do the casting this way
					double[] jetvec = (double[])jet2;
					float[] jet = new float[jetvec.Length];
					for (int i = 0; i < jetvec.Length; i++)
						jet [i] = (float)jetvec [i];
					jet_pt = (float)(double)jet [pt_index];
					jet_eta = (float)(double)jet [eta_index];
					jet_theta = (float)(double)jet [theta_index];
					jet_phi = (float)(double)jet [phi_index];
					jet_ptvec.x = jet_pt * Mathf.Cos (jet_phi);
					jet_ptvec.y = jet_pt * Mathf.Sin (jet_phi);
					jet_ptvec.z = jet_pt * (float)Math.Sinh (jet_eta);
				} else
					Debug.LogError ("Bad label: " + label);
			

				if (jet_pt > min_pt) { // render the jet
					jetcount++;

					jet_ptvec = CMSvar.rt * jet_ptvec;

					// the jet is rendered as a straight tube but use Bezier anyway
					p1 = Vector3.zero;
					p2 = jet_ptvec * CMSvar.jetRescale;
					p3 = p1;
					p4 = p2;

					// now we have the 4 Vector3's that define the Bezier curve
					controlPoints = new Vector3[4] {p1,p2,p3,p4}; 
					POINT_COUNT = CMSvar.nsegments;
					curve = new CubicBezierCurve(controlPoints); // this is the curve
					// the curve will be rendered as <POINT_COUNT> straight segments
					bezierPoints = new Vector3[POINT_COUNT]; 
					for (int i = 0; i < POINT_COUNT; i++) {
						t = i / (float)POINT_COUNT;
						bezierPoints[i] = curve.GetPoint(t);
					}

					// create game object and add TubeRenderer component
					TubeRender.TubeRenderer tube = new GameObject( "jet" + jetcount ).AddComponent<TubeRenderer>();
					// add Jet component
					Jet jet = tube.gameObject.AddComponent<Jet>() as Jet;
					jet.jet_pt = jet_pt;
					jet.jet_eta = jet_eta;
					jet.jet_theta = jet_theta;
					jet.jet_phi = jet_phi;
					jet.jet_name = label + jetcount;
					// make tube a child of the game object
					tube.transform.parent = GC.transform;
					//tube_material = Resources.Load("Materials/Emissive_white", typeof(Material)) as Material;
					RenderTubes(POINT_COUNT, bezierPoints, tube);
				

				} // if (jet_pt > min_pt)
			} // foreach

			return jetcount;

		} // DrawJets

		public int DrawMET(object[][] METs, int pt_index, int phi_index, 
			float min_pt, string label, int METcount, GameObject GD) {

			METcount = 0;
			if (METs == null) return 0; // if JSON collection was empty, return immediately

			tube_min_radius = CMSvar.min_MET_radius;
			tube_max_radius = CMSvar.max_MET_radius;
			tube_base_color = CMSvar.MET_color;
			tube_alpha = CMSvar.MET_transparency;


			// extract the data for each track in the JSON event
			foreach (object[] MET2 in METs) {

				float[] MET = new float[MET2.Length];
				for (int i = 0; i < MET2.Length; i++)
					MET[i] = Single.Parse(MET2[i].ToString());
				
				MET_pt = (float)(double)MET [pt_index];
				MET_phi = (float)(double)MET [phi_index];
				MET_ptvec.x = MET_pt * Mathf.Cos (MET_phi);
				MET_ptvec.y = MET_pt * Mathf.Sin (MET_phi);
				MET_ptvec.z = 0f;


				if (MET_pt > min_pt) { // render the MET
					METcount++;

					MET_ptvec = CMSvar.rt * MET_ptvec;

					// the MET is rendered as a straight tube but use Bezier anyway
					p1 = Vector3.zero;
					p2 = MET_ptvec * CMSvar.jetRescale;
					p3 = p1;
					p4 = p2;

					// now we have the 4 Vector3's that define the Bezier curve
					controlPoints = new Vector3[4] {p1,p2,p3,p4}; 
					POINT_COUNT = CMSvar.nsegments;
					curve = new CubicBezierCurve(controlPoints); // this is the curve
					// the curve will be rendered as <POINT_COUNT> straight segments
					bezierPoints = new Vector3[POINT_COUNT]; 
					for (int i = 0; i < POINT_COUNT; i++) {
						t = i / (float)POINT_COUNT;
						bezierPoints[i] = curve.GetPoint(t);
					}

					// create game object and add TubeRenderer component
					TubeRender.TubeRenderer tube = new GameObject( "MET" ).AddComponent<TubeRenderer>();
					// add MET component
					MET met = tube.gameObject.AddComponent<MET>() as MET;
					met.MET_pt = MET_pt;
					met.MET_phi = MET_phi;
					met.MET_name = label;
					// make tube a child of the game object
					tube.transform.parent = GD.transform;
					tube_material = Resources.Load("Materials/Emissive_magenta", typeof(Material)) as Material;
					tube_material = null;
					RenderTubes(POINT_COUNT, bezierPoints, tube);

				} // if (MET_pt > min_pt)
			} // foreach

			return METcount;

		} // DrawMET

		public int DrawMuons(object[] muons, object[] points, object[] assocs, int pt_index, int eta_index, int phi_index, 
			int charge_index, float min_pt, string label, int muoncount, GameObject GE) {

			muoncount = 0;
			if (muons == null) return 0; // if JSON collection was empty, return immediately

			// assign rendering attributes according to the label
			tube_min_radius = CMSvar.min_muon_radius;
			tube_max_radius = CMSvar.max_muon_radius;
			tube_base_color = CMSvar.muon_color;
			tube_alpha = CMSvar.muon_transparency;

			int nrawmuons = 0;
			// can accommodate up to 12 muons:
			float[] m_pt = {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
			float[] m_eta = {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
			float[] m_phi = {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
			float[] m_charge = {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
			List<Vector3>[] muonPoints = new List<Vector3>[12];
			for (int im = 0; im < 12; im++) 
				muonPoints[im] = new List<Vector3>();

			// recast all the points as an array of Vector3's
			int i = 0;
			int npoints = points.Length;
			Vector3[] pointsf = new Vector3[npoints];
			foreach (double[][] pointsd in points) {
				pointsf[i].x = (float)pointsd[0][0];
				pointsf[i].y = (float)pointsd[0][1];
				pointsf[i].z = (float)pointsd[0][2];
				i++;
			}

			// extract the kinematic data for each muon in the JSON event
			nrawmuons = muons.Length;
			mi = 0;
			foreach (object[] mu in muons) {
				m_pt [mi] = (float)(double)mu[pt_index];
				m_eta [mi] = (float)(double)mu[eta_index];
				m_phi [mi] = (float)(double)mu[phi_index];
				m_charge [mi] = (float)(int)mu[charge_index];
				mi++;
			}

			// extract the point data for each muon in the JSON event
			foreach (int[][] asc in assocs) {

				mi = asc [0] [1]; // labels which muon it is
				pi = asc [1] [1]; // labels a point on the muon track

				// For a particular muon we don't want all the muon points, only the ones
				// labeled by the pi's.
				muonPoints[mi].Add(CMSvar.rt * pointsf[pi]);

			} // foreach

			// Now go back through the muons and render the ones that pass the pt cut:

			for (int im = 0; im < nrawmuons; im++) {
				if (m_pt[im] > min_pt) { // render the muon
					muoncount++;

					// we have all the points that define the muon track
					Vector3[] mpoints = muonPoints[im].ToArray();
					POINT_COUNT = mpoints.Length;
					// create game object and add TubeRenderer component
					TubeRender.TubeRenderer tube = new GameObject ("muon" + muoncount).AddComponent<TubeRenderer> ();
					// add Muon component
					Muon muon = tube.gameObject.AddComponent<Muon>() as Muon;
					muon.muon_pt = m_pt[im];
					muon.muon_eta = m_eta[im];
					muon.muon_phi = m_phi[im];
					muon.muon_charge = m_charge[im];
					muon.muon_name = label + muoncount;
					// make tube a child of the game object
					tube.transform.parent = GE.transform;
					tube_material = Resources.Load("Materials/Emissive_red", typeof(Material)) as Material;
					RenderTubes (POINT_COUNT, mpoints, tube);
				}
			} // for

			return muoncount;

		} // DrawMuons

		public int DrawRechits(object[] rechits, int energy_index, int eta_index, int phi_index, int time_index,
			int detid_index, float min_energy, float energy_scale, string label, int rechitcount, GameObject GF) {

			rechitcount = 0;
			if (rechits == null) return 0; // if JSON collection was empty, return immediately

			// assign rendering attributes according to the label
			if (label == "HBrechit") {
				energy_index = CMSvar.HBrecHits_V2_energy_index;
				eta_index = CMSvar.HBrecHits_V2_eta_index;
				phi_index = CMSvar.HBrecHits_V2_phi_index;
				time_index = CMSvar.HBrecHits_V2_time_index;
				detid_index = CMSvar.HBrecHits_V2_detid_index;
				front_1_index = CMSvar.HBrecHits_V2_front_1_index;
				front_2_index = CMSvar.HBrecHits_V2_front_2_index;
				front_3_index = CMSvar.HBrecHits_V2_front_3_index;
				front_4_index = CMSvar.HBrecHits_V2_front_4_index;
				back_1_index = CMSvar.HBrecHits_V2_back_1_index;
				back_2_index = CMSvar.HBrecHits_V2_back_2_index;
				back_3_index = CMSvar.HBrecHits_V2_back_3_index;
				back_4_index = CMSvar.HBrecHits_V2_back_4_index;
			} else if (label == "HErechit") {
				energy_index = CMSvar.HErecHits_V2_energy_index;
				eta_index = CMSvar.HErecHits_V2_eta_index;
				phi_index = CMSvar.HErecHits_V2_phi_index;
				time_index = CMSvar.HErecHits_V2_time_index;
				detid_index = CMSvar.HErecHits_V2_detid_index;
				front_1_index = CMSvar.HErecHits_V2_front_1_index;
				front_2_index = CMSvar.HErecHits_V2_front_2_index;
				front_3_index = CMSvar.HErecHits_V2_front_3_index;
				front_4_index = CMSvar.HErecHits_V2_front_4_index;
				back_1_index = CMSvar.HErecHits_V2_back_1_index;
				back_2_index = CMSvar.HErecHits_V2_back_2_index;
				back_3_index = CMSvar.HErecHits_V2_back_3_index;
				back_4_index = CMSvar.HErecHits_V2_back_4_index;
			} else if (label == "EBrechit") {
				energy_index = CMSvar.EBrecHits_V2_energy_index;
				eta_index = CMSvar.EBrecHits_V2_eta_index;
				phi_index = CMSvar.EBrecHits_V2_phi_index;
				time_index = CMSvar.EBrecHits_V2_time_index;
				detid_index = CMSvar.EBrecHits_V2_detid_index;
				front_1_index = CMSvar.EBrecHits_V2_front_1_index;
				front_2_index = CMSvar.EBrecHits_V2_front_2_index;
				front_3_index = CMSvar.EBrecHits_V2_front_3_index;
				front_4_index = CMSvar.EBrecHits_V2_front_4_index;
				back_1_index = CMSvar.EBrecHits_V2_back_1_index;
				back_2_index = CMSvar.EBrecHits_V2_back_2_index;
				back_3_index = CMSvar.EBrecHits_V2_back_3_index;
				back_4_index = CMSvar.EBrecHits_V2_back_4_index;
			} else if (label == "EErechit") {
				energy_index = CMSvar.EErecHits_V2_energy_index;
				eta_index = CMSvar.EErecHits_V2_eta_index;
				phi_index = CMSvar.EErecHits_V2_phi_index;
				time_index = CMSvar.EErecHits_V2_time_index;
				detid_index = CMSvar.EErecHits_V2_detid_index;
				front_1_index = CMSvar.EErecHits_V2_front_1_index;
				front_2_index = CMSvar.EErecHits_V2_front_2_index;
				front_3_index = CMSvar.EErecHits_V2_front_3_index;
				front_4_index = CMSvar.EErecHits_V2_front_4_index;
				back_1_index = CMSvar.EErecHits_V2_back_1_index;
				back_2_index = CMSvar.EErecHits_V2_back_2_index;
				back_3_index = CMSvar.EErecHits_V2_back_3_index;
				back_4_index = CMSvar.EErecHits_V2_back_4_index;
			} else
				Debug.LogError ("Bad label: " + label);

			// extract the data for each rechit in the JSON event
			foreach (object[] rh in rechits) {
				// be careful of cases where the double is really an int:
				float rh_energy = (rh[energy_index].GetType() == typeof(System.Double)) ? (float)(double)rh[energy_index] : (float)(int)rh[energy_index];
				float rh_eta = (rh[eta_index].GetType() == typeof(System.Double)) ? (float)(double)rh[eta_index] : (float)(int)rh[eta_index];
				float rh_phi = (rh[phi_index].GetType() == typeof(System.Double)) ? (float)(double)rh[phi_index] : (float)(int)rh[phi_index];
				float rh_time = (rh[time_index].GetType() == typeof(System.Double)) ? (float)(double)rh[time_index] : (float)(int)rh[time_index];
				long rh_detid = (long)(int)rh[detid_index];

				if (rh_energy > min_energy) { // render the rechit
					rechitcount++;

					// a cube or parallelpiped is defined by its 8 vertices
					double[] p; 
					p = (double[])rh[front_1_index];
					Vector3 f1 = new Vector3((float)p[0],(float)p[1],(float)p[2]);
					p = (double[])rh[front_2_index];
					Vector3 f2 = new Vector3((float)p[0],(float)p[1],(float)p[2]);
					p = (double[])rh[front_3_index];
					Vector3 f3 = new Vector3((float)p[0],(float)p[1],(float)p[2]);
					p = (double[])rh[front_4_index];
					Vector3 f4 = new Vector3((float)p[0],(float)p[1],(float)p[2]);
					p = (double[])rh[back_1_index];
					Vector3 b1 = new Vector3((float)p[0],(float)p[1],(float)p[2]);
					p = (double[])rh[back_2_index];
					Vector3 b2 = new Vector3((float)p[0],(float)p[1],(float)p[2]);
					p = (double[])rh[back_3_index];
					Vector3 b3 = new Vector3((float)p[0],(float)p[1],(float)p[2]);
					p = (double[])rh[back_4_index];
					Vector3 b4 = new Vector3((float)p[0],(float)p[1],(float)p[2]);

					Vector3 diff1 = b1 - f1;
					Vector3 diff2 = b2 - f2;
					Vector3 diff3 = b3 - f3;
					Vector3 diff4 = b4 - f4;

					diff1.Normalize();
					diff2.Normalize();
					diff3.Normalize();
					diff4.Normalize();

					Vector3 scale = new Vector3(energy_scale, energy_scale, energy_scale);
					diff1 = Vector3.Scale (diff1, scale);
					diff2 = Vector3.Scale (diff2, scale);
					diff3 = Vector3.Scale (diff3, scale);
					diff4 = Vector3.Scale (diff4, scale);

					Vector3 f1d = f1 + diff1;
					Vector3 f2d = f2 + diff2;
					Vector3 f3d = f3 + diff3;
					Vector3 f4d = f4 + diff4;

					f1 = CMSvar.rt * f1;
					f2 = CMSvar.rt * f2;
					f3 = CMSvar.rt * f3;
					f4 = CMSvar.rt * f4;
					f1d = CMSvar.rt * f1d;
					f2d = CMSvar.rt * f2d;
					f3d = CMSvar.rt * f3d;
					f4d = CMSvar.rt * f4d;

					Vector3[] vertices = {f1, f2, f3, f4, f1d, f2d, f3d, f4d};
					int[] triangles = {
						0, 2, 1, //face front
						0, 3, 2,
						2, 3, 4, //face top
						2, 4, 5,
						1, 2, 5, //face right
						1, 5, 6,
						0, 7, 4, //face left
						0, 4, 3,
						5, 4, 7, //face back
						5, 7, 6,
						0, 6, 7, //face bottom
						0, 1, 6
					};

					GameObject rechit = new GameObject(label + rechitcount);
					// make tube a child of the game object
					rechit.transform.parent = GF.transform;
					MeshFilter meshFilter = rechit.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer = rechit.AddComponent<MeshRenderer>();
					Material Emissive_yellow = Resources.Load("Materials/Emissive_yellow", typeof(Material)) as Material;
					meshRenderer.material = Emissive_yellow;
					Mesh mesh = rechit.GetComponent<MeshFilter>().mesh;
					mesh.Clear ();
					mesh.vertices = vertices;
					mesh.triangles = triangles;
					mesh.RecalculateNormals ();
					// add Rechit component
					Rechit re = rechit.AddComponent<Rechit>() as Rechit;
					re.rechit_energy = rh_energy;
					re.rechit_eta = rh_eta;
					re.rechit_phi = rh_phi;
					re.rechit_time = rh_time;
					re.rechit_detid = rh_detid;
					re.rechit_name = label + rechitcount;


				} // if (rh_energy > min_energy)
			} // foreach

			return rechitcount;

		} // DrawRechits

	} // DrawEvents

} // CMSdraw
