using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Shape[] allShapes;
    public Transform[] queuedXforms = new Transform[3];

    private Shape[] queuedShapes = new Shape[3];

    private float queueScale = 0.5f;

    private void Start()
    {
        InitQueue();
    }

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
        shape = GetQueuedShape();
        shape.transform.position = transform.position;
        shape.transform.localScale = Vector3.one;

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

    private void InitQueue()
    {
        for (int i = 0; i < queuedShapes.Length; i++)
        {
            queuedShapes[i] = null;
        }

        FillQueue();
    }

    private void FillQueue()
    {
        for (int i = 0; i < queuedShapes.Length; i++)
        {
            if (!queuedShapes[i])
            {
                queuedShapes[i] =
                    Instantiate<Shape>(GetRandomShape(), transform.position, Quaternion.identity);
                queuedShapes[i].transform.position =
                    queuedXforms[i].position + queuedShapes[i].queueOffset;
                queuedShapes[i].transform.localScale =
                    new Vector3(queueScale, queueScale, queueScale);
            }
        }
    }

    private Shape GetQueuedShape()
    {
        Shape firstShape = null;

        if (queuedShapes[0])
        {
            firstShape = queuedShapes[0];
        }

        for (int i = 1; i < queuedShapes.Length; i++)
        {
            queuedShapes[i - 1] = queuedShapes[i];
            queuedShapes[i - 1].transform.position =
                queuedXforms[i - 1].position + queuedShapes[i].queueOffset;
        }

        queuedShapes[queuedShapes.Length - 1] = null;

        FillQueue();

        return firstShape;
    }
}
