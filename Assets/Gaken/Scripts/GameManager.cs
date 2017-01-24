using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    bool isClear;
    public float clearHeight = 5f;

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
            //    if (GameObject.FindGameObjectWithTag("Point").transform.position.y <= clearHeight)
            //    {
            //        isClear = true;
            //        Debug.Log("GameClear");
            //    }
        }

    }
}
