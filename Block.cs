/*
*SCRIPT MADE BY ALEXANDER SEMENOV

*USED FOR HIS OWN PEOJECT
*/
using UnityEngine;

public class Block : MonoBehaviour
{
    // Health pool of the brick
    public float health;    

    /// <summary>
    /// Brick takes damage and if health is below 0 it dies
    /// </summary>
    /// <param name="damage">ammount of damage taken</param>
    public void ReceiveDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
