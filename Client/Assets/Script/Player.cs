using System.Collections;
using System.Drawing;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject shield;
    Shield ShieldScript;

    public RuntimeAnimatorController[] animatorController = new RuntimeAnimatorController[2];

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;
    SpriteRenderer spriteShield;

    public CinemachineCamera BossCam;

    public GameObject moon;

    public CapsuleCollider2D AttackCol;
    Vector2[] offset = { new Vector2(0.48f, 1.18f), new Vector2(0.1f, 1.05f), new Vector2(0.48f, 1f) };
    Vector2[] size = { new Vector2(2.6f, 1.5f), new Vector2(3.3f, 1.17f), new Vector2(2.9f, 1.9f) };

    public Vector3 boxCastOffset = new Vector3(-0.075f, 0.06f, 0.0f);
    public Vector2 boxCastSize = new Vector2(0.98f, 0.03f);
    public float boxCastMaxDistance = 0.07f;

    bool isGrounded;
    bool isLeft = false;
    bool isShieldEquipped = false;
    bool throwDelay = false;
    float speed = 5.0f;

    bool isAttacking = false;
    bool isBlocking = false;
    int attackState = 1;

    bool isRolling = false;

    bool doubleJump = true;
    bool shieldJump = true;

    bool invincibilityFrame = false;
    bool isDead = false;

    int hp = 5;
    private void Awake()
    {
        
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        spriteShield = shield.GetComponent<SpriteRenderer>();
        ShieldScript = shield.GetComponent<Shield>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.runtimeAnimatorController = animatorController[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + boxCastOffset, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground", "Shield"));

            isGrounded = raycastHit.collider?.tag == "Ground";

            if (shieldJump && raycastHit.collider?.tag == "Shield")
            {
                jump();
                shieldJump = false;
                ShieldScript.Stemped();
            }

            anim.SetBool("Grounded", isGrounded);

            anim.SetFloat("AirSpeedY", rigid.linearVelocityY);


            float axis = Input.GetAxisRaw("Horizontal");
            if (!isRolling)
            {
                rigid.linearVelocity = new Vector2(speed * axis, rigid.linearVelocity.y);

                if (axis > 0)
                {
                    sprite.flipX = isLeft = false;
                    anim.SetInteger("AnimState", 1);
                }
                else if (axis < 0)
                {
                    sprite.flipX = isLeft = true;
                    anim.SetInteger("AnimState", 1);
                }
                else
                    anim.SetInteger("AnimState", 0);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded)
                {
                    shieldJump = true;
                    doubleJump = true;
                    jump();
                }
                else if (doubleJump)
                {
                    doubleJump = false;
                    jump();
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift) && !isRolling)
            {
                isRolling = true;
                anim.SetTrigger("Roll");
                StartCoroutine("Roll");
            }
            else if (Input.GetKeyDown(KeyCode.E) && !throwDelay)
            {
                if (isShieldEquipped)
                {
                    StartCoroutine(ShieldThrow());
                    anim.SetTrigger("Throw");
                }
                else
                {
                    ShieldScript.ReturnToPlayer();
                }
            }
            else if (Input.GetMouseButton(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
            {
                StopCoroutine("AttackStateDel");
                StartCoroutine("AttackStateDel");
                anim.SetTrigger("Attack" + attackState);
                attackState++;
                if (attackState == 4)
                    attackState = 1;
            }
            else if (isShieldEquipped && Input.GetMouseButton(1))
            {
                anim.SetTrigger("Block");
                isBlocking = true;
                Invoke("isBlockingEnd", 0.4f);
            }
        }
    }

    void isBlockingEnd()
    {
        isBlocking = false;
    }

    public void AttackColSet(int n)
    {
        AttackCol.enabled = true;
        AttackCol.offset = isLeft ? new Vector2(-offset[n].x, offset[n].y) : offset[n];
        AttackCol.size = size[n];
    }

    public void AttackColOff()
    {
        AttackCol.enabled = false;
    }

    IEnumerator AttackStateDel()
    {
        yield return new WaitForSeconds(0.8f);
        attackState = 1;
    }

    void jump()
    {
        anim.SetTrigger("Jump");
        rigid.linearVelocityY = 0;
        rigid.AddForce(Vector2.up * 290);
    }

    IEnumerator Roll()
    {
        Vector2 force = (isLeft ? Vector2.left : Vector2.right) * 10;
        for (int i = 0; i < 50; i++)
        {
            rigid.AddForce(force);
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.05f);
        isRolling = false;
    }

    IEnumerator ShieldThrow()
    {
        int direction = isLeft ? -1 : 1;
        isShieldEquipped = false;
        throwDelay = true;

        shield.SetActive(true);
        spriteShield.flipX = isLeft;
        shield.transform.localPosition = new Vector3(0.265f * direction, 0.8435f, -1.0f);
        shield.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f * direction);

        for (int i = 1; i <= 10; i++)
        {
            yield return new WaitForSeconds(0.005f);
            shield.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0.0f, 0.0f, -90.0f * direction), Quaternion.Euler(0.0f, 0.0f, 0.0f), i / 10.0f);
            shield.transform.localPosition = Vector3.Lerp(new Vector3(0.265f * direction, 0.845f, -1.0f), new Vector3(0.0f, 0.845f, -1.0f), i / 10.0f);
        }
        yield return new WaitForSeconds(0.01f);
        for (int i = 1; i <= 10; i++)
        {
            yield return new WaitForSeconds(0.01f);
            shield.transform.localPosition = Vector3.Lerp(new Vector3(0.0f, 0.845f, -1.0f), new Vector3(0.565f * direction, 0.92f, -1.0f), i / 10.0f);
        }
        shield.transform.parent = null;
        anim.runtimeAnimatorController = animatorController[1];
        ShieldScript.shieldThrow(isLeft);
        yield return new WaitForSeconds(0.05f);
        throwDelay = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (throwDelay == false && collision.tag == "Shield")
        {
            ShieldScript.Retrieved();
            anim.runtimeAnimatorController = animatorController[0];
            isShieldEquipped = true;
        }
        else if ((collision.tag == "Enemy" || collision.tag == "Moon") && invincibilityFrame == false && !isDead)
        {
            if (isBlocking)
            {
                Debug.Log(collision.name);
                if(collision.tag == "Moon")
                    moon.GetComponent<Moon>().grogyCountUp();

                anim.SetTrigger("BlockSuccess");
                invincibilityFrame = true;
                Invoke("invincibilityFrameEnd", 1f);
            }
            else
            {
                hp -= 1;
                if (hp == 0)
                {
                    anim.SetTrigger("Death");
                    isDead = true;
                }
                anim.SetTrigger("Hurt");
                invincibilityFrame = true;
                Invoke("invincibilityFrameEnd", 1f);
            }
        }
        else if(collision.name == "Boss Entrance")
        {
            hp = 5;
            BossCam.Priority = 1;
            moon.SetActive(true);
        }
    }

    void invincibilityFrameEnd()
    {
        invincibilityFrame = false;
    }

    void OnDrawGizmos()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position + boxCastOffset, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground", "Sheild"));

        Gizmos.color = UnityEngine.Color.red;
        if (raycastHit.collider != null)
        {
            Gizmos.DrawRay(transform.position + boxCastOffset, Vector2.down * raycastHit.distance);
            Gizmos.DrawWireCube(transform.position + boxCastOffset + Vector3.down * raycastHit.distance, boxCastSize);
        }
        else
        {
            Gizmos.DrawRay(transform.position+ boxCastOffset, Vector2.down * boxCastMaxDistance);
        }
    }
}
