using UnityEngine;
using System.Collections;

public class TouchHandler : MonoBehaviour
{
    private Vector3 startPosition;
    private Game game;

    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
    }

    void Update()
    {
        // Look for all fingers
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            
            // drag
            if (touch.phase == TouchPhase.Began) {
                startPosition = Camera.main.ScreenToWorldPoint(touch.position);
                game.StartTouch();

            } else if (touch.phase == TouchPhase.Moved) {
                Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);
                if (position.y - startPosition.y >= 1) {
                    game.HandleTouch(Vector2.up);
                    startPosition = position;
                } else if (position.y - startPosition.y <= -1) {
                    game.HandleTouch(-Vector2.up);
                    startPosition = position;
                }
            
            } else if (touch.phase == TouchPhase.Ended) {
                game.CompleteTouch();
            }
        }
    }
}
