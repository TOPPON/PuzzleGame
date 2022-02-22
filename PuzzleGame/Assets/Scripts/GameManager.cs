using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameManager Instance;
    public enum GameState
    {
        GameStart,
        LeftRightSelect,
        BallThrowingAnimation,
        FirstJudge,
        FirstJudgeFallingAnimation,
        RollDirectionSelect,
        RollingAnimation,
        RollingAfterFallingAnimation,
        SecondJudge,
        SecondJudgeFallingAnimation,
        FinalBallThrowingAnimation,
        GameOver
    }
    public GameState gameState;

    float stateTimer;

    private bool pushLeftButton;
    private bool pushRightButton;
    private bool pushLeftTurnButton;
    private bool pushRightTurnButton;
    private bool pushNotTurnButton;
    // Start is called before the first frame update
    /*void Start()
    {
        ChangeGameState(GameState.GameStart);
    }*/
    private void OnLevelWasLoaded(int level)
    {
        if(Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        ChangeGameState(GameState.GameStart);
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.GameStart:
                PuzzleView.Instance.ResetObjectList();
                ChangeGameState(GameState.LeftRightSelect);
                break;
            case GameState.LeftRightSelect:
                if (Input.GetKeyDown(KeyCode.LeftArrow)|pushLeftButton)
                {
                    int line = PuzzleManager.Instance.SelectLine(-1);
                    print("lineは" + line);
                    int row = PuzzleManager.Instance.PutBall(line, PuzzleManager.Instance.nextColor);
                    PuzzleView.Instance.BallThrow(-1, line, row, PuzzleManager.Instance.nextColor);
                    if (row == -1) ChangeGameState(GameState.FinalBallThrowingAnimation);
                    else ChangeGameState(GameState.BallThrowingAnimation);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow)|pushRightButton)
                {
                    int line = PuzzleManager.Instance.SelectLine(1);
                    print("lineは" + line);
                    int row = PuzzleManager.Instance.PutBall(line, PuzzleManager.Instance.nextColor);
                    PuzzleView.Instance.BallThrow(1, line, row, PuzzleManager.Instance.nextColor);
                    if (row == -1) ChangeGameState(GameState.FinalBallThrowingAnimation);
                    else ChangeGameState(GameState.BallThrowingAnimation);
                }
                break;
            case GameState.BallThrowingAnimation:
                if (PuzzleView.Instance.finishThrow)
                {
                    ChangeGameState(GameState.FirstJudge);
                    PuzzleManager.Instance.combo =0;
                    PuzzleManager.Instance.comboRound = 0;
                    PuzzleView.Instance.ResetWall();
                    PuzzleView.Instance.DisplayNextColor(0);//次の色を消す
                }
                break;
            case GameState.FirstJudge:
                List<int> deleteLine1 = PuzzleManager.Instance.JudgeLine();
                if (deleteLine1.Count > 0)
                {
                    PuzzleView.Instance.DeleteBalls(deleteLine1);
                    PuzzleManager.Instance.combo += deleteLine1.Count;
                    PuzzleManager.Instance.comboRound += 1;
                    PuzzleManager.Instance.AddScore(deleteLine1.Count);
                    PuzzleManager.Instance.Fall();
                    ChangeGameState(GameState.FirstJudgeFallingAnimation);
                    PuzzleView.Instance.Fall();
                }
                else 
                {
                    ChangeGameState(GameState.RollDirectionSelect);
                    PuzzleView.Instance.ResetWall();
                }
                break;
            case GameState.FirstJudgeFallingAnimation:
                if (PuzzleView.Instance.finishFall)
                {
                    ChangeGameState(GameState.FirstJudge);
                }
                break;
            case GameState.RollDirectionSelect:
                if (Input.GetKeyDown(KeyCode.Z)|pushLeftTurnButton)
                {
                    PuzzleManager.Instance.Roll(-1);
                    PuzzleView.Instance.RollField(-1);
                    ChangeGameState(GameState.RollingAnimation);
                }
                else if (Input.GetKeyDown(KeyCode.X)|pushNotTurnButton)
                {
                    PuzzleManager.Instance.Roll(0);
                    ChangeGameState(GameState.SecondJudge);
                }
                else if (Input.GetKeyDown(KeyCode.C)| pushRightTurnButton)
                {
                    PuzzleManager.Instance.Roll(1);
                    PuzzleView.Instance.RollField(1);
                    ChangeGameState(GameState.RollingAnimation);
                }
                break;
            case GameState.RollingAnimation:
                if (PuzzleView.Instance.finishRoll)
                {
                    ChangeGameState(GameState.RollingAfterFallingAnimation);
                    PuzzleManager.Instance.combo = 0;
                    PuzzleManager.Instance.comboRound = 0;
                    PuzzleManager.Instance.Fall();
                    PuzzleView.Instance.ResetWall();
                    PuzzleView.Instance.Fall();
                }
                break;
            case GameState.RollingAfterFallingAnimation:
                if(PuzzleView.Instance.finishFall)
                {
                    ChangeGameState(GameState.SecondJudge);
                }
                break;
            case GameState.SecondJudge:
                List<int> deleteLine2 = PuzzleManager.Instance.JudgeLine();
                if (deleteLine2.Count > 0)
                {
                    PuzzleView.Instance.DeleteBalls(deleteLine2);
                    PuzzleManager.Instance.combo += deleteLine2.Count;
                    PuzzleManager.Instance.comboRound += 1;
                    PuzzleManager.Instance.AddScore(deleteLine2.Count);
                    PuzzleManager.Instance.Fall();
                    ChangeGameState(GameState.SecondJudgeFallingAnimation);
                    PuzzleView.Instance.Fall();
                }
                else
                {
                    ChangeGameState(GameState.LeftRightSelect);
                    PuzzleView.Instance.ResetWall();
                }
                break;
            case GameState.SecondJudgeFallingAnimation:
                if (PuzzleView.Instance.finishFall)
                {
                    ChangeGameState(GameState.SecondJudge);
                }
                break;
            case GameState.FinalBallThrowingAnimation:
                stateTimer += Time.deltaTime;
                if(stateTimer>10)
                {
                    ChangeGameState(GameState.GameOver);
                }
                break;
            case GameState.GameOver:
                int rank = RankingManager.Instance.InsertScore(PuzzleManager.Instance.score, PuzzleManager.Instance.round);
                if (rank != 0) RankingManager.Instance.Save();
                SystemManager.Instance.GameOver(rank, PuzzleManager.Instance.score, PuzzleManager.Instance.round);
                break;
        }
        pushLeftButton = false;
        pushRightButton = false;
        pushLeftTurnButton = false;
        pushRightTurnButton = false;
        pushNotTurnButton = false;
    }
    void ChangeGameState(GameState nextState)
    {
        stateTimer = 0;
        PuzzleView.Instance.DisappearOperationButton();
        switch (nextState)
        {
            case GameState.LeftRightSelect:
                PuzzleManager.Instance.ChooseNextColor();
                PuzzleView.Instance.DisplayNextColor(PuzzleManager.Instance.nextColor);
                print("NextColorは" + PuzzleManager.Instance.nextColor);
                if(PuzzleManager.Instance.round%10==0)
                {
                    print("ラウンド数ボーナス:" + PuzzleManager.Instance.round.ToString());
                    PuzzleView.Instance.ScoreChipSpawn(PuzzleManager.Instance.round, true);
                    PuzzleManager.Instance.score += PuzzleManager.Instance.round;
                }
                PuzzleManager.Instance.round++;
                PuzzleView.Instance.AppearInButton();
                break;
            case GameState.RollDirectionSelect:
                PuzzleView.Instance.AppearTurnButton();
                break;
            case GameState.BallThrowingAnimation:
                break;
        }
        gameState = nextState;
    }
    public void PushLeftButton()
    {
        pushLeftButton = true;
    }
    public void PushRightButton()
    {
        pushRightButton = true;
    }
    public void PushTurnLeftButton()
    {
        pushLeftTurnButton = true;
    }
    public void PushTurnRightButton()
    {
        pushRightTurnButton = true;
    }
    public void PushNotTurnButton()
    {
        pushNotTurnButton = true;
    }

}
