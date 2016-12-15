using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ListLineDraw))]
public class LockRope : MonoBehaviour
{
    [SerializeField]
    Transform ropePoint1;

    [SerializeField]
    Transform ropePoint2;
    
    [SerializeField]
    float ignoreDistance;

    ListLineDraw lineDraw;

    void Awake()
    {
        lineDraw = GetComponent<ListLineDraw>();
        lineDraw.AddDrawList(ropePoint1);
        lineDraw.AddDrawList(ropePoint2);
    }

    public void Initialize(Vector3 point1, Vector3 point2)
    {
        Initialize(point1, ropePoint1);
        Initialize(point2, ropePoint2);
    }

    void Initialize(Vector3 point, Transform ropePoint)
    {
        ropePoint.position = point;
        SphereCollider col = ropePoint.GetComponent<SphereCollider>();

        bool isHit = Physics.CheckSphere(ropePoint.position, col.radius, -1 - (1 << gameObject.layer));

        if(isHit)
        {
            ropePoint.gameObject.AddComponent<SyncObject>();
        }
        else
        {
            Rigidbody rig = ropePoint.GetComponent<Rigidbody>();
            rig.isKinematic = false;

            Destroy(gameObject, 5.0f);
        }

        lineDraw.DrawStart();
    }

    void Update()
    {
        Ray ray = new Ray()
        {
            origin    = ropePoint1.position,
            direction = ropePoint2.position - ropePoint1.position
        };

        float distance = Vector3.Distance(ropePoint1.position, ropePoint2.position);
        distance -= ignoreDistance;
        RaycastHit hitInfo;
        
        bool isHit = Physics.Raycast(ray, out hitInfo, distance);

        if(isHit && hitInfo.collider.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}