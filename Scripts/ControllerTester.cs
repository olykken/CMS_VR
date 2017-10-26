using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTester : MonoBehaviour {

    private SteamVR_TrackedController _controller;
    private PrimitiveType _currentPrimitiveType = PrimitiveType.Sphere;

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.Gripped += HandleGrip;
    }

    private void OnDisable()
    {
        _controller.Gripped -= HandleGrip;
    }

    private void HandleGrip(object sender, ClickedEventArgs e)
    {
        SpawnCurrentPrimitiveAtController();
    }

    private void SpawnCurrentPrimitiveAtController()
    {
        Debug.Log("Gripped");
    }
}
