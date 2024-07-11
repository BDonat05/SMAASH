using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    PhotonView view;

    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject attackPoint;
    public GameObject attackPointOpposite;
    //public Vector2 attackPointPosition;
    //public Vector2 attackPointPositionOpposite;

    public Animator animator;
    public Joystick joystick;
    public SpriteRenderer spriteRenderer;

    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 10f;
    private int extraJumps;
    private int extraJumpValue = 2;

    void Start()
    {
        extraJumps = extraJumpValue;
        joystick = GameObject.Find("Floating Joystick").GetComponent<Joystick>();
        view = GetComponent<PhotonView>();
    }

    void FixedUpdate()
    {
        if(view.IsMine){
            horizontalMove();
            flipCharacter();
            checkJumpAnimation();
            //updateAttackPoints();
        }
    }

/*
    void updateAttackPoints(){
        //attackPointPosition = new Vector2(attackPoint.transform.position.x, attackPoint.transform.position.y);
        //attackPointPositionOpposite = new Vector2(attackPoint.transform.position.x * -1, attackPoint.transform.position.y);
    }
*/
    void horizontalMove(){
        
        if(joystick.Horizontal >= .2f)
        {
            horizontal = speed;
        }
        else if(joystick.Horizontal <= -.2f)
        {
            horizontal = -speed;
        }
        else
        {
            horizontal = 0f;
        }

        rb.velocity = new Vector2(horizontal, rb.velocity.y);
        animator.SetFloat("speed", Math.Abs(horizontal));
    }

    void flipCharacter(){

        if (horizontal > 0f)
        {
            spriteRenderer.flipX = false;
            //attackPoint.transform.position = new Vector2(this.transform.position.x * 1, this.transform.position.y);
            view.RPC("OnDirectionChange_RIGHT", RpcTarget.Others);
        }
        else if (horizontal < 0f)
        {
            spriteRenderer.flipX = true;
            //attackPoint.transform.position = new Vector2(attackPointPositionOpposite.x, this.transform.position.y);
            view.RPC("OnDirectionChange_LEFT", RpcTarget.Others);
        }
    }

    void checkJumpAnimation(){

        if (!IsGrounded())
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }
    [PunRPC]
    void OnDirectionChange_LEFT()
    {
        spriteRenderer.flipX = true;
        //attackPoint.transform.position = new Vector2(this.transform.position.x - 1f, this.transform.position.y);
    }
    [PunRPC]
    void OnDirectionChange_RIGHT()
    {
        spriteRenderer.flipX = false;
        //attackPoint.transform.position = new Vector2(this.transform.position.x + 1f, this.transform.position.y);
    }  

    public void Jump(InputAction.CallbackContext context)
    {
        if(view.IsMine)
        {
            if(IsGrounded())
            {
                extraJumps = extraJumpValue;
            }

            if(context.performed && extraJumps > 0)
            {
                rb.velocity = Vector2.up * jumpingPower;
                extraJumps--;
            }
            else if(context.performed && extraJumps == 0 && IsGrounded())
            {
                rb.velocity = Vector2.up * jumpingPower;
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer); 
    }
}