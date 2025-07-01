using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;
    public Vector2 newPlayerPosition;
    private Transform player;

    private void OnTriggerEnter2D(Collider2D collision) {
        player = collision.transform;
        NextScene();
    }

    private void NextScene() {
        player.position = newPlayerPosition;
        SceneManager.LoadScene(this.nextSceneName);
    }
}
