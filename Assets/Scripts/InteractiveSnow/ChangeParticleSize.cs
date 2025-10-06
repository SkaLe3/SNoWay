using UnityEngine;

public class ChangeParticleSize : MonoBehaviour
{
    public float NewSize = 2.0f;
    public float StartSize = 2.0f;
    
    private ParticleSystem m_Ps;

    void Start()
    {
        if (m_Ps == null)
            m_Ps = GetComponent<ParticleSystem>();
    }
    
    void Update()
    {
        var main = m_Ps.main;
        main.startSize = new ParticleSystem.MinMaxCurve(NewSize, NewSize * 1.5f);
    }

}
