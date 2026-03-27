using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Moon : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    public GameObject Player;
    public GameObject WavePrefab;

    bool isMoonLeft = false;

    int hp = 20;
    int groggyGage = 0;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("Do");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "AttackCol")
        {
            hp -= 1;
            anim.SetTrigger("OnHit");
            if (hp == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void grogyCountUp()
    {
        groggyGage += 1;
        Debug.Log(groggyGage);
    }

    IEnumerator Do()
    {
        yield return new WaitForSeconds(3.0f);
        while (true)
        {
            StartCoroutine("fall");
            yield return new WaitForSeconds(7.0f);

            if (groggyGage == 5)
            {
                groggyGage = 0;
                transform.DOMove(new Vector3(0, 2, -1), 1.0f);
                yield return new WaitForSeconds(4.0f);
                transform.DOMove(new Vector3(5.5f, 6, -1), 1.0f);
                yield return new WaitForSeconds(7.0f);
                isMoonLeft = false;
                continue;
            }

            StartCoroutine("fall");
            yield return new WaitForSeconds(7.0f);
            if (groggyGage == 5)
            {
                groggyGage = 0;
                transform.DOMove(new Vector3(0, 2, -1), 1.0f);
                yield return new WaitForSeconds(4.0f);
                transform.DOMove(new Vector3(5.5f, 6, -1), 1.0f);
                yield return new WaitForSeconds(7.0f);
                isMoonLeft = false;
                continue;
            }

            StartCoroutine("ToCharactor");
            yield return new WaitForSeconds(4.0f);

            if (groggyGage == 5)
            {
                groggyGage = 0;
                transform.DOMove(new Vector3(0, 2, -1), 1.0f);
                yield return new WaitForSeconds(4.0f);
                transform.DOMove(new Vector3(5.5f, 6, -1), 1.0f);
                yield return new WaitForSeconds(7.0f);
                isMoonLeft = false;
                continue;
            }

            StartCoroutine("ToCharactor");
            yield return new WaitForSeconds(7.0f);
            if (groggyGage == 5)
            {
                groggyGage = 0;
                transform.DOMove(new Vector3(0, 2, -1), 1.0f);
                yield return new WaitForSeconds(4.0f);
                transform.DOMove(new Vector3(5.5f, 6, -1), 1.0f);
                yield return new WaitForSeconds(7.0f);
                isMoonLeft = false;
                continue;
            }
        }
    }

    IEnumerator fall()
    {
        var tween = transform.DOMoveY(0.58f, 0.2f).SetEase(Ease.Linear);
        yield return tween.WaitForCompletion();
        anim.SetBool("Melting", true);

        yield return tween.WaitForCompletion();

        GameObject wave = Instantiate(WavePrefab);
        transform.position = new Vector3(isMoonLeft ? 10f : -10f, 7f, -1);
        wave.transform.position = new Vector3(isMoonLeft ? -5 : 5, 0.88f, 0);

        yield return new WaitForSeconds(4f);
        anim.SetBool("Melting", false);
        tween = transform.DOMoveX(isMoonLeft ? 5.5f : -5.5f, 0.5f).SetEase(Ease.Linear);
        transform.DOMoveY(6f, 0.5f).SetEase(Ease.InSine);
        isMoonLeft = !isMoonLeft;
    }

    IEnumerator ToCharactor()
    {
        Vector3 center = transform.position;
        center.x += isMoonLeft? 1f : -1f;
        float radiusX = 1f;
        float radiusY = 0.5f;
        float angle = isMoonLeft ? 180f : 0f;
        float targetAngle = isMoonLeft ? 900f : -720f;
        Tween tween;

        tween = DOTween.To(() => angle, x => angle = x, targetAngle, 1f)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float r = angle * Mathf.Deg2Rad;
                transform.position = center + new Vector3(Mathf.Cos(r) * radiusX, Mathf.Sin(r) * radiusY, 0);
            });
        yield return tween.WaitForCompletion();

        yield return new WaitForSeconds(1.0f);
        float posX = transform.position.x;
        Vector2 vector2 = new Vector2(posX + (Player.transform.position.x - posX) / 6 * 8.5f, -2.5f);
        tween = transform.DOMoveX(vector2.x, 0.5f).SetEase(Ease.Linear);
        transform.DOMoveY(vector2.y, 0.5f).SetEase(Ease.InSine);
        yield return tween.WaitForCompletion();

        yield return new WaitForSeconds(0.5f);
        transform.position = new Vector3(isMoonLeft ? 10f : -10f, 7f, -1);
        tween = transform.DOMoveX(isMoonLeft ? 5.5f : -5.5f, 0.5f).SetEase(Ease.Linear);
        transform.DOMoveY( 6f, 0.5f).SetEase(Ease.InSine);
        isMoonLeft = !isMoonLeft;
    }
}
