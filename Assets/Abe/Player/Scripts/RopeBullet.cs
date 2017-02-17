using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeBullet : MonoBehaviour
{
    private Collision collisionInfo = null;

    private LineRenderer lineRenderer;
    public  Transform target;
    private Vector3[] positions = new Vector3[2];
    List<GameObject> firstCollisions = new List<GameObject>();
    Rigidbody rig;

    public float speed;
    public Vector3 direction;

    public bool IsHit
    {
        get
        {
            return collisionInfo != null;
        }
    }

    public bool IsHitEnemy
    {
        get
        {
            return collisionInfo.gameObject.tag == "Enemy";
        }
    }

    public Vector3 HitPosition
    {
        get
        {
            return collisionInfo.contacts[0].point;
        }
    }

    public Collision HitInfo
    {
        get
        {
            return collisionInfo;
        }
    }

    public float Distance
    {
        get
        {
            return Vector3.Distance(transform.position, target.position);
        }
    }

    void Awake()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        Collider[]     cols     = Physics.OverlapSphere(transform.position, collider.radius);
        rig = GetComponent<Rigidbody>();
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetVertexCount(2);
        SoundManager.Instance.PlaySE(AUDIO.SE_ropeFire);
    }

    void FixedUpdate()
    {
        rig.velocity = direction * speed;
    }

    void LateUpdate()
    {
        positions[0] = transform.position;
        positions[1] = target.position;

        lineRenderer.SetPositions(positions);
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionInfo = collision;

        SoundManager.Instance.PlaySE(AUDIO.SE_ropeHit);
    }
}
