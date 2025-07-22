using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{
    public TextMeshProUGUI interactText;
    public Image burger;
    public Image stew;
    public Image fried_egg;
    void OnTriggerEnter2D(Collider2D other) 
        {
            if(other.CompareTag("Player"))
            {
                GameManager.Instance.lastCheckpointPos = transform.position;
                GameManager.Instance.sceneName = SceneManager.GetActiveScene().name;
            }
        }
}
