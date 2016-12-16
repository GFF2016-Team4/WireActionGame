using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SyncObject : MonoBehaviour
{
    [SerializeField, Tooltip("説明文")]
    private Transform sync;

    private Vector3 offset;

    void LateUpdate()
    {
        if(sync == null) return;
        
        transform.position = sync.position - offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(sync != null) return;
        sync = other.transform;

        offset = sync.position - transform.position;
    }

    public void SetSyncTransform(Transform syncTrans, Vector3 offsetPosition)
    {
        sync   = syncTrans;
        offset = offsetPosition;
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.transform != sync) return;
    //    sync = null;
    //}
}