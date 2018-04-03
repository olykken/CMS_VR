using UnityEngine;

public class MainMenuButtons : MonoBehaviour {

    public  GameObject[] teleportSystem;
    private SimpleTouchpadMovement simpleTouchpadMovement;

    private void Awake()
    {
        //teleportSystem = GameObject.FindGameObjectsWithTag("TeleportSystem");
        foreach (GameObject g in teleportSystem)
        {
            g.SetActive(false);
        }
        simpleTouchpadMovement = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SimpleTouchpadMovement>();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void MoveByTeleport()
    {
        simpleTouchpadMovement.enabled = false;
        foreach(GameObject g in teleportSystem)
        {
            g.SetActive(true);
        }
    }

    public void MoveByTouchpad()
    {
        simpleTouchpadMovement.enabled = true;
        foreach (GameObject g in teleportSystem)
        {
            g.SetActive(false);
        }
    }
}
