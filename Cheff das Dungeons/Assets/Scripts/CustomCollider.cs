using Unity.VisualScripting;
using UnityEngine;

public class CustomCollider : MonoBehaviour
{
    public event System.Action<Collider2D> OnTriggerEnterEvent;
    public event System.Action<Collider2D> OnTriggerExitEvent;

    private void OnCollisionEnter2D(Collision2D other)
    {
        OnTriggerEnterEvent?.Invoke(other.collider);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        OnTriggerExitEvent?.Invoke(other.collider);
    }
}