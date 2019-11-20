using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Board gameBoard;

    [SerializeField]
    private Spawner spawner;

    private Shape activeShape;

    private float dropInterval = 0.1f;

    private float timeToDrop = 1f;

    private void Start()
    {
        gameBoard = GameObject.FindObjectOfType<Board>();
        spawner = GameObject.FindObjectOfType<Spawner>();

        if (spawner)
        {
            if (activeShape == null)
            {
                activeShape = spawner.SpawnShape();
            }

            spawner.transform.position = Vectorf.Round(spawner.transform.position);
        }

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
        if (!gameBoard || !spawner)
        {
            return;
        }

        if (Time.time > timeToDrop)
        {
            timeToDrop = Time.time + dropInterval;

            if (activeShape)
            {
                activeShape.MoveDown();

                if (!gameBoard.IsValidPosition(activeShape))
                {
                    activeShape.MoveUp();
                    gameBoard.StoreShapeInGrid(activeShape);

                    if (spawner)
                    {
                        activeShape = spawner.SpawnShape();
                    }
                }
            }

        }
    }
}
