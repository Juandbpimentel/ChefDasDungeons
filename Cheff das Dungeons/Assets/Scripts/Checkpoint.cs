using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Checkpoint : MonoBehaviour
{
    public TextMeshProUGUI interactText;
    public Image burger;
    public Image stew;
    public Image fried_egg;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.lastCheckpointPos = transform.position;
            GameManager.Instance.sceneName = SceneManager.GetActiveScene().name;
            interactText.enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                burger.gameObject.SetActive(true);
                stew.gameObject.SetActive(true);
                fried_egg.gameObject.SetActive(true);
            }
        }
    }
            
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactText.enabled = false;
            burger.gameObject.SetActive(false);
            stew.gameObject.SetActive(false);
            fried_egg.gameObject.SetActive(false);
        }
    }
}
