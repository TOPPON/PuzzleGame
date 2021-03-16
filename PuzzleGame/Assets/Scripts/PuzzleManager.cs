using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    List<List<int>> puzzleField = new List<List<int>>();//-1:空、0:空 1:色1 2:色2 3:色3
    public int Size { get; private set; } = 4;
    public int Exheight { get; private set; } = 1;
    int Colornum = 2;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Size + Exheight; i++)
        {
            List<int> tempList = new List<int>();
            for (int j = 0; j < Size; j++)
            {
                tempList.Add(Random.Range(0, 3));
            }
            puzzleField.Add(tempList);
        }
    }
    /*
     x=0,y=size+ex x=1,
     
     x=0,y=1 x=1,y=1   
     x=0,y=0 x=1,y=0
         */
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Roll(1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Roll(-1);
        }
    }
    public void Roll(int rollDirection)//-1:左 0:真ん中 1:右 
    {
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
        switch (rollDirection)
        {
            case -1://左回転
                for (int i = 0; i < Size; i++)
                {
                    int count = 0;
                    for (int j = 0; j < Size; j++)
                    {
                        if (puzzleField[i][j] > 0)
                        {
                            rolledPuzzleField[count][Size - i-1] = puzzleField[i][j];
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
                    for (int j = Size - 1; j >=0; j--)
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
        }
        PrintField(puzzleField);
    }
    private int PutBall(int line, int color)
    {
        for (int i = Size + Exheight - 1; i >= 0; i--)
        {
            if (puzzleField[i][line] > 0)
            {
                if (i == Size + Exheight - 1)//ゲームオーバー
                {
                    return -1;
                }
                else
                {
                    puzzleField[i + 1][line] = color;
                    return 0;
                }
            }
        }
        puzzleField[0][line] = color;
        return 0;
    }
    void PrintField(List<List<int>> field)
    {
        print("--------------------");
        for (int i = Size - 1; i >= 0; i--)
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
}
