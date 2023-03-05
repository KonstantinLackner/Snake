using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Mono.CompilerServices.SymbolWriter;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BoardManager : MonoBehaviour
{
    private List<GameTile> openList;
    private List<GameTile> closedList;
    private List<List<GameTile>> tileMap; // starts at the top left with 0(y)/0(x)
    [SerializeField] private int mapDimensions;
    [SerializeField] private GameObject protoTile;
    [SerializeField] private Vector3 firstTileCentre;
    [SerializeField] private float tileSize;

    public List<GameTile> result;

    private List<GameTile> aStarSearch(GameTile start, GameTile destination)
    {
        openList = new List<GameTile>();
        closedList = new List<GameTile>();

        start.f = 0f;
        openList.Add(start);

        while (openList.Count > 0)
        {
            GameTile q = openList[0]; // == tile with minimum f
            foreach (GameTile tile in openList)
            {
                if (tile.f < q.f)
                {
                    q = tile;
                }
            }

            openList.Remove(q);

            List<GameTile> neighbours = findNeighbours(q.position);
            foreach (var neighbour in neighbours)
            {
                neighbour.parent = q;
            }

            foreach (var successor in neighbours)
            {
                if (successor.Equals(destination))
                {
                    return closedList;
                }

                int distanceSuccessorQ = manhattanDistance(successor.position, q.position);
                int distanceSuccessorDestination = manhattanDistance(successor.position, destination.position);

                successor.g = q.g + distanceSuccessorQ;
                successor.h = distanceSuccessorDestination;
                successor.f = successor.g + successor.h;

                foreach (var tile in openList)
                {
                    if (tile.position == successor.position && tile.f < successor.f)
                    {
                        goto LoopEnd;
                    }
                }

                foreach (var tile in closedList)
                {
                    if (tile.position == successor.position && tile.f < successor.f)
                    {
                        goto LoopEnd;
                    }
                }

                openList.Add(successor);
                LoopEnd:
                Debug.Log("not the droid you're looking for...");
            }

            closedList.Add(q);
        }

        return closedList;
    }

    private int manhattanDistance(Vector2Int start, Vector2Int end)
    {
        return Math.Abs(start.x - end.x) + Math.Abs(start.y - end.y);
    }

    List<GameTile> findNeighbours(Vector2Int position)
    {
        List<GameTile> neighbours = new List<GameTile>();
        // left tile
        if (position.x > 0)
        {
            neighbours.Add(tileMap[position.y][position.x - 1]);
        }

        // right tile
        if (position.x < tileMap[0].Count - 1)
        {
            neighbours.Add(tileMap[position.y][position.x + 1]);
        }

        // top tile
        if (position.y > 0)
        {
            neighbours.Add(tileMap[position.y - 1][position.x]);
        }

        // bottom tile
        if (position.y < tileMap.Count - 1)
        {
            neighbours.Add(tileMap[position.y + 1][position.x]);
        }

        return neighbours;
    }

    private void createMap()
    {
        tileMap = new List<List<GameTile>>();

        for (int i = 0; i < mapDimensions; i++)
        {
            List<GameTile> line = new List<GameTile>();
            for (int j = 0; j < mapDimensions; j++)
            {
                Vector3 positionAdd = new Vector3(i * tileSize, j * tileSize, 0);
                GameObject newGameTile = Instantiate(protoTile, firstTileCentre + positionAdd, Quaternion.identity);
                newGameTile.GetComponent<GameTile>().position = new Vector2Int(i, j);
                line.Add(newGameTile.GetComponent<GameTile>());
            }

            tileMap.Add(line);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        createMap();
        Debug.Log("ey");
        result = aStarSearch(tileMap[0][0], tileMap[3][3]);
    }

    // Update is called once per frame
    void Update()
    {
    }
}