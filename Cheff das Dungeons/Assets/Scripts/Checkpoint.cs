using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint1 : MonoBehaviour
{   
    void OnTriggerEnter2D(Collider2D other) 
        {
            if(other.CompareTag("Player"))
            {
                GameManager.Instance.lastCheckpointPos = transform.position;
                GameManager.Instance.sceneName = SceneManager.GetActiveScene().name;
            }
        }
}
