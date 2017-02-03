using UnityEngine;
using System.Collections;

public class reticleRay : MonoBehaviour
{
    public float radius;
    public float direction;

    public Transform target;
    public RaycastHit hit;

    private Vector3 position;
    private float distance = -30.0f;

    Vector3 Center;
    int layerMask;


    void Start()
    {
        Center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        layerMask = LayerMask.GetMask("Player", "Rope/Normal", "Rope/Lock", "Bullet");
        layerMask = ~layerMask;

    }

    void Update()
    {
        position = transform.forward * distance;
    }

    public bool isShpereHit()
    {

        Ray ray = Camera.main.ScreenPointToRay(Center);
        //radius = transform.lossyScale.x;

        var sphereHit = (Physics.SphereCast(transform.position + position, radius, transform.forward, out hit, direction, layerMask));
        Vector3 hitScreenPoint = Camera.main.WorldToScreenPoint(hit.point);

        if (sphereHit && isScreen(hitScreenPoint))
        {
            target.position = hit.point;

            //target = hit.transform;
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * (hit.distance), radius);

        //Gizmos.DrawWireSphere(transform.position + transform.forward * direction, radius);
    }
    bool isScreen(Vector3 position)
    {
        if ((position.x >= 0 && position.x <= Screen.width) &&
            (position.y >= 0 && position.y <= Screen.height))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
