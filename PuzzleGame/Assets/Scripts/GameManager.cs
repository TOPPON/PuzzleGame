using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        GameStart,
        LeftRightSelect,
        BallFallingAnimation,
        FirstJudge,
        RollDirectionSelect,
        RollingAnimation,
        SecondJudge,
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
        switch(gameState)
        {
            case GameState.GameStart:
                ChangeGameState(GameState.LeftRightSelect);
                break;
            case GameState.LeftRightSelect:
                if(Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    int line = PuzzleManager.Instance.SelectLine(-1);
                    print("lineは" + line);
                    PuzzleManager.Instance.PutBall(line, PuzzleManager.Instance.nextColor);
                    PuzzleView.Instance.BallThrow(-1, line, PuzzleManager.Instance.nextColor);
                    ChangeGameState(GameState.BallFallingAnimation);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    int line=PuzzleManager.Instance.SelectLine(1);
                    print("lineは" + line);
                    PuzzleManager.Instance.PutBall(line,PuzzleManager.Instance.nextColor);
                    PuzzleView.Instance.BallThrow(1, line, PuzzleManager.Instance.nextColor);//親オブジェクトに引っ付ける作業がまだです。
                    ChangeGameState(GameState.BallFallingAnimation);
                }
                break;
            case GameState.BallFallingAnimation://枠の当たり判定をいじるところがまだです。
                if(PuzzleView.Instance.finishFall)
                {
                    ChangeGameState(GameState.RollDirectionSelect);
                    PuzzleView.Instance.ResetWall();
                }
                break;
            case GameState.FirstJudge://判定部分も作れていません。判定後落として再判定する部分も作れてないです。

                break;
            case GameState.RollDirectionSelect:
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PuzzleManager.Instance.Roll(-1);
                    PuzzleView.Instance.RollField(-1);
                    ChangeGameState(GameState.RollingAnimation);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    PuzzleManager.Instance.Roll(1);
                    PuzzleView.Instance.RollField(1);
                    ChangeGameState(GameState.RollingAnimation);
                }
                break;
            case GameState.RollingAnimation:
                if (PuzzleView.Instance.finishRoll)
                {
                    ChangeGameState(GameState.LeftRightSelect);
                    PuzzleView.Instance.ResetWall();
                    PuzzleView.Instance.BallFall();
                }
                break;
            case GameState.SecondJudge:
                break;
            case GameState.GameOver:
                break;
        }
    }
    void ChangeGameState(GameState nextState)
    {
        switch(nextState)
        {
            case GameState.LeftRightSelect:
                PuzzleManager.Instance.ChooseNextColor();
                PuzzleView.Instance.DisplayNextColor(PuzzleManager.Instance.nextColor);
                print("NextColorは" + PuzzleManager.Instance.nextColor);
                break;
            case GameState.BallFallingAnimation:
                break;
        }
        gameState = nextState;
    }
}
