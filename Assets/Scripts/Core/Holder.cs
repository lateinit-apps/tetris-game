using UnityEngine;

public class Holder : MonoBehaviour
{
    public Transform holderXform;
    public Shape heldShape = null;

    public bool canRelease = false;

    private float scale = 0.5f;

    public void Catch(Shape shape)
    {
        if (heldShape || !shape)
        {
            return;
        }

        if (holderXform)
        {
            shape.transform.position = holderXform.position + shape.queueOffset;
            shape.transform.localScale  = new Vector3(scale, scale, scale);

            heldShape = shape;
        }
    }

    public Shape Release()
    {
        heldShape.transform.localScale = Vector3.one;
        
        Shape shape = heldShape;
        heldShape = null;

        canRelease = false;

        return shape;
    }
}
