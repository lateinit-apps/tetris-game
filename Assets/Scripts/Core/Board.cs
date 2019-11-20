using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform emptySprite;

    public int height = 30;
    public int width = 10;

    public int header = 8;

    private Transform [,] grid;
    
    private void Awake()
    {
        grid = new Transform[width, height];        
    }

    private void Start()
    {
        DrawEmptyCells();
    }

    private void Update()
    {
        
    }

    private void DrawEmptyCells()
    {
        if (emptySprite != null)
        {
            for (int y = 0; y < height - header; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Transform clone;
                    clone = Instantiate<Transform>(emptySprite, new Vector3(x, y, 0), Quaternion.identity);
                    clone.name = "Board Space ( x = " + x.ToString() + " , y = " + y.ToString() + " )";
                    clone.transform.parent = transform;
                }
            }
        }
        else
        {
            Debug.Log("WARNING! Please assign the emptySprite object!");
        }
    }
}
