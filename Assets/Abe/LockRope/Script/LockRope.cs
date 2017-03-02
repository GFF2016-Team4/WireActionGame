using UnityEngine;

[RequireComponent(typeof(ListLineDraw))]
public class LockRope : MonoBehaviour
{
    [SerializeField]
    Transform ropePoint1;

    [SerializeField]
    Transform ropePoint2;
    
    [SerializeField]
    float ignoreDistance;

    [SerializeField]
    float destroyDistance;

    ListLineDraw lineDraw;

    float nowDistance;

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
        nowDistance = Vector3.Distance(point1, point2);
    }

    void Initialize(Vector3 point, Transform ropePoint)
    {
        ropePoint.position = point;
        SphereCollider col = ropePoint.GetComponent<SphereCollider>();

        bool isHit = Physics.CheckSphere(ropePoint.position, col.radius, PlayersLayerMask.IgnorePlayerAndRopes);

        if(isHit)
        {
            ropePoint.gameObject.AddComponent<SyncObject>();
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

        if(distance > nowDistance + destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}