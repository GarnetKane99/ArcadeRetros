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
    public GameObject StartingNode;
    public bool pathFound = false;
    public List<GameObject> SpecialNodes;

    public GameObject Pacman;
    [SerializeField] private bool PacNear = false;


    [SerializeField] private Sprite LeftLook, RightLook, UpLook, DownLook;
    [SerializeField] private Sprite LeftBlueLook, RightBlueLook, UpBlueLook, DownBlueLook;
    [SerializeField] private Sprite UpDead;

    private SpriteRenderer CurSprite;

    private bool PinkyMove = false, ClydeMove = false;
    public bool CanMove = false;

    [SerializeField] Vector2 PrevPos, CurPos;

    PacGhostBehaviour GhostBehaviour;

    public float GhostSpeed = 4.0f;
    public bool PelletEaten = false;
    public bool GhostEaten = false;
    private bool NeedToRespawn = false;
    float RespawnTimer = 0.0f;

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
        Physics2D.IgnoreLayerCollision(3, 3);

        if (CanMove)
        {
            UpdateSprite();
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

        }
        else
        {
            Path.Clear();
        }
        if (PinkyMove && !CanMove)
        {
            MovePinky();
        }
        if (ClydeMove && !CanMove)
        {
            MoveToOrigin();
        }
    }

    // --------------------
    // BEHAVIOUR UPDATES
    // --------------------

    void BlinkyBehaviour()
    {
        if (Pacman != null)
            PacNear = Vector2.Distance(transform.position, Pacman.transform.position) < 10.0f;

        switch (GhostBehaviour)
        {
            case PacGhostBehaviour.PATROL:
                Patrol();
                break;
            case PacGhostBehaviour.HUNT:
                Hunt();
                break;
            case PacGhostBehaviour.RUN:
                if (PacNear)
                {
                    Run();
                }
                else
                {
                    Patrol();
                }
                break;
            case PacGhostBehaviour.RETURN:
                ReturnHome();
                break;
            case PacGhostBehaviour.RESPAWN:
                Respawn();
                break;
        }

        if (GhostBehaviour != PacGhostBehaviour.PATROL && !PacNear && !PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.PATROL;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.HUNT && PacNear && !PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.HUNT;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RUN && PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.RUN;
            Path.Clear();
        }
        else if(GhostBehaviour != PacGhostBehaviour.RETURN && GhostEaten)
        {
            RespawnTimer = 7.0f;
            GhostBehaviour = PacGhostBehaviour.RETURN;
            Path.Clear();
        }
        else if(GhostBehaviour != PacGhostBehaviour.RESPAWN && NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.RESPAWN;
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
            case PacGhostBehaviour.RUN:
                Patrol();
                break;
            case PacGhostBehaviour.RETURN:
                ReturnHome();
                break;
            case PacGhostBehaviour.RESPAWN:
                Respawn();
                break;
        }

        if (GhostBehaviour != PacGhostBehaviour.SPECIALISEDPATROL && !PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.SPECIALISEDPATROL;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RUN && PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.RUN;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RETURN && GhostEaten)
        {
            RespawnTimer = 7.0f;
            GhostBehaviour = PacGhostBehaviour.RETURN;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RESPAWN && NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.RESPAWN;
            Path.Clear();
        }
    }

    void ClydeBehaviour()
    {
        if (Pacman != null)
            PacNear = Vector2.Distance(transform.position, Pacman.transform.position) < 5.0f;

        switch (GhostBehaviour)
        {
            case PacGhostBehaviour.PATROL:
                Patrol();
                break;
            case PacGhostBehaviour.HUNT:
                Hunt();
                break;
            case PacGhostBehaviour.RUN:
                if (PacNear)
                {
                    Run();
                }
                else
                {
                    Patrol();
                }
                break;
            case PacGhostBehaviour.RETURN:
                ReturnHome();
                break;
            case PacGhostBehaviour.RESPAWN:
                Respawn();
                break;
        }

        if (GhostBehaviour != PacGhostBehaviour.PATROL && !PacNear && !PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.PATROL;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.HUNT && PacNear && !PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.HUNT;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RUN && PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.RUN;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RETURN && GhostEaten)
        {
            RespawnTimer = 7.0f;
            GhostBehaviour = PacGhostBehaviour.RETURN;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RESPAWN && NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.RESPAWN;
            Invoke("MovePinky", 1.0f);
            Path.Clear();
        }
    }

    void InkyBehaviour()
    {
        GhostSpeed = 5.0f;
        if (Pacman != null)
            PacNear = Vector2.Distance(transform.position, Pacman.transform.position) < 5.0f;

        switch (GhostBehaviour)
        {
            case PacGhostBehaviour.PATROL:
                Patrol();
                break;
            case PacGhostBehaviour.RUN:
                if (PacNear)
                {
                    Run();
                }
                else
                {
                    Patrol();
                }
                break;
            case PacGhostBehaviour.RETURN:
                ReturnHome();
                break;
            case PacGhostBehaviour.RESPAWN:
                Respawn();
                break;
        }

        if (GhostBehaviour != PacGhostBehaviour.PATROL && !PacNear && !PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.PATROL;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RUN && PelletEaten && !GhostEaten && !NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.RUN;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RETURN && GhostEaten)
        {
            RespawnTimer = 7.0f;
            GhostBehaviour = PacGhostBehaviour.RETURN;
            Path.Clear();
        }
        else if (GhostBehaviour != PacGhostBehaviour.RESPAWN && NeedToRespawn)
        {
            GhostBehaviour = PacGhostBehaviour.RESPAWN;
            Invoke("MovePinky", 1.0f);
            Path.Clear();
        }
    }

    // --------------------
    // BEHAVIOUR STATES
    // --------------------

    void SpecialisedPatrol()
    {
        if (Path.Count == 0)
        {
            pathFound = false;
            int RandomPath = Random.Range(0, PacMap.NodeList.Count);
            if (Vector2.Distance(transform.position, PacMap.NodeList[RandomPath].transform.position) < 8.0f)
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
        if (Path.Count == 0)
        {
            pathFound = false;
            Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacAIManager.FindNearestNode(Pacman.transform.position, PacMap.NodeList), PacMap.NodeList);
            pathFound = true;
        }
    }

    void Run()
    {
        if (Path.Count == 0)
        {
            pathFound = false;
            Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacAIManager.FindFurthestNode(Pacman.transform.position, PacMap.NodeList), PacMap.NodeList);
            pathFound = true;
        }
    }

    void ReturnHome()
    {
        RespawnTimer -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(0.5f,9), GhostSpeed * Time.deltaTime);

        if(RespawnTimer <= 0.0f)
        {
            GhostEaten = false;
            NeedToRespawn = true;
        }
    }

    void Respawn()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(StartingNode.transform.position.x - 0.5f, StartingNode.transform.position.y), 3.0f * Time.deltaTime);
        CurrentNode = StartingNode;
        if(transform.position == new Vector3(StartingNode.transform.position.x-0.5f,StartingNode.transform.position.y,transform.position.z))
        {
            NeedToRespawn = false;
            PelletEaten = false;
        }
    }

    // --------------------
    // Sprite Updates
    // --------------------

    public void UpdateSprite()
    {
        CurPos = transform.position;
        if (!PelletEaten && !GhostEaten)
        {
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
        }
        else if(PelletEaten && !GhostEaten)
        {
            if (PrevPos.x < CurPos.x && PrevPos.y == CurPos.y)
            {
                CurSprite.sprite = RightBlueLook;
            }
            else if (PrevPos.x > CurPos.x && PrevPos.y == CurPos.y)
            {
                CurSprite.sprite = LeftBlueLook;
            }
            else if (PrevPos.y < CurPos.y && PrevPos.x == CurPos.x)
            {
                CurSprite.sprite = UpBlueLook;
            }
            else if (PrevPos.y > CurPos.y && PrevPos.x == CurPos.x)
            {
                CurSprite.sprite = DownBlueLook;
            }
        }
        else if(GhostEaten)
        {
            CurSprite.sprite = UpDead;
        }
        PrevPos = CurPos;
    }

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

                foreach (GameObject Special in SpecialNodes)
                {
                    if (Path[x] == Special)
                    {
                        transform.position = Path[x].transform.position;
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
                        return;
                    }
                }

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
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(CurrentNode.transform.position.x - 0.5f, CurrentNode.transform.position.y), 3.0f * Time.deltaTime);
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
