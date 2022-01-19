using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private bool Up = false, Down = false, Left = false, Right = true;
    public List<PacmanGhostController> GhostsInGame;

    public bool HasRotated = false;

    float SpecialTimer = 0.0f;
    bool PelletEaten = false;

    public GameObject GameOverScreen;

    public int Score = 0;
    public TextMeshProUGUI ScoreText, LevelText, HighScoreText;
    public int PelletCounter = default;
    public int PelletsEaten = 0;

    public bool GameWon = false;

    public PacmanMapGenerator PacMap;

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
            if(PelletsEaten == 0)
            {
                GameStarted = false;
                GameWon = true;
                foreach(PacmanGhostController g in GhostsInGame)
                {
                    g.CanMove = false;
                }
                PacMap.StartCoroutine(PacMap.RespawnPellets());
            }
            if (PelletEaten)
            {
                CheckSpecialTimer();
            }
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

    void CheckSpecialTimer()
    {
        if (SpecialTimer > 0.0f)
        {
            SpecialTimer -= Time.deltaTime;
        }
        else
        {
            PelletEaten = false;
            foreach(PacmanGhostController g in GhostsInGame)
            {
                g.PelletEaten = false;
            }
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
        if(NextPos == new Vector2(0,11))
        {
            NextPos = new Vector2(0, 12);
            CurrentPos = transform.position;
            transform.position = NextPos;
            IsMoving = false;
            PacAnim.SetBool("Moving", false);
            return;
        }else if(NextPos == new Vector2(1, 11))
        {
            NextPos = new Vector2(1, 12);
            CurrentPos = transform.position;
            transform.position = NextPos;
            IsMoving = false;
            PacAnim.SetBool("Moving", false);
            return;
        }

        if(NextPos.x >= 15)
        {
            NextPos = new Vector2(-13, 9);
            CurrentPos = transform.position;
            transform.position = NextPos;
            IsMoving = false;
            return;
        }
        else if(NextPos.x <= -14)
        {
            NextPos = new Vector2(14, 9);
            CurrentPos = transform.position;
            transform.position = NextPos;
            IsMoving = false;
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, NextPos, PacmanSpeed * Time.deltaTime);
        CurrentPos = transform.position;

        if (CurrentPos == NextPos)
        {
            IsMoving = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Pellet" && !GameWon)
        {
            PelletsEaten--;
            Score += 100;
            ScoreText.text = Score.ToString();
            
            if(Score > int.Parse(HighScoreText.text))
            {
                HighScoreText.text = Score.ToString();
                PlayerPrefs.SetInt("PacmanHighScore", Score);
            }

            Destroy(collision.gameObject);
        }
        if(collision.tag == "Enemy" && !PelletEaten)
        {
            GameOverScreen.SetActive(true);
            GameStarted = false;
            PacAnim.SetBool("Dead", true);
        }
        else if(collision.tag =="Enemy" && PelletEaten)
        {
            collision.GetComponent<PacmanGhostController>().GhostEaten = true;
        }
        if(collision.tag == "LargePellet")
        {
            SpecialTimer = 8.0f;
            PelletEaten = true;
            foreach(PacmanGhostController g in GhostsInGame)
            {
                g.PelletEaten = true;
                g.UpdateSprite();
            }
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
