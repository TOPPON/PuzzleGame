using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public class Ranking
    {
        int score;
        int round;
        Ranking(int newScore,int newRound)
        {
            this.score = newScore;
            this.round = newRound;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
