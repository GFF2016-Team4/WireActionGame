using UnityEngine;
using System.Collections;

public class LoadAnm : MonoBehaviour
{

    public Animator m_animator;

    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_animator.Stop();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine("Load");
            //m_animator.Play(m_animator.ToString());

            Debug.Log(StartCoroutine("Load"));
        }

    }

    IEnumerator Load()
    {
        m_animator.Play("Oishi/Scene/Load/LoadAnimation");

        yield return null;
    }
}
