using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GameState
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
    GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        ChangeGameState(GameState.LeftRightSelect);
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameState)
        {
            case GameState.GameStart:
                break;
            case GameState.LeftRightSelect:
                if(Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    int line = PuzzleManager.Instance.SelectLine(-1);
                    PuzzleManager.Instance.PutBall(line, PuzzleManager.Instance.nextColor);
                    PuzzleView.Instance.BallThrow(-1, line, PuzzleManager.Instance.nextColor);
                    ChangeGameState(GameState.BallFallingAnimation);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    int line=PuzzleManager.Instance.SelectLine(1);
                    PuzzleManager.Instance.PutBall(line,PuzzleManager.Instance.nextColor);
                    PuzzleView.Instance.BallThrow(1, line, PuzzleManager.Instance.nextColor);//親オブジェクトに引っ付ける作業がまだです。
                    ChangeGameState(GameState.BallFallingAnimation);
                }
                break;
            case GameState.BallFallingAnimation://枠の当たり判定をいじるところがまだです。
                break;
            case GameState.FirstJudge://判定部分も作れていません。判定後落として再判定する部分も作れてないです。
                break;
            case GameState.RollDirectionSelect:
                break;
            case GameState.RollingAnimation:
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
                break;
            case GameState.BallFallingAnimation:
                PuzzleView.Instance
                break;
        }
        gameState = nextState;
    }
}
