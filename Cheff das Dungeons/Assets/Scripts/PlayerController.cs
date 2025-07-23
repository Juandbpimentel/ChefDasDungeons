using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private List<Image> lifes;

    private int maxLifes = 5;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private int egg = 0;
    [SerializeField]
    public TextMeshProUGUI eggText;

    private int meat = 0;
    [SerializeField]
    public TextMeshProUGUI meatText;

    private int slime = 0;
    [SerializeField]
    public TextMeshProUGUI slimeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initLifes(maxLifes);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(horizontal, vertical);

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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Egg"))
        {
            egg++;
            Destroy(other.gameObject);
            eggText.text = egg.ToString();
        }
    }

    private void initLifes(int max)
    {
        for (int i = 1; i < max; i++)
        {
            //instancia nova imagem
            Image newImage = Instantiate(lifes[^1], lifes[^1].transform.parent);
            lifes.Add(newImage);

            //definir a posicao
            Vector3 newPosition = lifes[^1].GetComponent<RectTransform>().anchoredPosition;
            newPosition.x += 80; // desloca no eixo X
            newImage.GetComponent<RectTransform>().anchoredPosition = newPosition;

            newImage.name = "" + i;
        }
    }

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
                    // respawnar player
                    GameManager.Instance.Respaw();
                    recuperateLife(maxLifes);
                }
                //sair
                return;
            }
        }
    }

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
}
