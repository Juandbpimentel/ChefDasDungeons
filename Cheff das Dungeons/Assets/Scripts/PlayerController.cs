using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private List<Image> lifes;

    private int maxLifes = 5;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

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
            GameManager.Instance.Respaw();
        }
    }

    private void FixedUpdate()
    {
        Vector3 movePosition = (speed * Time.fixedDeltaTime * moveDirection.normalized) + rb.position;
        rb.MovePosition(movePosition);
    }

    private void initLifes(int max)
    {
        for (int i = 0; i <= max; i++)
        {
            //instancia nova imagem
            Image newImage = Instantiate(lifes[^1], lifes[^1].transform.parent);
            lifes.Add(newImage);

            //definir a posicao
            Vector3 newPosition = lifes[^1].GetComponent<RectTransform>().anchoredPosition;
            newPosition.x += -80; // desloca no eixo X
            newImage.GetComponent<RectTransform>().anchoredPosition = newPosition;

            newImage.name = "life" + i;
        }
    }
}
