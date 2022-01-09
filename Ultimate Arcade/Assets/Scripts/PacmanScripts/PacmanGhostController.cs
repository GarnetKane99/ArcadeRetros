using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PacGhostBehaviour
{
    PATROL, 
    TRAP, //inky
    HUNT, //blinky
    SPECIALISEDPATROL, //pinky
    RUN,
    RETURN,
    RESPAWN
}

public class PacmanGhostController : MonoBehaviour
{
    public List<GameObject> Path;
    public PacmanAIManager PacAIManager;
    public PacmanMapGenerator PacMap;
    public GameObject CurrentNode;
    public bool pathFound = false;

    public GameObject Pacman;
    [SerializeField] private bool PacNear = false;


    [SerializeField] private Sprite LeftLook, RightLook, UpLook, DownLook;
    private SpriteRenderer CurSprite;

    private bool PinkyMove = false, ClydeMove = false;

    private bool CanMove = false;

    Vector2 PrevPos, CurPos;

    PacGhostBehaviour GhostBehaviour;

    float GhostSpeed = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        Path = new List<GameObject>();
        CurPos = transform.position;
        PrevPos = CurPos;
        CurSprite = GetComponent<SpriteRenderer>();

        switch (name)
        {
            case "Pinky":
                GhostBehaviour = PacGhostBehaviour.SPECIALISEDPATROL;
                Invoke("MovePinky", 2.0f);
                Invoke("GeneratePath", 3.0f);
                break;
            case "Blinky":
                GhostBehaviour = PacGhostBehaviour.PATROL;
                Invoke("GeneratePath", 1.0f);
                break;
            case "Inky":
                GhostBehaviour = PacGhostBehaviour.PATROL;
                Invoke("MoveToOrigin", 6.0f);
                Invoke("GeneratePath", 8.0f);
                break;
            case "Clyde":
                GhostBehaviour = PacGhostBehaviour.PATROL;
                Invoke("MoveToOrigin", 3.0f);
                Invoke("GeneratePath", 5.0f);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            switch (name)
            {
                case "Blinky":
                    BlinkyBehaviour();
                    break;
                case "Pinky":
                    PinkyBehaviour();
                    break;
                case "Inky":
                    InkyBehaviour();
                    break;
                case "Clyde":
                    ClydeBehaviour();
                    break;
                default:
                    break;
            }

            if (pathFound)
            {
                FindPath();
            }

            UpdateSprite();
        }
        if(PinkyMove && !CanMove)
        {
            MovePinky();
        }
        if(ClydeMove && !CanMove)
        {
            MoveToOrigin();
        }
    }

    // --------------------
    // BEHAVIOUR UPDATES
    // --------------------

    void BlinkyBehaviour()
    {
        switch (GhostBehaviour)
        {
            case PacGhostBehaviour.PATROL:
                Patrol();
                break;
            case PacGhostBehaviour.HUNT:
                Hunt();
                break;
        }

        if(Pacman != null)
            PacNear = Vector2.Distance(transform.position, Pacman.transform.position) < 10.0f;

        if(GhostBehaviour != PacGhostBehaviour.PATROL && !PacNear)
        {
            GhostBehaviour = PacGhostBehaviour.PATROL;
            Path.Clear();
        }
        else if(GhostBehaviour != PacGhostBehaviour.HUNT && PacNear)
        {
            GhostBehaviour = PacGhostBehaviour.HUNT;
            Path.Clear();
        }
    }

    void PinkyBehaviour()
    {
        switch (GhostBehaviour)
        {
            case PacGhostBehaviour.SPECIALISEDPATROL:
                SpecialisedPatrol();
                break;
        }

        if(GhostBehaviour != PacGhostBehaviour.SPECIALISEDPATROL)
        {
            GhostBehaviour = PacGhostBehaviour.SPECIALISEDPATROL;
            Path.Clear();
        }
    }

    void ClydeBehaviour()
    {
        switch (GhostBehaviour)
        {
            case PacGhostBehaviour.PATROL:
                Patrol();
                break;
            case PacGhostBehaviour.HUNT:
                Hunt();
                break;
        }

        if (Pacman != null)
            PacNear = Vector2.Distance(transform.position, Pacman.transform.position) < 5.0f;

        if(GhostBehaviour != PacGhostBehaviour.PATROL && !PacNear)
        {
            GhostBehaviour = PacGhostBehaviour.PATROL;
            Path.Clear();
        }
        else if(GhostBehaviour != PacGhostBehaviour.HUNT && PacNear)
        {
            GhostBehaviour = PacGhostBehaviour.HUNT;
            Path.Clear();
        }
    }

    void InkyBehaviour()
    {
        switch (GhostBehaviour)
        {
            case PacGhostBehaviour.PATROL:
                Patrol();
                break;
        }

        if (Pacman != null)
            PacNear = Vector2.Distance(transform.position, Pacman.transform.position) < 5.0f;

        if (GhostBehaviour != PacGhostBehaviour.PATROL && !PacNear)
        {
            GhostBehaviour = PacGhostBehaviour.PATROL;
            Path.Clear();
        }
    }

    // --------------------
    // BEHAVIOUR STATES
    // --------------------

    void SpecialisedPatrol()
    {
        if(Path.Count == 0)
        {
            pathFound = false;
            int RandomPath = Random.Range(0, PacMap.NodeList.Count);
            if(Vector2.Distance(transform.position, PacMap.NodeList[RandomPath].transform.position) < 8.0f)
            {
                Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacMap.NodeList[RandomPath], PacMap.NodeList);
                pathFound = true;
            }
        }
    }

    void Patrol()
    {
        if (Path.Count == 0)
        {
            pathFound = false;
            int RandomPath = Random.Range(0, PacMap.NodeList.Count);
            Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacMap.NodeList[RandomPath], PacMap.NodeList);
            pathFound = true;
        }
    }

    void Hunt()
    {
        if(Path.Count == 0)
        {
            pathFound = false;
            Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacAIManager.FindNearestNode(Pacman.transform.position, PacMap.NodeList), PacMap.NodeList);
            pathFound = true;
        }
    }

    void UpdateSprite()
    {
        CurPos = transform.position;
        if (PrevPos.x < CurPos.x && PrevPos.y == CurPos.y)
        {
            CurSprite.sprite = RightLook;
        }
        else if (PrevPos.x > CurPos.x && PrevPos.y == CurPos.y)
        {
            CurSprite.sprite = LeftLook;
        }
        else if (PrevPos.y < CurPos.y && PrevPos.x == CurPos.x)
        {
            CurSprite.sprite = UpLook;
        }
        else if (PrevPos.y > CurPos.y && PrevPos.x == CurPos.x)
        {
            CurSprite.sprite = DownLook;
        }
        PrevPos = CurPos;
    }

