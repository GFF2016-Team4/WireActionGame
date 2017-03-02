using UnityEngine;
using System.Collections;

namespace Enemy
{
    public class laserpointer : MonoBehaviour
    {
        public GameObject Shooter;
        public GameObject laserBlue;
        private GameObject m_LaserAttack;
        public GameObject Charge_P;
        public GameObject Charge_L;
        public GameObject Charge_C;

        public float PointLaserTime;
        public float AttackLaserTime;
        private float pointTime;
        private float AttackTime;

        [System.NonSerialized]
        public bool AttackLaser;

        LineRenderer laser;
        laserAttack m_laserBlue;

        public float speed = 1.0f;

        void Start()
        {
            laser = this.GetComponent<LineRenderer>();
            m_laserBlue = laserBlue.GetComponent<laserAttack>();
            AttackLaser = false;
        }

        public void Update()
        {
            RaycastHit hit;
            
            //Debug.DrawRay(ray.origin, ray.direction, Color.blue, 1f);

            //transform.Rotate(0, Input.GetAxis("Horizontal") * speed, 0);

            m_LaserAttack = transform.root.gameObject;

            //レーザーポイントの発動処理
            var lineRenderer = GetComponent<LineRenderer>();
            if (m_LaserAttack.GetComponent<EnemyPattern>().Laser == true && lineRenderer.enabled == false && AttackLaser == false)
            {
                lineRenderer.enabled = true;

                //チャージ
                Charge_P.SetActive(true);
                //Charge_C.SetActive(true);
                SoundManager.Instance.PlaySE(AUDIO.SE_charge);
            }
            if (lineRenderer.enabled == true)
            {
                pointTime += Time.deltaTime;
                Ray ray = new Ray(transform.position, Shooter.transform.forward);

                //レーザーポイントの処理
                bool RayHit = Physics.Raycast(ray, out hit);
                laser.SetPosition(0, transform.position);
                if(RayHit)
                {
                    if (hit.collider.tag != "Enemy")
                    {
                        laser.SetPosition(1, hit.point);
                    }
                    else
                    {
                        laser.SetPosition(1, transform.forward * 400);
                    }
                }
            }

            //ポイントレーザーからレーザーアタック
            if (pointTime >= PointLaserTime)
            {
                //ポイントレーザー
                pointTime = 0.0f;
                lineRenderer.enabled = false;

                //チャージ
                Charge_C.SetActive(false);
                SoundManager.Instance.StopSE();

                //レーザーアタック
                //Laser.SetActive(true);
                AttackLaser = true;
                Charge_L.SetActive(true);
                SoundManager.Instance.PlaySE(AUDIO.SE_beamFire);

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
                //Laser.SetActive(false);
                m_laserBlue.DrawEnd();
                m_LaserAttack.GetComponent<EnemyPattern>().Laser = false;
                SoundManager.Instance.StopSE();
                Charge_L.SetActive(false);

                m_LaserAttack.GetComponent<EnemyPattern>().counter = false;
            }
            if (AttackTime >= AttackLaserTime - 0.5)
            {
                Charge_C.SetActive(true);
                Charge_P.SetActive(false);
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
