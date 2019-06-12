using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class Hand_Controller : MonoBehaviour
{
    public SteamVR_Action_Boolean m_TeleportAction;

    [HideInInspector]
    public SteamVR_Behaviour_Pose m_Pose = null;
    [HideInInspector]
    public FixedJoint m_joint = null;

    private bool LookForNearestInteractable = false;
    private bool CoroutineisActive = false;
    [SerializeField]
    private Interactable m_NearestInteractable = null;
    public Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();

    // Start is called before the first frame update
    void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_joint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_NearestInteractable != null)
        {
            if (m_NearestInteractable.ActiveHand == null)
            {
                m_NearestInteractable.ActiveHand = this;
            }
            if(m_NearestInteractable.ActiveHand == this)
            {
                SendInputActions();
            } 
        }
    }

    private void SendInputActions()
    {
        m_NearestInteractable.ActionsToPerform.Invoke();
    }

    #region Detecting and adding to list all interactables within hand's sphere collider 

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Interactable>() == null)
        {
            return;
        }

        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
        if (m_CurrentInteractable == null)
        {
            if (m_ContactInteractables.Count > 1)
            {
                LookForNearestInteractable = true;
                if(CoroutineisActive == false)
                {
                    StartCoroutine(SetNearestInteractable());
                }
            }
            else
            {
                LookForNearestInteractable = false;
                UpdateNearestInteractable(GetNearestInteractable());
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<Interactable>() == null)
        {
            return;
        }
        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
        if (m_CurrentInteractable == null)
        {
            if (m_ContactInteractables.Count <= 1)
            {
                UpdateNearestInteractable(GetNearestInteractable());
                LookForNearestInteractable = false;
            }
        }
    }

    public IEnumerator SetNearestInteractable()
    {
        CoroutineisActive = true;
        while(LookForNearestInteractable && m_CurrentInteractable == null)
        {
            UpdateNearestInteractable(GetNearestInteractable());
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("stopped looking for nearest interactable");
        CoroutineisActive = false;
        yield return null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0f;

        if(m_ContactInteractables.Count == 0)
        {
            return null;
        }
        foreach(Interactable interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;
            if(distance < minDistance)
            {
                nearest = interactable;
                minDistance = distance;
            }
        }
        return nearest;
    }

    #endregion

    public void FlushInteractable()
    {
        m_CurrentInteractable = null;
        UpdateNearestInteractable(GetNearestInteractable());
    }

    public void UpdateNearestInteractable(Interactable interactable)
    {
        if(m_NearestInteractable != null)
        {
            m_NearestInteractable.ActiveHand = null;
        }

        m_NearestInteractable = interactable;
    }
}