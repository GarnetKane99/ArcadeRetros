using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlockMovement : MonoBehaviour
{
    //Factors that affect the speed of the block
    //Level and Score (might change to public shortly)
    //Maybe write up an algorithm for default speed in which the blocks fall down?
    private int SCORE, LEVEL;

    public Vector2 CurrentPos;
    public Vector3 CurrentRot;
    public Vector2 DirToGo;

    public Vector2 RotationValue;

    private bool IsPressed = false;

    private bool collisionFound = false;

    void Start()
    {
        CurrentPos = transform.position;
        InvokeRepeating("BlockDescent", 1.0f, 1.0f);
    }

    void Update()
    {
        GetInput();
        UpdateBlockPos();
        UpdateBlockRot();
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DirToGo.x = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            DirToGo.x = 1;
        }
        else
        {
            DirToGo.x = 0;
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            DirToGo.y = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            DirToGo.y = -1;
        }
        else
        {
            DirToGo.y = 0;
        }
    }

    void UpdateBlockPos()
    {
        if (DirToGo.x != 0)
        {
            if (DirToGo.x < 0)
            {
                CurrentPos.x -= 1;
            }
            else if (DirToGo.x > 0)
            {
                CurrentPos.x += 1;
            }
            gameObject.transform.position = CurrentPos;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionFound = true;
    }

    void UpdateBlockRot()
    {
        if (DirToGo.y != 0)
        {
            if (DirToGo.y > 0)
            {
                CurrentRot.z += 90;
            }
            else if (DirToGo.y < 0)
            {
                CurrentRot.z -= 90;
            }

            gameObject.transform.rotation = Quaternion.Euler(CurrentRot);

            switch (FoundPiece())
            {
                case "Tetris_O":
                    break;
                case "Tetris_I":
                    break;
                default:
                    break;
            }
        }
    }

    string FoundPiece()
    {
        return name.Split('(')[0];
    }

    void BlockDescent()
    {
        CurrentPos.y -= 1;
        gameObject.transform.position = CurrentPos;
    }
}
