//Created by Oliver Lykken
//Place this script on a Vive controller to enable trackpad movement
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SimpleTouchpadMovement : MonoBehaviour {
    
    [Tooltip("The maximum speed the play area will be moved when the touchpad is being touched at its edges.")]
    public float maxSpeed = 3f;

    [Tooltip("Assign the main scene camera here")]
    public Transform headSet;

    [Tooltip("Check this if you are working with the [CameraRig] prefab provided from SteamVR")]
    public bool CameraRig;

    private SteamVR_Controller.Device activeController;
    private SteamVR_Controller.Device cameraRigController { get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    private SteamVR_TrackedObject trackedObject;
    private Transform playArea;
    private Vector2 touchAxis;
    private bool isTouching;
    private float movementSpeed;//measured on Y axis of touchpad
    private float strafeSpeed;//measured on X axis of touchpad

    private void Awake()
    {
        if (CameraRig)
        {
            trackedObject = GetComponent<SteamVR_TrackedObject>();
            activeController = cameraRigController;
            //Debug.Log("Controller Assigned");
        }

        if (!CameraRig)
        {
            playArea = GetComponentInParent<Player>().gameObject.transform;
        }
        else if (CameraRig)
        {
            playArea = GetComponentInParent<SteamVR_PlayArea>().gameObject.transform;
        }

        //If no headset is assigned find the main camera
        if(headSet == null)
        {
            GameObject hmd = GameObject.FindGameObjectWithTag("MainCamera");
            headSet = hmd.transform;
        }
    }

    private void OnEnable()
    {
        movementSpeed = 0f;
        strafeSpeed = 0f;
        touchAxis = Vector2.zero;
    }

    public void TouchState(bool state)
    {
        isTouching = state;
    }

    private void FixedUpdate()
    {
        if (isTouching && !CameraRig)
        {
            touchAxis = Player.instance.leftController.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            SetSpeed(ref movementSpeed, touchAxis.y);
            SetSpeed(ref strafeSpeed, touchAxis.x);
            MovePlayer();
        }else if (isTouching && CameraRig)
        {
            touchAxis = activeController.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            SetSpeed(ref movementSpeed, touchAxis.y);
            SetSpeed(ref strafeSpeed, touchAxis.x);
            MovePlayer();
        }
    }

    private void SetSpeed(ref float speed, float input)
    {
        speed = (maxSpeed * input);
    }

    private void MovePlayer()
    {
        var movement = headSet.forward * movementSpeed * Time.deltaTime;
        var strafe = headSet.right * strafeSpeed * Time.deltaTime;
        float fixY = playArea.position.y;
        playArea.position += (movement + strafe);
        playArea.position = new Vector3(playArea.position.x, fixY, playArea.position.z);
    }
}
