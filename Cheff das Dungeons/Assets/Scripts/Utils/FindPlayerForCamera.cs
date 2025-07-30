
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FindPlayerForCamera : MonoBehaviour
{
    private Transform playerTransform;
    private PolygonCollider2D borderCollider;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        SetupCamera();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupCamera();
    }

    private void SetupCamera()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var confinerObj = GameObject.FindGameObjectWithTag("Confiner");
        if (playerObj != null && confinerObj != null)
        {
            playerTransform = playerObj.transform;
            borderCollider = confinerObj.GetComponent<PolygonCollider2D>();
            GetComponent<CinemachineCamera>().Follow = playerTransform;
            GetComponent<CinemachineConfiner2D>().BoundingShape2D = borderCollider;
        }
    }
}
