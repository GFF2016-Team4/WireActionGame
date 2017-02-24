using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class reticlemove : MonoBehaviour
{

    public GameObject img;
    [Header("Rayを飛ばすカメラ")]
    public GameObject rayCamera;

    public float radius;
    public float direction;

    RectTransform m_rectTransform;
    changeColor m_changeColor;
    reticleRay m_reticleRay;
    public Sprite orgTex;
    public Sprite chgTex;

    void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();

    }

    void Start()
    {
        m_changeColor = GetComponent<changeColor>();
        m_reticleRay = rayCamera.AddComponent<reticleRay>();
        m_reticleRay.target = gameObject.GetComponent<RectTransform>();
        m_reticleRay.radius = radius;
        m_reticleRay.direction = direction;
    }

    void Update()
    {

        if (m_reticleRay.isShpereHit())
        {
            m_rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, m_reticleRay.target.position);
            img.GetComponent<Image>().sprite = orgTex;

            iTween.MoveTo(img, iTween.Hash("position", m_rectTransform.position,
                                           "time", 0.3f));
            m_changeColor.chgColor();
        }
        else
        {
            //rectTransform.localPosition = new Vector2(0, 0);
            img.GetComponent<Image>().sprite = chgTex;

            iTween.MoveTo(img, iTween.Hash("x", 0f,
                                           "y", 0f,
                                           "time", 0.3f,
                                           "isLocal", true));

            m_changeColor.orgColor();
        }
    }
}