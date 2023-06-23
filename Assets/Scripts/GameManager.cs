using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public GamePhase gamePhase = GamePhase.MainMenu;
    public bool isBallWaiting = true;
    public int playerLives;
    public int score;

    [Space(20)]
    [Header("ScriptReferances")]
    [SerializeField] UIManager uiManager;
    [SerializeField] Player player;
    public Ball ball;
    DataManager dataManager;


    private void Awake()
    {
        Instance = this;
        if (DataManager.Instance != null)
        {
            dataManager = DataManager.Instance;
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
    private void Start()
    {
        ball.ResetBall();
    }
    private void Update()
    {
        if (gamePhase == GamePhase.MainMenu && Input.GetKeyDown(KeyCode.Space))
        {
            uiManager.PlayButton();
        }
        if (gamePhase == GamePhase.InGame && isBallWaiting && Input.GetKey(KeyCode.Space))
        {
            ball.SendRandomDirection();
            isBallWaiting = false;
        }
        if (gamePhase == GamePhase.GameOver && Input.GetKeyDown(KeyCode.Space))
        {
            uiManager.NextButton();
        }
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        uiManager.ChangeScoreUI(scoreToAdd);
    }
    public void GameOver(bool didWeWin)
    {
        gamePhase = GamePhase.GameOver;
        Destroy(ball.gameObject);

        uiManager.ChangeGameOverMenuVisuals(didWeWin);
        uiManager.ChangeMenu("GameOverMenu");

        if (didWeWin)
        {
            LevelManager.Instance.IncreaseLevel();
        }
    }

    public void BallHitFloor(Ball currentBall)
    {
        if (playerLives > 0)
        {
            playerLives--;
            uiManager.ChangeLivesUI(-1);
            isBallWaiting = true;
            currentBall.ResetBall();
        }
        else
        {
            GameOver(false);
        }
    }
}

public enum GamePhase {MainMenu, InGame, GameOver}

public class Extentions
{
    /// <summary>
    /// Remaps the input value within the specified range and returns the remapped value within the second specified range as a float.
    /// </summary>
    /// <param name="value">The value you want to be calculated in the second range</param>
    /// <param name="from1">Minimum value of the first range</param>
    /// <param name="to1">Maximum value of the first range</param>
    /// <param name="from2">Minimum value of the second range</param>
    /// <param name="to2">Maximum value of the second range</param>
    /// <returns>A float within the second range based on the given value </returns>
    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /// <summary>Converts a decimal number to a string representation with a suffix indicating the scale of the number.</summary>
    /// <param name="num">The decimal number to convert</param>
    /// <returns>A string representation of the input number with a suffix indicating its scale</returns>
    public static string ToKMB(decimal num)
    {
        if (num > 999999999999 || num < -999999999999)
        {
            return num.ToString("0,,,,.#T", CultureInfo.InvariantCulture);
        }
        else if (num > 999999999 || num < -999999999)
        {
            return num.ToString("0,,,.#B", CultureInfo.InvariantCulture);
        }
        else if (num > 999999 || num < -999999)
        {
            return num.ToString("0,,.#M", CultureInfo.InvariantCulture);
        }
        else if (num > 999 || num < -999)
        {
            return num.ToString("0,.#K", CultureInfo.InvariantCulture);
        }
        else
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }
    }
}