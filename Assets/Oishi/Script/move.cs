using UnityEngine;
using System.Collections;

public class move : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = new Vector3(0, 0, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //SoundManager.Instance.PlaySE(AUDIO.SE_WALK);
        Debug.Log("当たった");
    }
}
