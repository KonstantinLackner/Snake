using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyElement : MonoBehaviour
{
    public bool isHead { get; set; } = false;
    public BodyElement beforeMe { get; set; }
    public Vector2Int position { get; set; }
    private Vector2Int lastPosition;

    public void moveHead(Vector2Int newPosition)
    {
        lastPosition = position;
        position = newPosition;
        transform.position = new Vector3(position.x, position.y, 0);
    }

    public void moveBody()
    {
        lastPosition = position;
        position = beforeMe.lastPosition;
        transform.position = new Vector3(position.x, position.y, 0);
        Debug.Log("moved to position" + position.ToString());
    }
}