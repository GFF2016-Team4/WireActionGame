using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeBullet : MonoBehaviour
{
    private Collision collisionInfo = null;

    public bool IsCollision
    {
        get { return collisionInfo != null; }
    }

    public Collision CollisionInfo
    {
        get { return collisionInfo; }
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