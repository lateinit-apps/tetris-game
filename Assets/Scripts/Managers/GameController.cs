using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Board gameBoard;

    [SerializeField]
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
        else
        {
            spawner.transform.position = Vectorf.Round(spawner.transform.position);
        }
    }

    private void Update()
    {

    }
}
