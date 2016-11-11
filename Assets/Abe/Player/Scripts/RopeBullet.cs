using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeBullet : MonoBehaviour
{
    private Collision collisionInfo = null;

    private LineRenderer lineRenderer;
    public  Transform target;
    private Vector3[] positions = new Vector3[2];

    public bool IsCollision
    {
        get
        {
            return collisionInfo != null;
        }
    }

    public Collision CollisionInfo
    {
        get
        {
            return collisionInfo;
        }
    }

    public void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetVertexCount(2);
    }

    public void LateUpdate()
    {
        
        positions[0] = transform.position;
        positions[1] = target.position;

        lineRenderer.SetPositions(positions);
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionInfo = collision;
    }

    void OnCollisionExit(Collision collision)
    {
        collisionInfo = null;
    }
}