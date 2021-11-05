using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreChip : MonoBehaviour
{
    public RectTransform goalPosition;
    public RectTransform startPosition;
    public Vector3 nowMovement;
    // Start is called before the first frame update
    void Start()
    {
        nowMovement = Random.insideUnitCircle*10;
        gameObject.GetComponent<RectTransform>().localPosition = startPosition.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<RectTransform>().localPosition += nowMovement * Time.deltaTime;
        nowMovement += (goalPosition.position - gameObject.GetComponent<RectTransform>().localPosition) * Time.deltaTime * 0.1f;
        if((goalPosition.position-gameObject.GetComponent<RectTransform>().localPosition).magnitude<0.05f)
        {
            PuzzleView.Instance.AddScore();
            Destroy(gameObject);
        }
    }
}
