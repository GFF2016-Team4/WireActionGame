using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField]
        private Camera playerCamera;

        [SerializeField]
        private float moveSpeed;

        [SerializeField]
        private float airMoveSpeed;

        [SerializeField, Tooltip("ジャンプの強さ")]
        private float jumpPower;

        [SerializeField, Range(0, 1), Header("上方向の減衰値")]
        private float upVecDampingPow;

        [SerializeField, Range(0, 1), Header("横方向の減衰値")]
        private float sideVecDampingPow;

        [SerializeField]
        private float ropeAcceleration;

        [SerializeField, Header("ロープの縮めるスピード")]
        private float ropeTakeUpSpeed;

        [SerializeField, Header("ロープの伸ばすスピード")]
        private float ropeTakeDownSpeed;

        [SerializeField, Header("ロープの伸縮時に加える力")]
        private float ropeTakeForce;

        [SerializeField, Header("ロープの加える力")]
        private float ropeForcePower;

        [SerializeField]
        private Transform footOrigin;
    
    void Awake()
    {
        
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}