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
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y), speed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        int layerMask = LayerMask.GetMask(new string[] { "Foreground&Map", "Player" });
        Vector2 direction = player.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, layerMask);
        if (hit.collider == null)
        {
            hasLineOfSight = false;
            return;
        }
        hasLineOfSight = hit.collider.CompareTag("Player");
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hasLineOfSight = true;
        }
        else
        {
            hasLineOfSight = false;
        }
        Debug.DrawLine(transform.position, hit.point, hasLineOfSight ? Color.green : Color.red);
    }
}
