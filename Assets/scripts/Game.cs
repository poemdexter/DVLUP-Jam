using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Type
{
    Empty,
    P_Knight,
    E_Knight
}
public class Game : MonoBehaviour
{
    private int[][] level;
    private int width = 14;
    private int height = 8;

    private bool gameStarted = false;
    public float tickTime;
    private float currentTime;
    public float tickMoveDelay;

    private List<GameObject> mobs;

    void Start()
    {
        InitLevel();
        mobs = new List<GameObject>();
        gameStarted = true;
    }

    private void InitLevel()
    {
        // create the int[][]
        List<int[]> rows = new List<int[]>();
        for (int i = 0; i < width; i++) {
            rows.Add(new int[height]);
        }
        level = rows.ToArray();

        // set everything to 0
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                level[x][y] = 0;
            }
        }
    }

    // game tick
    private void Tick()
    {
        // - check enemies can move and move enemies
        EnemyMovement();
        // - small delay
        StartCoroutine(DelayMove());
        // - check players can move and move players
        PlayerMovement();
        // - resolve damage
    }

    private void EnemyMovement()
    {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (level[x][y] == (int)Type.E_Knight) {                    // got enemy
                    if (x - 1 > 0 && level[x - 1][y] == (int)Type.Empty) {  // not lose state && space to left empty
                        level[x - 1][y] = (int)Type.E_Knight;               // move enemy in array
                        level[x][y] = (int)Type.Empty;                      // set old space to empty
                        MoveMob(new Vector2(x, y));                         // move the gameobject
                    }
                }
            }
        }
    }

    private void PlayerMovement()
    {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (level[x][y] == (int)Type.P_Knight) {                            // got player
                    if (x + 1 < width - 1 && level[x + 1][y] == (int)Type.Empty) {  // not win state && space to left empty
                        level[x + 1][y] = (int)Type.P_Knight;                       // move enemy in array
                        level[x][y] = (int)Type.Empty;                              // set old space to empty
                        MoveMob(new Vector2(x, y));                                 // move the gameobject
                    }
                }
            }
        }
    }

    private void MoveMob(Vector2 mobPosition)
    {
        var m = mobs.Single(mob => mob.GetComponent<Mob>().gridPosition == mobPosition);
        m.GetComponent<Movement>().Move();
    }

    IEnumerator DelayMove()
    {
        float t = tickMoveDelay;
        if ((t -= Time.deltaTime) > 0)
            yield return 0;

        return 0;
    }

    void Update()
    {
        if (gameStarted) {
            if ((currentTime += Time.deltaTime) > tickTime) {
                currentTime = 0;
                Tick();
            }
        }
    }
}
