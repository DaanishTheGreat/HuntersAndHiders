using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapTestSceneHandler : MonoBehaviour
{
    public float zoomSpeed = 0.2f;
    public float maxZoom = 2.0f;
    public float minZoom = 1.0f;
    public float slideSpeed = 30.0f;

     private Vector3 startingScale;
    private Vector3 zoomedScale;

    private Vector2 touchStartPos;
    private Vector3 slideVelocity;
    private float initialDistance;

    private const float slideDeceleration = 0.5f;
    private const float minSlideSpeed = 0.01f;

    private void Start()
    {
        startingScale = transform.localScale;
        zoomedScale = startingScale * maxZoom;
    }

    private void Update()
    {
        Vector3 newScale;
        // Handle touch input
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                touchStartPos = (touch1.position + touch2.position) * 0.5f;
                initialDistance = Vector2.Distance(touch1.position, touch2.position);
                slideVelocity = Vector3.zero;
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                float deltaDistance = currentDistance - initialDistance;

                // Zooming
                newScale = transform.localScale + Vector3.one * deltaDistance * zoomSpeed * Time.deltaTime;
                newScale.x = Mathf.Clamp(newScale.x, startingScale.x * minZoom, zoomedScale.x);
                newScale.y = Mathf.Clamp(newScale.y, startingScale.y * minZoom, zoomedScale.y);
                transform.localScale = newScale;

                initialDistance = currentDistance;
            }
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                slideVelocity = Vector3.zero;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // Sliding
                Vector2 touchDelta = touch.position - touchStartPos;
                slideVelocity = new Vector3(touchDelta.x, touchDelta.y, 0) * slideSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + slideVelocity;
                transform.position = newPosition;

                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                // Apply slide deceleration
                slideVelocity *= slideDeceleration;
                if (slideVelocity.magnitude < minSlideSpeed)
                {
                    slideVelocity = Vector3.zero;
                }
            }
        }

        // Apply slide velocity
        Vector3 newPositionAfterSlide = transform.position + slideVelocity;
        transform.position = newPositionAfterSlide;

        // Clamp zoom
        newScale = transform.localScale;
        newScale.x = Mathf.Clamp(newScale.x, startingScale.x * minZoom, zoomedScale.x);
        newScale.y = Mathf.Clamp(newScale.y, startingScale.y * minZoom, zoomedScale.y);
        transform.localScale = newScale;
    }
}
