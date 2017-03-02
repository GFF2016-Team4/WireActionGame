using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchRope : MonoBehaviour
{
    [SerializeField, Tooltip("説明文")]
    float destroyDistance = 20.0f;

    Transform rootOrigin;

    [System.NonSerialized]
    public Transform sync;

    NormalRopeSimulate simulate;

    void Awake()
    {
        simulate = GetComponent<NormalRopeSimulate>();
    }
    
    void Start()
    {
        rootOrigin = simulate.nowOrigin;
    }
    
    void Update()
    {
        if(!simulate.isContorll) return;

        //距離をみて距離が一定以上離れていればロープを外す
        float nowDistance = Vector3.Distance(simulate.tailPosition, rootOrigin.position);
        if(nowDistance >= destroyDistance)
        {
            simulate.SimulationEnd(sync);
        }
    }
}