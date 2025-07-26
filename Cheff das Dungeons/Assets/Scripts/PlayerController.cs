using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private List<Image> lifes;
    public int maxLifes = 5;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    //Ingredientes
    public int egg = 0;
    [SerializeField]
    public TextMeshProUGUI eggText;

    public int meat = 0;
    [SerializeField]
    public TextMeshProUGUI meatText;

    public int slime = 0;
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
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        initLifes(maxLifes);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(horizontal, vertical);

        //Função de teste para remover vidas (tomar dano)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //GameManager.Instance.Respaw();
            removeLife();
        }
    }

    private void FixedUpdate()
    {
        Vector3 movePosition = (speed * Time.fixedDeltaTime * moveDirection.normalized) + rb.position;
        rb.MovePosition(movePosition);

        //Comer um hamburger
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            eat("burger");
        }

        //Comer uma sopa
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            eat("stew");
        }

        //Comer um ovo
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            eat("fried_egg");
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
    private void recuperateLife(int n)
    {
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
            }
        }
    }

    private void eat(string foodName)
    {
        if (foodName == "burger")
        {
            burger--;
            recuperateLife(3);
        }

        if (foodName == "stew")
        {
            stew--;
            recuperateLife(2);
        }
        
        if (foodName == "fried_egg")
        {
            fried_egg--;
            recuperateLife(1);
        }
    }
}
