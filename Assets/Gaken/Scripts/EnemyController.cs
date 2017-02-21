﻿using UnityEngine;
using System.Collections;

namespace Gaken
{
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent agent;

        public Transform Destination;
        public Transform player;

        public Material material;

        public float m_Speed = 12f;                     // 前進速度（前進はプラス、後退はマイナス）

        public float m_LazerCoolDown = 10f;
        public float m_CountDown = 5.0f;
        public float m_WaitTime = 2.0f;

        public float m_WaitTimeCount = 2f;

        bool m_IsExplosion;
        float m_ExplosionDelay;
        /************************************************************************
                                      仮宣言 
        ************************************************************************/
        CharacterController m_Controller;    //キャラクタコントローラ
        Animator m_Animator;                 //アニメター
        Camera m_Camera;
        public GameObject m_Lazer;
        Rigidbody m_Rigidbody;

        public GameObject body;
        public GameObject leftArm;
        public GameObject rightArm;
        public GameObject dynamite;

        Renderer bodyRender;
        Renderer leftArmRender;
        Renderer rightArmRender;

        float emissionPlus;

        private bool isDead;            //死亡切替を行うか?

        bool EnemyForward = false, EnemyLeft = false, EnemyRight = false, EnemyBack = false;
        int deathCount = 0;

        GameObject target;
        float rotateSpeed = 0.2f;
        Transform m_Transform;

        void Start()
        {
            //初期化
            m_IsExplosion = false;
            m_ExplosionDelay = 1.5f;
            emissionPlus = 0;

            isDead = false;
            m_Controller = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();
            m_Camera = GameObject.Find("EnemyCamera").GetComponent<Camera>();
            m_Rigidbody = transform.GetComponent<Rigidbody>();
            m_Transform = transform.GetComponent<Transform>();

            target = GameObject.FindGameObjectWithTag("Player");
            agent.speed = m_Speed = 12f;

            //m_Lazer = transform.GetComponent<GameObject>();
            //time2 += 1;
            //ropeCounter = 0;

            //アニメターは子のアニメターを取得
            m_Animator = transform.Find("Enemy3").GetComponent<Animator>();             //こっちはfbx形式

            bodyRender = body.transform.GetComponent<Renderer>();
            leftArmRender = leftArm.transform.GetComponent<Renderer>();
            rightArmRender = rightArm.transform.GetComponent<Renderer>();

            /************************************************************************
                                        仮初期化 
            ************************************************************************/
            //右腕の取得（使えるかどうか確定できていない）
            //leftHand = transform.GetComponent<Transform>();
            //player = transform.Find("Player").GetComponent<Transform>();

        }

        void Update()
        {
            Debug.Log(bodyRender.material);

            //エネミーを即死させる
            if (Input.GetKey(KeyCode.Z))
            {
                isDead = true;
            }
            //死亡交代
            if (isDead)
            {
                m_Animator.SetBool("IsDead", true);
                agent.enabled = false;
                emissionPlus += 0.1f * Time.deltaTime;

                Mathf.Clamp(emissionPlus, 0, 0.8f);

                bodyRender.material.SetColor("_EmissionColor", new Color(emissionPlus, emissionPlus, emissionPlus));
                leftArmRender.material.SetColor("_EmissionColor", new Color(emissionPlus, emissionPlus, emissionPlus));
                rightArmRender.material.SetColor("_EmissionColor", new Color(emissionPlus, emissionPlus, emissionPlus));

                if(emissionPlus >= 0.8f)
                {
                    dynamite.transform.gameObject.SetActive(true);
                }

                //Debug.Log(body.material.GetColor("_EmissionColor"));
                m_Controller.enabled = false;
                Debug.Log("Clear");
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                //transform.GetComponent<CapsuleCollider>().enabled = false;
            }

            agent.speed = m_Speed;
            agent.destination = Destination.position;

            m_Animator.SetFloat("Speed", agent.speed);

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
                deathCount += CircleCount();
                if (deathCount >= 4)
                {
                    isDead = true;
                }
            }

