using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.UI;
using CMSdraw;

public class InfoGrab : MonoBehaviour {

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private SteamVR_Controller.Device steamController { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    private OVRInput.Axis1D trigger = OVRInput.Axis1D.PrimaryHandTrigger;

    private GameObject target;
    private GameObject info;
    private GameObject infoText;

    private int grabCount = 0;

    private GameObject display;


    // Use this for initialization
    void Start() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        display = gameObject.GetComponentInChildren<Canvas>().gameObject;
        display.SetActive(false);

    }

    // Update is called once per frame
    void Update() {
        if (steamController.GetPressDown(gripButton) || OVRInput.Get(trigger)==1 && info != null)
        {
            info.transform.position = GameObject.Find("HandPanel").transform.position;
            info.SetActive(true);
            info.transform.rotation = transform.rotation;
            info.transform.Rotate(20f, 0f, 0f);
            info.transform.parent = transform;
            info.transform.localPosition = new Vector3(.02f, .18f, .12f);
            WriteText();

        }
        if (steamController.GetPressUp(gripButton) || OVRInput.Get(trigger) == 0 && info != null)
        {
            info.transform.parent = target.transform;
            info.transform.position = target.transform.position;
        }
        if (steamController.GetPressDown(gripButton) || OVRInput.Get(trigger) == 1)
        {
            display.SetActive(true);
        }else if (gameObject.transform.position.y >=0)
        {
            display.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        target = collider.gameObject;
        info = target.transform.Find("InfoCanvas").gameObject;
        infoText = info.transform.GetChild(0).gameObject;
    }

    private void OnTriggerExit(Collider collider)
    {
        target = null;
        info = null;
        infoText = null;
    }

    void WriteText()
    {
        if (target.name.Contains("track")) {
            infoText.GetComponent<Text>().text = target.GetComponent<Track>().track_name + "\n"
                              + " pt = " + target.GetComponent<Track>().track_pt + "\n"
                              + " eta = " + target.GetComponent<Track>().track_eta + "\n"
                              + " phi = " + target.GetComponent<Track>().track_phi + "\n"
                              + " charge = " + target.GetComponent<Track>().track_charge + "\n";
        }
        else if (target.name.Contains("jet")) {
            infoText.GetComponent<Text>().text = target.GetComponent<Jet>().jet_name + "\n"
                              + " pt = " + target.GetComponent<Jet>().jet_pt + "\n"
                              + " eta = " + target.GetComponent<Jet>().jet_eta + "\n"
                              + " phi = " + target.GetComponent<Jet>().jet_phi + "\n"
                              + " theta = " + target.GetComponent<Jet>().jet_theta + "\n";
        }
        else if (target.name.Contains("muon")) {
            infoText.GetComponent<Text>().text = target.GetComponent<Muon>().muon_name + "\n"
                              + " pt = " + target.GetComponent<Muon>().muon_pt + "\n"
                              + " eta = " + target.GetComponent<Muon>().muon_eta + "\n"
                              + " phi = " + target.GetComponent<Muon>().muon_phi + "\n"
                              + " charge = " + target.GetComponent<Muon>().muon_charge + "\n";
        }
        else if (target.name.Contains("electron")) {
            infoText.GetComponent<Text>().text = target.GetComponent<Track>().track_name + "\n"
                              + " pt = " + target.GetComponent<Track>().track_pt + "\n"
                              + " eta = " + target.GetComponent<Track>().track_eta + "\n"
                              + " phi = " + target.GetComponent<Track>().track_phi + "\n"
                              + " charge = " + target.GetComponent<Track>().track_charge + "\n";
        }
        else if (target.name.Contains("MET")) {
            infoText.GetComponent<Text>().text = target.GetComponent<MET>().MET_name + "\n"
                              + " pt = " + target.GetComponent<MET>().MET_pt + "\n"
                              + " phi = " + target.GetComponent<MET>().MET_phi + "\n";
        }
        else if (target.name.Contains("rechit"))
        {
            infoText.GetComponent<Text>().text = target.GetComponent<Rechit>().rechit_name + "\n"
                              + " energy = " + target.GetComponent<Rechit>().rechit_energy + "\n"
                              + " eta = " + target.GetComponent<Rechit>().rechit_eta + "\n"
                              + " phi = " + target.GetComponent<Rechit>().rechit_phi + "\n"
                              + " time = " + target.GetComponent<Rechit>().rechit_time + "\n";
        } else
            infoText.GetComponent<Text>().text = "No Data Available";
    }
}

