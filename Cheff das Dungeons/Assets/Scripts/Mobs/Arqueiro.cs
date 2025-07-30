using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Arqueiro : MonoBehaviour, ITriggerListener, IEnemy
{
    public int vidaMaxima = 3;
    public float speed = 1.5f;
    public float stunTime = 0.1f;
    [Header("Raycast Settings")]
    [Tooltip("Deslocamento da origem do raycast para ajustar ao centro visual do sprite.")]
    public Vector2 raycastOriginOffset = new Vector2(0, -1f);

    Animator animator;
    GameObject player;
    NavMeshAgent agent;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Color originalColor;
    Vector2 playerOffset;

    public int vida;
    [SerializeField] private GameObject arrowPrefab;
    private float attackCooldown = 0f;
    [Header("Attack Settings")]
    [Tooltip("Tempo de recarga entre ataques.")]
    public float attackRate = 1f;
    [Tooltip("Tempo máximo de recarga entre ataques.")]
    public float maxAttackCooldown = 2.5f;
    [Tooltip("Força do empurrão ao atingir o jogador.")]
    public float KnockbackForce = 2f;
    [SerializeField] private float attackRange = 10f;
    private float flashRedTimer = 0f;
    private float flashRedDuration = 0.15f;

    bool hasLineOfSight = false;
    private bool isDying = false;
    public bool haveDied = false;

    private bool isWalking = false;
    private bool isPlayerInFearArea = false;
    private bool isPlayerEnteredInAttackArea = false;
    private bool isKnockedback = false;
    private bool isAttacking = false;
    private bool haveMakeAttack = false;

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
            attackCooldown -= Time.deltaTime;

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
            generateDrop();
            Destroy(gameObject);
        }

        // Só ataca se não estiver atacando
        if (hasLineOfSight && isPlayerEnteredInAttackArea && attackCooldown <= 0f && !isDying && !isAttacking)
        {
            isAttacking = true;
            haveMakeAttack = false;
            attackCooldown = maxAttackCooldown;
            AtirarNoPlayer();
            // O Animation Event deve chamar OnAttackAnimationEnd() ao final da animação
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandlePlayerLineOfSight();
        HandleArcherAnimation();
    }

    private void HandleArcherAnimation()
    {
        ArcherState state = ArcherState.Idle;

        if (isDying)
        {
            state = ArcherState.Dying;
        }
        else if (isAttacking)
        {
            state = ArcherState.Attacking;
        }
        else if (isWalking)
        {
            state = ArcherState.Walking;
        }

        animator.SetFloat("state", (float)state);
    }

    public void HandlePlayerLineOfSight()
    {
        if (player == null) return;

        int layerMask = LayerMask.GetMask("Foreground&Map", "Player");

        Vector2 raycastOrigin = (Vector2)transform.position + raycastOriginOffset;
        Vector2 playerCenter = (Vector2)player.transform.position + playerOffset;
        Vector2 direction = (playerCenter - raycastOrigin).normalized;
        Vector2 boxSize = new Vector2(0.5f, 0.5f);

        RaycastHit2D hit = Physics2D.BoxCast(raycastOrigin, boxSize, 0f, direction, attackRange, layerMask);

        if (hit.collider == null)
        {
            hasLineOfSight = false;
            UtilFunctions.DrawBox(raycastOrigin, boxSize, direction, 0f, attackRange, Color.red);
            return;
        }

        hasLineOfSight = hit.collider.CompareTag("Player");
        UtilFunctions.DrawBox(raycastOrigin, boxSize, direction, 0f, hit.distance, hasLineOfSight ? Color.green : Color.red);
    }

    public void OnChildTriggerEnter2D(GameObject triggerObject, Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerObject.name == "FearArea")
            {
                isPlayerInFearArea = true;
            }
            if (triggerObject.name == "AttackArea")
            {
                isPlayerEnteredInAttackArea = true;
            }
        }
    }

    public void OnChildTriggerStay2D(GameObject triggerObject, Collider2D other)
    {
        // Não é necessário lógica extra aqui para FearArea
    }

    public void OnChildTriggerExit2D(GameObject triggerObject, Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerObject.name == "FearArea")
            {
                isPlayerInFearArea = false;
            }
            else if (triggerObject.name == "AttackArea")
            {
                isPlayerEnteredInAttackArea = false;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Arqueiro colidiu com o jogador!");
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Arqueiro deixou de colidir com o jogador!");
        }
    }

    private void HandleMovement()
    {
        if (isDying)
        {
            agent.ResetPath();
            isWalking = false;
            return;
        }

        if (isKnockedback)
        {
            agent.ResetPath();
            isWalking = false;
            return;
        }

        if (isPlayerInFearArea)
        {
            // Foge do player: calcula direção oposta ao player e move para longe
            Vector2 fleeDirection = ((Vector2)transform.position - ((Vector2)player.transform.position + playerOffset)).normalized;
            Vector2 fleeTarget = (Vector2)transform.position + fleeDirection * 5f; // 5 unidades para longe
            agent.SetDestination(fleeTarget);
            agent.speed = speed;
            isWalking = true;
        }
        else
        {
            agent.ResetPath();
            isWalking = false;
        }

        if (isWalking && agent.velocity.magnitude > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Sign(agent.velocity.x), 1, 1);
        }
    }

    private void AtirarNoPlayer()
    {
        if (arrowPrefab == null || player == null) return;

        Vector3 center = rb.worldCenterOfMass;
        Vector3 playerCenter = player.transform.position + (Vector3)playerOffset;
        Vector2 direction = (playerCenter - center).normalized;
        Vector3 spawnPos = center + (Vector3)(direction * 2f);

        GameObject projectile = Instantiate(arrowPrefab);
        projectile.transform.position = spawnPos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 10f;
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        haveMakeAttack = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
        animator.StopPlayback();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 raycastOrigin = (Vector2)transform.position + raycastOriginOffset;
        Gizmos.DrawWireSphere(raycastOrigin, 0.1f);
    }

    public void levarDano(int dano)
    {
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
        isKnockedback = true;
        Vector2 direction = (transform.position - playerTransform.position).normalized;
        rb.linearVelocity = direction * knockedbackForce;
        StartCoroutine(StunTimer(stunTime));
    }

    private IEnumerator StunTimer(float stunTime)
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
            Debug.Log("Slime dropou um ovo!");
            Instantiate(eggDropPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance >= 55 && dropChance <= 56) // 55-56
        {
            Debug.Log("Slime dropou um ovo frito!");
            Instantiate(friedEggDropPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance >= 57 && dropChance < 65) // 57-64
        {
            Debug.Log("Slime dropou um ensopado de carne!");
            Instantiate(stewDropPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance >= 65 && dropChance <= 70) // 65-70
        {
            Debug.Log("Slime dropou um hamburguer!");
            Instantiate(burguerDropPrefab, transform.position, Quaternion.identity);
        }
        else if (dropChance > 70 && dropChance < 100) // 71-99
        {
            Debug.Log("Slime não dropou nada.");
        }
    }

    public enum ArcherState
    {
        Idle,
        Walking,
        Attacking,
        Dying
    }
}