using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootRope : MonoBehaviour
{
    //[SerializeField, Tooltip("説明文")]
    
    Vector3 position1;
    Vector3 position2;

    int layerMask = -1 - (1<<LayerMask.NameToLayer("Player") | 
                          1<<LayerMask.NameToLayer("Rope"  ) );

    void Awake()
    {
        
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    void Initialize(Vector3 start, Vector3 end)
    {
        position1 = start;
        position2 = end;
    }

    void Initialize(Ray ray)
    {
        RaycastHit hitInfo;
        Physics.Raycast(ray, out hitInfo, layerMask);
        
        position1 = ray.origin;
        position2 = hitInfo.point;
    }
}