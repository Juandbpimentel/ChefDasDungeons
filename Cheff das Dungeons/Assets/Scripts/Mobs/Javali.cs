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
    private float flashRedTimer = 0f;
    private float flashRedDuration = 0.15f;

    bool hasLineOfSight = false;
    private bool isDying = false;
    public bool haveDied = false;

    private bool isWalking = false;

    private bool isPlayerInInteractionArea = false;

    private bool isPlayerEnteredInAttackArea = false;
    private bool isKnockedback = false;

    // Charge logic
    private bool isCharging = false;
    private Vector2 chargeDirection;
    private float chargeSpeedMultiplier = 3f;
    private float chargeDuration = 1.5f;
    private float chargeTimer = 0f;
    private bool isStunned = false;
    private float stunDuration = 1.0f;
    private float stunTimer = 0f;

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
        if (attackCooldown > 0f && !isCharging)
        {
            attackCooldown -= Time.deltaTime;
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
        if (isStunned)
        {
            stunTimer -= Time.fixedDeltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
            }
            return;
        }

        if (isCharging)
        {
            chargeTimer -= Time.fixedDeltaTime;
            transform.position += (Vector3)(chargeDirection * speed * chargeSpeedMultiplier * Time.fixedDeltaTime);
            if (chargeTimer <= 0f)
            {
                StopChargeAndStun();
            }
            return;
        }

        // Movimento normal é controlado pelo NavMeshAgent apenas quando não está em charge.
        HandleMovement();

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
        else if (isStunned)
        {
            state = JavaliState.Sttuned;
        }
        else if (isCharging)
        {
            state = JavaliState.Charging;
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
            // CHARGE LOGIC - tanto para ChargeArea quanto para AttackArea
            if ((triggerObject.name == "ChargeArea" || triggerObject.name == "AttackArea") && !isCharging && !isStunned && hasLineOfSight)
            {
                StartCharge();
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
        if (isCharging && !isStunned)
        {
            // Se colidir com parede (Layer do mapa)
            if (collision.gameObject.layer == LayerMask.NameToLayer("Foreground&Map") || collision.gameObject.CompareTag("Wall"))
            {
                StopChargeAndStun();
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            // Lógica de dano ou interação com o jogador
            Debug.Log("Javali colidiu com o jogador!");
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
        // Usar a flag 'isCharging' é mais seguro e legível
        if (isDying || isCharging || isStunned)
        {
            agent.ResetPath(); // Garante que ele pare enquanto em charge/stun
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
        // Condições para iniciar um novo charge/ataque
        if (isPlayerEnteredInAttackArea && hasLineOfSight && !isCharging && !isStunned && attackCooldown <= 0)
        {
            StartCharge();
            attackCooldown = maxAttackCooldown; // Reinicia o cooldown
        }
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

    private void StartCharge()
    {
        isCharging = true;
        chargeTimer = chargeDuration;
        // Direção do charge é a direção do player no momento do início
        chargeDirection = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
        // Vira o javali para a direção do charge
        transform.localScale = new Vector3(Mathf.Sign(chargeDirection.x), 1, 1);
        // Para o NavMeshAgent
        if (agent != null && agent.isOnNavMesh) agent.ResetPath();
    }

    private void StopChargeAndStun()
    {
        isCharging = false;
        isStunned = true;
        stunTimer = stunDuration;
        // Para o movimento
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (agent != null && agent.isOnNavMesh) agent.ResetPath();
    }

    public void generateDrop()
    {
        System.Random rand = new();
        int dropChance = rand.Next(0, 100);
        if (dropChance < 55) // 0-54
        {
            Debug.Log("O Javali dropou uma carne!");
            Instantiate(meatDropPrefab, transform.position, Quaternion.identity);
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
        Charging,
        Sttuned,
        Dying
    }
}