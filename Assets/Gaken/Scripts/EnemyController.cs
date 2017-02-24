using UnityEngine;
using System.Collections;

namespace Gaken
{
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent m_Agent;

        public Transform m_Destination;
        public Transform m_Player;
        public GameObject m_Body;
        public GameObject m_LeftArm;
        public GameObject m_RightArm;
        public GameObject m_Dynamite;
        public Camera m_Camera;

        public float m_Speed = 12f;                     // 前進速度（前進はプラス、後退はマイナス）
        public float m_LazerCoolDown = 10f;
        public float m_CountDown = 5.0f;
        public float m_WaitTime = 2.0f;
        public float m_WaitTimeCount = 2f;

        private float m_ExplosionDelay = 1.5f;
        private float m_EmissionPlus = 0;
        private float m_DisappearTime = 1.0f;
        private float m_RotateSpeed = 0.2f;

        private int m_DeathCount = 0;

        private bool m_IsDisappear = false;
        private bool m_IsExplosion = false;
        private bool m_IsDead = false;            //死亡切替を行うか?
        private bool 
            m_EnemyForward = false, 
            m_EnemyLeft = false, 
            m_EnemyRight = false, 
            m_EnemyBack = false;

        private CharacterController m_Controller;    //キャラクタコントローラ
        private Rigidbody m_Rigidbody;
        private Transform m_Transform;
        private Animator m_Animator;                 //アニメター

        private Renderer m_BodyRender;
        private Renderer m_LeftArmRender;
        private Renderer m_RightArmRender;
        /************************************************************************
                                      仮宣言 
        ************************************************************************/
        //GameObject m_Player;

        void Start()
        {
            //初期化
            //m_IsExplosion = false;
            //m_ExplosionDelay = 1.5f;
            //m_EmissionPlus = 0;
            //m_IsDead = false;

            m_Controller = transform.GetComponent<CharacterController>();
            m_Agent = transform.GetComponent<NavMeshAgent>();
            m_Rigidbody = transform.GetComponent<Rigidbody>();
            m_Transform = transform.GetComponent<Transform>();

            m_Animator = transform.Find("Enemy3").GetComponent<Animator>();

            m_BodyRender = m_Body.transform.GetComponent<Renderer>();
            m_LeftArmRender = m_LeftArm.transform.GetComponent<Renderer>();
            m_RightArmRender = m_RightArm.transform.GetComponent<Renderer>();

            m_Agent.speed = m_Speed = 12f;
            /************************************************************************
                                        仮初期化 
            ************************************************************************/
            //右腕の取得（使えるかどうか確定できていない）
            //leftHand = transform.GetComponent<Transform>();
            //player = transform.Find("Player").GetComponent<Transform>();

            //m_Lazer = transform.GetComponent<GameObject>();
            //time2 += 1;
            //ropeCounter = 0;

        }

