using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

/// <summary>
/// This script handles the picking up and dropping of an objected. It requires a rigidbody, mesh collider 
/// TODO: Set up shader for pickupable
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(Interactable))]
public class Grabbable : MonoBehaviour
{
    private Interactable interactable;

    public UnityEvent OnPickUp;
    public UnityEvent OnRelease;
    public bool isGrabbable = true;
    public bool enableToggleGrab = false;
    private bool isGrabbed = false;
    private Rigidbody rb;
   
    public SteamVR_Action_Boolean OnGrabGrip;

    private void Awake()
    {
        if(OnGrabGrip == null)
        {
            OnGrabGrip = SteamVR_Actions.default_GrabGrip;
        }
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<Interactable>();
    }

    private void OnEnable()
    {
        OnPickUp.AddListener(Pickup);
        interactable.ActionsToPerform += PerformUpdateAction;
    }

    private void OnDisable()
    {
        interactable.ActionsToPerform -= PerformUpdateAction;
    }

    public void PerformUpdateAction()
    {
        if (enableToggleGrab == false)
        {
            if (isGrabbed)
            {
                if (OnGrabGrip.GetState(interactable.ActiveHand.m_Pose.inputSource) == true)
                {
                    return;
                }
                else
                {
                    interactable.RemoveCurrentInteractive();
                }
            }
            else
            {
                if (OnGrabGrip.GetState(interactable.ActiveHand.m_Pose.inputSource) == false)
                {
                    return;
                }
                else
                {
                    OnPickUp.Invoke();
                }

            }
        }
        //for when grabbing is toggle mode
        else
        {
            if(OnGrabGrip.GetChanged(interactable.ActiveHand.m_Pose.inputSource))
            {
                if(isGrabbed)
                {
                    if(OnGrabGrip.GetState(interactable.ActiveHand.m_Pose.inputSource) == true)
                    {
                        isGrabbed = false;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if(OnGrabGrip.GetState(interactable.ActiveHand.m_Pose.inputSource) == true)
                    {
                        OnPickUp.Invoke();
                    }
                    else
                    {
                        interactable.RemoveCurrentInteractive();
                    }
                }
            }
        }
  
    }

    private void Drop()
    {
        rb.velocity = interactable.ActiveHand.m_Pose.GetVelocity();
        rb.angularVelocity = interactable.ActiveHand.m_Pose.GetAngularVelocity();
        interactable.ActiveHand.m_joint.connectedBody = null;
        interactable.OnUnequip -= Drop;
        isGrabbed = false;
    }

    private void Pickup()
    {
        interactable.OnStartInteraction.Invoke();
        isGrabbed = true;
        transform.position = interactable.ActiveHand.transform.position;
        interactable.ActiveHand.m_joint.connectedBody = rb;
        interactable.OnUnequip += Drop;
    }
}
