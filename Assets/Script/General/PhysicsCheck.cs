using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D capsuleCollider;
    private PlayerController playerController;
    private Rigidbody2D rb;

    [Header("������")]
    public bool manual;
    public bool isPlayer;

    public Vector2 bottomLeftOffset;
    public Vector2 bottomRightOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRaduis;
    public LayerMask groundLayer;

    [Header("״̬")]
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
            //transform.localScale.x����࿴Ϊ�������Ҳ�Ϊ����
            //��ǰ�۲ⷽ���bottomOffset��۲ⷽ����һ�ξ���
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
        //�����������ڵ��������Ծɴ��ڵ�����
        //faceDir���������㲻�ڵ��������ж�Ϊ���ڱ�Ե״̬�����ñ�Ե״̬����Enemyתͷ
        //תͷ����Ҫȷ��faceDirһ����ж����·��Ƿ�Ϊ��
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
