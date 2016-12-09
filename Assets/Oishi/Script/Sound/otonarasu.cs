using UnityEngine;
using System.Collections;

public class otonarasu : MonoBehaviour 
{
	void Start ()
	{
        //BGMの再生
        SoundManager.Instance.PlayBGM(AUDIO.BGM_TITLE);
    }

	void Update () 
	{
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //SEの再生
            SoundManager.Instance.PlaySE(AUDIO.SE_WALK);
        }
	}

}