/*    void CreatePath()
    {
        if (Path.Count == 0)
        {
            pathFound = false;
            Path = new List<GameObject>();
            int RandomPath = Random.Range(0, PacMap.NodeList.Count);
            Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacMap.NodeList[RandomPath], PacMap.NodeList);
            pathFound = true;
        }
    }*/

    public void GeneratePath()
    {
        int RandomPath = Random.Range(0, PacMap.NodeList.Count);
        Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacMap.NodeList[RandomPath], PacMap.NodeList);
        pathFound = true;
        CanMove = true;
    }

    public void FindPath()
    {
        if (Path != null)
        {
            if (Path.Count > 0)
            {
                for (int i = 0; i < Path.Count - 1; i++)
                {
                    Debug.DrawLine(Path[i].transform.position, Path[i + 1].transform.position, new Color(0, 0, 1));
                }


                int x = 0;
                transform.position = Vector2.MoveTowards(transform.position, Path[0].transform.position, GhostSpeed * Time.deltaTime);
                if (transform.position == Path[x].transform.position)
                {
                    if (Path.Count > 1)
                    {
                        CurrentNode = Path[x + 1];
                        Path.RemoveAt(x);
                    }
                    else
                    {
                        CurrentNode = Path[x];
                        Path.RemoveAt(0);
                    }
                }
            }
        }
    }

    private void MovePinky()
    {
        PinkyMove = true;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(CurrentNode.transform.position.x-0.5f,CurrentNode.transform.position.y), 3.0f * Time.deltaTime);
    }

    private void MoveToOrigin()
    {
        ClydeMove = true;
        if (transform.position.x != CurrentNode.transform.position.x - 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(CurrentNode.transform.position.x - 0.5f, transform.position.y), 3.0f * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(CurrentNode.transform.position.x - 0.5f, CurrentNode.transform.position.y), 3.0f * Time.deltaTime);
        }
    }
}
