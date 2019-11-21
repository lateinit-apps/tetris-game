using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject gameOverPanel;

    private bool gameOver = false;

    private SoundManager soundManager;

    public IconToggle rotateIconToggle;

    private bool rotateClockwise = true;

    private void Start()
    {
        gameBoard = GameObject.FindObjectOfType<Board>();
        spawner = GameObject.FindObjectOfType<Spawner>();
        soundManager = GameObject.FindObjectOfType<SoundManager>();

        timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
        timeToNextKeyRotate = Time.time + keyRepeatRateRotate;
        timeToNextKeyDown = Time.time + keyRepeatRateDown;

        if (!gameBoard)
        {
            Debug.Log("WARNING! There is no game board defined!");
        }

        if (!soundManager)
        {
            Debug.Log("WARNING! There is no sound manager defined!");
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

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(false);
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
                PlaySound(soundManager.errorSound, 0.5f);
            }
            else
            {
                PlaySound(soundManager.moveSound, 0.5f);
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
                PlaySound(soundManager.errorSound, 0.5f);
            }
            else
            {
                PlaySound(soundManager.moveSound, 0.5f);
            }
        }
        else if (Input.GetButtonDown("Rotate") && Time.time > timeToNextKeyRotate)
        {
            activeShape.RotateClockwise(rotateClockwise);
            timeToNextKeyRotate = Time.time + keyRepeatRateRotate;

            if (!gameBoard.IsValidPosition(activeShape))
            {
                activeShape.RotateClockwise(!rotateClockwise);
                PlaySound(soundManager.errorSound, 0.5f);
            }
            else
            {
                PlaySound(soundManager.moveSound, 0.5f);
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
                    if (gameBoard.IsOverLimit(activeShape))
                    {
                        GameOver();
                    }
                    else
                    {
                        LandShape();
                    }
                }
            }
        }
        else if (Input.GetButtonDown("ToggleRotation"))
        {
            ToggleRotationDirection();
        }
    }

    private void GameOver()
    {
        activeShape.MoveUp();

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
        }

        PlaySound(soundManager.gameOverSound, 5f);
        PlaySound(soundManager.gameOverVocalClip, 5f);

        gameOver = true;
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

        PlaySound(soundManager.dropSound, 0.75f);

        if (gameBoard.completedRows > 0)
        {
            if (gameBoard.completedRows > 1)
            {
                AudioClip randomVocal = soundManager.GetRandomClip(soundManager.vocalClips);
                PlaySound(randomVocal);
            }

            PlaySound(soundManager.clearRowSound);
        }
    }

    private void PlaySound(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip && soundManager.fxEnabled)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position,
                                        Mathf.Clamp(soundManager.fxVolume * volumeMultiplier, 0.05f, 1f));
        }
    }

    private void Update()
    {
        if (!gameBoard || !spawner || !activeShape || gameOver || !soundManager)
        {
            return;
        }

        PlayerInput();
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void ToggleRotationDirection()
    {
        rotateClockwise = !rotateClockwise;

        if (rotateIconToggle)
        {
            rotateIconToggle.ToggleIcon(rotateClockwise);
        }
    }
}
