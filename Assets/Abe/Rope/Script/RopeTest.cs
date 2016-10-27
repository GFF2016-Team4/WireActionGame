using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeTest : MonoBehaviour
{
    [SerializeField, Tooltip("説明文")]
    GameObject ropeParent;
    
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            ropeParent.GetComponent<RopeSimulate>().SubRopeLength(1);
        }

        if(Input.GetKey(KeyCode.B))
        {
            ropeParent.GetComponent<RopeSimulate>().AddRopeLength(1);
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            ropeParent.GetComponent<RopeSimulate>().RopeEnd();
        }
    }
}