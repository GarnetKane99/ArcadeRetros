using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class TetrisBoardManager : MonoBehaviour
{
    public int[,] GridSize;
    public Sprite TileSprite;
    public GameObject BorderParent;
    private List<GameObject> BorderChild;
    [SerializeField] private List<GameObject> BorderInner;

    public GameObject GameOverScreen;
    [SerializeField] private TetrisScoreHandler Score;
    [SerializeField] private GameObject DefaultTile;
    //[SerializeField] private List<GameObject> Pieces;
    [SerializeField] private TetrisMainGeneration MainGen;
    [SerializeField] private ParticleSystem DestroyedParticles;

    private bool DestroyFound = false;

    [SerializeField] private TextMeshProUGUI TETRIS_TEXT;

    // Start is called before the first frame update
    void Start()
    {
        BorderChild = new List<GameObject>();
        BorderInner = new List<GameObject>();
        InitializeGrid();
        MainGen = GetComponent<TetrisMainGeneration>();
    }

    void InitializeGrid()
    {
        GridSize = new int[11, 22];
        for (int x = 0; x < GridSize.GetLength(0); x++)
        {
            for (int y = 0; y < GridSize.GetLength(1); y++)
            {
                if (x == 0 || x == GridSize.GetLength(0) - 1 || y == 0)
                {
                    DrawGrid(x, y, 1);
                    GridSize[x, y] = 2;
                }
                else
                {
                    DrawGrid(x, y, 0);
                    GridSize[x, y] = 0;
                }
            }
        }
    }

    public void DrawGrid(int x, int y, int val)
    {
        GameObject Tile = Instantiate(DefaultTile, new Vector2(x - 5, y - 9), Quaternion.identity);
        var Sprite = Tile.GetComponent<SpriteRenderer>();
        if (val == 0)
        {
            Sprite.color = new Color(1, 1, 1, 25f / 255f);
            BorderInner.Add(Tile);
        }
        else
        {
            BorderChild.Add(Tile);
        }
        Tile.transform.parent = BorderParent.transform;
    }

    void Update()
    {
        /*        StringBuilder sb = new StringBuilder();
                for (int y = GridSize.GetLength(1) - 1; y >= 0; y--)
                {
                    for (int x = 0; x < GridSize.GetLength(0); x++)
                    {
                        if (y == GridSize.GetLength(1) - 1)
                        {
                            GridSize[x, y] = 0;
                        }
                        sb.Append(GridSize[x, y]);
                        sb.Append(' ');
                    }
                    sb.AppendLine();
                }
                Debug.Log(sb.ToString());*/
    }

    public void UpdateIndividualPieces(int xPos, int yPos)
    {
        for (int i = 0; i < BorderInner.Count; i++)
        {
            if (BorderInner[i].transform.position.x == xPos - 5 && BorderInner[i].transform.position.y == yPos - 9)
            {
                switch (GridSize[xPos, yPos])
                {
                    case 0:
                        BorderInner[i].GetComponent<UpdateTileSprite>().PieceToUse(7);
                        break;
                    case 3:
                        BorderInner[i].GetComponent<UpdateTileSprite>().PieceToUse(0);
                        break;
                    case 4:
                        BorderInner[i].GetComponent<UpdateTileSprite>().PieceToUse(1);
                        break;
                    case 5:
                        BorderInner[i].GetComponent<UpdateTileSprite>().PieceToUse(2);
                        break;
                    case 6:
                        BorderInner[i].GetComponent<UpdateTileSprite>().PieceToUse(3);
                        break;
                    case 7:
                        BorderInner[i].GetComponent<UpdateTileSprite>().PieceToUse(4);
                        break;
                    case 8:
                        BorderInner[i].GetComponent<UpdateTileSprite>().PieceToUse(5);
                        break;
                    case 9:
                        BorderInner[i].GetComponent<UpdateTileSprite>().PieceToUse(6);
                        break;
                }
            }
        }
    }

    private void UpdateVisuals(int x, int y)
    {
        float t = 0;

        for (int i = 0; i < BorderInner.Count; i++)
        {
            if (BorderInner[i].transform.position.x == x - 5 && BorderInner[i].transform.position.y == y - 9)
            {
                SpriteRenderer Sprite = BorderInner[i].GetComponent<SpriteRenderer>();
                SpriteRenderer DefaultSprite = DefaultTile.GetComponent<SpriteRenderer>();
                do
                {
                    t += Time.deltaTime / 2.0f;
                    Sprite.color = Color.Lerp(Sprite.color, new Color(DefaultSprite.color.r, DefaultSprite.color.g, DefaultSprite.color.b, 25f / 255f), t);
                }
                while (t < 1.0f);
                ParticleSystem ParticlesFound = BorderInner[i].GetComponentInChildren<ParticleSystem>();
                ParticlesFound.textureSheetAnimation.SetSprite(0, BorderInner[i].GetComponent<SpriteRenderer>().sprite);
                ParticlesFound.Play();
            }
        }
    }

    public void FindFinishedRow()
    {
        DestroyFound = false;
        int LinesDestroyed = 0;
        for (int y = GridSize.GetLength(1) - 1; y >= 0; y--)
        {
            int Found = 0;
            for (int x = 0; x < GridSize.GetLength(0); x++)
            {
                if (y != 0 && y != GridSize.GetLength(1) - 1)
                {
                    if (FoundRow(x, y))
                    {
                        Found++;
                    }
                }
                else if (y == GridSize.GetLength(1) - 1)
                {
                    if (FoundRow(x, y - 1))
                    {
                        EndGame();
                        return;
                    }
                }

            }

            if (Found == 9)
            {
                LinesDestroyed++;
                StartCoroutine(DeleteRow(y));
                DestroyFound = true;
            }
        }

        float t = 0;
        if (LinesDestroyed == 4)
        {
            TETRIS_TEXT.color = new Color(1, 1, 1, 0);
            TETRIS_TEXT.gameObject.SetActive(true);
            TETRIS_TEXT.text = "TETRIS\n" + (Score.GetLevel() * 1000).ToString();
            do
            {
                t += Time.deltaTime / 2.0f;
                TETRIS_TEXT.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), t);
            } while (t < 1.0f);
        }

        if (!DestroyFound)
        {
            MainGen.GenerateBlock();
        }
    }

    void EndGame()
    {
        foreach (GameObject BorderChildren in BorderChild)
        {
            SpriteRenderer ColourFound = BorderChildren.GetComponent<SpriteRenderer>();
            ColourFound.color = new Color(ColourFound.color.r, ColourFound.color.g, ColourFound.color.b, 100f / 255f);
        }
        foreach (GameObject BorderIn in BorderInner)
        {
            SpriteRenderer ColourFound = BorderIn.GetComponent<SpriteRenderer>();
            if (ColourFound.color.a == 255)
            {
                ColourFound.color = new Color(ColourFound.color.r, ColourFound.color.g, ColourFound.color.b, 100f / 255f);
            }
            else
            {
                ColourFound.color = new Color(ColourFound.color.r, ColourFound.color.g, ColourFound.color.b, ColourFound.color.a - 5f / 255f);
            }
        }

        MainGen.enabled = false;
        GameOverScreen.SetActive(true);
    }

    bool FoundRow(int x, int y)
    {
        if (GridSize[x, y] == 3
            || GridSize[x, y] == 4
            || GridSize[x, y] == 5
            || GridSize[x, y] == 6
            || GridSize[x, y] == 7
            || GridSize[x, y] == 8
            || GridSize[x, y] == 9)
        {
            return true;
        }

        return false;
    }

    IEnumerator DeleteRow(int RowFound)
    {
        Score.SetCurrentScore();
        Score.SetCurrentLevel();

        int[,] tempArray = GridSize;

        for (int y = 0; y < tempArray.GetLength(1); y++)
        {
            for (int x = 1; x < tempArray.GetLength(0) - 1; x++)
            {
                if (y == RowFound)
                {
                    UpdateVisuals(x, y);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }


        yield return new WaitForSeconds(0.7f);
        for (int y = 0; y < tempArray.GetLength(1); y++)
        {
            for (int x = 1; x < tempArray.GetLength(0) - 1; x++)
            {


                if (y >= RowFound)
                {
                    if (y < tempArray.GetLength(1) - 1)
                    {
                        if (tempArray[x, y + 1] != 2)
                        {
                            tempArray[x, y] = tempArray[x, y + 1];
                            UpdateIndividualPieces(x, y);
                        }
                    }
                }
            }
        }

        GridSize = tempArray;
        if (TETRIS_TEXT.gameObject.activeInHierarchy && DestroyFound)
        {
            MainGen.GenerateBlock();
            DestroyFound = false;
            yield return new WaitForSeconds(0.5f);
            TETRIS_TEXT.gameObject.SetActive(false);
        }
        else if (DestroyFound)
        {
            MainGen.GenerateBlock();
            DestroyFound = false;
        }
    }
}
