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
        //ȷ��һ��Ͷ���������һ���˺�
        if (!DamageUsed) {
            Character collisionCharacter=collision.gameObject.GetComponent<Character>();
            //ȷ���п��˺���Character�ű�
            if (collisionCharacter != null) { 
                //ȷ���������Լ�����Ѳ��ᱻ�����Լ�����������˺�
                if(collision.gameObject.layer != AttackerLayer)
                {
                    //Ҳ����Dice
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
        //ȷ��һ��Ͷ���������һ���˺�
        if (!DamageUsed)
        {
            Character collisionCharacter = collision.gameObject.GetComponent<Character>();
            //ȷ���п��˺���Character�ű�
            if (collisionCharacter != null)
            {
                //ȷ���������Լ�����Ѳ��ᱻ�����Լ�����������˺�
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
