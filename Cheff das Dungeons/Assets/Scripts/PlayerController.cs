using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField]
    private float speed = 3f;
    [SerializeField]
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        //Função de teste para remover vidas (tomar dano)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            removeLife();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            // ATTACK
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
        if (inCheckpoint)
        {
            handleCheckpointInteraction();
        }

    }

    private void FixedUpdate()
    {
        ControlPlayerMoviment();
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
        if (other.CompareTag("Egg"))
        {
            egg++;
            Destroy(other.gameObject);
            eggText.text = egg.ToString();
        }

        //Se for uma "carne", adiciona ao contador e destroi
        if (other.CompareTag("Meat"))
        {
            meat++;
            Destroy(other.gameObject);
            meatText.text = meat.ToString();
        }

        //Se for uma "gosma", adiciona ao contador e destroi
        if (other.CompareTag("Slime"))
        {
            slime++;
            Destroy(other.gameObject);
            slimeText.text = slime.ToString();
        }
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
        if (currentLife <= maxLifes && currentLife > 0)
        {
            currentLife -= 1;
        }

        if (currentLife == 0)
        {
            GameManager.Instance.Respaw();
            recuperateLife(maxLifes);
        }
    }

    private bool recuperateLife(int n)
    {
        if (n > 0)
        {
            currentLife = n;
        }
        else
        {
            currentLife = maxLifes;
        }

        return true;
    }

    private void eat(FoodEnum food)
    {
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
}
