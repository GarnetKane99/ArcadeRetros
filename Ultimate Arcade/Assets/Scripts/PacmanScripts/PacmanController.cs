using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public GameObject CurrentNode;
    public bool GameStarted = false;
    private bool CanMove = true;
    private bool IsMoving = false;
    private SpriteRenderer PacSprite;
    private Animator PacAnim;

    private Vector2 CurrentPos;
    private Vector2 NextPos;

    private float PacmanSpeed = 5;

    [SerializeField] private Transform Positive, Negative;

    // Start is called before the first frame update
    void Start()
    {
        PacSprite = GetComponent<SpriteRenderer>();
        PacAnim = GetComponent<Animator>();
        CurrentPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStarted)
        {
            if (CanMove)
            {
                GetInput();
                if (!IsMoving)
                {
                    FindNextMovement();
                }
                DoMovement();
            }
            else
            {
                StopMovement();
            }

        }
        else
        {
            StopMovement();
        }
    }

    void FindNextMovement()
    {
        if (transform.rotation.z == 0 && !PacSprite.flipX)
        {

            NextPos = new Vector2(CurrentPos.x + 1, CurrentPos.y);
        }
    }

    void GetInput()
    {

    }

    void DoMovement()
    {
        PacAnim.SetBool("Moving", true);

        transform.position = Vector2.MoveTowards(transform.position, NextPos, PacmanSpeed * Time.deltaTime);
        CurrentPos = transform.position;

        if (CurrentPos == NextPos)
        {
            IsMoving = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Pellet")
        {
            Destroy(collision.gameObject);
        }
    }

    void StopMovement()
    {
        PacAnim.SetBool("Moving", false);
    }
}
