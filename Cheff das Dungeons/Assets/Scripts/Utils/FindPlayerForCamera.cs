using Unity.Cinemachine;
using UnityEngine;

public class FindPlayerForCamera : MonoBehaviour
{
    private Transform playerTransform;
    private PolygonCollider2D borderCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        borderCollider = GameObject.FindGameObjectWithTag("Confiner").GetComponent<PolygonCollider2D>();
        GetComponent<CinemachineCamera>().Follow = playerTransform;
        GetComponent<CinemachineConfiner2D>().BoundingShape2D = borderCollider;
    }
}
