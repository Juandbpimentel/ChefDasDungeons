using UnityEngine;

public class LevelController : MonoBehaviour
{
    public int inimigoController = 0;

    public Door entryDoor;
    public Door exitDoor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (entryDoor == null || exitDoor == null)
        {
            Debug.LogError("Entry or Exit door is not set in the LevelController.");
            return;
        }

        // Set the player's initial position at the entry door
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = entryDoor.newPlayerPosition;
        }
        else
        {
            Debug.LogError("Player object not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
