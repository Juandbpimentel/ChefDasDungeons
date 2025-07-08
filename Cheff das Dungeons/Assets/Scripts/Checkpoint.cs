using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint1 : MonoBehaviour
{   
    [SerializeField]
    private GameManager gm;

    void OnTriggerEnter2D(Collider2D other) 
        {
            if(other.CompareTag("Player"))
            {
                gm.firePos = transform.position;
                gm.sceneName = SceneManager.GetActiveScene().name;
            }
        }
}
