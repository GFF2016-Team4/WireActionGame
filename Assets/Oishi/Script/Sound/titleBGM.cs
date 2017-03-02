using UnityEngine;
using System.Collections;

public class titleBGM : MonoBehaviour
{
    void Start()
    {
        Resources.UnloadUnusedAssets();
        SoundManager.Instance.PlayBGM(AUDIO.BGM_Title);
    }
}
