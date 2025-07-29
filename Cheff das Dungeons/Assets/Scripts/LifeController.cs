using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeController : MonoBehaviour
{
    public int maxLifes;
    public int currentLife;
    public Sprite lifeIconFull;
    public Sprite lifeIconEmpty;
    public Image[] lifes;

    public PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentLife = player.currentLife;
        maxLifes = player.maxLifes;

        for (int i = 0; i < lifes.Length; i++)
        {
            if (i < currentLife)
            {
                lifes[i].sprite = lifeIconFull;
            }
            else
            {
                lifes[i].sprite = lifeIconEmpty;
            }

            if (i < maxLifes)
            {
                lifes[i].gameObject.SetActive(true);
            }
            else
            {
                lifes[i].gameObject.SetActive(false);
            }
        }
    }
}
