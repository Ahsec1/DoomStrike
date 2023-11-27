using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxcollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    

    private void Awake(){
        //references from Objext
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxcollider = GetComponent<BoxCollider2D>();
    }

    private void Update(){

        horizontalInput = Input.GetAxis("Horizontal");
        

        //to flip player left or right
        if(horizontalInput > 0.01f){
            transform.localScale = new Vector3(2, 2, 2);    
        }else if(horizontalInput < -0.01f){
            transform.localScale = new Vector3(-2, 2, 2);
        }

       

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        //wall jump
        if(wallJumpCooldown > 0.2f){
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            
            if(onWall() && !isGrounded()){
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }else{
                body.gravityScale = 5;
            }
            
            if(Input.GetKey(KeyCode.Space)){
                Jump();
            }

        }else{
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump(){
        if(isGrounded()){
            body.velocity = new Vector2(body.velocity.x, jumpPower);
        }else if(onWall() && !isGrounded()){
            if(horizontalInput == 0){
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.x);
            }else{
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            }

            wallJumpCooldown = 0;
        }
        anim.SetTrigger("jump");
    }

    private bool isGrounded(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxcollider.bounds.center, boxcollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxcollider.bounds.center, boxcollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack(){
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
