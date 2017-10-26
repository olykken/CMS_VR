using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHisto : MonoBehaviour {

    private GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        target = other.gameObject;
        target.GetComponentInParent<cubeScript>().enabled=true;
    }
}
