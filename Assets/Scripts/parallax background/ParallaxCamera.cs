using UnityEngine;

[DefaultExecutionOrder(100)]
[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(Vector2 deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    public bool useX = true;
    public bool useY = false;

    private Vector3 oldPosition;

    void Start()
    {
        oldPosition = transform.position;
    }

    void LateUpdate()
    {
        if (transform.position != oldPosition)
        {
            if (onCameraTranslate != null)
            {
                Vector3 delta = oldPosition - transform.position;

                Vector2 filteredDelta = new Vector2(
                    useX ? delta.x : 0f,
                    useY ? delta.y : 0f
                );

                onCameraTranslate(filteredDelta);
            }

            oldPosition = transform.position;
        }
    }

}
