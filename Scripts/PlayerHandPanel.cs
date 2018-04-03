using UnityEngine;
using Valve.VR.InteractionSystem;

public class PlayerHandPanel : MonoBehaviour {

    private GameObject currTarget;
    private GameObject lastTarget;
    private GameObject info;

    private int grabCount = 0;

    public GameObject display;
    public NewSteamVR_LaserPointer laserPointer;


    void Start()
    {
        display.SetActive(false);
        laserPointer.enabled = false;

        laserPointer = gameObject.GetComponent<NewSteamVR_LaserPointer>();
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;
    }

    void Update()
    {

        if (Player.instance.rightController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu) && grabCount == 0)
        {
            display.SetActive(true);
            laserPointer.enabled = true;
            grabCount = 1;
            Debug.Log("HandPanel Activated");
        }
        else if (Player.instance.rightController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu) && grabCount == 1)
        {
            display.SetActive(false);
            laserPointer.enabled = false;
            grabCount = 0;
            Debug.Log("HandPanel Deactivated");
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        if (info == null)
        {
            currTarget = e.target.gameObject;
            lastTarget = e.target.gameObject;
            info = e.target.gameObject.transform.Find("InfoCanvas").gameObject;
            info.gameObject.transform.SetParent(display.transform);
            Debug.Log("Data Found");

            info.transform.localPosition = new Vector3(27f,-34.4f,11.3f);
            info.transform.localRotation = new Quaternion(0, 0, 0, 0);
            Debug.Log("Showing Data");
        }
        else
        {
            info.gameObject.transform.SetParent(currTarget.transform);
            info.transform.localPosition = new Vector3(0, 500, 0);
            info = null;
            currTarget = null;
            Debug.Log("Removing Data");

            currTarget = e.target.gameObject;
            lastTarget = e.target.gameObject;
            info = e.target.gameObject.transform.Find("InfoCanvas").gameObject;
            info.gameObject.transform.SetParent(display.transform);
            Debug.Log("Data Found");

            info.transform.localPosition = new Vector3(27f, -34.4f, 11.3f);
            info.transform.localRotation = new Quaternion(0, 0, 0, 0);
            Debug.Log("Showing Data");
        }
    }

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        if (info != null)
        {
            info.gameObject.transform.SetParent(currTarget.transform);
            info.transform.localPosition = new Vector3(0, 500, 0);
            info = null;
            currTarget = null;
            Debug.Log("Removing Data");
        }
    }
}
