using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

[RequireComponent(typeof(Grabbable))]
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
        interactable.OnUnequip += PerformOnAttach;
        interactable.OnUnequip += targetSnapArea.TriggerOnSnappedEvent;
    }

    private void PerformOnAttach()
    {
        isAttached = true;
        QuickAttach();
    }

    public void InitiateUnReadyToAttach()
    {
        isAttached = false;
        interactable.OnUnequip -= TargetSnapArea.TriggerOnSnappedEvent;
        interactable.OnUnequip -= PerformOnAttach;
        ReadyToAttach = false;
        TargetSnapArea = null;
    }

    public void QuickAttach()
    {
        transform.position = TargetSnapArea.transform.position;
        transform.rotation = TargetSnapArea.transform.rotation;
        Joint joint;
        if ((joint = gameObject.GetComponent<FixedJoint>()) == null)
        {
            joint = gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
        }
        joint.connectedBody = TargetSnapArea.Host.GetComponent<Rigidbody>();
    }
}
