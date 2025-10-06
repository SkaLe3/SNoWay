using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image HealthbarSprite;
    [SerializeField] private float ReduceSpeed = 2;
    [SerializeField] private bool bRotate = true;
    private float m_Target = 1;



    private Camera m_Cam;

    void Start()
    {
        m_Cam = Camera.main;
    }

    public void UpdateHealthbar(float healthPercent)
    {
        m_Target = healthPercent;
    }

    void Update()
    {
        if (bRotate)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - m_Cam.transform.position);
        }
        HealthbarSprite.fillAmount = Mathf.MoveTowards(HealthbarSprite.fillAmount, m_Target, ReduceSpeed * Time.deltaTime);
    }
}
