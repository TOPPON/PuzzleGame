using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    List<List<int>> puzzleField = new List<List<int>>();//-1:空、0:未使用 1:色1 2:色2 3:色3
    public int Size { get; private set; } = 4;
    public int Exheight { get; private set; } = 1;
    int Colornum = 2;
    public int nextColor;
    public int score;//点数
    public int combo;//何個連鎖したか
    public int round;//ラウンド数、基本スコアになる
    public int comboRound;//一回の操作での連鎖数(Fallが呼ばれた回数分)

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        for (int i = 0; i < Size + Exheight; i++)
        {
            List<int> tempList = new List<int>();
            for (int j = 0; j < Size; j++)
            {
                tempList.Add(-1);
            }
            puzzleField.Add(tempList);
        }
    }
    /*
     x=0,y=size+ex-1 x=1,
     
     x=0,y=1 x=1,y=1   
     x=0,y=0 x=1,y=0
         */
    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Roll(1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Roll(-1);
        }
        */
    }
    public void Roll(int rollDirection)//-1:左 0:真ん中 1:右 
    {
        print("Rolling\n");
        PrintField(puzzleField);
        List<List<int>> rolledPuzzleField = new List<List<int>>();
        for (int i = 0; i < Size; i++)
        {
            List<int> tempList = new List<int>();
            for (int j = 0; j < Size; j++)
            {
                tempList.Add(-1);
            }
            rolledPuzzleField.Add(tempList);
        }
        for (int i = Size; i < Size + Exheight; i++)
        {
            rolledPuzzleField.Add(puzzleField[i]);
        }
        switch (rollDirection)
        {
            case -1://左回転
                for (int i = 0; i < Size; i++)
                {
                    int count = 0;
                    for (int j = Size - 1; j >= 0; j--)
                    {
                        if (puzzleField[i][j] > 0)
                        {
                            rolledPuzzleField[count][i] = puzzleField[i][j];
                            count++;
                        }
                    }
                }
                puzzleField = rolledPuzzleField;
                break;
            case 0:
                break;
            case 1:
                for (int i = 0; i < Size; i++)
                {
                    int count = 0;
                    for (int j = 0; j < Size; j++)
                    {
                        if (puzzleField[i][j] > 0)
                        {
                            rolledPuzzleField[count][Size - i - 1] = puzzleField[i][j];
                            count++;
                        }
                    }
                }
                puzzleField = rolledPuzzleField;
                break;
        }
        print("Rolled\n");
        PrintField(puzzleField);
    }
    public int PutBall(int line, int color)
    {
        for (int i = Size + Exheight - 1; i >= 0; i--)
        {
            if (puzzleField[i][line] > 0)//上から見て行って入っているものがあれば一個上に配置
            {
                if (i == Size + Exheight - 1)//ゲームオーバー
                {
                    return -1;
                }
                else
                {
                    puzzleField[i + 1][line] = color;
                    return i + 1;
                }
            }
        }
        puzzleField[0][line] = color;
        return 0;
    }
    void PrintField(List<List<int>> field)
    {
        print("--------------------");
        for (int i = Size +Exheight- 1; i >= 0; i--)
        {
            string a = "";
            for (int j = 0; j < Size; j++)
            {
                a = a + field[i][j].ToString() + " ";
            }
            print(a);
        }
        print("--------------------");
    }
    public int SelectLine(int direction)
    {
        int temp = 0;
        for (int i = 0; i < Size; i++)
        {
            temp += i + 1;
        }
        temp = Random.Range(0, temp);
        for (int i = 0; i < Size; i++)
        {
            temp -= i + 1;
            if (temp <= 0)
            {
                if (direction == -1)
                {
                    return Size - i - 1;
                }
                else
                {
                    return i;
                }
            }
        }
        return -1;
    }
    public void ChooseNextColor()
    {
        nextColor = Random.Range(1, Colornum + 1);
    }
    public List<int> JudgeLine()
    {
        //y,x 50 03
        List<int> deleteLine = new List<int>();
        for (int i = 0; i < Size; i++)
        {
            if (puzzleField[0][0] == puzzleField[i][i] & puzzleField[i][i] != -1)
            {
                if (i == Size - 1)
                {
                    deleteLine.Add(100);
                    print("右上斜めに消えた");
                }
            }
            else break;
        }
        for (int i = 0; i < Size; i++)
        {
            if (puzzleField[0][Size-1] == puzzleField[i][Size-1-i] & puzzleField[i][Size-1-i] != -1)
            {
                if (i == Size - 1)
                {
                    deleteLine.Add(200);
                    print("左上斜めに消えた");
                }
            }
            else break;
        }
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (puzzleField[i][0] == puzzleField[i][j] & puzzleField[i][j] != -1)
                {
                    if (j == Size - 1)
                    {
                        deleteLine.Add((i + 1) * 10);
                        print("横に消えた");
                    }
                }
                else break;
            }
            for (int j = 0; j < Size; j++)
            {
                if (puzzleField[0][i] == puzzleField[j][i] & puzzleField[j][i] != -1)
                {
                    if (j == Size - 1)
                    {
                        deleteLine.Add(i + 1);
                        print("縦に消えた");
                    }
                }
                else break;
            }
        }
        foreach (int temp in deleteLine)
        {
            if (temp == 100)
            {
                for (int i = 0; i < Size; i++)
                {
                    puzzleField[i][i] = -1;
                }
            }
            else if (temp == 200)
            {
                for (int i = 0; i < Size; i++)
                {
                    puzzleField[i][Size - 1 - i] = -1;
                }
            }
            else if (temp % 10 == 0)
            {
                for (int i = 0; i < Size; i++)
                {
                    puzzleField[temp / 10 - 1][i] = -1;
                }
            }
            else
            {
                for (int i = 0; i < Size; i++)
                {
                    puzzleField[i][temp - 1] = -1;
                }
            }
        }
        return deleteLine;
    }
    public void Fall()
    {
        for (int j = 0; j < Size; j++)
        {
            for (int i = 0; i < Size + Exheight; i++)
            {
                if (puzzleField[i][j] > 0 & i > 0)//下から見て何かあれば一個下にずらす
                {
                    if (puzzleField[i - 1][j] < 0)
                    {
                        puzzleField[i - 1][j] = puzzleField[i][j];
                        puzzleField[i][j] = -1;
                        i -= 2;
                    }
                }
            }
        }
        print("Falled\n");
        PrintField(puzzleField);
    }
    public void AddScore(int deleteLines)
    {
        score += (int)(deleteLines*combo*Mathf.Pow(3,comboRound)*(round/5.0+10));
        PuzzleView.Instance.ScoreChipSpawn((int)(deleteLines * combo * Mathf.Pow(3, comboRound) * (round / 5.0 + 10)),false);
        print("ライン数:x"+deleteLines.ToString()+" Combo:x"+combo.ToString()+" comboRoundBonus:x"+ Mathf.Pow(3, comboRound).ToString()+" baseScore:"+ (round / 5.0 + 10).ToString());
    }
}
