using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
/// <summary>
/// TODO: This script is very mess and needs to be cleaned up.
 ///      1.Segment all OnUnequip/OnEquip callbacks into seperate actions and re-organize all code to methods for:
///            OnEnteringSnapArea, OnLeavingSnapArea
///            On Attaching when OnUnequip is called.
///            if SnapArea if permanent or non-permanent, call respective methods for acting on detachment 
///     
/// </summary>
[RequireComponent(typeof(Grabbable))]
public class Snappable : MonoBehaviour
{
    private Interactable interactable;

    [Tooltip("This string is used to filter out snapping with SnapAreas with a different 'Lock' string")]
    public string Key;
    private bool isAttached = false;
    private SnapArea TargetSnapArea;
    private Action ReadyToAttachAction;
    public bool ReadyToAttach = false;
    private Joint joint;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    public void InitiateReadyToAttach(SnapArea targetSnapArea)
    {
        TargetSnapArea = targetSnapArea;
        ReadyToAttach = true;
        interactable.OnUnequip += PerformToAttach;
        interactable.OnUnequip += SetJoint;
        interactable.OnUnequip += targetSnapArea.TriggerOnSnappedEvent;
    }

    /// <summary>
    /// What Needs to execute before being pickuped up. Basically, detaching the snappable object from the snaparea.
    ///         -----THIS WILL BE EXECUTED BEFORE PICKUP() FROM GRABBABLE-----
    /// </summary>
    public void InitiateReadyToDetach()
    {
        isAttached = false;
        if(joint == null)
        {
            Debug.Log("can't remove joint as it doesn't exist");
        }
        else
        {
            Destroy(joint);
        }
        TargetSnapArea.TriggerOnUnsnappedEvent();
        interactable.OnEquip -= InitiateReadyToDetach;
    }

    /// <summary>
    /// Executed when interactable.Unequip is invoked when inside the SnapArea.
    /// This is meant to take care of snapping the object to the snaparea.
    /// This Also is incharge of setting the callbacks for when it is picked up agian after snapping. 
    /// </summary>
    private void PerformToAttach()
    {
        isAttached = true;
        QuickAttach();
        if(TargetSnapArea.PermanentSnap == false)
        {
            //this makes sure that InitiateReadyToDetach always occurs first
            Action temp = interactable.OnEquip;
            interactable.OnEquip = null;
            interactable.OnEquip += InitiateReadyToDetach;
            interactable.OnEquip += temp;

            interactable.OnUnequip -= TargetSnapArea.TriggerOnSnappedEvent;
            interactable.OnUnequip -= SetJoint;
            interactable.OnUnequip -= PerformToAttach;
        }
        else
        {
            interactable.OnEquip -= InitiateReadyToDetach;
            //quick fix for when when the permanemt snap toggle is changed during runtime
            interactable.OnUnequip -= TargetSnapArea.TriggerOnSnappedEvent;
            interactable.OnUnequip -= SetJoint;
            interactable.OnUnequip -= PerformToAttach;
        }
    }

    /// <summary>
    /// Called when the snappable enters the SnapArea
    /// </summary>
    public void InitiateUnReadyToAttach()
    {
        isAttached = false;
        interactable.OnUnequip -= TargetSnapArea.TriggerOnSnappedEvent;
        interactable.OnUnequip -= SetJoint;
        interactable.OnUnequip -= PerformToAttach;
        ReadyToAttach = false;
        TargetSnapArea = null;
    }

    /// <summary>
    /// A quickAttach script to just snap it in position of the snapArea.
    /// This can be replaced with an animation or a lerp function. 
    /// </summary>
    public void QuickAttach()
    {
        transform.position = TargetSnapArea.transform.position;
        transform.rotation = TargetSnapArea.transform.rotation;
        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    /// <summary>
    /// This Creates and sets a joint 
    /// </summary>
    public void SetJoint()
    {
        //Adding this line would make sure that there is only one joint for a snappable,
        //but also means that it cant be fixed into more than 1 Snap Area
        //if ((joint = gameObject.GetComponent<FixedJoint>()) == null)
        {
            joint = gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
        }
        joint.connectedBody = TargetSnapArea.Host.GetComponent<Rigidbody>();
    }
}
