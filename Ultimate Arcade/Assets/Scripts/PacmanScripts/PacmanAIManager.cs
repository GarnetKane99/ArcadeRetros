using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanAIManager : MonoBehaviour
{
    public int TotalNodeCount;

    public List<GameObject> BreadthFirstSearch(GameObject Start, GameObject End, List<GameObject> AllNodes)
    {
        List<GameObject> Temp = new List<GameObject>();
        List<GameObject> ToFind = new List<GameObject>();
        if (Start == End)
        {
            return Temp;
        }

        foreach (GameObject Node in AllNodes)
        {
            Node.GetComponent<PacNodeController>().Visited = false;
            //Node.GetComponent<PacNodeController>().Distance = int.MaxValue;
            //Node.GetComponent<PacNodeController>().Previous = default;
        }

        Start.GetComponent<PacNodeController>().Visited = true;

        Temp.Add(Start);

        while (Temp.Count > 0)
        {
            Start = Temp[0];
            Temp.RemoveAt(0);
            List<GameObject> List = Start.GetComponent<PacNodeController>().ConnectedNodes;
            float MaxDist = Vector2.Distance(new Vector2(int.MinValue, int.MinValue), new Vector2(int.MaxValue, int.MaxValue));
            GameObject NodeFound = null;

            foreach (GameObject Node in List)
            {
                if (Node == End)
                {
                    ToFind.Add(Node);
                    return ToFind;
                }
            }

            foreach (GameObject Node in List)
            {
                if (!Node.GetComponent<PacNodeController>().Visited)
                {
                    if (Vector2.Distance(Node.transform.position, End.transform.position) <= MaxDist)
                    {
                        MaxDist = Vector2.Distance(Node.transform.position, End.transform.position);

                        if (NodeFound != null)
                        {
                            Temp.Remove(NodeFound);
                            ToFind.Remove(NodeFound);
                        }
                        NodeFound = Node;
                        Temp.Add(Node);
                        ToFind.Add(Node);
                    }
                    Node.GetComponent<PacNodeController>().Visited = true;
                }
            }
        }

        return ToFind;
    }

    public GameObject FindNearestNode(Vector2 Pos, List<GameObject> AllNodes)
    {
        GameObject NodeFound = null;

        float NearestDist = float.MaxValue;

        foreach (GameObject Node in AllNodes)
        {
            float CurrentNodeDistance = Vector2.Distance(Pos, Node.transform.position);
            if (CurrentNodeDistance < NearestDist)
            {
                NearestDist = CurrentNodeDistance;
                NodeFound = Node;
            }
        }
        return NodeFound;
    }

    public GameObject FindFurthestNode(Vector2 Pos, List<GameObject> AllNodes)
    {
        GameObject NodeFound = null;

        float FurthestDist = float.MinValue;

        foreach(GameObject Node in AllNodes)
        {
            float CurrentNodeDistance = Vector2.Distance(Pos, Node.transform.position);
            if(CurrentNodeDistance > FurthestDist)
            {
                FurthestDist = CurrentNodeDistance;
                NodeFound = Node;
            }
        }

        return NodeFound;
    }
}
