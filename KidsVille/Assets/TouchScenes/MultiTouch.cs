using UnityEngine;
using UnityEngine.UI;

public class MultiTouch : MonoBehaviour
{
    int touchesInt, touchesControl;
    public Text text;
    bool isTouching = false;

    //Pinch elements:
    public bool canPinch;
    public float perspectiveZoomSpeed = 0.05f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.05f;        // The rate of change of the orthographic size in orthographic mode.
    private Camera camera;
    private Transform cameraTransform;
    Vector3 posCam;

    //Double Touch:
    float doubleTapTimer;
    int tapCount;

    private void Start()
    {
        camera = Camera.main;
        cameraTransform = camera.transform;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touchesInt = Input.touchCount;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            touchesInt = 0;
            isTouching = false;
            print("Is touching? " + isTouching);
            text.text = "Nothing is touching the screen.";

        }

        if (touchesInt != touchesControl)
        {
            print(touchesInt);
            text.text = "Number of touches: " + touchesInt.ToString();
            touchesControl = touchesInt;
        }

        if (canPinch)
        {
            if (Input.touchCount == 2)
            {               
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                if (touchZero.phase == TouchPhase.Moved && touchOne.phase == TouchPhase.Moved)
                {
                    ZoomWithPinch(touchZero, touchOne);
                }
            }
        }

        DoubleTap();
    }

    void DoubleTap()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            tapCount++;
        }
        if (tapCount > 0)
        {
            doubleTapTimer += Time.deltaTime;
        }
        if (tapCount >= 2)
        {
            print("Double tap");
            text.text = "Double tap.";
            doubleTapTimer = 0.0f;
            tapCount = 0;
        }
        if (doubleTapTimer > 0.5f)
        {
            doubleTapTimer = 0f;
            tapCount = 0;
        }
    }

    void ZoomWithPinch(Touch touchZero, Touch touchOne)
    {
        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        if (prevTouchDeltaMag > touchDeltaMag)
        {
            text.text = "Zoom out.";
        }
        else if (prevTouchDeltaMag < touchDeltaMag)
        {
            text.text = "Zoom in.";
        }

        // If the camera is orthographic...
        if (camera.orthographic)
        {
            // ... change the orthographic size based on the change in distance between the touches.
            camera.orthographicSize += deltaMagnitudeDiff * (orthoZoomSpeed);

            // Make sure the orthographic size never drops below zero.
            camera.orthographicSize = Mathf.Max(camera.orthographicSize, 0.1f);
        }
        else
        {
            // Otherwise change the field of view based on the change in distance between the touches.
            camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

            // Clamp the field of view to make sure it's between 0 and 180.
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
        }
    }
}
