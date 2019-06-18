using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    LayerMask TeleportableLayers = 1 << 8;
    public Hand_Controller Right_Hand, Left_Hand;
    private Hand_Controller CurrentHand;
    public GameObject CameraRig;
    public bool HandAvailable = true;
    public bool CanTeleport = false;
    public bool IsTeleporting = false;
    public float MaxDistance = 10f;
    public float FadeDuration = 0.2f;
    public Vector3 offset = Vector3.zero;
    public SteamVR_Action_Boolean TeleportAction;
    public SteamVR_Action_Boolean SwapHandAction;

    private void Awake()
    {
        if(TeleportAction == null)
        {
            TeleportAction = SteamVR_Actions.default_Teleport;
        }
        CurrentHand = Left_Hand == null ? Right_Hand : Left_Hand;
        CurrentHand = Right_Hand == null ? Left_Hand : Right_Hand;
        Left_Hand.UpdatedCurrentInteractable.AddListener(UpdateHandAvailable);
        Right_Hand.UpdatedCurrentInteractable.AddListener(UpdateHandAvailable);
    }

    private void UpdateHandAvailable()
    {
        if(Left_Hand == null || Right_Hand == null)
        {
            return;
        }
        if(CurrentHand.IsHoldingInteractable)
        {
            SwapCurrentHand();
        }
    }

    private void SwapCurrentHand()
    {
        if(CurrentHand == Right_Hand)
        {
            CurrentHand = Left_Hand;
            return;
        }
        else if(CurrentHand == Left_Hand)
        {
            CurrentHand = Right_Hand;
            return;
        }
        else
        {
            return;
        }
    }

    void Update()
    {
        if (!HandAvailable)
        {
            return;
        }
        CheckforSwap();
        UpdatePointer();
        if(!CanTeleport)
        {
            return;
        }
        if (TeleportAction.GetStateDown(CurrentHand.m_Pose.inputSource))
        {
            TeleportToPoint();
        }

    }

    private void CheckforSwap()
    {
        if(Left_Hand == null || Right_Hand == null)
        {
            return;
        }
        if(CurrentHand == Right_Hand)
        {
            if(SwapHandAction.GetStateDown(Left_Hand.m_Pose.inputSource))
            {
                SwapCurrentHand();
                return;
            }
        }
        if(CurrentHand == Left_Hand)
        {
            if(SwapHandAction.GetStateDown(Right_Hand.m_Pose.inputSource))
            {
                SwapCurrentHand();
                return;
            }
        }
    }

    private void TeleportToPoint()
    {
        IsTeleporting = true;
        Transform RigTransform = SteamVR_Render.Top().origin;
        Vector3 HeadPos = SteamVR_Render.Top().head.position;
        Vector3 TranslateVector = transform.position - HeadPos;
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

    private void UpdatePointer()
    {
        Ray ray = new Ray(CurrentHand.transform.position + offset, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, MaxDistance))
        {
            Debug.Log(hit.collider.gameObject.layer);
            if(TeleportableLayers == (TeleportableLayers | (1 << hit.collider.gameObject.layer)))
            {
                GetComponent<MeshRenderer>().enabled = true;
                transform.position = hit.point;
                CanTeleport = true;
            }
            else
            {
                GetComponent<MeshRenderer>().enabled = false;
                CanTeleport = false;
            }
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
            CanTeleport = false;
        }
    }
}
