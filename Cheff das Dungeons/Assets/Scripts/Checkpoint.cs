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

    bool isCrafting = false;

    public TextMeshProUGUI withoutIngredientsText;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered checkpoint");
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.lastCheckpointPos = transform.position;
            GameManager.Instance.sceneName = SceneManager.GetActiveScene().name;
            interactText.enabled = true;
            other.GetComponent<PlayerController>().inCheckpoint = true;
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
            withoutIngredientsText.enabled = false;
            PlayerController.Instance.isCrafting = false;
            PlayerController.Instance.inCheckpoint = false;
        }
    }

    //Ativa/Desativa as comidas de acordo com os ingredientes disponíveis
    public void enableDisableFoods()
    {
        isCrafting = PlayerController.Instance.isCrafting;
        bool hasMeat = PlayerController.Instance.meat > 0;
        bool hasSlime = PlayerController.Instance.slime > 0;
        bool hasEgg = PlayerController.Instance.egg > 0;

        fried_egg.gameObject.SetActive(hasEgg && isCrafting);
        stew.gameObject.SetActive(hasMeat && hasSlime && isCrafting);
        burger.gameObject.SetActive(hasMeat && hasSlime && hasEgg && isCrafting);
        withoutIngredientsText.enabled = !hasSlime && !hasEgg && isCrafting;
    }
}
