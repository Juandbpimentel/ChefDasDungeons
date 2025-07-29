using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Vector3 lastCheckpointPos;
    public string sceneName;

    [Header("Persistent Objects")]
    public GameObject[] persistentObjects;

    private void Start()
    {
        Instance = this;
        sceneName = SceneManager.GetActiveScene().name;
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            CleanUpAndDestroy();
            return;
        }

        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObjects();
        }
    }

    private void MarkPersistentObjects()
    {
        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    private void CleanUpAndDestroy()
    {
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }
        Destroy(gameObject);
    }

    public void Respaw()
    {
        SceneManager.LoadSceneAsync(sceneName);
        GameObject player = System.Array.Find(persistentObjects, obj =>
                obj != null && obj.GetComponent<PlayerController>() != null);
        if (player != null)
        {
            player.transform.position = lastCheckpointPos;
            return;
        }
    }
}
