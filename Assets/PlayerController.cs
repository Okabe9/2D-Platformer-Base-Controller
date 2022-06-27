using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    public int groundMoveForce = 20; 
	  public float smoothing = .05f; 
    private Vector3 smoothVelVect = Vector3.zero;

    
    public int airMoveForce = 10; 
    public float jumpHeight = 2; 
    public float coyoteTime = .2f; 
    private float timeInAir = 0f; 
    private bool jumpFromGround = false;

    public LayerMask groundLayer;
    public float feetOffset = .25f; 

    public float baseGravity = 10; 
    public float fallGravity = 20; 

    // Start is called before the first frame update
    void Start()
    {
      rb = gameObject.GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {      
      movement();       
      globalVariableUpdate(); 

      if (Input.GetKeyDown("w")){
        // Jump from ground
        if(isGrounded())
        {
          jump(); 
        }

        // Coyote
        else if((timeInAir < coyoteTime) && jumpFromGround==false)
        {
          jump(); 
        }
      }
    }

    void movement()
    {
      int applyForce = groundMoveForce; 
      if (!isGrounded()){
        applyForce = airMoveForce; 
      }
      Vector3 targetVel = new Vector2(Input.GetAxisRaw("Horizontal") * applyForce, rb.velocity.y);
      rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVel, ref smoothVelVect, smoothing);
    }

    // void jump()
    // {
    //   jumpFromGround = true; 
    //   rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
    // }

    void jump()
    {
      jumpFromGround = true; 
      float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * baseGravity));
      rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }
    
    void globalVariableUpdate()
    {

      
      if (isGrounded())
      {
        // Without timeInAir check it automatically turns it to false
        if(timeInAir > 0f){
          jumpFromGround = false; 
        }
        timeInAir = 0f;
       
      }
      else{
        timeInAir += Time.deltaTime; 
      }

      if (rb.velocity.y >= 0) 
      {
        rb.gravityScale = baseGravity;
      }

      else{
        rb.gravityScale = fallGravity; 
      }


    }
    bool isGrounded()
    {
      Vector2 rcPosLeft = new Vector2(transform.position.x - feetOffset, transform.position.y); 
      bool leftGrounded = Physics2D.Raycast(rcPosLeft, Vector2.down, .5f, groundLayer);
      Debug.DrawRay(rcPosLeft, Vector2.down, Color.white);

      Vector2 rcPosRight = new Vector2(transform.position.x + feetOffset, transform.position.y); 
      bool rightGrounded = Physics2D.Raycast(rcPosRight, Vector2.down, .5f, groundLayer);
      Debug.DrawRay(rcPosRight, Vector2.down, Color.white);

      bool centerGrounded = Physics2D.Raycast(transform.position, Vector2.down, .5f, groundLayer);
      Debug.DrawRay(transform.position, Vector2.down, Color.white);

      return leftGrounded || rightGrounded || centerGrounded; 
    }
}
