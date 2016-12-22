using UnityEngine;

public class StickyObject : MonoBehaviour
{
    [SerializeField, Tooltip("くっつきたいオブジェクト")]
    public Transform target = null;

    [SerializeField, Tooltip("速度")]
    public float moveAcceleration = 1.0f;

    Rigidbody body;

    public float Distance
    {
        get
        {
            Debug.Log(transform.position.Distance(target.position));
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
        float distance = transform.position.Distance(target.position);

        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        body.AddForce(moveAcceleration * distance / 10 * direction, ForceMode.Force);
    }
}