using System;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class BoardManager : MonoBehaviour
{
    private List<GameTile> openList;
    private List<GameTile> closedList;
    private List<List<GameTile>> tileMap; // starts at the top left with 0(x)/0(y)
    [SerializeField] private int mapDimensions;
    [SerializeField] private GameObject protoTile;
    [SerializeField] private Vector3 firstTileCentre;
    public List<GameTile> path;
    private float tileSize;

    
    /**
     * https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
     */
    private void aStarSearch(GameTile start, GameTile destination)
    {
        openList = new List<GameTile>();
        closedList = new List<GameTile>();

        start.f = 0f;
        openList.Add(start);

        while (openList.Count > 0)
        {
            GameTile currentTile = openList[0]; // == tile with minimum f
            foreach (GameTile tile in openList)
            {
                if (tile.f < currentTile.f)
                {
                    currentTile = tile;
                }
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile.Equals(destination))
            {
                Debug.Log("Succeeded to find a path!");
                return;
            }

            List<GameTile> successors = findNeighbours(currentTile.position);

            foreach (var successor in successors)
            {
                if (closedList.Contains(successor))
                {
                    continue;
                }

                if (!openList.Contains(successor))
                {
                    openList.Add(successor);
                    successor.parent = currentTile;

                    int distanceSuccessorQ = manhattanDistance(successor.position, currentTile.position);
                    int distanceSuccessorDestination = manhattanDistance(successor.position, destination.position);

                    successor.g = currentTile.g + distanceSuccessorQ;
                    successor.h = distanceSuccessorDestination;
                    successor.f = successor.g + successor.h;
                }
                else
                {
                    int newDistanceSuccessorQ = manhattanDistance(successor.position, currentTile.position);
                    float newG = currentTile.g + newDistanceSuccessorQ;

                    if (newG < successor.g)
                    {
                        successor.parent = currentTile;
                        successor.g = newG;
                        successor.f = successor.g + successor.h;
                    }
                }

                if (successor.position.Equals(destination.position))
                {
                    Debug.Log("Succeeded to find a path!");
                    return;
                }
            }
        }
        Debug.Log("Failed");
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
            neighbours.Add(tileMap[position.x - 1][position.y]);
        }

        // right tile
        if (position.x < tileMap[0].Count - 1)
        {
            neighbours.Add(tileMap[position.x + 1][position.y]);
        }

        // top tile
        if (position.y > 0)
        {
            neighbours.Add(tileMap[position.x][position.y - 1]);
        }

        // bottom tile
        if (position.y < tileMap.Count - 1)
        {
            neighbours.Add(tileMap[position.x][position.y + 1]);
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
                Vector2Int position = new Vector2Int(i, j);
                newGameTile.GetComponent<GameTile>().position = position;
                newGameTile.GetComponent<GameTile>().text.text = position.ToString();
                newGameTile.name = position.ToString();
                line.Add(newGameTile.GetComponent<GameTile>());
            }

            tileMap.Add(line);
        }
    }

    void Awake()
    {
        tileSize = protoTile.transform.localScale.x;
        createMap();
    }

    public List<GameTile> getPath(Vector2Int startPosition, Vector2Int destinationPosition)
    {
        GameTile start = tileMap[startPosition.x][startPosition.y];
        GameTile destination = tileMap[destinationPosition.x][destinationPosition.y];
        
        List<GameTile> path = new List<GameTile>();
        aStarSearch(start, destination);
        GameTile currentTile = destination;
        
        path.Add(destination);
        while (!currentTile.Equals(start))
        {
            currentTile = currentTile.parent;
            path.Add(currentTile);
        }
        
        path.Reverse();
        
        return path;
    }

    // Update is called once per frame
    void Update()
    {
    }
}