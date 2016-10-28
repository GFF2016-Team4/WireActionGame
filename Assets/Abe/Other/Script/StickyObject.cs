using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class StickyObject : MonoBehaviour
{
    [SerializeField, Tooltip("くっつきたいオブジェクト")]
    public Transform target = null;

    [SerializeField, Tooltip("速度")]
    float moveAcceleration = 1.0f;

    Rigidbody body;

    public float Distance
    {
        get
        {
            return transform.position.Distance(target.position);
        }
    }

    void Awake()
    {
        body = transform.GetComponent<Rigidbody>();
    }

    void Start()
    {
        //正常にくっつくように
        body.useGravity = false;
    }

    public void FixedUpdate()
    {
        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        body.AddForce(moveAcceleration * direction, ForceMode.Acceleration);
    }
}