        void Update()
        {
            //エネミーを即死させる
            if (Input.GetKey(KeyCode.Z))
            {
                m_IsDead = true;
            }

            //死亡交代
            if (m_IsDead)
            {
                m_Animator.SetBool("IsDead", true);
                m_Agent.enabled = false;
                m_EmissionPlus += 0.1f * Time.deltaTime;

                Mathf.Clamp(m_EmissionPlus, 0, 0.8f);

                m_BodyRender.material.SetColor("_EmissionColor", new Color(m_EmissionPlus, m_EmissionPlus, m_EmissionPlus));
                m_LeftArmRender.material.SetColor("_EmissionColor", new Color(m_EmissionPlus, m_EmissionPlus, m_EmissionPlus));
                m_RightArmRender.material.SetColor("_EmissionColor", new Color(m_EmissionPlus, m_EmissionPlus, m_EmissionPlus));

                if(m_EmissionPlus >= 0.8f)
                {
                    m_Dynamite.transform.gameObject.SetActive(true);
                    m_IsDisappear = true;
                }

                //Debug.Log(body.material.GetColor("_EmissionColor"));
                //m_Controller.enabled = false;
                Debug.Log("Clear");
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                //transform.GetComponent<CapsuleCollider>().enabled = false;
            }

            m_Agent.speed = m_Speed;
            m_Agent.destination = m_Destination.position;

            m_Animator.SetFloat("Speed", m_Agent.speed);
            /****************************************************************
                                        仮更新
            ****************************************************************/
            m_LazerCoolDown -= 1f * Time.deltaTime;
            m_WaitTime -= 1f * Time.deltaTime;
            if(m_IsExplosion)
            {
                m_ExplosionDelay -= 1f * Time.deltaTime;
                //Debug.Log(m_ExplosionDelay);
            }

            //攻撃
            if (m_Animator.GetBool("IsAttack"))
            {
                EnemyAttack();
            }
            else if (m_Animator.GetBool("IsLazer") && m_LazerCoolDown <= 0)
            {
                EnemyLazer();
                //agent.enabled = false;
            }
            else
            {
                EnemyNormal();
            }

            if (m_Animator.GetBool("IsKnee"))
            {
                m_DeathCount += CircleCount();
                if (m_DeathCount >= 4)
                {
                    m_IsDead = true;
                }
            }

            //自爆
            if (m_Animator.GetBool("IsExplosion"))
            {

                m_Camera.transform.root.gameObject.SetActive(true);

                if (m_CountDown <= 0)
                    EnemyExplosion();


                if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion"))
                {
                    m_Agent.speed = 0;
                    m_Camera.depth = 2;
                    m_IsExplosion = true;
                }
                m_CountDown -= 1 * Time.deltaTime;
            }

            if(m_IsDisappear)
            {
                m_DisappearTime -= 1f * Time.deltaTime;
            }
            if(m_DisappearTime <= 0)
            {
                Destroy(gameObject);
            }
        }

        //死んでいますか?
        public bool IsDead()
        {
            return m_IsDead;
        }