            //自爆
            if (m_Animator.GetBool("IsExplosion"))
            {

                if (m_CountDown <= 0)
                    EnemyExplosion();


                if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion"))
                {
                    agent.speed = 0;
                    m_Camera.depth = 2;
                    m_IsExplosion = true;
                }
                m_CountDown -= 1 * Time.deltaTime;

                //if (transform.Find("DYNAMITE").transform.gameObject.active)
                //{
                //    Destroy(gameObject);
                //}
            }
        }

        //死んでいますか?
        public bool IsDead()
        {
            return isDead;
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
                Debug.Log(other.ToString());

                m_Animator.SetBool("IsKnee", true);
            }
        }

        void RotateToPlayer(Quaternion offset)
        {
            //Debug.DrawLine(target.transform.position, this.transform.position, Color.yellow);
            m_Transform.rotation = Quaternion.Slerp(
                m_Transform.rotation,
                Quaternion.LookRotation(target.transform.position - m_Transform.transform.position) * offset,
                rotateSpeed * Time.deltaTime);
        }
        void RotateToPlayer()
        {
            //Debug.DrawLine(target.transform.position, this.transform.position, Color.yellow);
            m_Transform.rotation = Quaternion.Slerp(
                m_Transform.rotation,
                Quaternion.LookRotation(target.transform.position - m_Transform.transform.position),
                rotateSpeed * Time.deltaTime);
        }

        void EnemyAttack()
        {
            //m_Animator.SetBool("IsAttack", false);
            //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            agent.speed = 0;
            RotateToPlayer(Quaternion.AngleAxis(-25, Vector3.up));
        }

        void EnemyLazer()
        {
            agent.speed = 0;
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
            m_Animator.SetBool("IsAttack", false);
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                agent.speed = 0;
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Lazer") || m_Animator.GetCurrentAnimatorStateInfo(0).IsName("LazerOver"))
            {
                agent.speed = 0;
            }
            else
            {
                agent.speed = m_Speed;
            }
        }

        void EnemyExplosion()
        {
            if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion"))
            {
                if(m_ExplosionDelay <= 0)
                {
                    transform.Find("DYNAMITE").transform.gameObject.SetActive(true);
                }
            }
        }

        int CircleCount()
        {
            Vector3 relative = transform.InverseTransformPoint(player.position);
            float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

            if (angle < 45 && angle > -45)
            {
                EnemyForward = true;
            }
            if (angle > 45 && angle < 135)
            {
                EnemyRight = true;
            }
            if ((angle > 135 && angle < 180) || (angle > -180 && angle < -135))
            {
                EnemyBack = true;
            }
            if (angle > -45 && angle < -135)
            {
                EnemyLeft = true;
            }
            int cnt = 0;

            //Debug.Log(angle);


            //    //時計回り
            //    //前から
            if (angle < 45 && angle > -45) EnemyForward = true;
            if (EnemyForward && angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyForward && EnemyRight && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyForward && EnemyRight && EnemyBack && angle > -135 && angle < -45) EnemyLeft = true;

            //    //右から
            if (angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyRight && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyRight && EnemyBack && angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyRight && EnemyBack && EnemyLeft && angle > -45 && angle < 45) EnemyForward = true;

            //    //後ろから
            if (angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyBack && angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyBack && EnemyLeft && angle > -45 && angle < 45) EnemyForward = true;
            if (EnemyBack && EnemyLeft && EnemyForward && angle > 45 && angle < 135) EnemyRight = true;

            //    //左から
            if (angle > 45 && angle < 135) EnemyLeft = true;
            if (EnemyLeft && angle > 135 && angle < -135) EnemyForward = true;
            if (EnemyLeft && EnemyForward && angle > -135 && angle < -45) EnemyRight = true;
            if (EnemyLeft && EnemyForward && EnemyRight && angle > -45 && angle < 45) EnemyBack = true;


            //    //反時計回り
            //    //前から
            if (angle > -45 && angle < 45) EnemyForward = true;
            if (EnemyForward && angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyForward && EnemyLeft && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyForward && EnemyLeft && EnemyBack && angle > 45 && angle < 135) EnemyRight = true;

            //    //右から
            if (angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyRight && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyRight && EnemyBack && angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyRight && EnemyBack && EnemyLeft && angle > -45 && angle < 45) EnemyForward = true;

            //    //後ろから
            if (angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyBack && angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyBack && EnemyRight && angle > -45 && angle < 45) EnemyForward = true;
            if (EnemyBack && EnemyRight && EnemyForward && angle > -135 && angle < -45) EnemyLeft = true;

            //    //左から
            if (angle > -135 && angle < -45) EnemyLeft = true;
            if (EnemyLeft && angle > 135 && angle < -135) EnemyBack = true;
            if (EnemyLeft && EnemyBack && angle > 45 && angle < 135) EnemyRight = true;
            if (EnemyLeft && EnemyBack && EnemyRight && angle > -45 && angle < 45) EnemyForward = true;

            //Debug.Log(EnemyForward);
            //Debug.Log(EnemyBack);
            //Debug.Log(EnemyLeft);
            //Debug.Log(EnemyRight);

            if (EnemyForward && EnemyLeft && EnemyRight && EnemyBack)
            {
                cnt++;

                EnemyForward = false;
                EnemyLeft = false;
                EnemyRight = false;
                EnemyBack = false;
            }

            return cnt;
        }
    }
}

