using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestMovingObstacle : MonoBehaviour
{
    [SerializeField, Tooltip("説明文")]
    float speed;

    [SerializeField]
    float movingDistance;

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
        transform.position = transform.right * Mathf.Sin(deg * Mathf.Deg2Rad) * movingDistance + origin;
    }
}