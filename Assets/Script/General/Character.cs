using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    private Collider2D coll;

    [Header("基础属性")]
    public float maxHealth;
    public float currentHealth;


    [Header("无敌时间")]
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
        //无敌时间内不受伤
        if (invulnerable) return;

        //玩家正常执行
        if (this.gameObject.CompareTag("Player"))
        {
            Debug.Log("Character Suffer"+ attacker.transform.name + " OnTakeDamage, Damage =" + attacker.damage);
            CalculateHealth(attacker.damage);
            //Debug.Log("Character OnTakeDamage, currentHealth =" + currentHealth);
        }//除此之外的Character只有Enemy，交由Enemy处理
         //Enemy会将Enemy中的伤害事件绑定在OnTakeDamage上
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
                //避免多次生效（或者使用BOOL）
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
