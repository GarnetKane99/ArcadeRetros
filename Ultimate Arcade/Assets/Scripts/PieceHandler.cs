using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceHandler : MonoBehaviour
{
    //not using this yet
    public List<GameObject> Neighbours;
    TetrisBlockMovement ParentObj;
    public Vector3 CurrentPos;
    public Vector3 CurrentRot;
    public Sprite TetrisSprite;

    private void Start()
    {
        ParentObj = GetComponentInParent<TetrisBlockMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //CurrentPos.x = Mathf.FloorToInt(transform.position.x);
        //CurrentPos.x = (int)transform.position.x;
        CurrentPos.x = transform.position.x;
        if(CurrentPos.x <= -5)
        {
            ParentObj.CurrentPos.x += 1;
            ParentObj.gameObject.transform.position = ParentObj.CurrentPos;
        }
        else if(CurrentPos.x >= 6)
        {
            ParentObj.CurrentPos.x -= 1;
            ParentObj.gameObject.transform.position = ParentObj.CurrentPos;
        }
        CurrentRot.z = Mathf.FloorToInt(transform.rotation.z);
    }

    public bool TryNegativeMovement()
    {
        if (CurrentPos.x - 1 > -5)
        {
            return true;
        }
        return false;
    }

    public bool TryPositiveMovement()
    {
        if (CurrentPos.x + 1 < 6)
        {
            return true;
        }
        return false;
    }

    public string TryPositiveRotation()
    {
        CurrentRot.z += 90;
        CurrentPos.x = transform.position.x;
        if(CurrentPos.x <= -5)
        {
            return "OffsetPosX";
        }
        else if(CurrentPos.x >= 6)
        {
            return "OffsetNegX";
        }
        return "";
    }

    public string TryNegativeRotation()
    {
        CurrentRot.z -= 90;
        CurrentPos.x = (int)transform.position.x;
        if (CurrentPos.x <= -5)
        {
            return "OffsetPosX";
        }
        else if (CurrentPos.x >= 6)
        {
            return "OffsetNegX";
        }
        return "";
    }
}
