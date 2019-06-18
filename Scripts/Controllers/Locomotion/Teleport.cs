using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    LayerMask TeleportableLayers = 1 << 8;
    Hand_Controller hand_Controller;
    public GameObject CameraRig;
    public bool HandAvailable = true;
    public bool CanTeleport = false;
    public float MaxDistance = 10f;
    public GameObject Pointer;
    public Vector3 offset = Vector3.zero;
    public SteamVR_Action_Boolean TeleportAction;

    private void Awake()
    {
        hand_Controller = GetComponent<Hand_Controller>();
        hand_Controller.UpdatedCurrentInteractable.AddListener(UpdateHandAvailable);
    }

    private void UpdateHandAvailable()
    {
        HandAvailable = hand_Controller.IsHoldingInteractable;
    }

    void Update()
    {
        if (HandAvailable)
        {
            return;
        }
        UpdatePointer();
        if(!CanTeleport)
        {
            return;
        }
        if (TeleportAction.GetStateDown(hand_Controller.m_Pose.inputSource))
        {
            Teleport();
        }

    }

    private void Teleport()
    {
        
    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position + offset, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, MaxDistance))
        {
            if(TeleportableLayers == (TeleportableLayers | (1 << hit.collider.gameObject.layer)))
            {
                Pointer.SetActive(true);
                Pointer.transform.position = hit.point;
                CanTeleport = true;
                return true;
            }
            else
            {
                Pointer.SetActive(false);
                CanTeleport = false;
                return false;
            }
        }
        else
        {
            Pointer.SetActive(false);
            CanTeleport = false;
            return false;
        }
    }
}
