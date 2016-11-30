using UnityEngine;
using System.Collections;

public class BoxcastTest : MonoBehaviour 
{
	RaycastHit hit;

    [SerializeField]
    Vector3 scale;
    
    [SerializeField]
    Vector3 direction;

	[SerializeField]
	bool isEnable = false;

	void OnDrawGizmos()
	{
		if (isEnable == false) return;

        direction.Normalize();

		var isHit = Physics.BoxCast (transform.position, scale/2, direction, out hit, transform.rotation);
        var isBoxHit = Physics.CheckBox(transform.position, scale/2, Quaternion.identity, ~(1<<gameObject.layer));

        int dis = (isBoxHit) ? 0 : 1; 

		if (isHit)
        {
			Gizmos.DrawRay (transform.position, direction * hit.distance * dis);
			Gizmos.DrawWireCube (transform.position + direction * hit.distance * dis, scale);
		}
        else
        {
			Gizmos.DrawRay (transform.position, direction * 100 * dis);
		}
	}
}