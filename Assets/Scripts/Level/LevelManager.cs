using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { set; get; } 


    [Header("Border Colliders")]
    public Transform borderParent;
    public float colDepth;

    [Header("Level")]
    [SerializeField] GameObject[] levelPrefabs;
    [SerializeField] LevelCreator levelCreator;
    LevelCreator lc;
    [HideInInspector] public Level currentLevel;
    public List<BlockData> blockDatasCustom;

    DataManager dataManager;

    private void Start()
    {
        dataManager = DataManager.Instance;
        Instance = this;

        SpawnLevel();
        GenerateBorderColliders();
    }

    public void RespawnLevel(bool isCustom)
    {
        Destroy(currentLevel);

        lc = Instantiate(levelCreator, transform);
        if (isCustom)
        {
            lc.CreateLevel(blockDatasCustom);
        }
        else
        {
            lc.CreateLevel();
        }

        StartCoroutine(SetCurrentLevelAfterLevelCreation());
    }

    public void SpawnLevel()
    {
        currentLevel = Instantiate(levelPrefabs[dataManager.data.levelToPlay - 1], transform).GetComponent<Level>();
        currentLevel.InitGrid();
        UIManager.Instance.ChangeLevelUI(dataManager.data.level);
    }
    private IEnumerator SetCurrentLevelAfterLevelCreation()
    {
        yield return null; //To skip this frame and continue working on the next frame
        currentLevel = lc.levelScript;
        currentLevel.transform.parent = transform;
        Destroy(lc.gameObject);
        currentLevel.InitGrid();
        UIManager.Instance.ChangeLevelUI(dataManager.data.level);
    }

    public void IncreaseLevel()
    {
        var data = dataManager.data;
        data.level++;
        int currentLevelToPlay = data.levelToPlay;
        data.levelToPlay = data.level > levelPrefabs.Length ? Random.Range(2, levelPrefabs.Length - 1) : data.level;
        data.levelToPlay = data.levelToPlay == currentLevelToPlay ? data.levelToPlay++ : data.levelToPlay;

        SaveSystem.SaveData();
    }

    public void GenerateBorderColliders()
    {
        Transform topCollider = new GameObject("TopCollider").transform;
        Transform bottomCollider = new GameObject("BottomCollider").transform;
        Transform rightCollider = new GameObject("RightCollider").transform;
        Transform leftCollider = new GameObject("LeftCollider").transform;

        topCollider.parent = borderParent;
        bottomCollider.parent = borderParent;
        rightCollider.parent = borderParent;
        leftCollider.parent = borderParent;

        bottomCollider.tag = "BottomWall";

        topCollider.gameObject.AddComponent<BoxCollider2D>();
        bottomCollider.gameObject.AddComponent<BoxCollider2D>();
        rightCollider.gameObject.AddComponent<BoxCollider2D>();
        leftCollider.gameObject.AddComponent<BoxCollider2D>();

        //Generate world space point information for position and scale calculations
        Vector3 cameraPos = Camera.main.transform.position;
        Vector2 screenSize;
        screenSize.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
        screenSize.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;

        //Change the scales and positions to match the edges of the screen
        rightCollider.localScale = new Vector3(colDepth, screenSize.y * 2, 1);
        rightCollider.position = new Vector3(cameraPos.x + screenSize.x + (rightCollider.localScale.x * 0.5f), cameraPos.y, 0);

        leftCollider.localScale = new Vector3(colDepth, screenSize.y * 2, 1);
        leftCollider.position = new Vector3(cameraPos.x - screenSize.x - (leftCollider.localScale.x * 0.5f), cameraPos.y, 0);

        topCollider.localScale = new Vector3(screenSize.x * 2, colDepth, 1);
        topCollider.position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (topCollider.localScale.y * 0.5f), 0);

        bottomCollider.localScale = new Vector3(screenSize.x * 2, colDepth, 1);
        bottomCollider.position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (bottomCollider.localScale.y * 0.5f), 0);
    }
}
