//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;


public struct PointerEventArgs
{
    public uint controllerIndex;
    public uint flags;
    public float distance;
    public Transform target;
}

public delegate void PointerEventHandler(object sender, PointerEventArgs e);

[System.Serializable]
public class PointerEvent : UnityEvent<PointerEventArgs> { }

[RequireComponent(typeof(SteamVR_TrackedController))]
public class NewSteamVR_LaserPointer : MonoBehaviour
{

    public Color color;
    public float thickness = 0.002f;
    public float thicknessPressed = .01f;
    public GameObject holder;
    public GameObject pointer;
    public bool addRigidBody = false;


    public event PointerEventHandler PointerIn;
    public event PointerEventHandler PointerOut;

    public PointerEvent unityPointerIn = new PointerEvent();
    public PointerEvent unityPointerOut = new PointerEvent();

    protected Transform previousContact = null;

    private SteamVR_TrackedController controller;

	// Use this for initialization
	void Awake ()
    {
        controller = GetComponent<SteamVR_TrackedController>();

        if (holder == null)
        {
            holder = new GameObject("Holder");
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;
        }
        holder.gameObject.SetActive(false);

        if (pointer == null)
        {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.name = "Pointer";
            pointer.transform.parent = holder.transform;
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;
        }
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.GetComponent<MeshRenderer>().material.SetColor("_Color", color);

        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if(collider)
            {
                Object.Destroy(collider);
            }
        }
	}

    public void OnEnable()
    {
        holder.gameObject.SetActive(true);
        SteamVR_Events.InputFocus.Listen(OnInputFocus);
    }

    public void OnDisable()
    {
        holder.gameObject.SetActive(false);
        SteamVR_Events.InputFocus.Remove(OnInputFocus);
    }

    private void OnInputFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            holder.gameObject.SetActive(true);
        }
        else
        {
            holder.gameObject.SetActive(false);
        }
    }

    public virtual void OnPointerIn(PointerEventArgs e)
    {
        if (PointerIn != null)
        {
            PointerIn(this, e);
        }
        unityPointerIn.Invoke(e);
    }

    public virtual void OnPointerOut(PointerEventArgs e)
    {
        if (PointerOut != null)
        {
            PointerOut(this, e);
        }
        unityPointerOut.Invoke(e);
    }


    // Update is called once per frame
	void Update ()
    {
        float dist = 100f;

        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool hasTarget = Physics.Raycast(raycast, out hit);

        if (previousContact && previousContact != hit.transform)
        {
           PointerEventArgs args = new PointerEventArgs();
           if (controller != null)
            {
                args.controllerIndex = controller.controllerIndex;
            }
            args.distance = 0f;
            args.flags = 0;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        if (hasTarget && previousContact != hit.transform)
        {
           PointerEventArgs argsIn = new PointerEventArgs();
           if (controller != null)
           {
               argsIn.controllerIndex = controller.controllerIndex;
           }
           argsIn.distance = hit.distance;
           argsIn.flags = 0;
           argsIn.target = hit.transform;
           OnPointerIn(argsIn);
           previousContact = hit.transform;
            }
            if (!hasTarget)
            {
                previousContact = null;
            }
            if (hasTarget && hit.distance < 100f)
            {
                dist = hit.distance;
            }

        if (controller != null && controller.triggerPressed)
        {
            pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
        }
        else
        {
            pointer.transform.localScale = new Vector3(thickness, thickness, dist);
        }
        pointer.transform.localPosition = new Vector3(0f, 0f, dist/2f);
    }
}
