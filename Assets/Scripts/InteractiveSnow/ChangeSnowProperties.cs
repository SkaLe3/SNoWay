using Unity.VisualScripting;
using UnityEngine;

public class ChangeSnowProperties : MonoBehaviour
{

    private Material m_SnowMaterial;

    public float NewHeight = 0.07f;
    public float StartHeight = 0.07f;


    private float m_LastHeight = 0.07f;
    void Start()
    {
        var renderer = GetComponent<Renderer>();
        m_SnowMaterial = renderer.material;
        NewHeight = StartHeight = m_LastHeight = 0.07f;
    }

    void Update()
    {
        if (m_LastHeight != NewHeight)
        {
            m_SnowMaterial.SetFloat("_SnowHeight", NewHeight);
            m_LastHeight = NewHeight;
        }
        
    }
}
