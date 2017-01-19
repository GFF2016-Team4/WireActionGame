using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestMovingObstacle : MonoBehaviour
{
    [SerializeField, Tooltip("説明文")]
    float speed;

    [SerializeField]
    float movingDistance;

    [SerializeField]
    Vector3 movingAxis;

    [SerializeField, Tooltip("説明文")]
    Vector3 rotateAxis;

    [SerializeField, Tooltip("説明文")]
    float   rotateSpeed;

    float deg;
    Vector3 origin;
    

    void Awake()
    {
        origin = transform.position;
    }
    
    void Start()
    {
        deg = 0;
    }
    
    void Update()
    {
        deg += Time.deltaTime * speed;
        transform.position = movingAxis * Mathf.Sin(deg * Mathf.Deg2Rad) * movingDistance + origin;
        transform.Rotate(rotateAxis, rotateSpeed);
    }
}