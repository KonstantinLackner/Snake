using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTile : MonoBehaviour
{

    public float f { get; set; }
    public float g { get; set; }
    public float h { get; set; }
    [SerializeField] public GameTile parent;
    public Vector2Int position { get; set; }
    public TMP_Text text;

    public GameTile(Vector2Int position)
    {
        this.position = position;
        f = 0f;
        g = 0f;
        h = 0f;
        parent = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
