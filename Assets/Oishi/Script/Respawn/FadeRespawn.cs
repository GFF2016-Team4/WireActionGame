using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class FadeRespawn : MonoBehaviour
{

    public Image panel;
    public GameObject respawnJudge;
    public GameObject respawnPosition;

    public float animationTImer;
    private float animationTimerTemp;

    private bool fadeIn = false;
    private bool fadeOut = false;

    CharacterController m_characterController;
    FadeManager m_fadeManager;

    void Start()
    {
        animationTimerTemp = animationTImer;
        m_characterController = GetComponent<CharacterController>();
        m_fadeManager = panel.GetComponent<FadeManager>();

    }

    void Update()
    {
        if (fadeIn == false && fadeOut == false)
            m_characterController.Move(new Vector3(0, -0.1f, 0));

        if (fadeOut == true)
        {
            m_fadeManager.FadeOut("", 1);
            if (m_fadeManager.alfaOut >= 1.0f)
            {
                gameObject.transform.position = respawnPosition.transform.position;
                fadeOut = false;
                fadeIn = true;
                m_fadeManager.alfaOut = 0.0f;
            }
        }
        if (fadeIn == true)
        {
            m_fadeManager.FadeIn(1);
            if (m_fadeManager.alfaIn <= 0)
            {
                fadeIn = false;
                respawnJudge.SetActive(true);
                m_fadeManager.alfaIn = 1.0f;
                animationTImer = animationTimerTemp;
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //animationTImer -= Time.deltaTime;
        //if (respawnJudge != null)
        //{
        //    if (animationTImer <= 0)
        //        fadeOut = true;
        //}
        //respawnJudge.SetActive(false);
    }
}
