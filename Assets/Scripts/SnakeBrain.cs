using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Apple;

public class SnakeBrain : MonoBehaviour
{
    private List<BodyElement> snake = new List<BodyElement>();
    private BodyElement head;
    private Vector2Int destination;
    [SerializeField] private GameObject protoSnakeTile;
    [SerializeField] private BoardManager boardManager;
    public List<GameTile> path;
    private Vector2Int lastDestination;

    // Start is called before the first frame update
    void Start()
    {
        path = new List<GameTile>();
        buildSnake(new Vector2Int(0, 0), new Vector2Int(3, 3));
        lastDestination = head.position;
        StartCoroutine(moveSnake(new Vector2Int(7, 7)));
    }

    void buildSnake(Vector2Int headPosition, Vector2Int tailPosition)
    {
        List<GameTile> snakePositions = boardManager.getPath(headPosition, tailPosition);

        BodyElement previousElement = null;
        for (int i = 0; i < snakePositions.Count; i++)
        {
            if (i == 0)
            {
                head = Instantiate(protoSnakeTile, new Vector3(headPosition.x, headPosition.y, 0), Quaternion.identity)
                    .GetComponent<BodyElement>();
                head.position = headPosition;
                head.isHead = true;
                previousElement = head;
                snake.Add(head);
            }
            else
            {
                BodyElement bodyPart =
                    Instantiate(protoSnakeTile,
                            new Vector3(snakePositions[i].position.x, snakePositions[i].position.y, 0),
                            Quaternion.identity)
                        .GetComponent<BodyElement>();
                bodyPart.position = snakePositions[i].position;
                bodyPart.beforeMe = previousElement;
                previousElement = bodyPart;
                snake.Add(bodyPart);
            }
        }
        Debug.Log("Built snake!");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void addPath(Vector2Int destination)
    {
        List<GameTile> toAdd = boardManager.getPath(lastDestination, destination);
        lastDestination = destination;
        path.AddRange(toAdd);
    }
    
    IEnumerator moveSnake(Vector2Int destination)
    {
        while (true)
        {
            if (path.Count > 0)
            {
                List<GameTile> currentPath = new List<GameTile>(path);
                foreach (var tile in currentPath)
                {
                    foreach (var bodyPart in snake)
                    {
                        if (bodyPart.isHead)
                        {
                            bodyPart.moveHead(tile.position);
                        }
                        else
                        {
                            bodyPart.moveBody();
                        }
                    }
                
                    yield return new WaitForSeconds(0.1f);
                }
                path.RemoveRange(0, currentPath.Count);
            }
            
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}