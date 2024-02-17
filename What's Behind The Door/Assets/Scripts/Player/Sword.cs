using UnityEngine;

public class Sword : MonoBehaviour
{
    Player player;
    public bool shouldCheck = true;
    private void Start()
    {
        player = Player.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!shouldCheck) return;

        IDamageble damageble = other.GetComponent<IDamageble>();
        Enemy enemy = other.GetComponent<Enemy>();

        if (damageble != null && enemy.isAlive && enemy.canGetHit == true)
        {
            var damage = (float)Random.Range(player.force, player.force * 2);

            player.SwordHitSound();

            if (damage > player.force * 2 * .70f && !enemy.isDamaged)
            {
                enemy.TakeDamage(damage, true);
            }

            else
            {
                enemy.TakeDamage(damage, false);
            }
        }
    }
}
