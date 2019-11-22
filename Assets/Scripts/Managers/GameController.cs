using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class GameController : MonoBehaviour
{
    private Board gameBoard;
    private Spawner spawner;
    private Shape activeShape;
    private Ghost ghost;
    private Holder holder;

    private SoundManager soundManager;
    private ScoreManager scoreManager;

    private float dropInterval = 1f;
    private float dropIntervalModded;

    private float timeToDrop;
    private float timeToNextKeyLeftRight;
    private float timeToNextKeyDown;
    private float timeToNextKeyRotate;

    [Range(0.02f, 1)]
    public float keyRepeatRateLeftRight = 0.1f;

    [Range(0.02f, 1)]
    public float keyRepeatRateDown = 0.05f;

    [Range(0.02f, 1)]
    public float keyRepeatRateRotate = 0.05f;

    public GameObject pausePanel;
    public GameObject gameOverPanel;

    public IconToggle rotateIconToggle;
    public ParticlePlayer gameOverFx;

    private bool gameOver = false;
    private bool rotateClockwise = true;

    public bool isPaused = false;

    private enum Direction { none, left, right, up, down }

    private Direction swipeDirection = Direction.none;
    private Direction swipeEndDirection = Direction.none;

    private void OnEnable()
    {
        TouchController.SwipeEvent += SwipeHandler;
        TouchController.SwipeEndEvent += SwipeEndHandler;
    }

    private void OnDisable()
    {
        TouchController.SwipeEvent -= SwipeHandler;
        TouchController.SwipeEndEvent -= SwipeEndHandler;
    }

    private void Start()
    {
        gameBoard = GameObject.FindObjectOfType<Board>();
        spawner = GameObject.FindObjectOfType<Spawner>();

        ghost = GameObject.FindObjectOfType<Ghost>();
        holder = GameObject.FindObjectOfType<Holder>();

        soundManager = GameObject.FindObjectOfType<SoundManager>();
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

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

        if (!scoreManager)
        {
            Debug.Log("WARNING! There is no score manager defined!");
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

        if (pausePanel)
        {
            pausePanel.SetActive(false);
        }

        dropIntervalModded = dropInterval;
    }

    private void MoveRight()
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

    private void MoveLeft()
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

    private void Rotate()
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

    private void MoveDown()
    {
        timeToDrop = Time.time + dropIntervalModded;
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

    private void PlayerInput()
    {
        if (!gameBoard || !spawner)
        {
            return;
        }

        if ((Input.GetButton("MoveRight") && Time.time > timeToNextKeyLeftRight) ||
             Input.GetButtonDown("MoveRight"))
        {
            MoveRight();
        }
        else if ((Input.GetButton("MoveLeft") && Time.time > timeToNextKeyLeftRight) ||
                  Input.GetButtonDown("MoveLeft"))
        {
            MoveLeft();
        }
        else if (Input.GetButtonDown("Rotate") && Time.time > timeToNextKeyRotate)
        {
            Rotate();
        }
        else if ((Input.GetButton("MoveDown") && Time.time > timeToNextKeyDown) ||
                  Time.time > timeToDrop)
        {
            MoveDown();
        }
        else if ((swipeDirection == Direction.right && Time.time > timeToNextKeyLeftRight) ||
                  swipeEndDirection == Direction.right)
        {
            MoveRight();

            swipeDirection = Direction.none;
            swipeEndDirection = Direction.none;
        }
        else if ((swipeDirection == Direction.left && Time.time > timeToNextKeyLeftRight) ||
                  swipeEndDirection == Direction.left)
        {
            MoveLeft();

            swipeDirection = Direction.none;
            swipeEndDirection = Direction.none;
        }
        else if (swipeEndDirection == Direction.up)
        {
            Rotate();

            swipeEndDirection = Direction.none;
        }
        else if (swipeDirection == Direction.down && Time.time > timeToNextKeyDown)
        {
            MoveDown();

            swipeDirection = Direction.none;
        }
        else if (Input.GetButtonDown("ToggleRotation"))
        {
            ToggleRotationDirection();
        }
        else if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
        else if (Input.GetButtonDown("Hold"))
        {
            Hold();
        }
    }

    private void GameOver()
    {
        activeShape.MoveUp();

        StartCoroutine(GameOverRoutine());

        PlaySound(soundManager.gameOverSound, 5f);
        PlaySound(soundManager.gameOverVocalClip, 5f);

        gameOver = true;
    }

    private IEnumerator GameOverRoutine()
    {
        if (gameOverFx)
        {
            gameOverFx.Play();
        }

        yield return new WaitForSeconds(0.5f);

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void LandShape()
    {
        timeToNextKeyLeftRight = Time.time;
        timeToNextKeyRotate = Time.time;
        timeToNextKeyDown = Time.time;

        activeShape.MoveUp();
        gameBoard.StoreShapeInGrid(activeShape);

        if (ghost)
        {
            ghost.Reset();
        }

        if (holder)
        {
            holder.canRelease = true;
        }

        activeShape = spawner.SpawnShape();

        gameBoard.StartCoroutine(gameBoard.ClearAllRows());

        PlaySound(soundManager.dropSound, 0.75f);

        if (gameBoard.completedRows > 0)
        {
            scoreManager.ScoreLines(gameBoard.completedRows);

            if (scoreManager.didLevelUp)
            {
                PlaySound(soundManager.levelUpVocalClip);
                dropIntervalModded = dropInterval - Mathf.Clamp(
                        (((float)scoreManager.level - 1) * 0.1f), 0.05f, 1f);
            }
            else
            {
                if (gameBoard.completedRows > 1)
                {
                    AudioClip randomVocal = soundManager.GetRandomClip(soundManager.vocalClips);
                    PlaySound(randomVocal);
                }
            }

            PlaySound(soundManager.clearRowSound);
        }
    }

    private void PlaySound(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip && soundManager.fxEnabled)
        {
            AudioSource.PlayClipAtPoint(
                clip, Camera.main.transform.position,
                Mathf.Clamp(soundManager.fxVolume * volumeMultiplier, 0.05f, 1f));
        }
    }

    private void Update()
    {
        if (!gameBoard || !spawner || !activeShape || gameOver || !soundManager || !scoreManager)
        {
            return;
        }

        PlayerInput();
    }

    private void LateUpdate()
    {
        if (ghost)
        {
            ghost.DrawGhost(activeShape, gameBoard);
        }
    }

    private void SwipeHandler(Vector2 swipeMovement)
    {
        swipeDirection = GetDirection(swipeMovement);
    }

    private void SwipeEndHandler(Vector2 swipeMovement)
    {
        swipeEndDirection = GetDirection(swipeMovement);
    }

    private Direction GetDirection(Vector2 swipeMovement)
    {
        Direction swipeDirection = Direction.none;

        if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
        {
            swipeDirection = swipeMovement.x >= 0 ? Direction.right : Direction.left;
        }
        else
        {
            swipeDirection = swipeMovement.y >= 0 ? Direction.up : Direction.down;
        }

        return swipeDirection;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
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

    public void TogglePause()
    {
        if (gameOver)
        {
            return;
        }

        isPaused = !isPaused;

        if (pausePanel)
        {
            pausePanel.SetActive(isPaused);

            if (soundManager)
            {
                soundManager.musicSource.volume =
                    isPaused ? soundManager.musicVolume * 0.25f : soundManager.musicVolume;
            }

            Time.timeScale = isPaused ? 0 : 1;
        }
    }

    public void Hold()
    {
        if (!holder)
        {
            return;
        }

        if (!holder.heldShape)
        {
            holder.Catch(activeShape);
            activeShape = spawner.SpawnShape();
            PlaySound(soundManager.holdSound);
        }
        else if (holder.canRelease)
        {
            Shape shape = activeShape;
            activeShape = holder.Release();
            activeShape.transform.position = spawner.transform.position;
            holder.Catch(shape);
            PlaySound(soundManager.holdSound);
        }
        else
        {
            PlaySound(soundManager.errorSound);
        }

        if (ghost)
        {
            ghost.Reset();
        }
    }
}
