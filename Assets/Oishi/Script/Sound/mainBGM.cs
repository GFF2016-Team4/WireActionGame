using UnityEngine;
using System.Collections;

public class mainBGM : MonoBehaviour 
{
    void Start()
    {
        SoundManager.Instance.PlayBGM(AUDIO.BGM_Stage);
    }
}
