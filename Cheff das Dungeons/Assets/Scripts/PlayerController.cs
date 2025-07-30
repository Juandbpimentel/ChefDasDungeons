using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField]
    private float speed = 3f;
    [SerializeField]

    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public float knockedbackForce = 2f;
    public bool isDying = false;

    private bool isKnockedback;
    public float cooldownAttack = 0.8f;
    private float timerAttack;
    public int maxLifes = 5;
    public int currentLife;
    public int faceDirection = 1;
    public Rigidbody2D rb;
    public Animator animator;

    public bool isCrafting = false;

    public bool inCheckpoint = false;

    //Ingredientes
    public int egg = 3;
    [SerializeField]
    public TextMeshProUGUI eggText;

    public int meat = 4;
    [SerializeField]
    public TextMeshProUGUI meatText;

    public int slime = 3;
    [SerializeField]
    public TextMeshProUGUI slimeText;

    //Comidas
    public int burger = 0;
    [SerializeField]
    public TextMeshProUGUI burgerText;

    public int stew = 0;
    [SerializeField]
    public TextMeshProUGUI stewText;

    public int fried_egg = 0;
    [SerializeField]
    public TextMeshProUGUI fried_eggText;

    private float flashRedTimer = 0f;
    private float flashRedDuration = 0.15f;
    SpriteRenderer spriteRenderer;
    Color originalColor;

    private bool isDashing = false;
    [SerializeField] private float dashingPower = 3f;

    [SerializeField] private TrailRenderer tr;

    private bool inSign = false;

    public bool isTeleporting = false;

    private bool canTakeDamage = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        rb = GetComponent<Rigidbody2D>();
        initLifes(maxLifes);
        updateUIText();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerAttack > 0)
        {
            timerAttack -= Time.deltaTime;
        }

        if (inCheckpoint)
        {
            handleCheckpointInteraction();
        }

        if (inSign)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.Instance.Respaw();
            }
        }

        if (flashRedTimer > 0f)
        {
            flashRedTimer -= Time.deltaTime;
            if (flashRedTimer <= 0f && spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }

        if (isDying == false || isDashing == false)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Dashing();
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                Attack();
            }

            //Impede que o player coma o item quando estiver cozinhando (pois é a mesma tecla)
            if (!isCrafting)
            {
                //Comer um hamburger
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Debug.Log("Comendo um hamburger");
                    eat(FoodEnum.Burger);
                }

                //Comer uma sopa
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Debug.Log("Comendo uma sopa");
                    eat(FoodEnum.Stew);
                }

                //Comer um ovo
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Debug.Log("Comendo um ovo frito");
                    eat(FoodEnum.FriedEgg);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedback == false && isDying == false)
        {
            ControlPlayerMoviment();
        }
    }

    private void handleCheckpointInteraction()
    {
        Checkpoint checkpoint = GameObject.FindWithTag("Checkpoint").GetComponent<Checkpoint>();
        if (Input.GetKeyDown(KeyCode.E))
        {
            isCrafting = !isCrafting;
            checkpoint.enableDisableFoods();
        }

        if (checkpoint.burger.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Alpha1))
        {
            makeBurger();
            checkpoint.enableDisableFoods();
        }

        if (checkpoint.stew.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Alpha2))
        {
            makeStew();
            checkpoint.enableDisableFoods();
        }

        if (checkpoint.fried_egg.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Alpha3))
        {
            makeFried_egg();
            checkpoint.enableDisableFoods();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Se for um "ovo", adiciona ao contador e destroi
        string tag = other.tag;
        if (handleFoodDrop(tag, other)) return;
    }

    private bool handleFoodDrop(string tag, Collider2D other)
    {
        switch (tag)
        {
            case "Egg":
                egg++;
                Destroy(other.gameObject);
                eggText.text = egg.ToString();
                return true;
            case "Meat":
                meat++;
                Destroy(other.gameObject);
                meatText.text = meat.ToString();
                return true;
            case "Slime":
                slime++;
                Destroy(other.gameObject);
                slimeText.text = slime.ToString();
                return true;
            case "Burguer":
                burger++;
                Destroy(other.gameObject);
                burgerText.text = burger.ToString();
                return true;
            case "Stew":
                stew++;
                Destroy(other.gameObject);
                stewText.text = stew.ToString();
                return true;
            case "FriedEgg":
                fried_egg++;
                Destroy(other.gameObject);
                fried_eggText.text = fried_egg.ToString();
                return true;
            default:
                break;
        }
        return false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            isCrafting = false;
        }
    }

    public void makeBurger()
    {
        burger++;

        egg--;
        slime--;
        meat--;

        updateUIText();
    }

    public void makeStew()
    {
        stew++;

        slime--;
        meat--;

        updateUIText();
    }

    public void makeFried_egg()
    {
        fried_egg++;

        egg--;

        updateUIText();
    }

    private void updateUIText()
    {
        meatText.text = meat.ToString();
        eggText.text = egg.ToString();
        slimeText.text = slime.ToString();

        stewText.text = stew.ToString();
        fried_eggText.text = fried_egg.ToString();
        burgerText.text = burger.ToString();
    }

    //Cria as imagens da vida (até o máximo definido)
    private void initLifes(int max)
    {
        currentLife = max;
    }

    public void removeLife()
    {
        if (currentLife <= maxLifes && currentLife > 0 && canTakeDamage)
        {
            currentLife -= 1;
            spriteRenderer.color = Color.red;
            flashRedTimer = flashRedDuration;

            canTakeDamage = false;
            StartCoroutine(TakeDamage());
        }

        if (currentLife == 0)
        {
            isDying = true;
            animator.SetBool("isDying", isDying);
            animator.SetBool("isAttacking", false);
        }
    }

    IEnumerator TakeDamage()
    {
        yield return new WaitForSeconds(0.25f);
        canTakeDamage = true;
    }

    public void DyingReset()
    {
        isDying = false;
        animator.SetBool("isDying", isDying);
        GameManager.Instance.Respaw();
        recuperateLife(maxLifes);
    }

    private bool recuperateLife(int n)
    {
        if (n + currentLife > maxLifes)
        {
            currentLife = maxLifes;
        }
        else if (n + currentLife > 0)
        {
            currentLife += n;
        }
        else
        {
            currentLife = maxLifes;
        }

        return true;
    }

    private void eat(FoodEnum food)
    {
        if (currentLife == maxLifes)
        {
            Debug.Log("Player já está com a vida cheia!");
            return;
        }
        Debug.Log("Comendo: " + food.ToString());
        if (food == FoodEnum.Burger && burger > 0)
        {
            if (recuperateLife(3)) { burger--; }
        }

        if (food == FoodEnum.Stew && stew > 0)
        {
            if (recuperateLife(2)) { stew--; }
        }

        if (food == FoodEnum.FriedEgg && fried_egg > 0)
        {
            if (recuperateLife(1)) { fried_egg--; }
        }

        updateUIText();
    }
    public enum FoodEnum
    {
        Burger,
        Stew,
        FriedEgg
    }
    private void ControlPlayerMoviment()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(horizontal, vertical).normalized;

        if (
            (horizontal > 0 && transform.localScale.x < 0) ||
            (horizontal < 0 && transform.localScale.x > 0)
        )
        {
            faceDirection *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        animator.SetFloat("x", Mathf.Abs(horizontal));
        animator.SetFloat("y", Mathf.Abs(vertical));

        rb.linearVelocity = movement * speed;
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        if (isDying == false)
        {
            isKnockedback = true;
            Vector2 direction = (transform.position - enemy.position).normalized;
            rb.linearVelocity = direction * force;
            StartCoroutine(KnockbackCounter(stunTime));
        }
    }

    public void Attack()
    {
        if (timerAttack <= 0)
        {
            animator.SetBool("isAttacking", true);

            timerAttack = cooldownAttack;
        }
    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

        if (enemies.Length > 0)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponentInParent<IEnemy>().levarDano(damage);
                enemies[i].GetComponentInParent<IEnemy>().GetKnockedback(transform, knockedbackForce, stunTime: 1f);
            }
        }
    }

    public void FinishAttacking()
    {
        animator.SetBool("isAttacking", false);
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedback = false;
    }

    private IEnumerator Dash()
    {
        yield return new WaitForSeconds(0.2f);
        speed /= dashingPower;
        tr.emitting = false;

        yield return new WaitForSeconds(0.5f);
        isDashing = false;
    }

    private void Dashing()
    {
        if (!isDashing)
        {
            isDashing = true;
            speed *= dashingPower;
            tr.emitting = true;
            StartCoroutine(Dash());
        }
    }
}
