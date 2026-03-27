using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Ememy : MonoBehaviour
{
    GameObject Player;
    bool isTracking = false;
    int hp = 3;
    Animator anim;

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTracking == false && Vector2.Distance(Player.transform.position, transform.position) < 7)
        {
            isTracking = true;
            StartCoroutine("Tracking");
        }
    }

    IEnumerator Tracking()
    {
        while (true)
        {
            Vector2 target = Player.transform.position;
            target.y += 0.935f;
            transform.position = Vector2.MoveTowards(transform.position, target, 1.7f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "AttackCol")
        {
            hp--;
            anim.SetTrigger("Hurt");
            if (hp == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
