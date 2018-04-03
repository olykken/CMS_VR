//Created by Oliver Lykken
//Toggles a menu item and a SteamVR laser pointer
//Place this script on one of the controllers
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MenuManagerSimple : MonoBehaviour {

    private SteamVR_Controller.Device controller;
    private SteamVR_TrackedObject trackedObj;
    private ulong menuButton = SteamVR_Controller.ButtonMask.ApplicationMenu;

    [Tooltip("Check this if you are working with the [CameraRig] prefab provided from SteamVR")]
    public bool CameraRig;

    [Tooltip("Place the menu game object that will be toggeled here")]
    public GameObject menu;

    public GameObject mainMenu;

    private UIButtonManager buttonManager;

    private void Start()
    {
        if (CameraRig)
        {
            trackedObj = GetComponent<SteamVR_TrackedObject>();
            controller = SteamVR_Controller.Input((int)trackedObj.index);
        }

        if (menu == null)
        {
            menu = GameObject.Find("ScenePanel");
        }
        if (mainMenu == null)
        {
            mainMenu = GameObject.Find("MainMenu");
        }
        menu.SetActive(false);
        mainMenu.SetActive(false);
    }

    // Toggles the menu and laser pointer
    void Update()
    {
        if (CameraRig)
        {
            switch (menu.activeInHierarchy)
            {
                case false:
                    if (controller.GetPressDown(menuButton))
                    {
                        menu.SetActive(true);
                        mainMenu.SetActive(true);
                        buttonManager.BroadcastMessage("LaserToggle", true);
                        Debug.Log("Menu Open");
                    }
                    break;
                case true:
                    if (controller.GetPressDown(menuButton))
                    {
                        menu.SetActive(false);
                        mainMenu.SetActive(false);
                        buttonManager.BroadcastMessage("LaserToggle", false);
                        Debug.Log("Menu Closed");
                    }
                    break;
            }
        }else if(!CameraRig)
        {
            switch (menu.activeInHierarchy)
            {
                case false:
                    if (Player.instance.leftController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                    {
                        menu.SetActive(true);
                        mainMenu.SetActive(true);
                        Debug.Log("Menu Open");
                    }
                    break;
                case true:
                    if (Player.instance.leftController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
                    {
                        menu.SetActive(false);
                        mainMenu.SetActive(false);
                        Debug.Log("Menu Closed");
                    }
                    break;
            }
        }
    }
}
