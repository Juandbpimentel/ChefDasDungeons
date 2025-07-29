using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;
    public Vector2 newPlayerPosition;
    private Transform player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        player = collision.transform;
        NextScene();
    }

    private void NextScene()
    {
        player.position = newPlayerPosition;
        // primeiro, apaga a cena atual
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);
        // depois, carrega a próxima cena
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Next scene name is not set.");
            return;
        }
        SceneManager.LoadScene(this.nextSceneName);
    }
}
