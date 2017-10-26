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

    public void OnControlRoom()
    {
        ToControlRoom();
    }

    public void OnCavern()
    {
        TeleportDown();
    }

    public void OnSurface()
    {
        TeleportUp();
    }

    public void OnFarSide()
    {
        TeleportFar();
    }


    public void TeleportUp()
    {
        rig.transform.position = new Vector3(210f, 2f, -130f);
        rig.transform.Rotate(0.0f,40.0f,0.0f);
    }

    public void TeleportDown()
    {
        rig.transform.position = new Vector3(5.4f, -119f, -3.8f);
    }

    public void ToControlRoom()
    {
        rig.transform.position = new Vector3(36f, 2.5f, 7f);
        rig.transform.Rotate(0.0f,0.0f,0.0f);
    }
    public void TeleportFar() {
        rig.transform.position = new Vector3(150f,2f,160f);
        rig.transform.Rotate(0.0f,-145f,0.0f);
    }
}
