using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Shape[] allShapes;

    private Shape GetRandomShape()
    {
        int i = Random.Range(0, allShapes.Length);
        if (allShapes[i])
        {
            return allShapes[i];
        }
        else
        {
            Debug.Log("WARNING! Invalid shape in spawner!");
            return null;
        }
    }

    public Shape SpawnShape()
    {
        Shape shape = null;
        shape = Instantiate<Shape>(GetRandomShape(), transform.position, Quaternion.identity);
        if (shape)
        {
            return shape;
        }
        else
        {
            Debug.Log("WARNING! Invalid shape in spawner!");
            return null;
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
