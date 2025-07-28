using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour, ITriggerListener
{
    public int vidaMaxima = 5;
    public float speed = 1.5f;
    private int vida;
    private GameObject player;

    NavMeshAgent agent;
    private bool hasLineOfSight = false;

    private bool playerInInteractionArea = false;

    // Variável para armazenar o deslocamento do centro do jogador
    private Vector2 playerOffset;

    void Start()
    {
        vida = vidaMaxima;
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Please ensure there is a GameObject with the 'Player' tag.");
            return;
        }

        // Obtém o Collider2D do nó filho
        Collider2D playerCollider = player.GetComponentInChildren<BoxCollider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("O Hitbox do jogador não possui um Collider2D!");
            return;
        }

        // Calcula o deslocamento do centro do Hitbox em relação à posição do jogador
        playerOffset = (Vector2)playerCollider.bounds.center - (Vector2)player.transform.position;
        playerOffset.y -= 0.2f;
    }

    void Update()
    {
        // Todo: Implementar lógica de seguir só quando o jogador está dentro da área de interação
        agent.SetDestination((Vector2)player.transform.position + playerOffset);
        agent.speed = speed;
    }

    private void FixedUpdate()
    {
        int layerMask = LayerMask.GetMask("Foreground&Map", "Player");

        // Calcula o centro do jogador com base no deslocamento pré-calculado
        Vector2 playerCenter = (Vector2)player.transform.position + playerOffset;

        // Calcula a direção do Raycast para o centro do jogador
        Vector2 direction = (playerCenter - (Vector2)transform.position).normalized;

        // Define o tamanho da "grossura" do BoxCast
        Vector2 boxSize = new Vector2(0.5f, 0.5f); // Ajuste os valores conforme necessário

        // Realiza o BoxCast
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, direction, Mathf.Infinity, layerMask);

        if (hit.collider == null)
        {
            hasLineOfSight = false;
            DrawBox(transform.position, boxSize, direction, 0f, 10f, Color.red);
            return;
        }

        // Verifica se o Raycast atingiu o jogador
        hasLineOfSight = hit.collider.CompareTag("Player");

        // Desenha a linha do Raycast no editor
        DrawBox(transform.position, boxSize, direction, 0f, hit.distance, hasLineOfSight ? Color.green : Color.red);
    }


    private void DrawBox(Vector2 origin, Vector2 size, Vector2 direction, float angle, float distance, Color color)
    {
        // Calcula os vértices da caixa
        Vector2 right = new Vector2(direction.y, -direction.x) * size.x / 2;
        Vector2 up = direction * size.y / 2;

        Vector2 topLeft = origin - right + up;
        Vector2 topRight = origin + right + up;
        Vector2 bottomLeft = origin - right - up;
        Vector2 bottomRight = origin + right - up;

        // Calcula os vértices deslocados pela distância
        Vector2 topLeftEnd = topLeft + direction * distance;
        Vector2 topRightEnd = topRight + direction * distance;
        Vector2 bottomLeftEnd = bottomLeft + direction * distance;
        Vector2 bottomRightEnd = bottomRight + direction * distance;

        // Desenha as linhas da caixa inicial
        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);

        // Desenha as linhas da caixa final (após o deslocamento)
        Debug.DrawLine(topLeftEnd, topRightEnd, color);
        Debug.DrawLine(topRightEnd, bottomRightEnd, color);
        Debug.DrawLine(bottomRightEnd, bottomLeftEnd, color);
        Debug.DrawLine(bottomLeftEnd, topLeftEnd, color);

        // Desenha as linhas conectando os lados da caixa inicial e final
        Debug.DrawLine(topLeft, topLeftEnd, color);
        Debug.DrawLine(topRight, topRightEnd, color);
        Debug.DrawLine(bottomLeft, bottomLeftEnd, color);
        Debug.DrawLine(bottomRight, bottomRightEnd, color);

        // Desenha a linha central da caixa
        Vector2 centerStart = origin;
        Vector2 centerEnd = origin + direction * distance;
        Debug.DrawLine(centerStart, centerEnd, color);
    }

    public void OnChildTriggerEnter2D(GameObject triggerObject, Collider2D other)
    {
        // Verifica se o objeto que entrou no Trigger é o jogador
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador entrou na área de interação!");

            if (triggerObject.name == "InteractionArea")
            {
                Debug.Log("InteractionArea ativado pelo jogador!");
                playerInInteractionArea = true;
                // Lógica específica para o Trigger 1
            }
            else if (triggerObject.name == "AttackArea")
            {
                Debug.Log("AttackArea ativado pelo jogador!");
                // Lógica específica para o Trigger 2
            }
        }
    }

    public void OnChildTriggerExit2D(GameObject triggerObject, Collider2D other)
    {
        // Verifica se o objeto que saiu do Trigger é o jogador
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador saiu da área de interação!");

            if (triggerObject.name == "InteractionArea")
            {
                Debug.Log("InteractionArea desativado pelo jogador!");
                playerInInteractionArea = false;
                // Lógica específica para o Trigger 1
            }
            else if (triggerObject.name == "AttackArea")
            {
                Debug.Log("AttackArea desativado pelo jogador!");
                // Lógica específica para o Trigger 2
            }
        }
    }
}