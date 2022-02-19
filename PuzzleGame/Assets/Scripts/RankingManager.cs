using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;
    //(int score, int round) result = (0,0);
    List<(int score, int round)> ranking = new List<(int score, int round)>();
    //Ranking[] ranking = new Ranking[3];
    // Start is called before the first frame update
    void Start()
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
    }

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
                return count + 1;
            }
            count++;
        }
        if (count != 3)
        {
            ranking.Add((score, round));
            return count + 1;
        }
        else return 0;
    }
    public void Save()
    {
        PlayerPrefs.SetInt("FirstScore", ranking[0].score);
        PlayerPrefs.SetInt("FirstRound", ranking[0].round);
        PlayerPrefs.SetInt("SecondScore", ranking[1].score);
        PlayerPrefs.SetInt("SecondRound", ranking[1].round);
        PlayerPrefs.SetInt("ThirdScore", ranking[2].score);
        PlayerPrefs.SetInt("ThirdRound", ranking[2].round);
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
}
