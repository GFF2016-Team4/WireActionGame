using UnityEngine;
using System.Collections;

public class ColliderWithHand : MonoBehaviour
{
    Animator m_Animator;
    bool m_IsEnter;
    float m_WaitCounter;

    // Use this for initialization
    void Start()
    {
        m_Animator = transform.root.Find("EnemyRobot").GetComponent<Animator>();
        m_IsEnter = false;
        m_WaitCounter = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_WaitCounter <= 0)
            m_WaitCounter = 0;
        //Debug.Log(m_WaitCounter);

        if (m_IsEnter)
        {
            m_WaitCounter -= 1f * Time.deltaTime;

            m_Animator.speed -= 1f * Time.deltaTime;

            if (m_WaitCounter <= 0 && m_Animator.speed <= 0.01f)
                m_IsEnter = false;
        }

        if (!m_IsEnter && m_WaitCounter <= 0)
        {
            m_Animator.speed = 1;
            m_WaitCounter = 2f;
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Field")
        {
            Debug.Log("ok");
            m_IsEnter = true;
            //parent.SendMessage();
        }
    }
}
