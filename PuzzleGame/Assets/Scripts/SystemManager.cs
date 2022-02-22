using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    public static SystemManager Instance;
    int recentRank;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GameOver(int rank,int score,int round)
    {
        SceneManager.LoadScene("ResultScene");
        recentRank = rank;
    }
    public void PushStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
