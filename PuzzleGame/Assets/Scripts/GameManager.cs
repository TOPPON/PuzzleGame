using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    // Start is called before the first frame update
    void Start()
    {
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
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    int line = PuzzleManager.Instance.SelectLine(-1);
                    print("lineは" + line);
                    int row = PuzzleManager.Instance.PutBall(line, PuzzleManager.Instance.nextColor);
                    PuzzleView.Instance.BallThrow(-1, line, row, PuzzleManager.Instance.nextColor);
                    if (row == -1) ChangeGameState(GameState.FinalBallThrowingAnimation);
                    else ChangeGameState(GameState.BallThrowingAnimation);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    int line = PuzzleManager.Instance.SelectLine(1);
                    print("lineは" + line);
                    int row = PuzzleManager.Instance.PutBall(line, PuzzleManager.Instance.nextColor);
                    PuzzleView.Instance.BallThrow(1, line, row, PuzzleManager.Instance.nextColor);//親オブジェクトに引っ付ける作業がまだです。
                    if (row == -1) ChangeGameState(GameState.FinalBallThrowingAnimation);
                    else ChangeGameState(GameState.BallThrowingAnimation);
                }
                break;
            case GameState.BallThrowingAnimation://枠の当たり判定をいじるところがまだです。
                if (PuzzleView.Instance.finishThrow)
                {
                    ChangeGameState(GameState.FirstJudge);
                    PuzzleView.Instance.ResetWall();
                }
                break;
            case GameState.FirstJudge://判定部分も作れていません。判定後落として再判定する部分も作れてないです。
                List<int> deleteLine1 = PuzzleManager.Instance.JudgeLine();
                if (deleteLine1.Count > 0)
                {
                    PuzzleView.Instance.DeleteBalls(deleteLine1);
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
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    PuzzleManager.Instance.Roll(-1);
                    PuzzleView.Instance.RollField(-1);
                    ChangeGameState(GameState.RollingAnimation);
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    PuzzleManager.Instance.Roll(0);
                    ChangeGameState(GameState.SecondJudge);
                }
                else if (Input.GetKeyDown(KeyCode.C))
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
                break;
            case GameState.GameOver:
                break;
        }
    }
    void ChangeGameState(GameState nextState)
    {
        switch (nextState)
        {
            case GameState.LeftRightSelect:
                PuzzleManager.Instance.ChooseNextColor();
                PuzzleView.Instance.DisplayNextColor(PuzzleManager.Instance.nextColor);
                print("NextColorは" + PuzzleManager.Instance.nextColor);
                break;
            case GameState.BallThrowingAnimation:
                break;
        }
        gameState = nextState;
    }
}
