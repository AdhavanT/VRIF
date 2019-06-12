using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(MeshCollider), typeof(MeshRenderer))]
public class SnapArea : MonoBehaviour
{
    public SnapHost Host;
    public delegate void SnapAreaEvent();
    public SnapAreaEvent OnSnapped, OnPermanentSnap;
    public bool IsSnapped;
    public bool PermanentSnap;
    
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;

    public Snappable EnteredSnappable;
    public List<Snappable> EnteredSnappableList;
    [Tooltip("Snappables will only snap in this SnapArea if it's key value is also in Locks")]
    public List<string> Locks;

    void Awake()
    {
        Host = GetComponentInParent<SnapHost>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    #region Adding Event callbacks on OnEnable and removing them on OnDisable
    private void OnEnable()
    {
        #region Setting Events Callbacks
        AddOnSnappedCallbacks();
        AddOnPermanentSnapCallbacks();
        #endregion
    }

        #region Setting/Removing the Event's Callbacks
        private void AddOnPermanentSnapCallbacks()
        {
            OnSnapped += SetIsSetTrue;
            if (Host != null)
            {
                OnSnapped += Host.CheckCompletion;
            }
        }

        private void AddOnSnappedCallbacks()
        {
            OnSnapped += SetIsSetTrue;
            OnSnapped += DisableSnap;
            if (Host != null)
            {
                OnSnapped += Host.CheckCompletion;
            }
        }

        private void RemoveAllCallbacks(ref SnapAreaEvent EmptysnapAreaEvent)
        {
            EmptysnapAreaEvent = null;
        }
        #endregion

    private void DestroySnap()
    {
        
    }

    private void OnDisable()
    {
        #region Removing Callbacks
        RemoveAllCallbacks(ref OnSnapped);
        RemoveAllCallbacks(ref OnPermanentSnap);
        #endregion
    }
    #endregion

    #region triggering OnSnappedEvent or the OnSnappedPermanentEvent
    public void TriggerOnSnappedEvent()
    {
        StartCoroutine("CorTriggerOnSnappedEvent");
    }

    IEnumerator CorTriggerOnSnappedEvent()
    {
        if(PermanentSnap)
        {
            OnPermanentSnap.Invoke();
            RemoveAllCallbacks(ref OnSnapped);
            RemoveAllCallbacks(ref OnPermanentSnap);
        }
        else
        {
            OnSnapped.Invoke();
        }
        yield return null;
    }
    #endregion

    #region OnSnappedEvent Callback methods
    public void SetIsSetTrue()
    {
        IsSnapped = true;
    }
    #endregion

    #region Enabling and disabling SnapArea
    public void EnableSnap()
    {
        meshRenderer.enabled = true;
        meshCollider.enabled = true;
    }

    public void DisableSnap()
    {
        meshRenderer.enabled = false;
        meshCollider.enabled = false;
    }
    #endregion

    #region Detecting entered snappables and handling them
    private void OnTriggerEnter(Collider other)
    {
        EnteredSnappable = other.GetComponent<Snappable>(); 
        if(EnteredSnappable == null)
        {
            return;
        }
        if (Locks.Contains(EnteredSnappable.Key))
        {
            EnteredSnappableList.Add(EnteredSnappable);
            EnteredSnappable.InitiateReadyToAttach(this);

            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnteredSnappable = other.GetComponent<Snappable>();
        if(EnteredSnappable == null)
        {
            return;
        }
        if(EnteredSnappableList.Contains(EnteredSnappable))
        {
            EnteredSnappableList.Remove(EnteredSnappable);
            EnteredSnappable.InitiateUnReadyToAttach();
        }
        EnteredSnappable = null;
    }
    #endregion
}
