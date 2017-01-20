using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{


    void Start()
    {
        //respawnPosition.position = gameObject.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<FadeRespawn>().Respawn();
        }
    }

}
