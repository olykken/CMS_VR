using UnityEngine;
using System.Collections;

public class TeleportTrigger : MonoBehaviour {

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    public GameObject rig;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("ControlRoom"))
        {
            ToControlRoom();
        }
        else if (collider.gameObject.CompareTag("Surface"))
        {
            TeleportUp();
        }
        else if (collider.gameObject.CompareTag("DetectorRoom"))
        {
            TeleportDown();
        }
        else if (collider.gameObject.CompareTag("FarSide"))
        {
            TeleportFar();
        }
    }

    public void TeleportUp()
    {
        rig.transform.position = new Vector3(206f, .5f, -124.5f);
        rig.transform.Rotate(0.0f,40.0f,0.0f);
    }

    public void TeleportDown()
    {
        rig.transform.position = new Vector3(4.17f, -119f, -3.08f);
    }

    public void ToControlRoom()
    {
        rig.transform.position = new Vector3(38.5f, .5f, 9.22f);
        rig.transform.Rotate(0.0f,150.0f,0.0f);
    }
    public void TeleportFar() {
        rig.transform.position = new Vector3(146f,.5f,162f);
        rig.transform.Rotate(0.0f,-145f,0.0f);
    }
}
