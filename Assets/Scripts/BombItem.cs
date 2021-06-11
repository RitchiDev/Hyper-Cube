using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : Item
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player)
        {
            if (!player.Falling)
            {
                return;
            }

            GameManager.Instance.GameOver();
            base.SpawnInteractEffect();
        }
    }
}
