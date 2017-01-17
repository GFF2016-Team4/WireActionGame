using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDamage : MonoBehaviour
{
    //[SerializeField, Tooltip("説明文")]
    string damageTag;
    
    void Awake()
    {
        
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == damageTag)
        {

        }
    }
}