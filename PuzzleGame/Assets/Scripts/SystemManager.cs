using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    public static SystemManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 2;
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
    public void GameOver()
    {
        SceneManager.LoadScene("ResultScene");
    }
    public void PushStart()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void PushRetry()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void PushHome()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
