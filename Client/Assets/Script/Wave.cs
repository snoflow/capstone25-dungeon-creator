using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Wave : MonoBehaviour
{
    bool isLeft = false;

    private PolygonCollider2D polyCollider;
    private SpriteRenderer spriteRenderer;
    private Sprite lastSprite;
    private List<Vector2> pathPoints = new List<Vector2>();

    private void Awake()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isLeft = transform.position.x < 0;
        transform.localScale = new Vector3(isLeft ? 0.8f : -0.8f, 0.8f, 1f);
        StartCoroutine("Move");
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer.sprite != lastSprite)
        {
            UpdateColliderShape(spriteRenderer.sprite);
            lastSprite = spriteRenderer.sprite;
        }
    }

    IEnumerator Move()
    {
        Tween tween = transform.DOMoveX(isLeft ? 11.5f : -11.5f, 3.0f);
        yield return tween.WaitForCompletion();
        Destroy(gameObject);
    }

    void UpdateColliderShape(Sprite currentSprite)
    {
        if (currentSprite == null) return;

        int shapeCount = currentSprite.GetPhysicsShapeCount();
        polyCollider.pathCount = shapeCount;

        for (int i = 0; i < shapeCount; i++)
        {
            pathPoints.Clear();
            currentSprite.GetPhysicsShape(i, pathPoints);
            polyCollider.SetPath(i, pathPoints.ToArray());
        }
    }
}
