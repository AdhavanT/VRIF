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
    public SnapAreaEvent OnSnapped;
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
        #region Setting OnSnapped Callbacks
        OnSnapped += SetIsSetTrue;
        OnSnapped += DisableSnap;
        OnSnapped += Host.CheckCompletion;
        #endregion
    }
    private void OnDisable()
    {
        #region Removing OnSnapped Callbacks
        OnSnapped -= Host.CheckCompletion;
        OnSnapped -= SetIsSetTrue;
        #endregion
    }
    #endregion

    #region triggering OnAttachEvent
    public void TriggerOnAttachEvent()
    {
        StartCoroutine("CorTriggerOnAttachEvent");
    }

    IEnumerator CorTriggerOnAttachEvent()
    {
        OnSnapped.Invoke();
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
