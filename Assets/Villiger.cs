using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villiger : MonoBehaviour
{
    protected const float animSpeed = 0.1f;

    [SerializeField]
    protected float walkDistance, maxWalkDistance;
    [SerializeField]
    protected float walkSpeed, maxWalkSpeed;
    [SerializeField]
    protected float idleTime, maxIdleTime;
    protected float idleCount, idleTarget;

    protected bool walking;
    protected float targetX, speed;
    protected new Rigidbody2D rigidbody2D;

    [SerializeField]
    protected Sprite idle;
    [SerializeField]
    protected Sprite[] walks;
    protected int index;
    protected float animCounter;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        idleTarget = Random.Range(idleTime, maxIdleTime);

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update() {
        if (walking) {
            float distance = Mathf.Abs(targetX - transform.position.x);
            if (distance < 0.1f) {
                walking = false;
                spriteRenderer.sprite = idle;
            }
            else {
                Vector3 pos = transform.position;
                pos.x = Mathf.MoveTowards(pos.x, targetX, speed * Time.deltaTime);
                transform.position = pos;
            }

            animCounter += Time.deltaTime;
            if (animCounter > animSpeed) {
                animCounter = 0;
                spriteRenderer.sprite = walks[index];
                if (++index >= walks.Length)
                    index = 0;
            }
            return;
        }

        if (idleCount >= idleTarget) {
            idleCount = 0;
            walking = true;
            spriteRenderer.sprite = walks[0];
            animCounter = 0;

            speed = Mathf.Lerp(walkSpeed, maxWalkSpeed, Random.Range(0f, 1f));
            targetX = transform.position.x + (Mathf.Lerp(walkDistance, maxWalkDistance, Random.Range(0f, 1f)) * (Random.Range(0, 2) == 0? 1: -1));

            Vector2 castPoint = (Vector2)transform.position + Vector2.up;
            RaycastHit2D[] hits = Physics2D.LinecastAll(castPoint, new Vector2(targetX, castPoint.y));
            
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.CompareTag("Ground"))
                    targetX = hits[i].point.x;
            }

            transform.localScale = new Vector3(targetX > transform.position.x? -1: 1, 1, 1);
        }
        else
            idleCount += Time.deltaTime;
    }
}
