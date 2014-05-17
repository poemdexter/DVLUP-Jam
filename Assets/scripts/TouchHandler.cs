using UnityEngine;
using System.Collections;

public class TouchHandler : MonoBehaviour
{
    public float deadZone;
    private bool isTouching = false;
    private Vector3 startPosition;

    void Update()
    {
        // Look for all fingers
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            
            // drag
            if (touch.phase == TouchPhase.Began) {
                startPosition = Camera.main.ScreenToWorldPoint(touch.position);
                isTouching = true;
            } else if (touch.phase == TouchPhase.Moved) {
                Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);
                if (Mathf.Abs(position.y - startPosition.y) >= 1) {
                    Debug.Log("moved 1");
                    startPosition = position;
                }
            } else if (touch.phase == TouchPhase.Ended) {
                isTouching = false;
            }
        }
    }
}
