//Created by Oliver Lykken
//methods to feed to the teleport buttons

using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour {

	public void TeleportUp() {
		gameObject.transform.position = new Vector3 (87.04f, 0.14f, 108.7f);
	}

	public void TeleportDown() {
		gameObject.transform.position = new Vector3 (5.36f, -119f, -3.87f);
	}

    public void ToControlRoom()
    {
        gameObject.transform.position = new Vector3(77f,1.915f,14);
    }

    //private void OnTriggerEnter (Collider collider)
    //{
    //    if (collider.gameObject.CompareTag("ControlRoom"))
    //    {
    //        ToControlRoom();
    //    }else if (collider.gameObject.CompareTag("Surface")) {
    //        TeleportUp();
    //    }else if (collider.gameObject.CompareTag("DetectorRoom"))
    //    {
    //        TeleportDown();
    //    }
    //}
}
