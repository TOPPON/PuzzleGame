using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBall : MonoBehaviour
{
    [SerializeField] BoxCollider2D outRangeBox;
    private enum State
    {
        Throwing,
        Idle,
        Deleting
    };
    State myState;
    float deleteTimer;
    Vector3 defaultLocalScale;
    Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        myState = State.Throwing;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (myState)
        {
            case State.Throwing:
                if(rigidbody.velocity.magnitude<0.00001)
                {
                    myState = State.Idle;
                    SetActiveRangeBox(true);
                }
                break;
            case State.Idle:
                break;
            case State.Deleting:
                if (deleteTimer > 0)
                {
                    deleteTimer -= Time.deltaTime;
                    transform.localScale = defaultLocalScale * deleteTimer;
                }
                break;
        }
    }
    public void SetActiveRangeBox(bool isActive)
    {
        outRangeBox.enabled = isActive;
    }
    public void DeleteBall()
    {
        myState = State.Deleting;
        deleteTimer = 1;
        defaultLocalScale = transform.localScale;
    }
}
