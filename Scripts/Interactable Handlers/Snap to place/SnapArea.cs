using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshCollider), typeof(MeshRenderer))]
public class SnapArea : MonoBehaviour
{
    public SnapHost Host;
    public delegate void SnapAreaEvent();
    public SnapAreaEvent OnSnapped, OnUnSnapped, OnPermanentSnap;
    public bool IsSnapped;
    public bool PermanentSnap;
    public bool SetInvisibleOnStart = true;
    public bool AttachOnEnter = false;
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;
    public UnityEvent OnSnap; 
    public Snappable EnteredSnappable;
    public List<Snappable> EnteredSnappableList;
    [Tooltip("Snappables will only snap in this SnapArea if it's key value is also in Locks")]
    public List<string> Locks;
    private bool AttachedOnEnter = false;

    void Awake()
    {
        IsSnapped = false;
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
        if(SetInvisibleOnStart)
        {
            meshRenderer.enabled = false;
        }
        #endregion
    }


    #region Setting/Removing the Event's Callbacks

    private void AddOnUnSnappedCallbacks()
    {
        if(Host != null)
        {
            OnUnSnapped += Host.CheckCompletion;
        }
        OnUnSnapped += EnableSnap;
        OnUnSnapped += RemoveAllUnSnappedCallbacks;
    }

    public void RemoveAllUnSnappedCallbacks()
    {
        RemoveAllCallbacks(ref OnUnSnapped);
    }

    private void AddOnPermanentSnapCallbacks()
    {
        OnPermanentSnap += DisableSnap;
        if (Host != null)
        {
            OnPermanentSnap += Host.CheckCompletion;
        }
    }

    private void AddOnSnappedCallbacks()
    {
        OnSnapped += AddOnUnSnappedCallbacks;
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

    private void OnDisable()
    {
        #region Removing Callbacks
        RemoveAllCallbacks(ref OnSnapped);
        RemoveAllCallbacks(ref OnPermanentSnap);
        #endregion
    }
    #endregion

    #region triggering Events
        #region Trigger OnSnappedEvent
        public void TriggerOnSnappedEvent()
        {
        IsSnapped = true;
            StartCoroutine("CorTriggerOnSnappedEvent");
        }

        IEnumerator CorTriggerOnSnappedEvent()
        {
            if (PermanentSnap)
            {
                OnPermanentSnap.Invoke();
                RemoveAllCallbacks(ref OnSnapped);
                RemoveAllCallbacks(ref OnPermanentSnap);
            }
            else
            {
                OnSnapped.Invoke();
                
            }
            OnSnap.Invoke();
            yield return null;
        }
        #endregion

        #region Trigger OnUnsnappedEvent
        public void TriggerOnUnsnappedEvent()
        {
            StartCoroutine("CorTriggerOnUnsnappedEvent");
        }

        IEnumerator CorTriggerOnUnsnappedEvent()
        {
            if (PermanentSnap)
            {
                Debug.Log("Trying to Unsnap from a permanent snap!");
            }
            else
            {
                OnUnSnapped.Invoke();
            }
            yield return null;
        }
        #endregion
    #endregion

    #region OnSnappedEvent Callback methods

    #endregion

    #region Enabling and disabling SnapArea
    public void EnableSnap()
    {
        if(PermanentSnap && IsSnapped)
        {
            DisableSnap();
            return;
        }
        meshRenderer.enabled = true;
        meshCollider.enabled = true;
    }

    public void EnableSnapRenderer()
    {   
        if(PermanentSnap == true && IsSnapped == true)
        {
            return;
        }
        meshRenderer.enabled = true;
    }

    public void DisableSnap()
    {
        meshRenderer.enabled = false;
        meshCollider.enabled = false;
    }

    public void DisableSnapRenderer()
    {
        meshRenderer.enabled = false;
    }
    #endregion

    #region Detecting entered snappables and handling them
    private void OnTriggerEnter(Collider other)
    {
        EnteredSnappable = other.GetComponent<Snappable>();
        if (EnteredSnappable == null)
        {
            return;
        }
        if (Locks.Contains(EnteredSnappable.Key))
        {
            EnteredSnappableList.Add(EnteredSnappable);
            EnteredSnappable.InitiateReadyToAttach(this);
            //temp sol
            meshRenderer.enabled = true;
            //
            if(AttachOnEnter)
            {
                if (!PermanentSnap)
                {
                    AttachedOnEnter = true;
                    AttachOnEnter = false;
                }
                EnteredSnappable.ForceInvokeSnap();
            }
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnteredSnappable = other.GetComponent<Snappable>();
        if (EnteredSnappable == null)
        {
            return;
        }
        if (EnteredSnappableList.Contains(EnteredSnappable))
        {
            EnteredSnappableList.Remove(EnteredSnappable);
            EnteredSnappable.InitiateUnReadyToAttach();
            //temp sol
            if(EnteredSnappable.HighlightOnPickup == false)
            {
                meshRenderer.enabled = false;
            }
            //
        }
        if(AttachedOnEnter)
        {
            AttachedOnEnter = false;
            AttachOnEnter = true;
        }
        EnteredSnappable = null;
    }
    #endregion
}
