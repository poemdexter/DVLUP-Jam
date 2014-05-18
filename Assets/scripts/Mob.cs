using UnityEngine;
using System.Collections;

public class Mob : MonoBehaviour
{
    public Vector2 gridPosition;
    public int xRange;
    public int yRange;
    public int health;
    public int maxHealth;
    public int damage;
    public bool hasAttacked;
    public bool isPlayer;
    public Type type;
}
