using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficultcalculator : MonoBehaviour
{
    //機能：じょうけんを渡されて条件をもとに成功率を算出する
    // Start is called before the first frame update
    public enum ClearMethod
    {
        AllDelete,//
        Lines,//指定されたライン以上作る
        DoubleLine,//n個
        Survive,//生き残る
    }
    public struct Condition
    {
        public int operation;//手順数
        public List<int> nextColor;//手順、operation個必要
        public List<List<int>> initialBoard;//初期盤面
        public ClearMethod clearMethod;//クリア方法
        public int clearBasis;//クリア基準、いくつ作れるか
    }
    void Start()
    {
        for (int ope = 0; ope < 10; ope++)
        {
            Condition condition = new Condition();
            condition.operation = Random.Range(10, 50);
            print("ope:" + condition.operation);
            condition.nextColor = new List<int>();
            for (int i = 0; i < condition.operation; i++)
            {
                int color = Random.Range(1, 3);
                condition.nextColor.Add(color);
            }
            condition.initialBoard = new List<List<int>>();
            for (int i = 0; i < 6; i++)
            {
                List<int> templist = new List<int>();
                string teststring = "";
                for (int j = 0; j < 4; j++)
                {
                    int height = Random.Range(0, 4);
                    if (height > i)
                    {
                        int color = Random.Range(1, 3);
                        templist.Add(color);
                        teststring += color + " ";
                    }
                    else
                    {
                        templist.Add(-1);
                        teststring += "-1 ";
                    }
                }
                condition.initialBoard.Add(templist);
                print(teststring);
            }
            condition.clearBasis =condition.operation/ 4+Random.Range(-9,2);
            print("Basis:"+condition.clearBasis);
            condition.clearMethod = ClearMethod.Lines;
            print(CalcDifficult(condition, 10000));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public double CalcDifficult(Condition condition, int tryNum)
    {
        int success = 0;
        for (int i = 0; i < tryNum; i++)
        {
            switch (condition.clearMethod)
            {
                case ClearMethod.AllDelete:
                    success += CheckAllDeteleClear(condition);
                    break;
                case ClearMethod.Lines:
                    success += CheckLinesClear(condition);
                    break;
                case ClearMethod.DoubleLine:
                    success += CheckDoubleLineClear(condition);
                    break;
                case ClearMethod.Survive:
                    success+= CheckSurviveClear(condition);
                    break;
            }
        }
        return success * 1.0 / tryNum;
    }
    public int CheckAllDeteleClear(Condition condition)
    {
        List<List<int>> board = new List<List<int>>();
        InitialSetBoard(condition, ref board);
        for (int i = 0; i < condition.operation; i++)
        {
            //print("Turn" + (i + 1).ToString());
            //投入
            if (InsertRandom(ref board, condition.nextColor[i]) == -1)
            {
                //ゲームオーバー判定
                return 0;
            }
            //消える判定
            while (true)
            {
                List<int> deleteLine = JudgeLine(ref board);
                if (deleteLine.Count == 0) break;
                //  クリア判定
                if (CheckAllDelete(board)) return 1;
                Fall(ref board);
            }
            //回転
            int rollDirection = Random.Range(-1, 2);
            Roll(ref board, rollDirection);
            Fall(ref board);
            //消える判定
            while (true)
            {
                List<int> deleteLine = JudgeLine(ref board);
                if (deleteLine.Count == 0) break;
                //  クリア判定
                if (CheckAllDelete(board)) return 1;
                Fall(ref board);
            }

            //確認
            //PrintField(board);

        }
        return 0;
    }
    public int CheckLinesClear(Condition condition)
    {
        int sumLine=0;
        List<List<int>> board = new List<List<int>>();
        InitialSetBoard(condition, ref board);
        for (int i = 0; i < condition.operation; i++)
        {
            //print("Turn" + (i + 1).ToString());
            //投入
            if (InsertRandom(ref board, condition.nextColor[i]) == -1)
            {
                //ゲームオーバー判定
                return 0;
            }
            //消える判定
            while (true)
            {
                List<int> deleteLine = JudgeLine(ref board);
                if (deleteLine.Count == 0) break;
                sumLine += deleteLine.Count;
                if (sumLine >= condition.clearBasis) return 1;
                Fall(ref board);
            }
            //回転
            int rollDirection = Random.Range(-1, 2);
            Roll(ref board, rollDirection);
            Fall(ref board);
            //消える判定

            while (true)
            {
                List<int> deleteLine = JudgeLine(ref board);
                if (deleteLine.Count == 0) break;
                sumLine += deleteLine.Count;
                if (sumLine >= condition.clearBasis) return 1;
                Fall(ref board);
            }

            //確認
            //PrintField(board);

        }
        return 0;
    }
    public int CheckDoubleLineClear(Condition condition)
    {
        int doubleLine = 0;
        List<List<int>> board = new List<List<int>>();
        InitialSetBoard(condition, ref board);
        for (int i = 0; i < condition.operation; i++)
        {
            //print("Turn" + (i + 1).ToString());
            //投入
            if (InsertRandom(ref board, condition.nextColor[i]) == -1)
            {
                //ゲームオーバー判定
                return 0;
            }

            //消える判定
            int tempDoubleLine = 0;
            while (true)
            {
                List<int> deleteLine = JudgeLine(ref board);
                if (deleteLine.Count == 0) break;
                tempDoubleLine += deleteLine.Count;
                if (tempDoubleLine >= 2)
                {
                    doubleLine++;
                    if (doubleLine >= condition.clearBasis) return 1;
                }
                Fall(ref board);
            }
            //回転
            int rollDirection = Random.Range(-1, 2);
            Roll(ref board, rollDirection);
            Fall(ref board);
            
            //消える判定
            tempDoubleLine = 0;
            while (true)
            {
                List<int> deleteLine = JudgeLine(ref board);
                if (deleteLine.Count == 0) break;
                tempDoubleLine += deleteLine.Count;
                if (tempDoubleLine >= 2)
                {
                    doubleLine++;
                    if (doubleLine >= condition.clearBasis) return 1;
                }
                Fall(ref board);
            }

            //確認
            //PrintField(board);

        }
        return 0;
    }
    public int CheckSurviveClear(Condition condition)
    {
        List<List<int>> board = new List<List<int>>();
        InitialSetBoard(condition, ref board);
        for (int i = 0; i < condition.operation; i++)
        {
            //print("Turn" + (i + 1).ToString());
            //投入
            if (InsertRandom(ref board, condition.nextColor[i]) == -1)
            {
                //ゲームオーバー判定
                return 0;
            }
            //消える判定
            while (true)
            {
                List<int> deleteLine = JudgeLine(ref board);
                if (deleteLine.Count == 0) break;
                Fall(ref board);
            }
            //回転
            int rollDirection = Random.Range(-1, 2);
            Roll(ref board, rollDirection);
            Fall(ref board);
            //消える判定

            while (true)
            {
                List<int> deleteLine = JudgeLine(ref board);
                if (deleteLine.Count == 0) break;
                Fall(ref board);
            }

            //確認
            //PrintField(board);

        }
        return 1;
    }
    
    private int InsertRandom(ref List<List<int>> board, int color)
    {
        int line = Random.Range(0, 4);
        for (int i = 5; i >= 0; i--)
        {
            if (board[i][line] > 0)//上から見て行って入っているものがあれば一個上に配置
            {
                if (i == 5)//ゲームオーバー
                {
                    return -1;
                }
                else
                {
                    board[i + 1][line] = color;
                    return i + 1;
                }
            }
        }
        board[0][line] = color;
        return 0;
    }
    private void Rotate(ref List<List<int>> board, int rotateDict)
    {
        return;
    }
    private void InitialSetBoard(Condition condition, ref List<List<int>> board)
    {
        for (int i = 0; i < 6; i++)
        {
            List<int> templist = new List<int>();
            for (int j = 0; j < 4; j++)
            {
                if (i >= 4) templist.Add(-1);
                templist.Add(condition.initialBoard[i][j]);
            }
            board.Add(templist);
        }
    }
    private List<int> JudgeLine(ref List<List<int>> board)
    {
        //y,x 50 03
        List<int> deleteLine = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (board[0][0] == board[i][i] & board[i][i] != -1)
            {
                if (i == 4 - 1)
                {
                    deleteLine.Add(100);
                    //print("右上斜めに消えた");
                }
            }
            else break;
        }
        for (int i = 0; i < 4; i++)
        {
            if (board[0][4 - 1] == board[i][4 - 1 - i] & board[i][4 - 1 - i] != -1)
            {
                if (i == 4 - 1)
                {
                    deleteLine.Add(200);
                    //print("左上斜めに消えた");
                }
            }
            else break;
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (board[i][0] == board[i][j] & board[i][j] != -1)
                {
                    if (j == 4 - 1)
                    {
                        deleteLine.Add((i + 1) * 10);
                        //print("横に消えた");
                    }
                }
                else break;
            }
            for (int j = 0; j < 4; j++)
            {
                if (board[0][i] == board[j][i] & board[j][i] != -1)
                {
                    if (j == 4 - 1)
                    {
                        deleteLine.Add(i + 1);
                        //print("縦に消えた");
                    }
                }
                else break;
            }
        }
        foreach (int temp in deleteLine)
        {
            if (temp == 100)
            {
                for (int i = 0; i < 4; i++)
                {
                    board[i][i] = -1;
                }
            }
            else if (temp == 200)
            {
                for (int i = 0; i < 4; i++)
                {
                    board[i][4 - 1 - i] = -1;
                }
            }
            else if (temp % 10 == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    board[temp / 10 - 1][i] = -1;
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    board[i][temp - 1] = -1;
                }
            }
        }
        return deleteLine;
    }
    private bool CheckAllDelete(List<List<int>> board)
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (board[i][j] != -1)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void Fall(ref List<List<int>> board)
    {
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 6; i++)
            {
                if (board[i][j] > 0 & i > 0)//下から見て何かあれば一個下にずらす
                {
                    if (board[i - 1][j] < 0)
                    {
                        board[i - 1][j] = board[i][j];
                        board[i][j] = -1;
                        i -= 2;
                    }
                }
            }
        }
        // print("Falled\n");
        //PrintField(puzzleField);
    }
    public void Roll(ref List<List<int>> board, int rollDirection)//-1:左 0:真ん中 1:右 
    {
        //print("Rolling\n");
        //PrintField(board);
        List<List<int>> rolledboard = new List<List<int>>();
        for (int i = 0; i < 4; i++)
        {
            List<int> tempList = new List<int>();
            for (int j = 0; j < 4; j++)
            {
                tempList.Add(-1);
            }
            rolledboard.Add(tempList);
        }
        for (int i = 4; i < 6; i++)
        {
            rolledboard.Add(board[i]);
        }
        switch (rollDirection)
        {
            case -1://左回転
                for (int i = 0; i < 4; i++)
                {
                    int count = 0;
                    for (int j = 4 - 1; j >= 0; j--)
                    {
                        if (board[i][j] > 0)
                        {
                            rolledboard[count][i] = board[i][j];
                            count++;
                        }
                    }
                }
                board = rolledboard;
                break;
            case 0:
                break;
            case 1:
                for (int i = 0; i < 4; i++)
                {
                    int count = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        if (board[i][j] > 0)
                        {
                            rolledboard[count][4 - i - 1] = board[i][j];
                            count++;
                        }
                    }
                }
                board = rolledboard;
                break;
        }
        //print("Rolled\n");
        //PrintField(board);
    }

    private void PrintField(List<List<int>> board)
    {
        print("--------------------");
        for (int i = 5; i >= 0; i--)
        {
            string a = "";
            for (int j = 0; j < 4; j++)
            {
                a = a + board[i][j].ToString() + " ";
            }
            print(a);
        }
        print("--------------------");
    }
}
