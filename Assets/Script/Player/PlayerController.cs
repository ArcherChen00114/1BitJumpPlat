using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    private Character character;
    public PlayerInputControl inputControl;
    public Vector2 inputDir;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;

    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private GameObject rootObject;

    [Header("基本参数")]
    public float speed;

    public float runSpeed;
    public float walkSpeed => speed /2.5f;
    public float dashSpeed;
    public float jumpForce;
    public float wallJumpForce;
    public float hurtForce;
    public float slideDistance;
    public float slideSpeed;
    public float slidePowerCost;
    public int AirJump = 1;
    public int JumpStep = 0;
    //public ParticleSystem jumpParticle;
    //public ParticleSystem dashParticle;


    [Header("物理材质2D")]

    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;


    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("状态标记")]
    //因为根据按键来确定是否下蹲，因此写在Controller中
    public bool isCrouch;
    public bool isAttack;
    public bool isHurt;
    public bool isDead;
    public bool wallJump;
    public bool isSlide;
    public bool isDashing;
    private bool hasDashed;

    private void Awake()
    {


        runSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
        physicsCheck= GetComponent<PhysicsCheck>();
        coll= GetComponent<CapsuleCollider2D>();
        playerAnimation=GetComponent<PlayerAnimation>();
        character=GetComponent<Character>();

        originalOffset = coll.offset;
        originalSize = coll.size;

        inputControl = new PlayerInputControl();


        inputControl.Gameplay.Dash.started += Dash;
        inputControl.Gameplay.Jump.started += Jump;
        inputControl.Gameplay.Attack.started += PlayerAttack;
        //inputControl.Gameplay.Slide.started += Slide;
        rootObject=this.gameObject;

        #region ForceWalk
        inputControl.Gameplay.Walk.performed += ctx => {
            if (physicsCheck.isGround) {
                speed = walkSpeed;
            }
        };
        inputControl.Gameplay.Walk.canceled += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = runSpeed;
            }

        };
        #endregion

        inputControl.Enable();

    }

    private void OnEnable()
    {
        /*
        sceneLoadEvent.LoadRequestEvent += OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;*/
    }

    private void OnDisable()
    {
        inputControl.Disable();
        /*
        sceneLoadEvent.LoadRequestEvent -= OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;*/
    }



    private void Update()
    {
        inputDir = inputControl.Gameplay.Move.ReadValue<Vector2>();

        CheckState();

    }

    private void FixedUpdate()
    {
        if(!isHurt && !isAttack)
        Move();
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
    }

    //加载途中卸载控制器并在加载完之后启动
    /*private void OnLoadEvent(GameSceneSo arg0, Vector3 arg1, bool arg2)
    {
        inputControl.Gameplay.Disable();
    }*/
    private void OnLoadDataEvent()
    {
        isDead = false;
    }



    private void OnAfterSceneLoadedEvent()
    {
        inputControl.Gameplay.Enable();
    }

    public void Move() 
    {
        if (!isCrouch && !wallJump)
            rb.velocity = new Vector2(inputDir.x * speed * Time.deltaTime, rb.velocity.y);
        
        //翻转
        int faceDir = (int)transform.localScale.x;
        if (inputDir.x > 0)
            faceDir = 1;
        if (inputDir.x < 0)
            faceDir = -1;
        transform.localScale = new Vector3(faceDir, 1, 1);

        //下蹲
        isCrouch = inputDir.y < -0.5f && physicsCheck.isGround;
        if (isCrouch) {
            //修改碰撞体
            //碰撞体的默认高度为1，站立Size为1.9.
            //为了保证碰撞体在Size为1.9时，人物站立在地面上，OffsetY为0.95，即Size的一半
            //同理在修改SizeY的同时应当保障OffsetY为Size的一半，以确保人物碰撞体处于地面上。
            coll.size= new Vector2(coll.size.x, 1.7f);
            coll.offset = new Vector2(coll.offset.x, coll.size.y / 2);
        }
        else
        {//还原
            coll.size = originalSize;
            coll.offset = originalOffset;

        }

    }

    #region UnityEvent
    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.PlayAttack();
        isAttack = true;
    }
    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround || JumpStep < AirJump)
        {
            //jumpParticle.Play();
            rb.velocity = Vector3.zero;
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            JumpStep++;
            GetComponent<AudioDefination>()?.PlayAudioClip();

            //
            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDir.x, 1.5f) * wallJumpForce, ForceMode2D.Impulse);
            GetComponent<AudioDefination>()?.PlayAudioClip();
            wallJump = true;
        }
    }


    private void Dash(InputAction.CallbackContext obj)
    {
        if (hasDashed == false)
        {
            //cameraShakeEvent.OnEventRaised();
            //TODO完成屏幕震荡
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

            hasDashed = true;

            playerAnimation.PlayDash();

            rb.velocity = Vector2.zero;
            Vector2 dir = inputDir;
            rb.velocity += dir.normalized * dashSpeed;

            Debug.Log(rb.velocity+ " ,hasDashed= "+ hasDashed);
            StartCoroutine(DashWait());
        }

    }

    IEnumerator DashWait()
    {
        

        FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        //dashParticle.Play();
        rb.gravityScale = 0;

        wallJump = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        //dashParticle.Stop();
        rb.gravityScale = 4;
        wallJump = false;
        isDashing = false;
    }

    public void GroundTouch()
    {
        isDashing = false;
        hasDashed = false;
        JumpStep = 0;

        //jumpParticle.Play();
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }
    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (physicsCheck.isGround)
            hasDashed = false;
    }

    /*private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >=slidePowerCost)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x,
                transform.position.y);
            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }
    }*/

    private IEnumerator TriggerSlide(Vector3 target) {
        do
        {
            yield return null;

            if (!physicsCheck.isGround)
            {
                break;
            }
            if (physicsCheck.touchLeftWall && transform.localScale.x <0f 
                || physicsCheck.touchRightWall && transform.localScale.x > 0f) { 
                isSlide=false; break;
            }
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x *slideSpeed,
                transform.position.y));
        } while (Mathf.Abs(target.x - transform.position.x)>0.1f);

        isSlide =false;
    }
    #endregion

    public void GetHurt(Transform Attacker)
    {
        isHurt=true;
        rb.velocity = Vector2.zero;
        Vector2 dir = (transform.position - Attacker.position);
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }
    public void PlayerDead() { 
        isDead=true;
        inputControl.Gameplay.Disable();
    }

    private void CheckState()
    {
        if (isDead || isSlide)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");

        
        coll.sharedMaterial = physicsCheck.isGround? normal: wall;

        if (physicsCheck.onWall) 
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);


        if (wallJump&&rb.velocity.y <0) { 
            wallJump = false;
        }
        if (hasDashed && physicsCheck.isGround) {
            hasDashed = false;
        }
    }

}
