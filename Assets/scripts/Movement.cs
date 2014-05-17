using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public bool isPlayer;

    public void Move()
    {
        float direction = (isPlayer) ? 1 : -1;
        transform.Translate(direction * Vector2.right);
    }
}
