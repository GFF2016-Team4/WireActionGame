using UnityEngine;
using System.Collections;

public class reticleRay : MonoBehaviour
{
    public float radius = 10.0f;
    public float direction;

    public Transform target;
    Vector3 Center;
    public RaycastHit hit;

    void Start()
    {
        Center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    void Update()
    {
    }

    public bool isShpereHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Center);
        //radius = transform.lossyScale.x;

        var sphereHit = Physics.SphereCast(transform.position, radius, transform.forward, out hit, direction);

        if (sphereHit)
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
}
