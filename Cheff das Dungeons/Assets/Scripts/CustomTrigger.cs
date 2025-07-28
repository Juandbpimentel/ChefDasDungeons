using UnityEngine;

public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Collider2D> OnTriggerEnterEvent;
    public event System.Action<Collider2D> OnTriggerExitEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }
}