        //トリガーに入ると同時に
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Destination")
            {
                m_Animator.SetBool("IsAttack", false);
                m_Animator.SetBool("IsLazer", false);

                m_Animator.SetBool("IsExplosion", true);
            }
        }

        //トリガーに出ると同時に
        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Rope/Normal")
            {
                m_Animator.SetBool("IsKnee", false);
            }
        }

        //トリガに入っているときに
        public void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Rope/Normal")
            {
                m_Animator.SetBool("IsKnee", true);
            }
        }

        void RotateToPlayer(Quaternion offset)
        {
            //Debug.DrawLine(target.transform.position, this.transform.position, Color.yellow);
            m_Transform.rotation = Quaternion.Slerp(
                m_Transform.rotation,
                Quaternion.LookRotation(m_Player.transform.position - m_Transform.transform.position) * offset,
                m_RotateSpeed * Time.deltaTime);
        }
        void RotateToPlayer()
        {
            //Debug.DrawLine(target.transform.position, this.transform.position, Color.yellow);
            m_Transform.rotation = Quaternion.Slerp(
                m_Transform.rotation,
                Quaternion.LookRotation(m_Player.transform.position - m_Transform.transform.position),
                m_RotateSpeed * Time.deltaTime);
        }

        void EnemyAttack()
        {
            //m_Animator.SetBool("IsAttack", false);
            //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            m_Agent.speed = 0;
            RotateToPlayer(Quaternion.AngleAxis(-25, Vector3.up));
        }

        void EnemyLazer()
        {
            m_Agent.speed = 0;
            if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Lazer"))
            {
                RotateToPlayer();
            }
            else
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        void EnemyNormal()
        {
            //m_Animator.SetBool("IsAttack", false);
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                m_Agent.speed = 0;
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Lazer") || m_Animator.GetCurrentAnimatorStateInfo(0).IsName("LazerOver"))
            {
                m_Agent.speed = 0;
            }
            else
            {
                m_Agent.speed = m_Speed;
            }
        }

        void EnemyExplosion()
        {
            if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion"))
            {
                if(m_ExplosionDelay <= 0)
                {
                    m_Dynamite.transform.gameObject.SetActive(true);
                    m_IsDisappear = true;
                }
            }
        }

        int CircleCount()
        {
            Vector3 relative = transform.InverseTransformPoint(m_Player.position);
            float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

            if (angle < 45 && angle > -45)
            {
                m_EnemyForward = true;
            }
            if (angle > 45 && angle < 135)
            {
                m_EnemyRight = true;
            }
            if ((angle > 135 && angle < 180) || (angle > -180 && angle < -135))
            {
                m_EnemyBack = true;
            }
            if (angle > -45 && angle < -135)
            {
                m_EnemyLeft = true;
            }
            int cnt = 0;

            //Debug.Log(angle);

            //    //時計回り
            //    //前から
            if (angle < 45 && angle > -45) m_EnemyForward = true;
            if (m_EnemyForward && angle > 45 && angle < 135) m_EnemyRight = true;
            if (m_EnemyForward && m_EnemyRight && angle > 135 && angle < -135) m_EnemyBack = true;
            if (m_EnemyForward && m_EnemyRight && m_EnemyBack && angle > -135 && angle < -45) m_EnemyLeft = true;

            //    //右から
            if (angle > 45 && angle < 135) m_EnemyRight = true;
            if (m_EnemyRight && angle > 135 && angle < -135) m_EnemyBack = true;
            if (m_EnemyRight && m_EnemyBack && angle > -135 && angle < -45) m_EnemyLeft = true;
            if (m_EnemyRight && m_EnemyBack && m_EnemyLeft && angle > -45 && angle < 45) m_EnemyForward = true;

            //    //後ろから
            if (angle > 135 && angle < -135) m_EnemyBack = true;
            if (m_EnemyBack && angle > -135 && angle < -45) m_EnemyLeft = true;
            if (m_EnemyBack && m_EnemyLeft && angle > -45 && angle < 45) m_EnemyForward = true;
            if (m_EnemyBack && m_EnemyLeft && m_EnemyForward && angle > 45 && angle < 135) m_EnemyRight = true;

            //    //左から
            if (angle > 45 && angle < 135) m_EnemyLeft = true;
            if (m_EnemyLeft && angle > 135 && angle < -135) m_EnemyForward = true;
            if (m_EnemyLeft && m_EnemyForward && angle > -135 && angle < -45) m_EnemyRight = true;
            if (m_EnemyLeft && m_EnemyForward && m_EnemyRight && angle > -45 && angle < 45) m_EnemyBack = true;


            //    //反時計回り
            //    //前から
            if (angle > -45 && angle < 45) m_EnemyForward = true;
            if (m_EnemyForward && angle > -135 && angle < -45) m_EnemyLeft = true;
            if (m_EnemyForward && m_EnemyLeft && angle > 135 && angle < -135) m_EnemyBack = true;
            if (m_EnemyForward && m_EnemyLeft && m_EnemyBack && angle > 45 && angle < 135) m_EnemyRight = true;

            //    //右から
            if (angle > 45 && angle < 135) m_EnemyRight = true;
            if (m_EnemyRight && angle > 135 && angle < -135) m_EnemyBack = true;
            if (m_EnemyRight && m_EnemyBack && angle > -135 && angle < -45) m_EnemyLeft = true;
            if (m_EnemyRight && m_EnemyBack && m_EnemyLeft && angle > -45 && angle < 45) m_EnemyForward = true;

            //    //後ろから
            if (angle > 135 && angle < -135) m_EnemyBack = true;
            if (m_EnemyBack && angle > 45 && angle < 135) m_EnemyRight = true;
            if (m_EnemyBack && m_EnemyRight && angle > -45 && angle < 45) m_EnemyForward = true;
            if (m_EnemyBack && m_EnemyRight && m_EnemyForward && angle > -135 && angle < -45) m_EnemyLeft = true;

            //    //左から
            if (angle > -135 && angle < -45) m_EnemyLeft = true;
            if (m_EnemyLeft && angle > 135 && angle < -135) m_EnemyBack = true;
            if (m_EnemyLeft && m_EnemyBack && angle > 45 && angle < 135) m_EnemyRight = true;
            if (m_EnemyLeft && m_EnemyBack && m_EnemyRight && angle > -45 && angle < 45) m_EnemyForward = true;

            //Debug.Log(EnemyForward);
            //Debug.Log(EnemyBack);
            //Debug.Log(EnemyLeft);
            //Debug.Log(EnemyRight);

            if (m_EnemyForward && m_EnemyLeft && m_EnemyRight && m_EnemyBack)
            {
                cnt++;

                m_EnemyForward = false;
                m_EnemyLeft = false;
                m_EnemyRight = false;
                m_EnemyBack = false;
            }

            return cnt;
        }
    }
}

