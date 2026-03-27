using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Shield : MonoBehaviour
{
    Animator anim;

    public GameObject player;

    bool isLeft;

    bool isReturning = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shieldThrow(bool isleft)
    {
        isLeft = isleft;
        isReturning = false;
        StartCoroutine("shieldThrowCor");
    }

    IEnumerator shieldThrowCor()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        Vector3 target = transform.position;
        anim.SetBool("isSpin", true);
        target.x += isLeft ? -3.0f : 3.0f;
        for(int i =  1; i <= 20; i++)
        {
            transform.position = Vector3.Lerp(transform.position, target, i / 20.0f);
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator rebound()
    {
        Vector3 target = transform.position;
        target.x += isLeft ? 0.2f : -0.4f;
        for (int i = 1; i <= 5; i++)
        {
            yield return new WaitForSeconds(0.04f);
            transform.position = Vector3.Lerp(transform.position, target, i / 5.0f);
        }
    }

    public void Retrieved()
    {
        StopCoroutine("ReturnToPlayerCor");
        transform.parent = player.transform;
        gameObject.SetActive(false);
    }


    public void ReturnToPlayer()
    {
        isReturning = true;
        StopCoroutine("shieldThrowCor");
        StartCoroutine("ReturnToPlayerCor");
    }

    public void Stemped()
    {
        StartCoroutine("StempedCor");
    }
    IEnumerator StempedCor()
    {
        Vector3 target = transform.position;
        target.y -= 0.1f;
        for (int i = 1; i <= 5; i++)
        {
            yield return new WaitForSeconds(0.04f);
            transform.position = Vector3.Lerp(transform.position, target, i / 5.0f);
        }
        yield return new WaitForSeconds(0.04f);
        StartCoroutine("ReturnToPlayerCor");
    }

    IEnumerator ReturnToPlayerCor()
    {
        Vector2 direction;
        float speed = 0.2f;
        while (true) {
            yield return new WaitForSeconds(0.02f);
            direction = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y + 0.935f - transform.position.y);
            direction.Normalize();
            transform.position = new Vector3(transform.position.x + direction.x * speed, transform.position.y + direction.y * speed, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground" && !isReturning)
        {
            StopCoroutine("shieldThrowCor");
            StartCoroutine("rebound");
        }
    }
}
