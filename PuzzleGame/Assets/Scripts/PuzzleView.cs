using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleView : MonoBehaviour
{
    //基本方針：パズルに関する見た目の部分は全部管理する
    public static PuzzleView Instance;

    public GameObject size4Field;
    public GameObject size5Field;
    public GameObject redBallPrefab;
    public GameObject blueBallPrefab;
    public GameObject greenBallPrefab;
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
    [SerializeField] private GameObject GameSet;
    

    public bool finishRoll;
    public bool finishFall;
    private Rigidbody2D throwedBall;
    private int rollWay;//
    private float rollTimer;
    const float maxRollTime = 3;

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
        if (rollWay != 0)
        {
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
            }
            else//回転終了
            {
                ResetWall();
                finishRoll = true;
                rollWay = 0;
            }
        }
        if (throwedBall != null)
        {
            if (throwedBall.velocity.magnitude <= 0.00001)
            {
                throwedBall = null;
                finishFall = true;
            }
        }
    }
    public void BallThrow(int direction, int line, int color)//ボール転がす
    {
        finishFall = false;
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
                throwedBall = Instantiate(redBallPrefab, position, Quaternion.identity, GameSet.transform).GetComponent<Rigidbody2D>();
                break;
            case 2:
                throwedBall = Instantiate(blueBallPrefab, position, Quaternion.identity, GameSet.transform).GetComponent<Rigidbody2D>();
                break;
            case 3:
                throwedBall = Instantiate(greenBallPrefab, position, Quaternion.identity, GameSet.transform).GetComponent<Rigidbody2D>();
                break;
        }
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
        rollTimer = 0;
        switch (PuzzleManager.Instance.Size)
        {
            case 4:
                size4Vertical.SetActive(false);
                break;
            case 5:
                size5Vertical.SetActive(false);
                break;
        }
    }
    public void DisplayNextColor(int nextColor)
    {
        switch (nextColor)
        {
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
    public void BallFall()
    {
        switch(PuzzleManager.Instance.Size)
        {
            case 4:
                size4SideWall.SetActive(false);
                size4Cover.SetActive(false);
                size4Vertical.SetActive(true);
                break;
            case 5:
                size5SideWall.SetActive(false);
                size5Cover.SetActive(false);
                size5Vertical.SetActive(true);
                break;
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
                size4Field.transform.localPosition = new Vector3(0, -1, 0);
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
                size5Field.transform.localPosition = new Vector3(0,-1,0);
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
}
