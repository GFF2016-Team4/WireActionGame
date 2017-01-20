using UnityEngine;
using System.Collections;

namespace Enemy
{
    public class laserpointer : MonoBehaviour
    {

        public GameObject Laser;
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
        }

        public void Update()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Shooter.transform.forward);
            //Debug.DrawRay(ray.origin, ray.direction, Color.blue, 1f);

            transform.Rotate(0, Input.GetAxis("Horizontal") * speed, 0);

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
                Laser.SetActive(true);
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
                Laser.SetActive(false);
                m_LaserAttack.GetComponent<EnemyPattern>().Laser = false;
                SoundManager.Instance.StopSE();
                
                Charge_L.SetActive(false);
            }
            if(AttackTime >= AttackLaserTime - 0.5)
            {
                Charge_C.SetActive(true);
                Charge_P.SetActive(false);
            }

            //レーザーポイントの処理
            laser.SetPosition(0, transform.position);
            if (Physics.Raycast(ray, out hit))
            {
                laser.SetPosition(1, hit.point);
                //Debug.Log(hit.transform.gameObject.name);
            }
            else
            {
                laser.SetPosition(1, transform.forward * 400);
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
