using UnityEngine;
using System.Collections;

public class reticleRay : MonoBehaviour
{
    public float radius = 10.0f;
    public float direction;

    public Transform target;
    public RaycastHit hit;

    private float distance = -10.0f;
    private Vector3 position;
    Vector3 Center;
    LayerMask layerMask = ~(1 << 9 | 1 << 10 | 1 << 11 | 1 << 12);



    void Start()
    {
        Center = new Vector3(Screen.width / 2, Screen.height / 2, 0);

    }

    void Update()
    {
        position = transform.forward * distance;

    }

    public bool isShpereHit()
    {
        Debug.Log(position);

        Ray ray = Camera.main.ScreenPointToRay(Center);
        //radius = transform.lossyScale.x;

        //var sphereHit = Physics.SphereCast(transform.position, radius, transform.forward, out hit, direction, test.value);
        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);


        if (Physics.SphereCast(transform.position + position, radius, transform.forward, out hit, direction, layerMask))
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
;