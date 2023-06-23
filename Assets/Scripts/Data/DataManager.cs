using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { set; get; }

    public GameData data;

    private void Start()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        SaveSystem.LoadData();

        SceneManager.LoadScene("Game");
    }
}