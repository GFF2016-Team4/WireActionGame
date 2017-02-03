using UnityEngine;
using System.Collections;

public class PlayerWalkSound : MonoBehaviour
{
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Field")
        {
            SoundManager.Instance.PlaySE(AUDIO.SE_playerLanding);
        }
    }
}
