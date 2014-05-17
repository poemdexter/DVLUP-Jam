﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Type
{
    Empty,
    P_Knight,
    E_Knight,
    P_Archer,
    E_Archer
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

    public GameObject enemyKnight;
    public GameObject enemyArcher;

    private List<GameObject> mobs;

    public GameObject[] playerPrefabs;
    private GameObject nextPrefab;
    private Vector2 nextPrefabPosition;
    private bool playerTouchedPrefab;


    void Start()
    {
        InitLevel();
        mobs = new List<GameObject>();
        gameStarted = true;
        FieldPlayerPrefab();
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
        // - apply damage then resolve damage
        CheckDamage();
        ResolveDamage();
        // - field the enemy
        SpawnEnemy(2);
        // - field the player's next prefab if touched and let go
        FieldPlayerPrefab();
    }

    private void EnemyMovement()
    {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (level[x][y] != (int)Type.Empty) {                           // not empty
                    var mob = mobs.Single(m => m.GetComponent<Mob>().gridPosition == new Vector2(x, y));
                    if (!mob.GetComponent<Mob>().isPlayer) {                    // got enemy
                        if (x > 0 && level[x - 1][y] == (int)Type.Empty) {  // not lose state && space to left empty
                            level[x - 1][y] = level[x][y];                      // move enemy in array
                            level[x][y] = (int)Type.Empty;                      // set old space to empty
                            MoveMob(new Vector2(x, y));                         // move the gameobject
                            mob.GetComponent<Mob>().gridPosition = new Vector2(x - 1, y);
                        }
                    }
                }
            }
        }
    }

    // FIXME
    private void PlayerMovement()
    {
        for (int x = width -1; x >= 0; x--) {
            for (int y = height -1; y >= 0; y--) {
                if (level[x][y] != (int)Type.Empty) {                                   // not empty
                    var mob = mobs.Single(m => m.GetComponent<Mob>().gridPosition == new Vector2(x, y));
                    if (mob.GetComponent<Mob>().isPlayer) {                             // got player
                        Debug.Log("ha");
                        if (x + 1 < width - 1 && level[x + 1][y] == (int)Type.Empty) {  // not win state && space to left empty
                            level[x + 1][y] = level[x][y];                              // move enemy in array
                            level[x][y] = (int)Type.Empty;                              // set old space to empty
                            MoveMob(new Vector2(x, y));                                 // move the gameobject
                            Debug.Log("heh");
                            mob.GetComponent<Mob>().gridPosition = new Vector2(x + 1, y);
                        }
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
        yield return 1;
    }

    private void CheckDamage()
    {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (level[x][y] != (int)Type.Empty) { // not empty
                    var m1 = mobs.Single(i => i.GetComponent<Mob>().gridPosition == new Vector2(x, y));
                    Mob mob1 = m1.GetComponent<Mob>();

                    // hasn't attacked yet
                    if (!mob1.hasAttacked) {
                        // is a player
                        if (mob1.isPlayer) {
                            if (x + 1 < width - 1 && level[x + 1][y] != (int)Type.Empty) {
                                var m2 = mobs.Single(i => i.GetComponent<Mob>().gridPosition == new Vector2(x, y));
                                Mob mob2 = m2.GetComponent<Mob>();
                                // attacking an enemy
                                if (!mob2.isPlayer) {
                                    // apply damage to each other
                                    ApplyDamage(mob1, mob2);
                                }
                            }
                            // is a enemy
                        } else {
                            if (x - 1 > 0 && level[x - 1][y] == (int)Type.Empty) {
                                var m2 = mobs.Single(i => i.GetComponent<Mob>().gridPosition == new Vector2(x, y));
                                Mob mob2 = m2.GetComponent<Mob>();
                                // attacking a player
                                if (mob2.isPlayer) {
                                    // apply damage to each other
                                    ApplyDamage(mob1, mob2);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void ApplyDamage(Mob m1, Mob m2)
    {
        m1.health -= m2.damage;
        m1.hasAttacked = true;
        if (!m2.hasAttacked) {
            m2.damage -= m1.damage;
            m2.hasAttacked = true;
        }
    }
    
    private void ResolveDamage()
    {
        List<GameObject> deadMobs = new List<GameObject>();
        foreach (GameObject mob in mobs) {
            if (mob.GetComponent<Mob>().health <= 0) {
                deadMobs.Add(mob);
            }
        }

        foreach (GameObject dead in deadMobs) {
            mobs.Remove(dead);
        }

        for (int i = deadMobs.Count -1; i >= 0; i--) {
            Destroy(deadMobs[i]);
        }
    }

    private void FieldPlayerPrefab()
    {
        if (nextPrefab == null) { // instantiate prefab
            int r = Random.Range(0, playerPrefabs.Count());
            Instantiate(playerPrefabs[r], new Vector2(1, -4), Quaternion.identity);
        }

        if (playerTouchedPrefab) {
            playerTouchedPrefab = false;
        }
    }

    private void SpawnEnemy(int numberToSpawn)
    {	
        // Dirty jam code, clean up later
        int lastSpawn = 8;
        int spawnRow = 0;			
        int typeToSpawn;		// rolls a random number to pick the enemy to spawn
        GameObject enemy = enemyKnight;

        for (int x = 0; x < numberToSpawn; x++) {
            typeToSpawn = Random.Range(0, 4);
            if (typeToSpawn < 3)
                enemy = enemyKnight;
            if (typeToSpawn == 3)
                enemy = enemyArcher;

            spawnRow = Random.Range(0, 8);
            while (spawnRow == lastSpawn) {
                spawnRow = Random.Range(0, 8);									//Sets enemy to spawn at random row
            }										

            GameObject go = (GameObject)Instantiate(enemy, new Vector2((width - 1), -spawnRow), Quaternion.identity);
            go.GetComponent<Mob>().gridPosition = new Vector2(width - 1, spawnRow);
            mobs.Add(go);
            lastSpawn = spawnRow;
            if (enemy == enemyKnight)
                level[width - 1][spawnRow] = (int)Type.E_Knight;
            else if (enemy == enemyArcher)
                level[width - 1][spawnRow] = (int)Type.E_Archer;
        }
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

    public void HandleTouch(Vector2 direction)
    {
        if (nextPrefab != null)
            nextPrefab.transform.Translate(direction);
    }

    public void CompleteTouch()
    {
        playerTouchedPrefab = true;
    }
}
