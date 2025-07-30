using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Vector3 lastCheckpointPos;
    public string sceneName;

    [Header("Persistent Objects")]
    public GameObject[] persistentObjects;

    [SerializeField] private GameObject playerPrefab;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Apenas destrua o novo GameManager, NÃO os objetos persistentes!
            Destroy(gameObject);
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneName = SceneManager.GetActiveScene().name;
        // Se não existe player, crie um novo
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null && playerPrefab != null)
        {
            player = Instantiate(playerPrefab, lastCheckpointPos, Quaternion.identity);
            DontDestroyOnLoad(player);
        }
        else if (player != null)
        {
            // Move o player para a cena ativa, se quiser que ele apareça na hierarquia da cena
            DontDestroyOnLoad(player);
            player.transform.position = lastCheckpointPos;
        }
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
