using UnityEngine;
using System.Collections;

namespace Enemy
{
    public class RightCollideWithRope : MonoBehaviour
    {
        public Transform enemy;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Rope/Normal")
            {
                enemy.GetComponent<Animator>().SetBool("IsRightKnee", true);
                Debug.Log("Right");
            }
        }

    }

}
