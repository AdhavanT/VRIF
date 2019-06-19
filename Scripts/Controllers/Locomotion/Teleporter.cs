using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Hand_Controller))]
public class Teleporter : MonoBehaviour
{
    public LayerMask TeleportableLayers;
    private bool CanTeleport;
    private bool IsTeleporting = false;
    public GameObject Pointer;
    //public GameObject Rig, Head;
    public static Teleporter ActiveTeleporter;
    private Hand_Controller ThisHand;
    public SteamVR_Action_Boolean SwapHandAction;
    public SteamVR_Action_Boolean TeleportAction;
    public float FadeDuration = 0.2f;
    public Vector3 offset = Vector3.zero;
    public float MaxDistance = 10f;

    private void Awake()
    {
        ThisHand = GetComponent<Hand_Controller>();
    }

    private void Start()
    {
        if(TeleportAction == null)
        {
            TeleportAction = SteamVR_Actions.default_Teleport;
        }
        if (ActiveTeleporter == null)
        {
            ActiveTeleporter = this;
        }
    }

    void Update()
    {
        if(IsTeleporting)
        {
            return;
        }
        if (ActiveTeleporter != this)
        {
            if(SwapHandAction.GetStateDown(ThisHand.m_Pose.inputSource))
            {
                Debug.Log("Making this the active hand");
                ActiveTeleporter = this;
            }
            return;
        }
        if(ThisHand.IsHoldingInteractable)
        {
            return;
        }
        UpdatePointer();
        if (!CanTeleport)
        {
            return;
        }
        if (TeleportAction.GetStateDown(ThisHand.m_Pose.inputSource))
        {
            TeleportToPoint();
        }
    }

    private void TeleportToPoint()
    {
        IsTeleporting = true;
        Transform RigTransform = SteamVR_Render.Top().origin;
        Vector3 HeadPos = SteamVR_Render.Top().head.position;
        //
        //Transform RigTransform = Rig.transform;
        //Vector3 HeadPos = Head.transform.position;
        //
        float HeadYpos = SteamVR_Render.Top().head.transform.localPosition.y;
        Vector3 TranslateVector = Pointer.transform.position - HeadPos;
        TranslateVector.y += HeadYpos;
        StartCoroutine(MoveRig(RigTransform, TranslateVector));
    }

    public IEnumerator MoveRig(Transform CameraRig, Vector3 Translate)
    {
        SteamVR_Fade.View(Color.black, FadeDuration);
        yield return new WaitForSeconds(FadeDuration);
        CameraRig.position += Translate;
        SteamVR_Fade.View(Color.clear, FadeDuration);
        IsTeleporting = false;
    }

    private void UpdatePointer()
    {
        Ray ray = new Ray(transform.position + offset, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, MaxDistance))
        {
            if (TeleportableLayers == (TeleportableLayers | (1 << hit.collider.gameObject.layer)))
            {
                Pointer.SetActive(true);
                Pointer.transform.position = hit.point;
                CanTeleport = true;
            }
            else
            {
                CanTeleport = false;
                Pointer.SetActive(false);
            }
        }
        else
        {
            CanTeleport = false;
            Pointer.SetActive(false);
        }
    }


}
