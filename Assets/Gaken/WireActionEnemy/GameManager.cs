using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    bool isClear;

    // Use this for initialization
    void Start()
    {
        isClear = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isClear)
        {
            if (GameObject.FindGameObjectWithTag("Point").transform.position.y <= 2f)
            {
                isClear = true;
                Debug.Log("GameClear");
            }
        }

    }
}
