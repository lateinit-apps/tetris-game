using UnityEngine;

public class GameController : MonoBehaviour
{
    private Board gameBoard;
    private Spawner spawner;
    private Shape activeShape;

    private float dropInterval = 1f;
    private float timeToDrop;

    private float timeToNextKeyLeftRight;

    [Range(0.02f, 1)]
    public float keyRepeatRateLeftRight = 0.1f;

    private float timeToNextKeyDown;

    [Range(0.02f, 1)]
    public float keyRepeatRateDown = 0.05f;

    private float timeToNextKeyRotate;

    [Range(0.02f, 1)]
    public float keyRepeatRateRotate = 0.05f;

    private void Start()
    {
        gameBoard = GameObject.FindObjectOfType<Board>();
        spawner = GameObject.FindObjectOfType<Spawner>();

        timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
        timeToNextKeyRotate = Time.time + keyRepeatRateRotate;
        timeToNextKeyDown = Time.time + keyRepeatRateDown;

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
            if (activeShape == null)
            {
                activeShape = spawner.SpawnShape();
            }

            spawner.transform.position = Vectorf.Round(spawner.transform.position);
        }
    }

    private void PlayerInput()
    {
        if (!gameBoard || !spawner)
        {
            return;
        }

        if (Input.GetButton("MoveRight") &&
            Time.time > timeToNextKeyLeftRight || Input.GetButtonDown("MoveRight"))
        {
            activeShape.MoveRight();
            timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;

            if (!gameBoard.IsValidPosition(activeShape))
            {
                activeShape.MoveLeft();
            }
        }
        else if (Input.GetButton("MoveLeft") &&
                 Time.time > timeToNextKeyLeftRight || Input.GetButtonDown("MoveLeft"))
        {
            activeShape.MoveLeft();
            timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;

            if (!gameBoard.IsValidPosition(activeShape))
            {
                activeShape.MoveRight();
            }
        }
        else if (Input.GetButtonDown("Rotate") && Time.time > timeToNextKeyRotate)
        {
            activeShape.RotateRight();
            timeToNextKeyRotate = Time.time + keyRepeatRateRotate;

            if (!gameBoard.IsValidPosition(activeShape))
            {
                activeShape.RotateLeft();
            }
        }
        else if (Input.GetButton("MoveDown") &&
                 Time.time > timeToNextKeyDown || Time.time > timeToDrop)
        {
            timeToDrop = Time.time + dropInterval;
            timeToNextKeyDown = Time.time + keyRepeatRateDown;

            if (activeShape)
            {
                activeShape.MoveDown();

                if (!gameBoard.IsValidPosition(activeShape))
                {
                    LandShape();
                }
            }

        }
    }

    private void LandShape()
    {
        timeToNextKeyLeftRight = Time.time;
        timeToNextKeyRotate = Time.time;
        timeToNextKeyDown = Time.time;

        activeShape.MoveUp();
        gameBoard.StoreShapeInGrid(activeShape);

        activeShape = spawner.SpawnShape();

        gameBoard.ClearAllRows();
    }

    private void Update()
    {
        if (!gameBoard || !spawner || !activeShape)
        {
            return;
        }

        PlayerInput();
    }
}
