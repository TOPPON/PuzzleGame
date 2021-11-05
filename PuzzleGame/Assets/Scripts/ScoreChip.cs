using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreChip : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 goalPosition;
    public Vector3 nowMovement;
    // Start is called before the first frame update
    void Start()
    {

        nowMovement = Random.insideUnitCircle*500;
        nowMovement.x *= 2;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<RectTransform>().localPosition += nowMovement * Time.deltaTime*2;
        nowMovement =nowMovement*(1-Time.deltaTime*4)+ (goalPosition - gameObject.GetComponent<RectTransform>().localPosition) * Time.deltaTime * 3f;
        if((goalPosition-gameObject.GetComponent<RectTransform>().localPosition).magnitude<50f)
        {
            PuzzleView.Instance.AddScore();
            Destroy(gameObject);
        }
    }
}
