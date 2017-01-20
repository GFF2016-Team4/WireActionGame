using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    Camera m_Camera;
    Animator m_Animator;

    bool IsChange = false;

	// Use this for initialization
	void Start () {
        m_Camera = transform.GetComponent<Camera>();
        m_Animator = transform.Find("EnemyRobot").GetComponent<Animator>();

        IsChange = false;

        IsChange = m_Animator.GetBool("IsExplosion");
    }
	
	// Update is called once per frame
	void Update () {
        if (IsChange)
        {
            m_Camera.depth = 2;
        }
	}
}
