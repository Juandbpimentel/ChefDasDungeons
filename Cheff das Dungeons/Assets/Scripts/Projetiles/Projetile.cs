using UnityEngine;

public class Projetile : MonoBehaviour
{
    public float lifeTime = 5f; // Tempo de vida do projetil
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Destroi o projetil após um certo tempo
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other is BoxCollider2D)
        {
            // Aqui você pode adicionar a lógica para o que acontece quando o projetil atinge o jogador
            Debug.Log("Projetil atingiu o jogador!");
            other.GetComponent<PlayerController>().removeLife(); // Exemplo de dano ao jogador
            // Por exemplo, causar dano ou destruir o projetil
            Destroy(gameObject);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Foreground/MapColliders"))
        {
            // Aqui você pode adicionar a lógica para o que acontece quando o projetil atinge um inimigo
            Debug.Log("Projetil atingiu o mapa!");
            // Por exemplo, causar dano ao inimigo ou destruir o projetil
            Destroy(gameObject);
        }
    }
}
