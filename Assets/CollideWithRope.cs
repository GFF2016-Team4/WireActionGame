using UnityEngine;
using System.Collections;

namespace Enemy
{
    public class CollideWithRope : MonoBehaviour
    {

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
            int count = 0;

            if (count < 2)
            {
                if (other.gameObject.tag == "NormalRope")
                {
                    count++;
                    Debug.Log(count);
                }
            }
            else
            {
                Debug.Log("...");
            }

        }

    }

}
