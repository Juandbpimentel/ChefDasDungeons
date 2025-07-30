using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;
    public Vector2 newPlayerPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || PlayerController.Instance.isTeleporting) return;
        PlayerController.Instance.isTeleporting = true;
        NextScene();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.Instance.isTeleporting = false;
        }
    }


    private void NextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Next scene name is not set.");
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null && GameManager.Instance != null)
        {
            // Tenta pegar o player persistente do GameManager
            playerObj = System.Array.Find(GameManager.Instance.persistentObjects, obj =>
                obj != null && obj.GetComponent<PlayerController>() != null);
        }
        if (playerObj != null)
        {
            playerObj.transform.position = newPlayerPosition;
            SceneManager.MoveGameObjectToScene(playerObj, scene);
            DontDestroyOnLoad(playerObj);
        }
        else
        {
            Debug.LogWarning("Player não encontrado na nova cena!");
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
