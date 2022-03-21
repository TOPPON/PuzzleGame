using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Text highscore;
    // Start is called before the first frame update
    void Start()
    {
        highscore.text = "HIGHSCORE\n" + PlayerPrefs.GetInt("FirstScore", 0);
    }
    private void OnLevelWasLoaded(int level)
    {
        highscore.text = "HIGHSCORE\n" + PlayerPrefs.GetInt("FirstScore", 0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void PushStartButton()
    {
        SystemManager.Instance.PushStart();
    }
}
