using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloydWarshall : MonoBehaviour
{
    bool debug = true;
    Dictionary<Vector3, int> dict = new Dictionary<Vector3, int>();
    List<List<int>> adjList = new List<List<int>>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitFloydWarshall(string[] floors)
    {
        for (int i = 0; i < floors.Length; i++)
        {
            string[][] matrix = CreateMapMatrix(floors[i]);
            
            if (debug)
                PrintMatrix(matrix);

            MapTilesToNodes(matrix, i);
        }
    }

    // Takes a string representation of a 2D map and returns it
    // as a 2D array of strings.
    string[][] CreateMapMatrix(string floor)
    {
        string[] rows = floor.Split('\n');
        string[][] matrix = new string[rows.Length][];

        for (int i = 0; i < rows.Length; i++)
        {
            matrix[i] = rows[i].Split(',');
        }

        return matrix;
    }

    // Creates a mapping from a tile's position to a unique integer for each tile.
    void MapTilesToNodes(string[][] map, int floor)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                string tile = map[i][j];
                if (string.Compare(tile, "") == 0 || string.Compare(tile, ".") == 0)
                    dict.Add(new Vector3(floor, i, j), dict.Count + 1);
            }
        }
    }

    void RunFloydWarshall()
    {

    }

    void PrintMatrix(string[][] levels)
    {
        Debug.Log("Printing Level Map");

        for (int i = 0; i < levels.GetLength(0); i++)
            {
                for (int j = 0; j < levels[i].Length; j++)
                {
                    Debug.Log(levels[i][j]);
                }
            }
    }
}
