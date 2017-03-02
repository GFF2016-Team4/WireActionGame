using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Enemy
{
    public class laserAttack : MonoBehaviour
    {

        public GameObject Shooter;
        public GameObject SendPlayer;

        [System.NonSerialized]
        public float laserHP;

        [System.NonSerialized]
        public float radius = 0.8f;
        public float radiusDecision;       

        [System.NonSerialized]
        public LineRenderer laserBlue;
        RaycastHit hit;

        [System.NonSerialized]
        public float laserRadius;
        public float laserAttackTime;

        private GameObject m_laserpoint;
        private bool pointL;

        // Use this for initialization
        void Awake()
        {
            laserBlue = this.GetComponent<LineRenderer>();
            pointL = false;
        }

        // Update is called once per frame
        void Update()
        {
            m_laserpoint = transform.root.gameObject;
            pointL = Shooter.GetComponent<laserpointer>().AttackLaser;

            laserBlue.SetPosition(0, transform.position);

            //Ray ray = new Ray(transform.position, Shooter.gameObject.transform.forward);

            //レーザーの太さ
            laserBlue.SetWidth(laserRadius, laserRadius);

            //レーザー縮む
            if (laserRadius >= 0)
            {
                laserRadius -= laserAttackTime;
            }

            if (pointL == true)
            {
                var laserhitPoint = Physics.SphereCast(
                    transform.position, radius, Shooter.gameObject.transform.forward, out hit, 100);

                laserBlue.enabled = true;

                if (radius >= 0)
                {
                    radius -= radiusDecision;
                }

                if(laserhitPoint)
                {
                    if(hit.collider.tag != "Enemy")
                    {
                        laserBlue.SetPosition(1, hit.point);
                    }
                }
                else
                {
                    laserBlue.SetPosition(1, transform.forward * 400);
                }

                if(laserhitPoint)
                {
                    if(hit.collider.tag == "Player")
                    {
                        SendPlayer.GetComponent<Player>().OnDamage();
                    }
                }
            }
            if (pointL == false)
                laserBlue.enabled = false;
        }

        public void DrawEnd()
        {
            laserBlue.enabled = false;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + transform.forward * (hit.distance), radius);
        }
    }

}
