using UnityEngine;
using UnityEngine.UI;

public class TouchController : MonoBehaviour
{
    public delegate void TouchEventHandler(Vector2 swipe);

    public static event TouchEventHandler SwipeEvent;

    private Vector2 touchMovement;

    private int minSwipeDistance = 20;

    public Text diagnosticText1;
    public Text diagnosticText2;

    public bool useDiagnostic = false;

    private void OnSwipe()
    {
        if (SwipeEvent != null)
        {
            SwipeEvent(touchMovement);
        }
    }

    private void Diagnostic(string text1, string text2)
    {
        diagnosticText1.gameObject.SetActive(useDiagnostic);
        diagnosticText2.gameObject.SetActive(useDiagnostic);

        if (diagnosticText1 && diagnosticText2)
        {
            diagnosticText1.text = text1;
            diagnosticText2.text = text2;
        }
    }

    private string SwipeDiagnostic(Vector2 swipeMovement)
    {
        string direction = "";

        if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
        {
            direction = swipeMovement.x >= 0 ? "right" : "left";
        }
        else
        {
            direction = swipeMovement.y >= 0 ? "up" : "down";
        }

        return direction;
    }

    private void Start()
    {
        Diagnostic("", "");
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began)
            {
                touchMovement = Vector2.zero;
                Diagnostic("", "");
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                touchMovement += touch.deltaPosition;

                if (touchMovement.magnitude > minSwipeDistance)
                {
                    OnSwipe();
                    Diagnostic("Swipe detected",
                               touchMovement.ToString() + " " + SwipeDiagnostic(touchMovement));
                }
            }
        }
    }
}
