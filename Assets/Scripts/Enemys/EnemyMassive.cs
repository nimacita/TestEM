using UnityEngine;

public class EnemyMassive : EnemyController
{
    //не прерывается при получении урона
    protected override void OnTakedDamage(float damage, Vector2 damageSourcePosition)
    {
        if (!isAlive) return;

        flash.Flash();
        StopAttack();
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }
    }
}
