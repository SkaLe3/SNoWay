using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractableHover : MonoBehaviour
{
    public float Radius
    {
        get {return GetComponent<SphereCollider>().radius * transform.localScale.x;}
    }

    private GameObject m_HoverObject;

    public event System.Action<GameObject> HoverEvent;

    void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        Component comp = interactable as Component;
        if (comp != null)
        {
            m_HoverObject = comp.gameObject;
            HoverEvent?.Invoke(m_HoverObject);
        }
    }
}
