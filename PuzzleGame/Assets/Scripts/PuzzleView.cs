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
    [SerializeField] private GameObject[] size4Lid;
    [SerializeField] private GameObject[] size5Lid;
    [SerializeField] private GameObject GameSet;
    
    // Start is called before the first frame update
    void Start()
    {
        if(Instance==null)
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
        
    }
    public void BallThrow(int direction,int line,int color)
    {
        Vector3 position=new Vector3();
        if(direction==-1)
        {
            position = leftStartPoint.transform.position;
        }
        if (direction == -1)
        {
            position = rightStartPoint.transform.position;
        }
        switch (color)
        {
            case 1:
                Instantiate(redBallPrefab,position,Quaternion.identity);
                break;
            case 2:
                Instantiate(blueBallPrefab, position, Quaternion.identity);
                break;
            case 3:
                Instantiate(greenBallPrefab, position, Quaternion.identity);
                break;
        }
        switch(PuzzleManager.Instance.Size)
        {
            case 4:
                for (int i=0;i< PuzzleManager.Instance.Size;i++)
                {
                    size4Lid[i].SetActive(true);
                }
                size4Lid[line].SetActive(false);
                break;
            case 5:
                for (int i = 0; i < PuzzleManager.Instance.Size; i++)
                {
                    size5Lid[i].SetActive(true);
                }
                size5Lid[line].SetActive(false);
                break;
        }
    }
    public void DisplayNextColor(int nextColor)
    {
        switch(nextColor)
        {
            case 1:
                nextColorSprite.sprite = redBallPrefab.GetComponent<Sprite>();
                break;
            case 2:
                nextColorSprite.sprite = blueBallPrefab.GetComponent<Sprite>();
                break;
            case 3:
                nextColorSprite.sprite = greenBallPrefab.GetComponent<Sprite>();
                break;
        }
    }
}
