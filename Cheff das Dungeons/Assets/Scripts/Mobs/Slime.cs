using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour, ITriggerListener
{
    public int vidaMaxima = 5;
    public float speed = 1.5f;

    private Animator animator;
    private int vida;
    private GameObject player;

    NavMeshAgent agent;
    private bool hasLineOfSight = false;


    public float attackCooldown = 0f;

    // Variável para armazenar se o Slime está atacando
    private bool isAttacking = false;

    private bool isPlayerInInteractionArea = false;

    private bool isPlayerStayInAttackArea = false;

    [Header("Raycast Settings")]
    [Tooltip("Deslocamento da origem do raycast para ajustar ao centro visual do sprite.")]
    public Vector2 raycastOriginOffset = new Vector2(0, 0.4f);

    // Variável para armazenar o deslocamento do centro do jogador
    private Vector2 playerOffset;

    void Start()
    {
        vida = vidaMaxima;

        player = GameObject.FindGameObjectWithTag("Player");

        animator = GetComponent<Animator>();

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
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandlePlayerLineOfSight();
        HandleAttack();
    }

    public void HandlePlayerLineOfSight()
    {
        if (player == null) return;

        int layerMask = LayerMask.GetMask("Foreground&Map", "Player");

        // Calcula a origem do raycast com o deslocamento
        Vector2 raycastOrigin = (Vector2)transform.position + raycastOriginOffset;

        // Calcula o centro do jogador com base no deslocamento pré-calculado
        Vector2 playerCenter = (Vector2)player.transform.position + playerOffset;

        // Calcula a direção do Raycast para o centro do jogador
        Vector2 direction = (playerCenter - raycastOrigin).normalized;

        // Define o tamanho da "grossura" do BoxCast
        Vector2 boxSize = new Vector2(0.5f, 0.5f); // Ajuste os valores conforme necessário

        // Realiza o BoxCast
        RaycastHit2D hit = Physics2D.BoxCast(raycastOrigin, boxSize, 0f, direction, 5f, layerMask);

        if (hit.collider == null)
        {
            hasLineOfSight = false;
            UtilFunctions.DrawBox(raycastOrigin, boxSize, direction, 0f, 5f, Color.red);
            return;
        }

        // Verifica se o Raycast atingiu o jogador
        hasLineOfSight = hit.collider.CompareTag("Player");

        // Desenha a linha do Raycast no editor
        UtilFunctions.DrawBox(raycastOrigin, boxSize, direction, 0f, hit.distance, hasLineOfSight ? Color.green : Color.red);
    }



    public void OnChildTriggerEnter2D(GameObject triggerObject, Collider2D other)
    {
        // Verifica se o objeto que entrou no Trigger é o jogador
        if (other.CompareTag("Player"))
        {
            if (triggerObject.name == "InteractionArea")
            {
                Debug.Log("Jogador entrou na área de Interação!");
                isPlayerInInteractionArea = true;
                // Lógica específica para o Trigger 1
            }
        }
    }

    public void OnChildTriggerStay2D(GameObject triggerObject, Collider2D other)
    {
        // Verifica se o objeto que entrou no Trigger é o jogador
        if (other.CompareTag("Player"))
        {
            if (triggerObject.name == "AttackArea")
            {
                if (!isPlayerStayInAttackArea)
                {
                    Debug.Log("Jogador ficou na área de Ataque!");
                    isPlayerStayInAttackArea = true;
                }
                // Debug.Log("AttackArea ativado pelo jogador!");
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
                isPlayerInInteractionArea = false;
                // Lógica específica para o Trigger 1
            }
            else if (triggerObject.name == "AttackArea")
            {
                Debug.Log("AttackArea desativado pelo jogador!");
                // Lógica específica para o Trigger 2
                isPlayerStayInAttackArea = false;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Slime colidiu com o jogador!");
            // Lógica de dano ou interação com o jogador
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Slime saiu da colisão com o jogador!");
            // Lógica para quando o jogador sai da colisão
        }
    }

    private void HandleMovement()
    {

        if (isPlayerInInteractionArea)
        {
            if (isPlayerStayInAttackArea)
            {
                if (attackCooldown >= 0 && isAttacking)
                {
                    // Se o Slime está atacando ou o cooldown acabou, não se move
                    Debug.Log("Slime está na área de ataque e está no meio do ataque. attackCooldown: " + attackCooldown + "| isAttacking: " + isAttacking);
                    agent.ResetPath();
                    return;
                }
                else
                {
                    Debug.Log("Slime está na área de ataque, mas está com o ataque em cooldown e não está atacando. attackCooldown: " + attackCooldown + "| isAttacking: " + isAttacking);
                    agent.SetDestination((Vector2)player.transform.position + playerOffset);
                    agent.speed = speed;
                }
            }
            else
            {
                if (!isAttacking)
                {
                    Debug.Log("Slime não está na área de ataque, mas não está atacando. attackCooldown: " + attackCooldown + "| isAttacking: " + isAttacking);
                    agent.SetDestination((Vector2)player.transform.position + playerOffset);
                    agent.speed = speed;
                }
                else
                {
                    // Se o Slime está atacando, não se move
                    Debug.Log("Slime não está na área de ataque, mas está atacando. attackCooldown: " + attackCooldown + "| isAttacking: " + isAttacking);
                    agent.ResetPath();
                }
            }

        }
        else
        {
            Debug.Log("Slime não está na área de interação, parando o movimento. attackCooldown: " + attackCooldown + "| isAttacking: " + isAttacking);
            agent.ResetPath();
        }

    }

    private void HandleAttack()
    {
        if (isPlayerStayInAttackArea && hasLineOfSight && !isAttacking)
        {
            // Inicia o ataque
            isAttacking = true;
            Debug.Log("Slime está atacando o jogador!");

            // Aqui você pode adicionar a lógica de ataque, como causar dano ao jogador
            // Exemplo: player.GetComponent<PlayerHealth>().TakeDamage(1);

            // Reseta o cooldown do ataque
            // Defina o tempo de cooldown desejado
        }
        // verificar se o animator terminou a animação de ataque
        if (animator != null)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                attackCooldown = 2.5f;
            }
        }
        if (attackCooldown > 0f)
        {
            isAttacking = false;
            attackCooldown -= Time.deltaTime;
        }
        else
        {
            attackCooldown = 0f;
        }
    }
    // Slime está normal seguindo o player
    // Slime não atacando chega na área de ataque e se prepara pra atacar

    private void OnDrawGizmosSelected()
    {
        // Desenha um Gizmo para visualizar a origem do Raycast no Editor
        Gizmos.color = Color.yellow;
        Vector2 raycastOrigin = (Vector2)transform.position + raycastOriginOffset;
        Gizmos.DrawWireSphere(raycastOrigin, 0.1f);
    }
}