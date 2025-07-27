using UnityEngine;

public class Arqueiro : MonoBehaviour
{
    // Variáveis e métodos específicos do inimigo podem ser adicionados aqui
    public int vidaMaxima = 5;
    public float speed = 2f;
    private int vida;
    
    void Start()
    {
        vida = vidaMaxima;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
