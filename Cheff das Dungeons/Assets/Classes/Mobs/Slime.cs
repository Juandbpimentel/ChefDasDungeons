using UnityEngine;

public class Slime : MonoBehaviour
{
    // Variáveis e métodos específicos do inimigo podem ser adicionados aqui
    public int vidaMaxima = 5;
    public float speed = 2f;
    private int vida;
    private GameObject player;



    void Start()
    {
        vida = vidaMaxima;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            // Lógica para atacar o jogador
        }
    }
}
