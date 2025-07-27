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
    private List<Image> lifes;
    public int maxLifes = 5;
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

        if (other.CompareTag("Enemy"))
        {
            //Se for um inimigo, remove uma vida
            removeLife();
            Debug.Log("Player hit by enemy!");
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
        for (int i = 1; i < max; i++)
        {
            //instancia nova imagem
            Image newImage = Instantiate(lifes[^1], lifes[^1].transform.parent);
            lifes.Add(newImage);

            //define a posicao
            Vector3 newPosition = lifes[^1].GetComponent<RectTransform>().anchoredPosition;
            newPosition.x += 80; // desloca no eixo X para a direita
            newImage.GetComponent<RectTransform>().anchoredPosition = newPosition;

            newImage.name = "" + i;
        }
    }

    //Remove a vida da tela (não destroi o gameObject)
    private void removeLife()
    {
        //iterar sobre o array de vidas
        for (int i = maxLifes - 1; i >= 0; i--)
        {
            //se achar uma vida com enabled = true
            if (lifes[i].enabled == true)
            {
                // mudar para false
                lifes[i].enabled = false;

                // se era a ultima vida
                if (i == 0)
                {
                    // respawnar player e recuperar todas as vidas
                    GameManager.Instance.Respaw();
                    recuperateLife(maxLifes);
                }
                //sair
                return;
            }
        }
    }

    //Ativa x vidas de volta
    private bool recuperateLife(int n)
    {
        bool rec = false;
        //iterar sobre o array de vidas
        for (int i = 0, j = 0; i < maxLifes && j < n; i++)
        {
            //se achar vida com enabled = false
            if (lifes[i].enabled == false)
            {
                //mudar para true
                lifes[i].enabled = true;

                // fazer isso n vezes
                j++;

                //recuperou = true
                rec = true;
            }
        }

        return rec;
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

        if (
            (horizontal > 0 && transform.localScale.x < 0) ||
            (horizontal < 0 && transform.localScale.x > 0)
        )
        {
            faceDirection *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        animator.SetFloat("horizontal", Mathf.Abs(horizontal));
        animator.SetFloat("vertical", Mathf.Abs(vertical));

        rb.linearVelocity = new Vector2(horizontal, vertical) * speed;
    }
}
