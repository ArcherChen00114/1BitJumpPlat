using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    private Collider2D coll;

    [Header("��������")]
    public float maxHealth;
    public float currentHealth;


    [Header("�޵�ʱ��")]
    public float invulnerableDuration;
    public float invulnerableTimer;
    public bool invulnerable;


    //public UnityEvent<Character> OnHealthChange;
    public UnityEvent<DamageAbleObject> OnTakeDamage;
    public UnityEvent OnDeath;
    public bool calledDeath;

    private void Awake()
    {
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        coll = GetComponent<Collider2D>();
        Debug.Log(coll.name);
        EventHandler.LoadEvent += OnLoadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.LoadEvent -= OnLoadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        //EventHandler.HurtEvent -= OnTakeDamage;
        
    }


    public void TakeDamage(DamageAbleObject attacker)
    {
        //�޵�ʱ���ڲ�����
        if (invulnerable) return;

        //�������ִ��
        if (this.gameObject.CompareTag("Player"))
        {
            Debug.Log("Character Suffer"+ attacker.transform.name + " OnTakeDamage, Damage =" + attacker.damage);
            CalculateHealth(attacker.damage);
            //Debug.Log("Character OnTakeDamage, currentHealth =" + currentHealth);
        }//����֮���Characterֻ��Enemy������Enemy����
         //Enemy�ὫEnemy�е��˺��¼�����OnTakeDamage��
        else { 
            OnTakeDamage?.Invoke(attacker);
        }
    }

    public void CalculateHealth(float damage) {
        if (damage >= currentHealth)
        {
            if (!calledDeath)
            {
                currentHealth = 0;
                OnDeath?.Invoke();
                Death();
            }
        }
        else
        {
            currentHealth -= damage;
            TriggerInvulnerable();
        }
    }

    public virtual void Death() 
    {
        //EventHandler.CallDeathEvent();

        coll.enabled = false;
        calledDeath = true;
        //this.gameObject.SetActive(false);
    }

    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableTimer = invulnerableDuration;
        }
    }

    private void OnAfterSceneLoadedEvent()
    {
        coll.enabled = true;
        calledDeath = false;
        currentHealth = maxHealth;
    }
    public void OnLoadEvent() {
    }
    // Update is called once per frame
    void Update()
    {
        if (invulnerableTimer > -1)
        {
            invulnerableTimer -= Time.deltaTime;
            if (invulnerableTimer <= 0)
            {
                invulnerable = false;
            }
        }
        if (currentHealth <= 0) {
            if (this.gameObject.CompareTag("Player"))
            {
                //��������Ч������ʹ��BOOL��
                if (!calledDeath) { 
                    EventHandler.CallPlayerDeathEvent();
                    Death();
                }
            }
            else { 
                if (!calledDeath) { 
                    Death();
                    OnDeath?.Invoke();
                }
            }
        }
    }
}
