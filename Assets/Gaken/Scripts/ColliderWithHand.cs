using UnityEngine;
using System.Collections;

public class ColliderWithHand : MonoBehaviour
{
    Animator m_Animator;
    bool m_IsEnter;
    float m_WaitTime;
    Rigidbody m_Rigidbody;

    // Use this for initialization
    void Start()
    {
        m_Animator = transform.root.Find("EnemyRobot").GetComponent<Animator>();
        m_Rigidbody = transform.root.GetComponent<Rigidbody>();
        m_IsEnter = false;
        m_WaitTime = transform.root.GetComponent<Gaken.EnemyController>().m_WaitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_WaitTime <= 0)
            m_WaitTime = 0;
        //Debug.Log(m_WaitCounter);

        if (m_IsEnter)
        {
            m_WaitTime -= 1f * Time.deltaTime;
            m_Animator.speed -= 1f * Time.deltaTime;

            if(m_Animator.speed <= 0.01f)
            {
                m_Animator.SetBool("IsAttack", false);
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }

            if (m_WaitTime <= 0 && m_Animator.speed <= 0.01f)
            {
                m_Animator.SetBool("IsAttack", true);
                m_IsEnter = false;
            }
        }

        if (!m_IsEnter && m_WaitTime <= 0)
        {
            m_Animator.speed = 1;
            m_WaitTime = transform.root.GetComponent<Gaken.EnemyController>().m_WaitTimeCount;
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Field")
        {
            //Debug.Log("ok");
            m_IsEnter = true;
            //parent.SendMessage();
        }
    }
}
