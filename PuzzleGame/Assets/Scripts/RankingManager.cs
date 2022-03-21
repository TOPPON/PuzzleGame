using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;
    //(int score, int round) result = (0,0);
    List<(int score, int round)> ranking = new List<(int score, int round)>();
    [SerializeField] Text rank1Score;
    [SerializeField] Text rank1Round;
    [SerializeField] Text rank2Score;
    [SerializeField] Text rank2Round;
    [SerializeField] Text rank3Score;
    [SerializeField] Text rank3Round;
    // Start is called before the first frame update
    /*private void OnLevelWasLoaded(int level)
    {
        if (Instance == null)
        {
            Instance = this;
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.S)) 
        {
            print("Saved,rank:" + RankingManager.Instance.InsertScore(PuzzleManager.Instance.score, PuzzleManager.Instance.round));
            Save();
        }*/
    }

    public int InsertScore(int score, int round)
    {
        int count = 0;
        foreach ((int score, int round) rank in ranking)
        {
            if (rank.score < score)
            {
                ranking.Insert(count, (score, round));
                ranking.RemoveAt(3);
                RankTextAdjust();
                return count + 1;
            }
            count++;
        }
        if (count != 3)
        {
            ranking.Add((score, round));
            RankTextAdjust();
            return count + 1;
        }
        else
        {
            RankTextAdjust();
            return 0;
        }
    }
    public void Save()
    {
        if (ranking.Count >= 3)
        {
            PlayerPrefs.SetInt("ThirdScore", ranking[2].score);
            PlayerPrefs.SetInt("ThirdRound", ranking[2].round);
        }
        if (ranking.Count >= 2)
        {
            PlayerPrefs.SetInt("SecondScore", ranking[1].score);
            PlayerPrefs.SetInt("SecondRound", ranking[1].round);
        }
        if (ranking.Count >= 1)
        {
            PlayerPrefs.SetInt("FirstScore", ranking[0].score);
            PlayerPrefs.SetInt("FirstRound", ranking[0].round);
        }
        PrintRanking();
    }
    public void Load()
    {
        ranking.Clear();
        ranking.Add((PlayerPrefs.GetInt("FirstScore", 0), PlayerPrefs.GetInt("FirstRound", 0)));
        ranking.Add((PlayerPrefs.GetInt("SecondScore", 0), PlayerPrefs.GetInt("SecondRound", 0)));
        ranking.Add((PlayerPrefs.GetInt("ThirdScore", 0), PlayerPrefs.GetInt("ThirdRound", 0)));
        PrintRanking();
    }
    public void PrintRanking()
    {
        int count = 0;
        foreach ((int score, int round) rank in ranking)
        {
            count++;
            print("RANK" + count + " : " + rank.score + " - " + rank.round);
        }
    }
    private void RankTextAdjust()
    {
        if (ranking.Count >= 3)
        {
            rank3Score.text = "" + ranking[2].score;
            rank3Round.text = "" + ranking[2].round;
        }
        if (ranking.Count >= 2)
        {
            rank2Score.text = "" + ranking[1].score;
            rank2Round.text = "" + ranking[1].round;
        }
        if (ranking.Count >= 1)
        {
            rank1Score.text = "" + ranking[0].score;
            rank1Round.text = "" + ranking[0].round;
        }
    }
}
