using UnityEngine;
using System.Collections;

public class titleBGM : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlayBGM(AUDIO.BGM_Title);
    }
}
