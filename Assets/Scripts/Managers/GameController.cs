using UnityEngine;

public class GameController : MonoBehaviour
{
    private Board gameBoard;

    private Spawner spawner;

    private void Start()
    {
        gameBoard = GameObject.FindObjectOfType<Board>();
        spawner = GameObject.FindObjectOfType<Spawner>();

        if (!gameBoard)
        {
            Debug.Log("WARNING! There is no game board defined!");
        }

        if (!spawner)
        {
            Debug.Log("WARNING! There is no spawner defined!");
        }
    }

    private void Update()
    {

    }
}
