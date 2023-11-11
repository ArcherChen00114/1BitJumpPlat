using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageAbleObject : MonoBehaviour
{
    public int damage;
    public int AttackerLayer;
    public bool DamageUsed;
    private Collider2D coll;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //确保一次投掷仅能造成一次伤害
        if (!DamageUsed) {
            Character collisionCharacter=collision.gameObject.GetComponent<Character>();
            //确保有可伤害的Character脚本
            if (collisionCharacter != null) { 
                //确保攻击者以及其队友不会被它们自己的射击物所伤害
                if(collision.gameObject.layer != AttackerLayer)
                {
                    //也就是Dice
                    if (AttackerLayer == 6) { 
                        DamageUsed = true;
                    }
                    collisionCharacter?.TakeDamage(this);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //确保一次投掷仅能造成一次伤害
        if (!DamageUsed)
        {
            Character collisionCharacter = collision.gameObject.GetComponent<Character>();
            //确保有可伤害的Character脚本
            if (collisionCharacter != null)
            {
                //确保攻击者以及其队友不会被它们自己的射击物所伤害
                if (collision.gameObject.layer != AttackerLayer)
                {
                    if (AttackerLayer == 6)
                    {
                        DamageUsed = true;
                    }
                    collisionCharacter?.TakeDamage(this);
                }
            }
        }
    }

}
