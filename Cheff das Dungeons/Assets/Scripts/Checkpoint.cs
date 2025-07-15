using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint1 : MonoBehaviour
{   
    private GameManager gm;

    void OnTriggerEnter2D(Collider2D other) 
        {
            gm = GameObject.FindWithTag("GM").GetComponent<GameManager>();
            if(other.CompareTag("Player"))
            {
                gm.firePos = transform.position;
                gm.sceneName = SceneManager.GetActiveScene().name;
            }
        }
}
