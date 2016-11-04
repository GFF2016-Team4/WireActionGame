using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Player
{
    public class PlayerRopeMove : MonoBehaviour
    {
        [SerializeField, Tooltip("説明文")]
        GameObject rope;

        void Awake()
        {
            enabled = false;
        }
    
        void Start()
        {
        
        }
    
        void Update()
        {
            Debug.Log("RopeMove");
        }
    }
}