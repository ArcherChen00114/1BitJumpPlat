using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D capsuleCollider;
    private PlayerController playerController;
    private Rigidbody2D rb;

    [Header("检测参数")]
    public bool manual;
    public bool isPlayer;

    public Vector2 bottomLeftOffset;
    public Vector2 bottomRightOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRaduis;
    public LayerMask groundLayer;

    [Header("状态")]
    public bool isGround;
    public bool isEdge;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;

    private void Awake()
    {
        capsuleCollider=GetComponent<CapsuleCollider2D>();

            AutoSetOffset();
        if(isPlayer)
        {
            playerController=GetComponent<PlayerController>();
            rb= GetComponent<Rigidbody2D>();
        }

    }
    public void AutoSetOffset() {

        if (!manual) {
            if (rightOffset!=null && leftOffset !=null)
            {
                rightOffset = new Vector2(
                        (capsuleCollider.bounds.size.x / 2 - capsuleCollider.offset.x - 0.05f),
                        (capsuleCollider.bounds.size.y) / 2);
                leftOffset = new Vector2(-capsuleCollider.bounds.size.x / 2 + capsuleCollider.offset.x + 0.05f
                        , rightOffset.y);
            }
            //transform.localScale.x朝左侧看为正数，右侧为负数
            //当前观测方向的bottomOffset向观测方延申一段距离
            if (bottomLeftOffset != null && bottomRightOffset != null) { 
            bottomLeftOffset = new Vector2(-capsuleCollider.bounds.size.x / 3 
                - transform.localScale.x * capsuleCollider.bounds.size.x / 4
                + capsuleCollider.offset.x + 0.05f
                    , 0);
            bottomRightOffset = new Vector2(capsuleCollider.bounds.size.x / 3
                - transform.localScale.x * capsuleCollider.bounds.size.x / 3
                + capsuleCollider.offset.x + 0.05f
                    , 0);
            }
        }
    }
    private void Update()
    {
        Check();
    }
    public void CheckFaceDirEdge(float faceDir) {

        isEdge = faceDir<0 ? 
            Physics2D.OverlapCircle((Vector2)transform.position + bottomLeftOffset, checkRaduis, groundLayer)
            : Physics2D.OverlapCircle((Vector2)transform.position + bottomRightOffset, checkRaduis, groundLayer);
    }
    public void Check() {
        //任意地面检测点在地面上则仍旧处于地面上
        //faceDir方向地面监测点不在地面上则判断为处于边缘状态，利用边缘状态控制Enemy转头
        //转头后需要确认faceDir一侧的判定点下方是否为空
        if (onWall)
        {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomLeftOffset + new Vector2(0, checkRaduis)
                , checkRaduis, groundLayer)
                && Physics2D.OverlapCircle((Vector2)transform.position + bottomRightOffset + new Vector2(0, checkRaduis)
                , checkRaduis, groundLayer);
        }
        else {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomLeftOffset, checkRaduis, groundLayer)
                 && Physics2D.OverlapCircle((Vector2)transform.position + bottomRightOffset, checkRaduis, groundLayer);
        }

       touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
       touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);

        if (isPlayer)
        {
            onWall = (touchLeftWall && playerController.inputDir.x < 0f 
                || touchRightWall && playerController.inputDir.x > 0f) && rb.velocity.y<0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomLeftOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomRightOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
    }

}
