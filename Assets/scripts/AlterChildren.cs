using UnityEngine;
using System.Collections;

public class AlterChildren : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++) {
            Transform t = transform.GetChild(i);
            t.GetComponent<Mob>().gridPosition = new Vector2(1 + t.position.x, 4 + t.position.y);
        }
    }
}
