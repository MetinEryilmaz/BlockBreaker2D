using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { set; get; }

    [SerializeField] List<GameObject> menus;

    [SerializeField] Text scoreText;
    [SerializeField] Text livesText;
    [SerializeField] Animator scoreAnim;
    [SerializeField] Animator livesAnim;

    [SerializeField] Text nextButtonText;
    [SerializeField] Animator liveLostEffectAnim;
    [SerializeField] Animator endScoreAnim;
    [SerializeField] Text endScoreText;
    [SerializeField] GameObject newHighScore;
    [SerializeField] Text highScoreText;

    [SerializeField] Text levelText;

    [Space(30)]
    [Header("Settings Menu")]
    [Space(10)]
    [SerializeField] Slider verticalKiller_spawnProb_slider;
    [SerializeField] InputField verticalKiller_spawnProb_inputField;

    [SerializeField] Slider verticalKiller_health_slider;
    [SerializeField] InputField verticalKiller_health_inputField;

    [SerializeField] Slider verticalKiller_neighbourAmount_slider;
    [SerializeField] InputField verticalKiller_neighbourAmount_inputField;

    [Space(10)]

    [SerializeField] Slider horizontalKiller_spawnProb_slider;
    [SerializeField] InputField horizontalKiller_spawnProb_inputField;

    [SerializeField] Slider horizontalKiller_health_slider;
    [SerializeField] InputField horizontalKiller_health_inputField;

    [SerializeField] Slider horizontalKiller_neighbourAmount_slider;
    [SerializeField] InputField horizontalKiller_neighbourAmount_inputField;

    [Space(10)]

    [SerializeField] Slider neighbourKiller_spawnProb_slider;
    [SerializeField] InputField neighbourKiller_spawnProb_inputField;

    [SerializeField] Slider neighbourKiller_health_slider;
    [SerializeField] InputField neighbourKiller_health_inputField;

    [SerializeField] Slider neighbourKiller_neighbourAmount_slider;
    [SerializeField] InputField neighbourKiller_neighbourAmount_inputField;

    [Space(10)]
    [SerializeField] Slider normal_health_slider;
    [SerializeField] InputField normal_health_inputField;
    [Space(10)]
    [SerializeField] Slider ball_damage_slider;
    [SerializeField] InputField ball_damage_inputField;

    [Space(10)]
    [SerializeField] Slider levelCustomizerButtonSlider;
    [SerializeField] Image levelCustomizerButtonHandle;
    [SerializeField] Color levelCustomizerOpenColor;
    [SerializeField] Color levelCustomizerClosedColor;
    [SerializeField] Vector2 levelCustomizerOpenPosition;
    [SerializeField] Vector2 levelCustomizerClosedPosition;

    [Space(10)]
    [SerializeField] RectTransform settingsMenu;
    [SerializeField] GameObject settingsLongBG;
    [SerializeField] GameObject settingsShortBG;
    [SerializeField] GameObject levelCustomizer;

    [Space(10)]
    [SerializeField] RectTransform saveButton;
    [SerializeField] RectTransform savedTransform;
    [SerializeField] Text savedText;
    [SerializeField] Vector2 saveButton_levelCustomizerOpenPosition;
    [SerializeField] Vector2 saveButton_levelCustomizerClosedPosition;

    bool isLevelCustomizerActive = false;

    [Space(30)]
    [Header("ScriptReferances")]
    [SerializeField] GameManager gameManager;
    DataManager dataManager;
    LevelManager levelManager;

    private void Awake()
    {
        Instance = this;
        dataManager = DataManager.Instance;

        ChangeMenu("MainMenu");
    }
    private void Start()
    {
        levelManager = LevelManager.Instance;
    }

    #region ChangeUI
    public void ChangeScoreUI(int scoreToAdd)
    {
        scoreText.text = Extentions.ToKMB(gameManager.score);
        scoreAnim.Play("ScoreTextChange");
    }
    public void ChangeLivesUI(int livesToAdd)
    {
        livesText.text = gameManager.playerLives.ToString();
        livesAnim.Play("LivesTextChange");
        if (livesToAdd < 0)
        {
            liveLostEffectAnim.Play("LiveLostEffect");
        }
    }
    public void ChangeLevelUI(int level)
    {
        levelText.text = level.ToString();
    }
    #endregion

    /// <summary>Opens the given named ui menu and closes the rest. If any of the menus has the given name, all of them will be closed</summary>
    /// <param name="menuObjectName">the ui menu's name that you want to open</param>
    public void ChangeMenu(string menuObjectName)
    {
        for (int i = 0; i < menus.Count; i++)
        {
            if (menus[i].gameObject.name == menuObjectName)
            {
                menus[i].SetActive(true);
            }
            else
            {
                menus[i].SetActive(false);
            }
        }
    }
    public void ChangeGameOverMenuVisuals(bool didWeWin)
    {
        if (didWeWin)
        {
            nextButtonText.text = "CONTINUE";
        }
        else
        {
            nextButtonText.text = "RETRY";
        }
        endScoreText.text = Extentions.ToKMB(gameManager.score);
        highScoreText.text = "HIGH SCORE: " + Extentions.ToKMB(dataManager.data.highScore);

        if (gameManager.score >= dataManager.data.highScore)
        {
            newHighScore.SetActive(true);
            highScoreText.gameObject.SetActive(false);

            dataManager.data.highScore = gameManager.score;
            SaveSystem.SaveData();
        }
    }

    #region Buttons
    public void PlayButton()
    {
        gameManager.gamePhase = GamePhase.InGame;
        ChangeMenu("InGameMenu");
        ChangeScoreUI(0);
        ChangeLivesUI(0);
    }
    public void NextButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void SettingsMenuButton()
    {
        ChangeMenu("SettingsMenu");
    }
    public void LevelCustomizerButton()
    {
        if (isLevelCustomizerActive)
        {
            isLevelCustomizerActive = false;
            levelCustomizer.SetActive(false);
            levelCustomizerButtonSlider.value = 0;
            levelCustomizerButtonHandle.color = levelCustomizerClosedColor;
            settingsLongBG.SetActive(false);
            settingsShortBG.SetActive(true);
            settingsMenu.DOLocalMove(levelCustomizerClosedPosition, 0.5f);
            saveButton.DOLocalMove(saveButton_levelCustomizerClosedPosition, 0.2f);
        }
        else
        {
            isLevelCustomizerActive = true;
            levelCustomizer.SetActive(true);
            levelCustomizerButtonSlider.value = 1;
            levelCustomizerButtonHandle.color = levelCustomizerOpenColor;
            settingsLongBG.SetActive(true);
            settingsShortBG.SetActive(false);
            settingsMenu.DOLocalMove(levelCustomizerOpenPosition, 0.5f);
            saveButton.DOLocalMove(saveButton_levelCustomizerOpenPosition, 0.2f);
        }
    }

    public void SaveAndCloseButton()
    {
        if (isLevelCustomizerActive)
        {
            levelManager.RespawnLevel(true);
            savedText.text = "CUSTOM LEVEL SPAWNED";
            savedTransform.gameObject.SetActive(true);
            savedTransform.DOScale(Vector3.one, 1).OnComplete(() =>
            {
                ChangeMenu("MainMenu");
                savedTransform.gameObject.SetActive(false);
            });
        }
        else
        {
            levelManager.RespawnLevel(false);
            savedText.text = "PRE-GENERATED LEVEL SPAWNED";
            savedTransform.gameObject.SetActive(true);
            savedTransform.DOScale(Vector3.one, 1).OnComplete(() =>
            {
                ChangeMenu("MainMenu");
                savedTransform.gameObject.SetActive(false);
            });
        }
    }

    #endregion

    #region LevelCustomizerUI()
    //VerticalKiller
    public void VerticalKillerProbSliderChange()
    {
        verticalKiller_spawnProb_inputField.text = verticalKiller_spawnProb_slider.value.ToString();
        levelManager.blockDatasCustom[0].spawnProbability = (int)verticalKiller_spawnProb_slider.value;
    }
    public void VerticalKillerProbInputFieldChange()
    {
        verticalKiller_spawnProb_inputField.text = Mathf.Clamp(int.Parse(verticalKiller_spawnProb_inputField.text), 0, verticalKiller_spawnProb_slider.maxValue).ToString();
        verticalKiller_spawnProb_slider.value = int.Parse(verticalKiller_spawnProb_inputField.text);
        levelManager.blockDatasCustom[0].spawnProbability = (int)verticalKiller_spawnProb_slider.value;
    }
    public void VerticalKillerHealthSliderChange()
    {
        verticalKiller_health_inputField.text = ((int)verticalKiller_health_slider.value).ToString();
        levelManager.blockDatasCustom[0].health = (int)verticalKiller_health_slider.value;
    }
    public void VerticalKillerHealthInputFieldChange()
    {
        verticalKiller_health_inputField.text = Mathf.Clamp(int.Parse(verticalKiller_health_inputField.text), 0, verticalKiller_health_slider.maxValue).ToString();
        verticalKiller_health_slider.value = int.Parse(verticalKiller_health_inputField.text);
        levelManager.blockDatasCustom[0].health = (int)verticalKiller_health_slider.value;
    }
    public void VerticalKillerNeighbourSliderChange()
    {
        verticalKiller_neighbourAmount_inputField.text = ((int)verticalKiller_neighbourAmount_slider.value).ToString();
        levelManager.blockDatasCustom[0].health = (int)verticalKiller_neighbourAmount_slider.value;
    }
    public void VerticalKillerNeighbourInputFieldChange()
    {
        verticalKiller_neighbourAmount_inputField.text = Mathf.Clamp(int.Parse(verticalKiller_neighbourAmount_inputField.text), 0, verticalKiller_neighbourAmount_slider.maxValue).ToString();
        verticalKiller_neighbourAmount_slider.value = int.Parse(verticalKiller_neighbourAmount_inputField.text);
        levelManager.blockDatasCustom[0].health = (int)verticalKiller_neighbourAmount_slider.value;
    }

    //HorizontalKiller
    public void HorizontalKillerProbSliderChange()
    {
        horizontalKiller_spawnProb_inputField.text = horizontalKiller_spawnProb_slider.value.ToString();
        levelManager.blockDatasCustom[1].spawnProbability = (int)horizontalKiller_spawnProb_slider.value;
    }
    public void HorizontalKillerProbInputFieldChange()
    {
        horizontalKiller_spawnProb_inputField.text = Mathf.Clamp(int.Parse(horizontalKiller_spawnProb_inputField.text), 0, horizontalKiller_spawnProb_slider.maxValue).ToString();
        horizontalKiller_spawnProb_slider.value = int.Parse(horizontalKiller_spawnProb_inputField.text);
        levelManager.blockDatasCustom[1].spawnProbability = (int)horizontalKiller_spawnProb_slider.value;
    }
    public void HorizontalKillerHealthSliderChange()
    {
        horizontalKiller_health_inputField.text = ((int)horizontalKiller_health_slider.value).ToString();
        levelManager.blockDatasCustom[1].health = (int)horizontalKiller_health_slider.value;
    }
    public void HorizontalKillerHealthInputFieldChange()
    {
        horizontalKiller_health_inputField.text = Mathf.Clamp(int.Parse(horizontalKiller_health_inputField.text), 0, horizontalKiller_health_slider.maxValue).ToString();
        horizontalKiller_health_slider.value = int.Parse(horizontalKiller_health_inputField.text);
        levelManager.blockDatasCustom[1].health = (int)horizontalKiller_health_slider.value;
    }
    public void HorizontalKillerNeighbourSliderChange()
    {
        horizontalKiller_neighbourAmount_inputField.text = ((int)horizontalKiller_neighbourAmount_slider.value).ToString();
        levelManager.blockDatasCustom[1].health = (int)horizontalKiller_neighbourAmount_slider.value;
    }
    public void HorizontalKillerNeighbourInputFieldChange()
    {
        horizontalKiller_neighbourAmount_inputField.text = Mathf.Clamp(int.Parse(horizontalKiller_neighbourAmount_inputField.text), 0, horizontalKiller_neighbourAmount_slider.maxValue).ToString();
        horizontalKiller_neighbourAmount_slider.value = int.Parse(horizontalKiller_neighbourAmount_inputField.text);
        levelManager.blockDatasCustom[1].health = (int)horizontalKiller_neighbourAmount_slider.value;
    }

    //NeighbourKiller
    public void NeighbourKillerProbSliderChange()
    {
        neighbourKiller_spawnProb_inputField.text = neighbourKiller_spawnProb_slider.value.ToString();
        levelManager.blockDatasCustom[2].spawnProbability = (int)neighbourKiller_spawnProb_slider.value;
    }
    public void NeighbourKillerProbInputFieldChange()
    {
        neighbourKiller_spawnProb_inputField.text = Mathf.Clamp(int.Parse(neighbourKiller_spawnProb_inputField.text), 0, neighbourKiller_spawnProb_slider.maxValue).ToString();
        neighbourKiller_spawnProb_slider.value = int.Parse(neighbourKiller_spawnProb_inputField.text);
        levelManager.blockDatasCustom[2].spawnProbability = (int)neighbourKiller_spawnProb_slider.value;
    }
    public void NeighbourKillerHealthSliderChange()
    {
        neighbourKiller_health_inputField.text = ((int)neighbourKiller_health_slider.value).ToString();
        levelManager.blockDatasCustom[2].health = (int)neighbourKiller_health_slider.value;
    }
    public void NeighbourKillerHealthInputFieldChange()
    {
        neighbourKiller_health_inputField.text = Mathf.Clamp(int.Parse(neighbourKiller_health_inputField.text), 0, neighbourKiller_health_slider.maxValue).ToString();
        neighbourKiller_health_slider.value = int.Parse(neighbourKiller_health_inputField.text);
        levelManager.blockDatasCustom[2].health = (int)neighbourKiller_health_slider.value;
    }

    //Normal
    public void NormalHealthSliderChange()
    {
        normal_health_inputField.text = ((int)normal_health_slider.value).ToString();
        levelManager.blockDatasCustom[3].health = (int)normal_health_slider.value;
    }
    public void NormalHealthInputFieldChange()
    {
        normal_health_inputField.text = Mathf.Clamp(int.Parse(normal_health_inputField.text), 0, normal_health_slider.maxValue).ToString();
        normal_health_slider.value = int.Parse(normal_health_inputField.text);
        levelManager.blockDatasCustom[3].health = (int)normal_health_slider.value;
    }

    //Ball Damage
    public void BallDamageSliderChange()
    {
        ball_damage_inputField.text = ((int)ball_damage_slider.value).ToString();
        gameManager.ball.damage = (int)ball_damage_slider.value;
    }
    public void BallDamageInputFieldChange()
    {
        ball_damage_inputField.text = Mathf.Clamp(int.Parse(ball_damage_inputField.text), 0, ball_damage_slider.maxValue).ToString();
        ball_damage_slider.value = int.Parse(ball_damage_inputField.text);
        gameManager.ball.damage = (int)ball_damage_slider.value;
    }
    #endregion
}