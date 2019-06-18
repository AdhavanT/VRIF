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
    public bool IsTeleporting = false;
    public float MaxDistance = 10f;
    public GameObject Pointer;
    public float FadeDuration = 0.2f;
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
            TeleportToPoint();
        }

    }

    private void TeleportToPoint()
    {
        IsTeleporting = true;
        Transform RigTransform = SteamVR_Render.Top().origin;
        Vector3 HeadPos = SteamVR_Render.Top().head.position;
        Vector3 TranslateVector = Pointer.transform.position - HeadPos;
        StartCoroutine(MoveRig(RigTransform, TranslateVector));
    }

    public IEnumerator MoveRig(Transform CameraRig, Vector3 Translate)
    {
        SteamVR_Fade.Start(Color.black, FadeDuration, true);
        yield return new WaitForSeconds(FadeDuration);
        CameraRig.position += Translate;
        SteamVR_Fade.Start(Color.clear, FadeDuration, true);
        IsTeleporting = false;
        yield return null;
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
