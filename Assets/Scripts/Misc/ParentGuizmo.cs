using UnityEngine;

[ExecuteAlways]
public class ParentGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.yellow;
    public float size = 0.5f;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, size);
    }
}
