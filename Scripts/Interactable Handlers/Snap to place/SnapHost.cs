using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class SnapHost : MonoBehaviour
{
    public bool Completed = false;
    public SnapArea[] SnapAreas;
    public UnityEvent OnCompleted;

    private void Awake()
    {
        //SnapAreas = GetComponentsInChildren<SnapArea>();
        //foreach(SnapArea snapArea in SnapAreas)
        //{
        //    snapArea.Host = this;
        //}
    }

    public void CheckCompletion()
    {
        Completed = true;
        foreach(SnapArea snapArea in SnapAreas)
        {
            if(snapArea.IsSnapped == false)
            {
                Completed = false;
                return;
            }
        }
        if(Completed == true)
        {
            OnCompleted.Invoke();
        }
    }
}
