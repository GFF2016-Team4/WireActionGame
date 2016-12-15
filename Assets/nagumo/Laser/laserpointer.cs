using UnityEngine;
using System.Collections;

namespace Enemy
{
    public class laserpointer : MonoBehaviour
    {

        public GameObject Laser;
        public GameObject Shooter;
        public GameObject laserBlue;
        public float PointLaserTime;
        public float AttackLaserTime;
        private float pointTime;
        private float AttackTime;
        private bool AttackLaser;
        LineRenderer laser;
        laserAttack m_laserBlue;


        public float speed = 1.0f;
        void Start()
        {
            laser = this.GetComponent<LineRenderer>();
            m_laserBlue = laserBlue.GetComponent<laserAttack>();
        }

        void Update()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Shooter.transform.forward);
            //Debug.DrawRay(ray.origin, ray.direction, Color.blue, 1f);

            transform.Rotate(0, Input.GetAxis("Horizontal") * speed, 0);

            //レーザーポイントの発動処理
            var lineRenderer = GetComponent<LineRenderer>();
            if (Input.GetKeyDown(KeyCode.Space) && lineRenderer.enabled == false && AttackLaser == false)
            {
                lineRenderer.enabled = true;
            }
            if (lineRenderer.enabled == true)
            {
                pointTime += Time.deltaTime;
            }
            if (pointTime >= PointLaserTime)
            {
                pointTime = 0.0f;
                lineRenderer.enabled = false;

                Laser.SetActive(true);
                AttackLaser = true;

                m_laserBlue.laserRadius = 10;
                m_laserBlue.radius = 1.0f;
                m_laserBlue.laserHP = 2;
            }

            //レーザー攻撃の攻撃タイミング
            if (AttackLaser == true)
            {
                AttackTime += Time.deltaTime;

            }
            if (AttackTime >= AttackLaserTime)
            {
                AttackTime = 0.0f;
                AttackLaser = false;
                Laser.SetActive(false);
            }

            //レーザーポイントの処理
            laser.SetPosition(0, transform.position);
            if (Physics.Raycast(ray, out hit))
            {
                laser.SetPosition(1, hit.point);
                Debug.Log(hit.transform.gameObject.name);
            }
            else
            {
                laser.SetPosition(1, transform.forward * 100);
            }
        }
        //void FiringBeam(GameObject[] obj)
        //{
        //    var lineRenderer = GetComponent<LineRenderer>();
        //    lineRenderer.SetPosition(0, obj[0].transform.position);
        //    lineRenderer.SetPosition(1, obj[1].transform.position);
        //}
    }

}
