using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleView : MonoBehaviour
{
    //基本方針：パズルに関する見た目の部分は全部管理する
    public static PuzzleView Instance;

    public GameObject size4Field;
    public GameObject size5Field;
    public GameObject redBallPrefab;
    public GameObject blueBallPrefab;
    public GameObject greenBallPrefab;
    public GameObject scoreChipPrefab;
    public GameObject size4Cover;
    public GameObject size5Cover;
    public SpriteRenderer nextColorSprite;
    [SerializeField] private GameObject leftStartPoint;
    [SerializeField] private GameObject rightStartPoint;
    [SerializeField] private GameObject[] size4ex1Lid;
    [SerializeField] private GameObject[] size4ex2Lid;
    [SerializeField] private GameObject[] size5ex1Lid;
    [SerializeField] private GameObject[] size5ex2Lid;
    [SerializeField] private GameObject size4ex1Side;
    [SerializeField] private GameObject size4ex2Side;
    [SerializeField] private GameObject size5ex1Side;
    [SerializeField] private GameObject size5ex2Side;
    [SerializeField] private GameObject size4Vertical;
    [SerializeField] private GameObject size5Vertical;
    [SerializeField] private GameObject size4SideWall;
    [SerializeField] private GameObject size5SideWall;
    [SerializeField] private BoxCollider2D size4FieldCollider;
    [SerializeField] private BoxCollider2D size5FieldCollider;//当たり判定に入っているボールを盤面ごと登録しておいて消すときや転がすときに活用。
    [SerializeField] private Text scoreDisplay;
    [SerializeField] private Text roundDisplay;
    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;
    [SerializeField] private Button LeftTurnButton;
    [SerializeField] private Button RightTurnButton;
    [SerializeField] private Button NotTurnButton;
    [SerializeField] private GameObject GameSet;
    private List<List<GameObject>> ballObjects = new List<List<GameObject>>();


    public bool finishRoll;
    public bool finishThrow;
    public bool finishFall;
    private int rollWay;//回転する方向、-1:左、1:右、0:なし
    private float rollTimer;
    const float maxRollTime = 3;
    private int displayScore = 0;
    private enum RollState
    {
        BeforeDown,
        Rolling,
        AfterUp
    }
    RollState rollState;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!finishRoll)
        {
            switch (rollState)
            {
                case RollState.BeforeDown:
                    if (rollTimer < 1)
                    {
                        rollTimer += Time.deltaTime;
                        switch (PuzzleManager.Instance.Size)
                        {
                            case 4:
                                size4Field.transform.localPosition = new Vector3(0, -1 - rollTimer);
                                break;
                            case 5:
                                size5Field.transform.localPosition = new Vector3(0, -1 - rollTimer);
                                break;
                        }
                    }
                    else
                    {
                        rollTimer = 0;
                        rollState = RollState.Rolling;
                        switch (PuzzleManager.Instance.Size)
                        {
                            case 4:
                                size4Field.transform.localPosition = new Vector3(0, -2);
                                break;
                            case 5:
                                size5Field.transform.localPosition = new Vector3(0, -2);
                                break;
                        }
                    }
                    break;
                case RollState.Rolling:
                    if (rollTimer < maxRollTime)
                    {
                        //if(rollTimer>maxRollTime/2&rollTimer-Time.deltaTime<maxRollTime/2)
                        //{
                        //    ResetWall();
                        //}
                        rollTimer += Time.deltaTime;
                        switch (PuzzleManager.Instance.Size)
                        {
                            case 4:
                                size4Field.transform.rotation = Quaternion.Euler(0, 0, rollTimer / maxRollTime * rollWay * 90);
                                break;
                            case 5:
                                size5Field.transform.rotation = Quaternion.Euler(0, 0, rollTimer / maxRollTime * rollWay * 90);//ゲームマネージャーから回転の指示が来た瞬間に当たり判定いじる
                                break;
                        }
                        CheckFreezeRoll();
                    }
                    else//回転終了
                    {
                        if (CheckAndFreezeBallsPosition())
                        {
                            ResetWall();
                            rollWay = 0;
                            rollTimer = 0;
                            rollState = RollState.AfterUp;
                        }
                    }
                    break;
                case RollState.AfterUp:
                    if (rollTimer < 1)
                    {
                        rollTimer += Time.deltaTime; switch (PuzzleManager.Instance.Size)
                        {
                            case 4:
                                size4Field.transform.localPosition = new Vector3(0, -2 + rollTimer);
                                break;
                            case 5:
                                size5Field.transform.localPosition = new Vector3(0, -2 + rollTimer);
                                break;
                        }
                    }
                    else
                    {
                        finishRoll = true;
                        switch (PuzzleManager.Instance.Size)
                        {
                            case 4:
                                size4Field.transform.localPosition = new Vector3(0, -1, 0);
                                break;
                            case 5:
                                size5Field.transform.localPosition = new Vector3(0, -1, 0);
                                break;
                        }

                    }
                    CheckAndFreezeBallsPosition();
                    break;
            }

        }
        if (!finishThrow)//投入終了判定
        {
            if (CheckAndFreezeBallsPosition())
            {
                finishThrow = true;
            }
        }
        if (!finishFall)
        {
            finishFall = CheckAndFreezeBallsPosition();
        }

        //点数の更新
        scoreDisplay.text = "Score\n" + displayScore;//PuzzleManager.Instance.score.ToString();
        roundDisplay.text = "Round\n" + PuzzleManager.Instance.round.ToString();
    }
    public void ResetObjectList()
    {
        for (int i = 0; i < PuzzleManager.Instance.Size + PuzzleManager.Instance.Exheight; i++)
        {
            List<GameObject> tempList = new List<GameObject>();
            for (int j = 0; j < PuzzleManager.Instance.Size; j++)
            {
                tempList.Add(null);
            }
            ballObjects.Add(tempList);
        }
    }
    public void BallThrow(int direction, int line, int row, int color)//ボール転がす
    {
        GameObject throwedBall = null;
        finishThrow = false;
        Vector3 position = new Vector3();
        if (direction == -1)
        {
            position = leftStartPoint.transform.position;
        }
        if (direction == 1)
        {
            position = rightStartPoint.transform.position;
        }
        switch (color)
        {
            case 1:
                throwedBall = Instantiate(redBallPrefab, position, Quaternion.identity, GameSet.transform);
                break;
            case 2:
                throwedBall = Instantiate(blueBallPrefab, position, Quaternion.identity, GameSet.transform);
                break;
            case 3:
                throwedBall = Instantiate(greenBallPrefab, position, Quaternion.identity, GameSet.transform);
                break;
        }
        if (row != -1) ballObjects[row][line] = throwedBall;
        //蓋やフィールドの当たり判定をいじる
        switch (PuzzleManager.Instance.Size)
        {
            case 4:
                switch (PuzzleManager.Instance.Exheight)
                {
                    case 1:
                        for (int i = 0; i < PuzzleManager.Instance.Size; i++)
                        {
                            size4ex1Lid[i].SetActive(true);
                        }
                        size4ex1Lid[line].SetActive(false);
                        size4ex1Side.SetActive(false);
                        break;
                    case 2:
                        for (int i = 0; i < PuzzleManager.Instance.Size; i++)
                        {
                            size4ex2Lid[i].SetActive(true);
                        }
                        size4ex2Lid[line].SetActive(false);
                        size4ex2Side.SetActive(false);
                        break;
                }
                size4SideWall.SetActive(false);
                size4Cover.SetActive(false);
                size4Vertical.SetActive(true);
                break;
            case 5:
                switch (PuzzleManager.Instance.Exheight)
                {
                    case 1:
                        for (int i = 0; i < PuzzleManager.Instance.Size; i++)
                        {
                            size5ex1Lid[i].SetActive(true);
                        }
                        size5ex1Lid[line].SetActive(false);
                        size5ex1Side.SetActive(false);
                        break;
                    case 2:
                        for (int i = 0; i < PuzzleManager.Instance.Size; i++)
                        {
                            size5ex2Lid[i].SetActive(true);
                        }
                        size5ex2Lid[line].SetActive(false);
                        size5ex2Side.SetActive(false);
                        break;
                }
                size5SideWall.SetActive(false);
                size5Cover.SetActive(false);
                size5Vertical.SetActive(true);
                break;
        }
    }
    public void RollField(int rollWay)
    {
        this.rollWay = rollWay;
        finishRoll = false;
        rollState = RollState.BeforeDown;
        rollTimer = 0;
        MeltBalls();
        switch (PuzzleManager.Instance.Size)
        {
            case 4:
                size4Vertical.SetActive(false);
                break;
            case 5:
                size5Vertical.SetActive(false);
                break;
        }
        RollData(rollWay);
    }
    public void DisplayNextColor(int nextColor)
    {
        switch (nextColor)
        {
            case 0:
                nextColorSprite.sprite = null;
                break;
            case 1:
                nextColorSprite.sprite = redBallPrefab.GetComponent<SpriteRenderer>().sprite;
                break;
            case 2:
                nextColorSprite.sprite = blueBallPrefab.GetComponent<SpriteRenderer>().sprite;
                break;
            case 3:
                nextColorSprite.sprite = greenBallPrefab.GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }
    public void Fall()
    {
        finishFall = false;
        MeltBalls();
        float Size = PuzzleManager.Instance.Size;
        float Exheight = PuzzleManager.Instance.Exheight;
        switch (Size)
        {
            case 4:
                size4SideWall.SetActive(false);
                size4Cover.SetActive(false);
                size4Vertical.SetActive(true);
                switch (Exheight)
                {
                    case 1:
                        size4ex1Side.SetActive(false);
                        break;
                    case 2:
                        size4ex2Side.SetActive(false);
                        break;
                }
                break;
            case 5:
                size5SideWall.SetActive(false);
                size5Cover.SetActive(false);
                size5Vertical.SetActive(true);
                switch (Exheight)
                {
                    case 1:
                        size5ex1Side.SetActive(false);
                        break;
                    case 2:
                        size5ex2Side.SetActive(false);
                        break;
                }
                break;
        }
        for (int j = 0; j < Size; j++)
        {
            for (int i = 0; i < Size + Exheight; i++)
            {
                if (ballObjects[i][j] != null & i > 0)//下から見て何かあれば一個下にずらす
                {
                    if (ballObjects[i - 1][j] == null)
                    {
                        ballObjects[i - 1][j] = ballObjects[i][j];
                        ballObjects[i][j] = null;
                        i -= 2;
                    }
                }
            }
        }
    }
    public void ResetWall()
    {
        switch (PuzzleManager.Instance.Size)
        {
            case 4:
                size4Cover.SetActive(true);
                size4SideWall.SetActive(true);
                size4Vertical.SetActive(true);
                size4Field.transform.rotation = Quaternion.identity;
                switch (PuzzleManager.Instance.Exheight)
                {
                    case 1:
                        size4ex1Side.SetActive(true);
                        for (int i = 0; i < PuzzleManager.Instance.Size; i++)
                        {
                            size4ex1Lid[i].SetActive(true);
                        }
                        break;
                    case 2:
                        size4ex2Side.SetActive(true);
                        for (int i = 0; i < PuzzleManager.Instance.Size; i++)
                        {
                            size4ex2Lid[i].SetActive(true);
                        }
                        break;
                }
                break;
            case 5:
                size5Cover.SetActive(true);
                size5SideWall.SetActive(true);
                size5Vertical.SetActive(true);
                size5Field.transform.rotation = Quaternion.identity;
                switch (PuzzleManager.Instance.Exheight)
                {
                    case 1:
                        size5ex1Side.SetActive(true);
                        for (int i = 0; i < PuzzleManager.Instance.Size; i++)
                        {
                            size5ex1Lid[i].SetActive(true);
                        }
                        break;
                    case 2:
                        size5ex2Side.SetActive(true);
                        for (int i = 0; i < PuzzleManager.Instance.Size; i++)
                        {
                            size5ex2Lid[i].SetActive(true);
                        }
                        break;
                }
                break;
        }
    }
    public void DeleteBalls(List<int> deleteLine)
    {
        int Size = PuzzleManager.Instance.Size;
        foreach (int temp in deleteLine)
        {
            if (temp % 100 == 0)//斜め
            {
                if (temp == 100)//右斜め上
                {
                    for (int i = 0; i < Size; i++)
                    {
                        if (ballObjects[i][i] != null)
                        {
                            ballObjects[i][i].GetComponent<PuzzleBall>().DeleteBall();
                            ballObjects[i][i] = null;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Size; i++)
                    {
                        if (ballObjects[Size - 1 - i][i] != null)
                        {
                            ballObjects[Size - 1 - i][i].GetComponent<PuzzleBall>().DeleteBall();
                            ballObjects[Size - 1 - i][i] = null;
                        }
                    }
                }
            }
            else if (temp % 10 == 0)
            {
                for (int i = 0; i < Size; i++)
                {
                    if (ballObjects[temp / 10 - 1][i] != null)
                    {
                        ballObjects[temp / 10 - 1][i].GetComponent<PuzzleBall>().DeleteBall();
                        ballObjects[temp / 10 - 1][i] = null;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Size; i++)
                {
                    if (ballObjects[i][temp - 1] != null)
                    {
                        ballObjects[i][temp - 1].GetComponent<PuzzleBall>().DeleteBall();
                        ballObjects[i][temp - 1] = null;
                    }
                }
            }
        }
    }
    private bool CheckAndFreezeBallsPosition()
    {
        bool isCorrectPosition = true;
        for (int i = 0; i < PuzzleManager.Instance.Size + PuzzleManager.Instance.Exheight; i++)
        {
            for (int j = 0; j < PuzzleManager.Instance.Size; j++)
            {
                if (ballObjects[i][j] == null) continue;
                float rollingUnderY = 0;
                switch (PuzzleManager.Instance.Size)
                {
                    case 4:
                        rollingUnderY = size4Field.transform.localPosition.y + 1;
                        break;
                    case 5:
                        rollingUnderY = size5Field.transform.localPosition.y + 1;
                        break;
                }
                Vector3 correctPosition = new Vector3(j - (PuzzleManager.Instance.Size - 1) / 2.0f,
                    (i >= PuzzleManager.Instance.Size) ? (i - (PuzzleManager.Instance.Size - 1) / 2.0f + rollingUnderY - 0.9f) : (i - (PuzzleManager.Instance.Size - 1) / 2.0f + rollingUnderY - 1));
                if ((ballObjects[i][j].gameObject.transform.localPosition - correctPosition).magnitude < 0.1f)
                {
                    if (ballObjects[i][j].gameObject.GetComponent<Rigidbody2D>().isKinematic == false)
                    {
                        ballObjects[i][j].gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                        ballObjects[i][j].gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2();
                        ballObjects[i][j].gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
                        ballObjects[i][j].gameObject.transform.localPosition = correctPosition;
                        ballObjects[i][j].gameObject.transform.localRotation = Quaternion.identity;
                        ballObjects[i][j].gameObject.GetComponent<PuzzleBall>().SetActiveRangeBox(true);
                    }
                    else
                    {
                        ballObjects[i][j].gameObject.transform.localPosition = correctPosition;
                    }
                }
                else isCorrectPosition = false;
            }
        }
        //print("チェック:" + isCorrectPosition);
        return isCorrectPosition;
    }
    private bool CheckFreezeRoll()
    {
        bool isCorrectPosition = true;
        for (int i = 0; i < PuzzleManager.Instance.Size; i++)
        {
            for (int j = 0; j < PuzzleManager.Instance.Size; j++)
            {
                if (ballObjects[i][j] == null) continue;
                float rollingUnderY = 0;
                switch (PuzzleManager.Instance.Size)
                {
                    case 4:
                        rollingUnderY = size4Field.transform.localPosition.y + 1;
                        break;
                    case 5:
                        rollingUnderY = size5Field.transform.localPosition.y + 1;
                        break;
                }
                Vector3 correctPosition = new Vector3();
                switch (PuzzleManager.Instance.Size)
                {
                    case 4:
                        print(rollWay.ToString() + ":" + size4Field.transform.localRotation.eulerAngles.z);
                        //1:0-90
                        //-1:360-270
                        if (rollWay == 1)
                        {
                            correctPosition = size4Field.transform.localRotation * Quaternion.Euler(0, 0, -90) * new Vector3(j - (PuzzleManager.Instance.Size - 1) / 2.0f, i - (PuzzleManager.Instance.Size - 1) / 2.0f) + new Vector3(0, -1 + rollingUnderY);
                        }
                        else if (rollWay == -1)
                        {
                            correctPosition = size4Field.transform.localRotation * Quaternion.Euler(0, 0, 90) * new Vector3(j - (PuzzleManager.Instance.Size - 1) / 2.0f, i - (PuzzleManager.Instance.Size - 1) / 2.0f) + new Vector3(0, -1 + rollingUnderY);
                        }
                        break;
                    case 5:
                        //correctPosition = size5Field.transform.localRotation * new Vector3(j - (PuzzleManager.Instance.Size - 1) / 2.0f, i - (PuzzleManager.Instance.Size - 1) / 2.0f) + new Vector3(0, -1, 0);
                        correctPosition = new Vector3(Mathf.Cos(size5Field.transform.localRotation.eulerAngles.z * Mathf.PI / 180) * new Vector3(j - (PuzzleManager.Instance.Size - 1) / 2.0f, i - (PuzzleManager.Instance.Size - 1) / 2.0f).magnitude,
                             Mathf.Sin(size5Field.transform.localRotation.eulerAngles.z * Mathf.PI / 180) * new Vector3(j - (PuzzleManager.Instance.Size - 1) / 2.0f, i - (PuzzleManager.Instance.Size - 1) / 2.0f).magnitude - 1);
                        break;
                }
                if ((ballObjects[i][j].gameObject.transform.localPosition - correctPosition).magnitude < 0.05f)
                {
                    //ballObjects[i][j].gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                    ballObjects[i][j].gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2();
                    ballObjects[i][j].gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
                    ballObjects[i][j].gameObject.transform.localPosition = correctPosition;
                    //ballObjects[i][j].gameObject.transform.localRotation = Quaternion.identity;
                    ballObjects[i][j].gameObject.GetComponent<PuzzleBall>().SetActiveRangeBox(true);
                    print("固定");
                }
                else isCorrectPosition = false;
                print("local-" + ballObjects[i][j].gameObject.transform.localPosition + ":::correct-" + correctPosition);
            }
        }
        return isCorrectPosition;
    }
    private void MeltBalls()
    {
        for (int i = 0; i < PuzzleManager.Instance.Size + PuzzleManager.Instance.Exheight; i++)
        {
            for (int j = 0; j < PuzzleManager.Instance.Size; j++)
            {
                if (ballObjects[i][j] == null) continue;
                if (ballObjects[i][j].gameObject.GetComponent<Rigidbody2D>().isKinematic == true)
                {
                    ballObjects[i][j].gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                    ballObjects[i][j].gameObject.transform.rotation = Quaternion.identity;
                    ballObjects[i][j].gameObject.GetComponent<PuzzleBall>().SetActiveRangeBox(false);
                }
            }
        }
    }
    private void RollData(int rollWay)
    {
        int Size = PuzzleManager.Instance.Size;
        int Exheight = PuzzleManager.Instance.Exheight;
        List<List<GameObject>> rolledPuzzleField = new List<List<GameObject>>();
        for (int i = 0; i < Size; i++)
        {
            List<GameObject> tempList = new List<GameObject>();
            for (int j = 0; j < Size; j++)
            {
                tempList.Add(null);
            }
            rolledPuzzleField.Add(tempList);
        }
        for (int i = Size; i < Size + Exheight; i++)
        {
            rolledPuzzleField.Add(ballObjects[i]);
        }
        switch (rollWay)
        {
            case -1://左回転
                for (int i = 0; i < Size; i++)
                {
                    int count = 0;
                    for (int j = Size - 1; j >= 0; j--)
                    {
                        if (ballObjects[i][j] != null)
                        {
                            rolledPuzzleField[count][i] = ballObjects[i][j];
                            count++;
                        }
                    }
                }
                ballObjects = rolledPuzzleField;
                break;
            case 0:
                break;
            case 1:
                for (int i = 0; i < Size; i++)
                {
                    int count = 0;
                    for (int j = 0; j < Size; j++)
                    {
                        if (ballObjects[i][j] != null)
                        {
                            rolledPuzzleField[count][Size - i - 1] = ballObjects[i][j];
                            count++;
                        }
                    }
                }
                ballObjects = rolledPuzzleField;
                break;
        }
    }
    public void DisappearOperationButton()
    {
        LeftButton.interactable = false;
        RightButton.interactable = false;
        LeftTurnButton.interactable = false;
        RightTurnButton.interactable = false;
        NotTurnButton.interactable = false;
    }
    public void AppearTurnButton()
    {
        LeftTurnButton.interactable = true;
        RightTurnButton.interactable = true;
        NotTurnButton.interactable = true;
    }
    public void AppearInButton()
    {
        LeftButton.interactable = true;
        RightButton.interactable = true;
    }
    public void AddScore(int plusScore = 1)
    {
        displayScore += plusScore;
    }
    public void ScoreChipSpawn(int chipNum, bool isRoundBonus)
    {
        for (int i = 0; i < chipNum; i++)
        {
            GameObject newChip;
            newChip = Instantiate(scoreChipPrefab);
            newChip.GetComponent<ScoreChip>().startPosition.position = scoreDisplay.GetComponent<RectTransform>().localPosition;
            if (isRoundBonus)
            {
                newChip.GetComponent<ScoreChip>().startPosition.position = roundDisplay.GetComponent<RectTransform>().localPosition;
            }
            else
            {
                newChip.GetComponent<ScoreChip>().startPosition.position = new Vector3(0, -166, 0);
            }
        }
    }
}
