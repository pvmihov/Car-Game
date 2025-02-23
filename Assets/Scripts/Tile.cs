using UnityEngine;
using UnityEngine.U2D;

public class Tile : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    // Reference to the SpriteShapeController on the child
    private SpriteShapeController spriteShape;
    private Spline spline;
    private float angleBetweenStartAndEnd;

    void Awake()
    {
        // Find the SpriteShapeController component in the child objects
        spriteShape = GetComponentInChildren<SpriteShapeController>();

        if (spriteShape != null)
        {
            // Access the spline from the SpriteShapeController
            spline = spriteShape.spline;
        }
        else
        {
            Debug.LogError($"SpriteShapeController not found in children of {gameObject.name}. Ensure it exists.");
        }
        angleBetweenStartAndEnd = endPoint.localEulerAngles.z - startPoint.localEulerAngles.z;
        if (angleBetweenStartAndEnd > 180) {
            angleBetweenStartAndEnd -= 360;
        }
    }

    public Transform GetStartPoint()
    {
        return startPoint;
    }

    public Transform GetEndPoint()
    {
        return endPoint;
    }

    public float GetAngleBetweenStartAndEnd() {
        return angleBetweenStartAndEnd;
    }

}
