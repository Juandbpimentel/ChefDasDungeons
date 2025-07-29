using UnityEngine;

public class Projetile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other is BoxCollider2D)
        {
            // Aqui você pode adicionar a lógica para o que acontece quando o projetil atinge o jogador
            Debug.Log("Projetil atingiu o jogador!");
            // Por exemplo, causar dano ou destruir o projetil
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Foreground&Map"))
        {
            // Aqui você pode adicionar a lógica para o que acontece quando o projetil atinge um inimigo
            Debug.Log("Projetil atingiu o mapa!");
            // Por exemplo, causar dano ao inimigo ou destruir o projetil
            Destroy(gameObject);
        }
    }
}
