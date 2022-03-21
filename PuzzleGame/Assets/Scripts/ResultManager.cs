using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField] Text yourScore;
    [SerializeField] Text yourRound;
    [SerializeField] Image resultShadowBox;
    [SerializeField] Image rankingShadowBox;
    [SerializeField] Button homeButton;
    [SerializeField] Button retryButton;
    [SerializeField] GameObject rankIn;
    [SerializeField] GameObject rankingManager;

    private float timer;
    public enum SceneState
    {
        ScoreAnim,
        ScoreOpen,
        RankAnim,
        AllOpen
    }
    SceneState sceneState;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnLevelWasLoaded(int level)
    {
        timer = 0;
        sceneState = SceneState.ScoreAnim;
        yourScore.text = "SCORE " + PuzzleManager.Instance.score;
        yourRound.text = "ROUND " + PuzzleManager.Instance.round;
        RankingManager.Instance = rankingManager.GetComponent<RankingManager>();
        RankingManager.Instance.Load();
        int rank = RankingManager.Instance.InsertScore(PuzzleManager.Instance.score, PuzzleManager.Instance.round);
        if (rank != 0) RankingManager.Instance.Save();
        switch (rank)
        {
            case 0://ランクインなし
                rankIn.SetActive(false);
                break;
            case 1:
                rankIn.GetComponent<RectTransform>().localPosition = new Vector3(rankIn.GetComponent<RectTransform>().localPosition.x, 105);
                break;
            case 2:
                rankIn.GetComponent<RectTransform>().localPosition = new Vector3(rankIn.GetComponent<RectTransform>().localPosition.x, -45);
                break;
            case 3:
                rankIn.GetComponent<RectTransform>().localPosition = new Vector3(rankIn.GetComponent<RectTransform>().localPosition.x, -195);
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        switch (sceneState)
        {
            case SceneState.ScoreAnim:
                resultShadowBox.color = new Color(resultShadowBox.color.r, resultShadowBox.color.g, resultShadowBox.color.b, 1 - timer * 2);
                if (timer > 0.5f)
                {
                    sceneState = SceneState.ScoreOpen;
                    resultShadowBox.color = new Color(resultShadowBox.color.r, resultShadowBox.color.g, resultShadowBox.color.b, 0);
                    timer = 0;
                }
                break;
            case SceneState.ScoreOpen:
                if (timer > 0.5f)
                {
                    sceneState = SceneState.RankAnim;
                    timer = 0;
                }
                break;
            case SceneState.RankAnim:
                rankingShadowBox.color = new Color(rankingShadowBox.color.r, rankingShadowBox.color.g, rankingShadowBox.color.b, 1 - timer * 2);
                if (timer > 0.5f)
                {
                    sceneState = SceneState.AllOpen;
                    rankingShadowBox.color = new Color(rankingShadowBox.color.r, rankingShadowBox.color.g, rankingShadowBox.color.b, 0);
                }
                break;
            case SceneState.AllOpen:
                rankIn.GetComponent<Text>().color = new Color(rankIn.GetComponent<Text>().color.r, rankIn.GetComponent<Text>().color.g, rankIn.GetComponent<Text>().color.b, Mathf.Sin(timer * 10) * 0.5f + 0.5f);
                break;
        }
    }
    public void PushRetry()
    {
        SystemManager.Instance.PushRetry();
    }
    public void PushHome()
    {
        SystemManager.Instance.PushHome();
    }
}
