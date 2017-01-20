using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{

    //private Transform respawnPosition;
    public GameObject resObj;
    bool respawn = false;

    void Start()
    {
        //respawnPosition.position = gameObject.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            respawn = true;
        }
        if (respawn == true) resObj.GetComponent<FadeRespawn>().Respawn();
    }

}
