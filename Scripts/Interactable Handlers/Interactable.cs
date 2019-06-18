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
    public bool isActive = false;
    //The Events triggered by every interaction script
    public delegate void InteractableEvent();
    /// <summary>
    /// The OnUnequip delegate should be used for adding methods that are meant to 'detach' the interactable. 
    /// OnEndInteractions should be used to call OnUnequip as this will completely remove the interactable from interaction.
    /// </summary>
    public Action OnUnequip;
    public Action OnEquip;
    public InteractableEvent OnEndInteraction;
    public InteractableEvent OnStartInteraction;
    public InteractableEvent ActionsToPerform;


    private void Awake()
    {
        OnStartInteraction += SetCurrentInteractive;
        OnEndInteraction += RemoveCurrentInteractive;
    }

    public void SetCurrentInteractive()
    {
        ActiveHand.SetCurrentInteractable(this);
        isActive = true;
        OnEquip.Invoke();
    }

    public void RemoveCurrentInteractive()
    {
        OnUnequip.Invoke();
        ActiveHand.FlushInteractable();
        ActiveHand = null;
        isActive = false;
    }

}
