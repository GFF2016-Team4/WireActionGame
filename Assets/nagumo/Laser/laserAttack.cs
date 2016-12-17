using UnityEngine;
using System.Collections;

namespace Enemy
{
    public class laserAttack : MonoBehaviour
    {

        public GameObject Shooter;
        [System.NonSerialized]
        public float laserHP;

        [System.NonSerialized]
        public float radius = 1;
        public float radiusDecision;

        LineRenderer laserBlue;
        RaycastHit hit;

        [System.NonSerialized]
        public float laserRadius;
        public float laserAttackTime;
        // Use this for initialization
        void Start()
        {
            laserBlue = this.GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            laserBlue.SetPosition(0, transform.position);

            //レーザーの太さ
            laserBlue.SetWidth(laserRadius, laserRadius);

            //レーザー縮む
            if (laserRadius >= 0)
            {
                laserRadius -= laserAttackTime;
            }

            Ray ray = new Ray(transform.position, Shooter.transform.forward);
            var laserhitPoint = Physics.SphereCast(transform.position, radius, transform.forward, out hit, 100);
            if (radius >= 0)
            {
                radius -= radiusDecision;
            }

            if (laserhitPoint)
            {
                laserBlue.SetPosition(1, hit.point);

                if (hit.collider.tag == "kabe")
                {
                    Debug.Log("hit");
                    if (laserHP == 2)
                    {
                        Destroy(hit.collider.gameObject);
                    }
                    laserHP -= 1;
                }
            }
            else
            {
                laserBlue.SetPosition(1, transform.forward * 400);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + transform.forward * (hit.distance), radius);
        }
    }

}
