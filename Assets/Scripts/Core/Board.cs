using System.Collections;

using UnityEngine;

public class Board : MonoBehaviour
{
    private Transform[,] grid;

    public ParticlePlayer[] rowGlowFx = new ParticlePlayer[4];

    public Transform emptySprite;

    public int height = 30;
    public int width = 10;

    public int header = 8;

    public int completedRows = 0;

    private void Start() => DrawEmptyCells();

    private void Awake() => grid = new Transform[width, height];

    private bool IsWithinBoard(int x, int y) => (x >= 0 && x < width && y >= 0);

    private bool IsOccupied(int x, int y, Shape shape) =>
        grid[x, y] != null && grid[x, y].parent != shape.transform;

    private void DrawEmptyCells()
    {
        if (emptySprite != null)
        {
            for (int y = 0; y < height - header; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Transform clone;
                    clone = Instantiate<Transform>(
                        emptySprite, new Vector3(x, y, 0), Quaternion.identity);
                    clone.name =
                        "Board Space ( x = " + x.ToString() + " , y = " + y.ToString() + " )";
                    clone.transform.parent = transform;
                }
            }
        }
        else
        {
            Debug.Log("WARNING! Please assign the emptySprite object!");
        }
    }

    private bool IsComplete(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    private void ClearRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
            }

            grid[x, y] = null;
        }
    }

    private void ShiftOneRowDown(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    private void ShiftRowsDown(int startY)
    {
        for (int i = startY; i < height; i++)
        {
            ShiftOneRowDown(i);
        }
    }

    private void ClearRowFx(int idx, int y)
    {
        if (rowGlowFx[idx])
        {
            rowGlowFx[idx].transform.position = new Vector3(0, y, -2);
            rowGlowFx[idx].Play();
        }
    }

    public bool IsValidPosition(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            Vector2 pos = Vectorf.Round(child.position);

            if (!IsWithinBoard((int)pos.x, (int)pos.y))
            {
                return false;
            }

            if (IsOccupied((int)pos.x, (int)pos.y, shape))
            {
                return false;
            }
        }

        return true;
    }

    public void StoreShapeInGrid(Shape shape)
    {
        if (shape == null)
        {
            return;
        }

        foreach (Transform child in shape.transform)
        {
            Vector2 pos = Vectorf.Round(child.position);
            grid[(int)pos.x, (int)pos.y] = child;
        }
    }

    public IEnumerator ClearAllRows()
    {
        completedRows = 0;

        for (int y = 0; y < height; y++)
        {
            if (IsComplete(y))
            {
                ClearRowFx(completedRows, y);
                completedRows++;
            }
        }

        yield return new WaitForSeconds(0.25f);

        for (int y = 0; y < height; y++)
        {
            if (IsComplete(y))
            {
                ClearRow(y);
                ShiftRowsDown(y + 1);
                yield return new WaitForSeconds(0.15f);
                y--;
            }
        }
    }

    public bool IsOverLimit(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            if (child.transform.position.y >= height - header - 1)
            {
                return true;
            }
        }

        return false;
    }
}
