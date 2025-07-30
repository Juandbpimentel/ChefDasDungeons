using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Javali : MonoBehaviour, ITriggerListener, IEnemy
{
    public int vidaMaxima = 3;
    public float speed = 1.5f;
    public float maxAttackCooldown = 2.5f;
    public float attackForce = 5f;
    public float KnockbackForce = 2f;
    public float stunTime = 0.1f;
    [Header("Raycast Settings")]
    [Tooltip("Deslocamento da origem do raycast para ajustar ao centro visual do sprite.")]
    public Vector2 raycastOriginOffset = new Vector2(0, 0.4f);

    Animator animator;
    GameObject player;
    NavMeshAgent agent;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Color originalColor;
    Vector2 playerOffset;

    public int vida;
    public float attackCooldown = 0f;
    private Vector2 attackDirection;
    private float flashRedTimer = 0f;
    private float flashRedDuration = 0.15f;

    bool hasLineOfSight = false;
    public bool isAttacking = false;
    public bool haveMakeAttack = false;
    private bool isDying = false;
    public bool haveDied = false;

    private bool isWalking = false;

    private bool isPlayerInInteractionArea = false;

    private bool isPlayerEnteredInAttackArea = false;
    private bool isKnockedback = false;

    [Header("Drops")]
    public GameObject slimeDropPrefab = null;
    public GameObject meatDropPrefab = null;
    public GameObject eggDropPrefab = null;
    public GameObject burguerDropPrefab = null;
    public GameObject stewDropPrefab = null;
    public GameObject friedEggDropPrefab = null;

    void Start()
    {
        StartCoroutine(FindPlayerAfterSceneLoad());
        vida = vidaMaxima;

        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private IEnumerator FindPlayerAfterSceneLoad()
    {
        // Aguarda até o player existir na cena
        while (GameObject.FindGameObjectWithTag("Player") == null)
            yield return null;

        player = GameObject.FindGameObjectWithTag("Player");

        // Aguarda até o player estar na posição correta (opcional, mas ajuda)
        yield return new WaitForEndOfFrame();

        // Recalcula o offset do player
        Collider2D playerCollider = player.GetComponentInChildren<BoxCollider2D>();
        if (playerCollider != null)
        {
            playerOffset = (Vector2)playerCollider.bounds.center - (Vector2)player.transform.position;
            playerOffset.y -= 0.2f;
        }
    }


    void Update()
    {
        if (attackCooldown > 0f && !isAttacking)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (isAttacking && haveMakeAttack)
        {
            isAttacking = false;
            haveMakeAttack = false;
            OnAttackAnimationEnd();
        }

        if (flashRedTimer > 0f)
        {
            flashRedTimer -= Time.deltaTime;
            if (flashRedTimer <= 0f && spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }

        if (haveDied)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            // Durante o ataque, o slime avança em linha reta com o dobro da velocidade.
            // A velocidade é zerada no final da animação pelo evento OnAttackAnimationEnd.
            transform.position += (Vector3)(attackDirection * (speed * 2) * Time.fixedDeltaTime);
        }
        else
        {
            // Movimento normal é controlado pelo NavMeshAgent apenas quando não está atacando.
            HandleMovement();
        }

        HandlePlayerLineOfSight();
        HandleAttack();
        HandleJavaliAnimation();
    }

    private void HandleJavaliAnimation()
    {
        JavaliState state = JavaliState.Idle;

        if (isDying)
        {
            state = JavaliState.Dying;
        }
        else if (isAttacking)
        {
            state = JavaliState.Attacking;
        }
        else if (isWalking)
        {
            state = JavaliState.Walking;
        }

        animator.SetFloat("state", (float)state);
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
                isPlayerInInteractionArea = true;
                // Lógica específica para o Trigger 1
            }
            // Todo: Remover depois isso aqui quando for terminar a integração entre player e mobs
            if (triggerObject.name == "HitboxArea")
            {
                if (other is CapsuleCollider2D)
                {
                    return;
                }

                if (other is BoxCollider2D)
                {
                    player.GetComponent<PlayerController>().removeLife();
                    player.GetComponent<PlayerController>().Knockback(transform, KnockbackForce, stunTime);
                }
            }
            if (triggerObject.name == "AttackArea")
            {
                if (!isPlayerEnteredInAttackArea)
                {
                    isPlayerEnteredInAttackArea = true;
                }
            }
        }
    }

    public void OnChildTriggerStay2D(GameObject triggerObject, Collider2D other)
    {
        // Verifica se o objeto que entrou no Trigger é o jogador
        if (other.CompareTag("Player"))
        {
        }
    }

    public void OnChildTriggerExit2D(GameObject triggerObject, Collider2D other)
    {
        // Verifica se o objeto que saiu do Trigger é o jogador
        if (other.CompareTag("Player"))
        {

            if (triggerObject.name == "InteractionArea")
            {
                isPlayerInInteractionArea = false;
                // Lógica específica para o Trigger 1
            }
            else if (triggerObject.name == "AttackArea")
            {
                // Lógica específica para o Trigger 2
                isPlayerEnteredInAttackArea = false;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Lógica de dano ou interação com o jogador
            Debug.Log("Slime colidiu com o jogador!");
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Lógica para quando o jogador sai da colisão
            Debug.Log("Slime deixou de colidir com o jogador!");
        }
    }

    private void HandleMovement()
    {
        // Usar a flag 'isAttacking' é mais seguro e legível
        if (isAttacking || isDying)
        {
            agent.ResetPath(); // Garante que ele pare enquanto ataca
            isWalking = false;
            return;
        }

        if (isPlayerInInteractionArea)
        {
            agent.SetDestination((Vector2)player.transform.position + playerOffset);
            agent.speed = speed;
            isWalking = true;
        }
        else
        {
            agent.ResetPath();
            isWalking = false;
        }
        // Atualiza a animação de movimento
        if (isWalking && agent.velocity.magnitude > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Sign(agent.velocity.x), 1, 1);
        }
    }

    private void HandleAttack()
    {
        // Condições para iniciar um novo ataque
        if (isPlayerEnteredInAttackArea && hasLineOfSight && !isAttacking && attackCooldown <= 0)
        {
            // Inicia o estado de ataque
            isAttacking = true;
            haveMakeAttack = false; // Garante que o ataque só será feito uma vez
            attackCooldown = maxAttackCooldown; // Reinicia o cooldown

            // Para o NavMeshAgent para que o movimento do ataque funcione sem interferência
            agent.ResetPath();
            isWalking = false; // Garante que a animação de andar pare

            // Armazena a direção do ataque para o movimento em linha reta
            attackDirection = ((Vector2)player.transform.position + playerOffset - (Vector2)transform.position).normalized;

            // Vira o slime para a direção do ataque
            transform.localScale = new Vector3(Mathf.Sign(attackDirection.x), 1, 1);

            // A lógica de movimento agora é tratada no FixedUpdate
        }
    }
    // Slime está normal seguindo o player
    // Slime não atacando chega na área de ataque e se prepara pra atacar
    // Esta função será chamada pelo Animation Event no final da animação de ataque
    public void OnAttackAnimationEnd()
    {
        // Zera a velocidade do Rigidbody2D para parar o movimento do impulso
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Garante que o NavMeshAgent também pare, caso ele tente se mover
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }

        animator.StopPlayback(); // Para a animação de ataque

        // Libera o slime para se mover ou atacar novamente
        isAttacking = false;
    }
    private void OnDrawGizmosSelected()
    {
        // Desenha um Gizmo para visualizar a origem do Raycast no Editor
        Gizmos.color = Color.yellow;
        Vector2 raycastOrigin = (Vector2)transform.position + raycastOriginOffset;
        Gizmos.DrawWireSphere(raycastOrigin, 0.1f);
    }

    public void levarDano(int dano)
    {
        // Lógica para levar dano ao slime
        if (dano > 0)
        {
            Debug.Log("AIAI" + dano);
            vida -= dano;
        }
        else
        {
            vida -= 1;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            flashRedTimer = flashRedDuration;
        }

        if (vida <= 0)
        {
            isDying = true;

        }
    }

    public void GetKnockedback(Transform playerTransform, float knockedbackForce, float stunTime)
    {
        if (rb != null)
        {
            Vector2 knockbackDirection = (transform.position - playerTransform.position).normalized;
            rb.AddForce(knockbackDirection * knockedbackForce, ForceMode2D.Impulse);
        }

        // Inicia o tempo de atordoamento
        StartCoroutine(StunTimer(stunTime));
    }

    IEnumerator StunTimer(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedback = false;
    }

    public void generateDrop()
    {
        System.Random rand = new();
        int dropChance = rand.Next(0, 100);
        if (dropChance < 55) // 0-54
        {
            Debug.Log("O Javali dropou uma carne!");
            Instantiate(eggDropPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance >= 55 && dropChance <= 56) // 55-56
        {
            Debug.Log("O Javali dropou um ovo frito!");
            Instantiate(friedEggDropPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance >= 57 && dropChance < 65) // 57-64
        {
            Debug.Log("O Javali dropou um ensopado de carne!");
            Instantiate(stewDropPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance >= 65 && dropChance <= 70) // 65-70
        {
            Debug.Log("O Javali dropou um hamburguer!");
            Instantiate(burguerDropPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance > 70 && dropChance < 100) // 71-99
        {
            Debug.Log("O Javali não dropou nada.");
        }
    }

    public enum JavaliState
    {
        Idle,
        Walking,
        Attacking,
        Dying
    }
}