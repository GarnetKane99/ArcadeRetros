using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public GameObject CurrentNode;
    public bool GameStarted = false;
    public bool CanMove = true;
    public bool IsMoving = false;
    private SpriteRenderer PacSprite;
    private Animator PacAnim;

    public Vector2 CurrentPos;
    public Vector2 NextPos;

    private float PacmanSpeed = 5;

    [SerializeField] private GameObject Positive;

    private bool Up = false, Down = false, Left = false, Right = true;

    public bool HasRotated = false;

    // Start is called before the first frame update
    void Start()
    {
        PacSprite = GetComponent<SpriteRenderer>();
        PacAnim = GetComponent<Animator>();
        CurrentPos = new Vector2(transform.position.x - 0.5f, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStarted)
        {
            GetInput();
            CheckMovement();
            if (CanMove)
            {
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
            DontMove();
        }
    }

    void FindNextMovement()
    {
        if (Right)
        {
            NextPos = new Vector2(CurrentPos.x + 1, CurrentPos.y);
            IsMoving = true;
        }
        else if (Up)
        {
            NextPos = new Vector2(CurrentPos.x, CurrentPos.y + 1);
            IsMoving = true;
        }
        else if (Down)
        {
            NextPos = new Vector2(CurrentPos.x, CurrentPos.y - 1);
            IsMoving = true;
        }
        else if (Left)
        {
            NextPos = new Vector2(CurrentPos.x - 1, CurrentPos.y);
            IsMoving = true;
        }
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Up)
            {
                return;
            }
            if (PacSprite.flipX)
            {
                PacSprite.flipX = false;
            }
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            Positive.transform.localPosition = new Vector2(1.25f, 0);
            Up = true;
            Right = false;
            Down = false;
            Left = false;
            HasRotated = true;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Down)
            {
                return;
            }
            if (PacSprite.flipX)
            {
                PacSprite.flipX = false;
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            Positive.transform.localPosition = new Vector2(1.25f, 0);
            Up = false;
            Right = false;
            Down = true;
            Left = false;

            HasRotated = true;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Left)
            {
                return;
            }
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            PacSprite.flipX = true;
            Positive.transform.localPosition = new Vector2(-1.25f, 0);
            Up = false;
            Right = false;
            Down = false;
            Left = true;

            HasRotated = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Right)
            {
                return;
            }
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            Positive.transform.localPosition = new Vector2(1.25f, 0);
            PacSprite.flipX = false;

            Up = false;
            Right = true;
            Down = false;
            Left = false;

            HasRotated = true;
        }
    }

    void CheckMovement()
    {
        RaycastHit2D hitMid = Physics2D.Raycast(transform.position, transform.position);
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, transform.position);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, transform.position);

        if (Up)
        {
            hitMid = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + .5f), Vector2.up, 1);
            hitUp = Physics2D.Raycast(new Vector2(transform.position.x + .45f, transform.position.y + .5f), Vector2.up, 1);
            hitDown = Physics2D.Raycast(new Vector2(transform.position.x - .45f, transform.position.y + .5f), Vector2.up, 1);
            Debug.DrawRay(new Vector2(transform.position.x + .45f, transform.position.y + .5f), Vector2.up * 1);
            Debug.DrawRay(new Vector2(transform.position.x - .45f, transform.position.y + .5f), Vector2.up * 1);
        }
        else if (Right)
        {
            hitMid = Physics2D.Raycast(new Vector2(transform.position.x + .5f, transform.position.y), Vector2.right, 1);
            hitUp = Physics2D.Raycast(new Vector2(transform.position.x + .5f, transform.position.y + .45f), Vector2.right, 1);
            hitDown = Physics2D.Raycast(new Vector2(transform.position.x + .5f, transform.position.y - .45f), Vector2.right, 1);
            Debug.DrawRay(new Vector2(transform.position.x + .5f, transform.position.y + .45f), Vector2.right * 1f);
            Debug.DrawRay(new Vector2(transform.position.x + .5f, transform.position.y - .45f), Vector2.right * 1f);
        }
        else if (Left)
        {
            hitMid = Physics2D.Raycast(new Vector2(transform.position.x - .5f, transform.position.y), Vector2.left, 1);
            hitUp = Physics2D.Raycast(new Vector2(transform.position.x - .5f, transform.position.y + .45f), Vector2.left, 1);
            hitDown = Physics2D.Raycast(new Vector2(transform.position.x - .5f, transform.position.y - .45f), Vector2.left, 1);
            Debug.DrawRay(new Vector2(transform.position.x - .5f, transform.position.y - .45f), Vector2.left * 1f);
            Debug.DrawRay(new Vector2(transform.position.x - .5f, transform.position.y + .45f), Vector2.left * 1f);
        }
        else if (Down)
        {
            hitMid = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .5f), Vector2.down, 1);
            hitUp = Physics2D.Raycast(new Vector2(transform.position.x - .45f, transform.position.y - .5f), Vector2.down, 1);
            hitDown = Physics2D.Raycast(new Vector2(transform.position.x + .45f, transform.position.y - .5f), Vector2.down, 1);
            Debug.DrawRay(new Vector2(transform.position.x - .45f, transform.position.y - .5f), Vector2.down * 1f);
            Debug.DrawRay(new Vector2(transform.position.x + .45f, transform.position.y - .5f), Vector2.down * 1f);
        }

        if (hitMid.collider != null)
        {
            if (hitMid.collider.CompareTag("Wall"))
            {
                CanMove = false;
                return;
            }
        }
        if (hitUp.collider != null)
        {
            if (hitUp.collider.CompareTag("Wall"))
            {
                CanMove = false;
                return;
            }
        }
        if (hitDown.collider != null)
        {
            if (hitDown.collider.CompareTag("Wall"))
            {
                CanMove = false;
                return;
            }
        }
        CanMove = true;
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
        if (CurrentPos != NextPos)
        {
            transform.position = Vector2.MoveTowards(transform.position, NextPos, PacmanSpeed * Time.deltaTime);
            CurrentPos = transform.position;
        }

        PacAnim.SetBool("Moving", false);
    }

    void DontMove()
    {
        PacAnim.SetBool("Moving", false);
    }
}
