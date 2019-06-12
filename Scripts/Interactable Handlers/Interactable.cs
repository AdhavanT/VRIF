using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using System;

public class Interactable : MonoBehaviour
{
    //[HideInInspector]
    public Hand_Controller ActiveHand = null;
    [HideInInspector]
    public Hand_Controller m_SecondaryHand = null;
    public bool isActive;
    //The Events triggered by every interaction script
    public delegate void InteractableEvent();
    public InteractableEvent OnStartInteraction;
    /// <summary>
    /// The OnUnequip delegate should be used for adding methods that are meant to 'detach' the interactable. 
    /// OnEndInteractions should be used to call OnUnequip as this will completely remove the interactable from interaction.
    /// </summary>
    public InteractableEvent OnUnequip;
    public InteractableEvent OnEndInteraction;
    public InteractableEvent ActionsToPerform;


    private void Awake()
    {
        OnStartInteraction += SetCurrentInteractive;
        OnEndInteraction += RemoveCurrentInteractive;
    }

    public void SetCurrentInteractive()
    {
        ActiveHand.m_CurrentInteractable = this;
        isActive = true;
    }

    public void RemoveCurrentInteractive()
    {
        Debug.Log("RemoveCurrentInteractive");
        OnUnequip.Invoke();
        ActiveHand.FlushInteractable();
        ActiveHand = null;
        isActive = false;
    }

}
