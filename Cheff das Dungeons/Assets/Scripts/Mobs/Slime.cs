using UnityEngine;

public class Slime : MonoBehaviour
{
    // Variáveis e métodos específicos do inimigo podem ser adicionados aqui
    public int vidaMaxima = 5;
    public float speed = 1.5f;
    private int vida;
    private GameObject player;
    private bool hasLineOfSight = false;


    void Start()
    {
        vida = vidaMaxima;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (hasLineOfSight)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
        if (hit.collider == null)
        {
            hasLineOfSight = false;
            return;
        }
        hasLineOfSight = hit.collider.CompareTag("Player");
        Debug.DrawLine(transform.position, hit.point, hasLineOfSight ? Color.green : Color.red);
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
