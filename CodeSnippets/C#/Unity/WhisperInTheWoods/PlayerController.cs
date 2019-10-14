using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventSystem;

public class PlayerController : MonoBehaviour {
    const float GRAVITY = 9.8f;

    public Transform CheckPoint;

    private Rigidbody2D myRB2d;
    private bool facingRight = true;
    [SerializeField]
    private SpriteRenderer sRenderer;
    [SerializeField]
    private Animator myAnim;
    private bool canMove = true;
    private bool dead = false;

    [Header("Player Physics")]
    public float walkSpeed;
    public float jumpPower;
    public float wallHopPower;
    public float jumpRefreshRate;
    //public float wallJumpSpeed;

    //Jump
    public static bool grounded = true;
    public Transform[] GroundRays = new Transform[3]; // Check ground with 3 rays;
    public Transform[] WallHopRays = new Transform[3]; // Check wall with 3 rays;
    public float RayLengthY = 0.52f; //Length of the rays, depends on assets.
    public float RayLengthX = 0.52f; //Length of the rays, depends on assets.
    //private float checkRadius = 0.2f; //depends on assets
    //private float castLengthDivider = 2f;
    private RaycastHit2D[] hits = new RaycastHit2D[1]; //Max returned intersections is 1;
    private float totalJumpRefreshTime;
    //private bool jumped = false;
    public float downAccelConstant = 20f; //Helps keep the character from feeling floaty.
    [Header("No Touchy")]
    public LayerMask groundLayer;
    public LayerMask wallJumpLayer;
    public Transform groundCheck;
    public float baseSpeed;

    //Wall Jump
    //private RaycastHit2D hit;
    //private bool wallJumping = false;
    public string wallTag;
    public float wallDetectionDistance = 1f;

