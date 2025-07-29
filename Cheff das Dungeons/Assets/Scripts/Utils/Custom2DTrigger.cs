using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Custom2DTrigger : MonoBehaviour
{
    public Color gizmoColor = Color.red;
    private ITriggerListener parentListener;

    void Awake()
    {
        // Procura por um componente no pai que implemente a interface ITriggerListener
        parentListener = GetComponentInParent<ITriggerListener>();
        if (parentListener == null)
        {
            Debug.LogWarning($"Custom2DTrigger no objeto '{gameObject.name}' não encontrou um ITriggerListener no pai.", this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Se encontrou um "ouvinte", avisa sobre a entrada
        parentListener?.OnChildTriggerEnter2D(this.gameObject, collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // Se encontrou um "ouvinte", avisa sobre a permanência
        parentListener?.OnChildTriggerStay2D(this.gameObject, collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Se encontrou um "ouvinte", avisa sobre a saída
        parentListener?.OnChildTriggerExit2D(this.gameObject, collision);
    }

    void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = gizmoColor; // Define a cor dos Gizmos
            if (collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                Gizmos.DrawWireSphere(circleCollider.bounds.center, circleCollider.radius);
            }
            else if (collider is PolygonCollider2D polygonCollider)
            {
                Vector2[] points = polygonCollider.points;
                Vector3[] worldPoints = new Vector3[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    worldPoints[i] = polygonCollider.transform.TransformPoint(points[i]);
                }
                for (int i = 0; i < worldPoints.Length; i++)
                {
                    Gizmos.DrawLine(worldPoints[i], worldPoints[(i + 1) % worldPoints.Length]);
                }
            }
        }
    }
}
