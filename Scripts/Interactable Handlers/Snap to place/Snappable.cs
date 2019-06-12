using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

[RequireComponent(typeof(Grabbable), typeof(FixedJoint))]
public class Snappable : MonoBehaviour
{
    private Interactable interactable;

    [Tooltip("This string is used to filter out snapping with SnapAreas with a different 'Lock' string")]
    public string Key;
    private bool isAttached = false;
    private SnapArea TargetSnapArea;
    public bool ReadyToAttach = false;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    public void InitiateReadyToAttach(SnapArea targetSnapArea)
    {
        TargetSnapArea = targetSnapArea;
        ReadyToAttach = true;
        interactable.OnUnequip += EnableOnAttach;
        interactable.OnUnequip += targetSnapArea.TriggerOnAttachEvent;
    }

    private void EnableOnAttach()
    {
        isAttached = true;
        QuickAttach();
    }

    public void InitiateUnReadyToAttach()
    {
        isAttached = false;
        interactable.OnUnequip -= EnableOnAttach;
        ReadyToAttach = false;
        TargetSnapArea = null;
    }

    public void QuickAttach()
    {
        Debug.Log("quick attach is called");
        transform.position = TargetSnapArea.transform.position;
        transform.rotation = TargetSnapArea.transform.rotation;
        Joint joint = GetComponent<Joint>();
        joint.connectedBody = TargetSnapArea.Host.GetComponent<Rigidbody>();
    }
}
