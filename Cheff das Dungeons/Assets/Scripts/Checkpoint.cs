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
                enableDisableFoods();
                PlayerController.Instance.isNotCrafting = false;
            }

            if (burger.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlayerController.Instance.makeBurger();
                enableDisableFoods();
            }

            if (stew.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlayerController.Instance.makeStew();
                enableDisableFoods();
            }

            if (fried_egg.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlayerController.Instance.makeFried_egg();
                enableDisableFoods();
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
            PlayerController.Instance.isNotCrafting = true;
        }
    }

    //Ativa/Desativa as comidas de acordo com os ingredientes disponíveis
    private void enableDisableFoods()
    {
        if (PlayerController.Instance.egg > 0)
        {
            fried_egg.gameObject.SetActive(true);
        }
        else
        {
            fried_egg.gameObject.SetActive(false);
        }

        if (PlayerController.Instance.meat > 0 & PlayerController.Instance.slime > 0)
        {
            stew.gameObject.SetActive(true);
        }
        else
        {
            stew.gameObject.SetActive(false);
        }

        if (PlayerController.Instance.meat > 0 & PlayerController.Instance.slime > 0 & PlayerController.Instance.egg > 0)
        {
            burger.gameObject.SetActive(true);
        }
        else
        {
            burger.gameObject.SetActive(false);
        }
    }
}