    // Use this for initialization
    void Start()
    {
        myRB2d = gameObject.GetComponent<Rigidbody2D>();
        //sRenderer = gameObject.GetComponent<SpriteRenderer>();
        //myAnim = gameObject.GetComponent<Animator>();

       // gameObject.GetComponent<CapsuleCollider2D>().offset.y;
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventLibrary.PlayerDead, OnDeath);
        EventManager.StartListening(EventLibrary.ConversationStarted, OnConvoStart);
        EventManager.StartListening(EventLibrary.COnversationEnded, OnConvoEnd);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventLibrary.PlayerDead, OnDeath);
        EventManager.StopListening(EventLibrary.ConversationStarted, OnConvoStart);
        EventManager.StopListening(EventLibrary.COnversationEnded, OnConvoEnd);
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            return;
        }
        Physics2D.queriesStartInColliders = false;
        //hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, wallDetectionDistance);

        CharacterJump();
        CharacterMovement(walkSpeed);

        //Sorry but I just need to test this mechanic
        if(myRB2d.velocity.y <= 0 && !grounded)
        {
            if (RayTotal(WallHopRays, facingRight ? Vector3.right : Vector3.left, RayLengthX, wallJumpLayer) > 0 && Input.GetButtonDown("Jump"))
            {
                myRB2d.velocity = new Vector2(myRB2d.velocity.x, 0);
                myRB2d.AddForce(Vector2.up * wallHopPower, ForceMode2D.Impulse);
            }
        }
        //I'll clean it up later I promise

        //JumpRefreshRate();
        //WallJump();
    }

    private void FixedUpdate()
    {
        if (dead)
        {
            return;
        }
        //Apply Gravity and speed up fall if no longer rising in the y
        float downV = myRB2d.velocity.y - (GRAVITY + (myRB2d.velocity.y > 0 ? 0 : downAccelConstant)) * Time.deltaTime;
        myRB2d.velocity = new Vector2(myRB2d.velocity.x, downV);
    }

    void CharacterMovement(float speed)
    {
        float move = Input.GetAxis("Horizontal");

        if (canMove)
        {
            if (move > 0 && !facingRight)
            {
                Flip();
            }
            else if (move < 0 && facingRight)
            {
                Flip();
            }

            myRB2d.velocity = new Vector2(move * speed, myRB2d.velocity.y);
            myAnim.SetFloat("WalkSpeed", Mathf.Abs(move));
        }
        else
        {
            myRB2d.velocity = new Vector2(0, myRB2d.velocity.y);
            myAnim.SetFloat("WalkSpeed", 0);
        }
    }

    void CharacterJump()
    {
        if (canMove && grounded && Input.GetButtonDown("Jump")/*(Input.GetAxis("PS4Jump") > 0 || Input.GetAxis("XBOXJump") > 0)*/ /*&& !jumped*/)
        {
            myAnim.SetBool("IsGrounded", false);
            myRB2d.velocity = new Vector2(myRB2d.velocity.x, 0f); //Apply appropriate force each time by setting to 0
            myRB2d.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);
            grounded = false;
            //jumped = true;
        }
 
        grounded = RayTotal(GroundRays, Vector3.down, RayLengthY, groundLayer) > 0; //Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        myAnim.SetBool("IsGrounded", grounded);
    }
    // Returns the total number of Linecasts in rays pointing in direction for raylength that are hitting a given layer.
    public int RayTotal (Transform[] rays, Vector3 direction, float raylength, int layer)
    {
        int rayTotal = 0;
        for (int i = 0; i < rays.Length; ++i)
        {
            rayTotal += Physics2D.LinecastNonAlloc(rays[i].position, rays[i].position + direction * raylength, hits, layer);
        }

        return rayTotal;
    }

    //void WallJump()
    //{
    //    if (!grounded && hit.collider != null)
    //    {
    //        GetComponent<Rigidbody2D>().velocity = new Vector2(wallJumpSpeed * hit.normal.x, wallJumpSpeed);

    //        transform.localScale = transform.localScale.x == 1 ? new Vector2(-1, 1) : Vector2.one;
    //        wallJumping = true;
    //    }
    //    else if (hit.collider != null && wallJumping)
    //    {

    //    }
    //}

   /* void JumpRefreshRate()
    {
        if(jumped == true && grounded)
        {
            totalJumpRefreshTime += Time.deltaTime;

            if(totalJumpRefreshTime >= jumpRefreshRate)
            {
                totalJumpRefreshTime = 0;
                jumped = false;
            }
        }
    }*/

    void Flip()
    {
        facingRight = !facingRight;
        sRenderer.flipX = !sRenderer.flipX;
    }

    public void ToggleCanMove()
    {
        canMove = !canMove;
    }

    public void FailSafeMove()
    {
        canMove = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * transform.localScale.x * wallDetectionDistance);
    }

    #region EventLogic
    void OnDeath()
    {
        myRB2d.velocity = new Vector2();
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        sRenderer.sortingOrder = 12;
        GameObject.Find("Canvas").GetComponent<Canvas>().sortingOrder = 10;
        dead = true;

        EventManager.StartListening(EventLibrary.LvFadeInEnd, OnFadeInEnd);
        EventManager.TriggerEvent(EventLibrary.LvFadeInStart);
    }

    void OnFadeInEnd()
    {
        EventManager.StopListening(EventLibrary.LvFadeInEnd, OnFadeInEnd);
        EventManager.StartListening(EventLibrary.LvFadeOutEnd, OnFadeOutEnd);

        StartCoroutine(GoToCheckPoint());
    }

    void OnFadeOutEnd()
    {
        sRenderer.sortingOrder = 5;
        GameObject.Find("Canvas").GetComponent<Canvas>().sortingOrder = 20;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        dead = false;

        EventManager.StopListening(EventLibrary.LvFadeOutEnd, OnFadeOutEnd);
    }

    void OnConvoStart()
    {
        canMove = false;
    }

    void OnConvoEnd()
    {
        canMove = true;
    }
    #endregion

    IEnumerator GoToCheckPoint()
    {
        while (Vector2.Distance(transform.position, CheckPoint.position) > 0.2f)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector2.MoveTowards(transform.position, CheckPoint.position, 8 * Time.deltaTime);
        }

        EventManager.TriggerEvent(EventLibrary.LvFadeOutStart);
    }
}
