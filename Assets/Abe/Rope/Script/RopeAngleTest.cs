using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeAngleTest : MonoBehaviour
{
    [SerializeField, Tooltip("説明文")]
    float distance = 0;

    Vector3 v;

    [SerializeField]
    Transform origin, middle, tail;

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        Vector3 origin2tail   = tail.position   - origin.position;
        Vector3 origin2middle = middle.position - origin.position;

        Vector3 normal = origin2middle.normalized;
        float dot = Vector3.Dot(origin2tail, normal);
        Vector3 height = -origin2tail + normal * dot;

        distance = height.magnitude;

        v = height.normalized;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(tail.position, tail.position + v * distance);
    }
}