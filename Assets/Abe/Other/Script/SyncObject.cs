using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SyncObject : MonoBehaviour
{
    [SerializeField, Tooltip("説明文")]
    private Transform sync;

    GameObject point;

    readonly List<string> ignore = new List<string>(new string[] 
    {
        "Player", "Rope/Normal", "Rope/Lock", "Bullet"
    });

    void Awake()
    {
        int layerMask = LayerMask.GetMask(ignore.ToArray());
        float radius = GetComponent<SphereCollider>().radius;
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, layerMask);

        foreach(Collider col in cols)
        {
            if(transform == col.transform) continue;

            sync = col.transform;

            GameObject obj         = new GameObject();
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.transform.parent   = sync.transform;
            point = obj;
            return;
        }
    }

    void LateUpdate()
    {
        if(sync == null) return;

        transform.position = point.transform.position;
        transform.rotation = point.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(sync != null) return;
        string otherLayer = LayerMask.LayerToName(other.gameObject.layer);
        bool exist = ignore.Contains(otherLayer);
        if(exist) return;

        sync = other.transform;

        GameObject obj         = new GameObject();
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.transform.parent   = sync.transform;
        point = obj;
    }

    //public void SetSyncTransform(Transform syncTrans, Vector3 offsetPosition)
    //{
    //    sync   = syncTrans;
    //    offset = offsetPosition;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.transform != sync) return;
    //    sync = null;
    //}
}