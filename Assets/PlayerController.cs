using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    private const float animSpeed = 0.1f;

    [SerializeField]
    private float walkSpeed, walkDamp, zombifyRunSpeed;
    
    [SerializeField]
    private Sprite idleSprite;
    [SerializeField]
    private Sprite[] walkSprites, runSprites;
    private int animLoopIndex = 0;
    private float animLoopTimer = 0;

    [SerializeField]
    private float zombieTime;
    private float zombieTimer;

    private enum AnimState { Idle, Walk, Run }
    private AnimState state = AnimState.Idle;

    [SerializeField]
    private GameObject enterIndicator;
    private List<Collider2D> enteredTrees = new List<Collider2D>();

    [SerializeField]
    private Image zombifyBar;
    [SerializeField]
    private Color fullColor, emptyColor;
    private Villiger targetVilliger;
    private bool zombify;
    [SerializeField]
    private ParticleSystem turnParticle;

    [SerializeField]
    private ParticleSystem eatingParticle;
    [SerializeField]
    private float eatingTime;
    private float eatingTimer;
    private bool eating;

    [System.NonSerialized]
    public bool hiding;
    private bool tryHiding;
    [SerializeField]
    private float hideTime;
    private float hideTimer;
    [SerializeField]
    private ParticleSystem hidingParticle;

    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private AudioClip[] eatSounds;
    [SerializeField]
    private AudioSource audioSource;

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        zombieTimer = zombieTime;
    }

    private void Update() {
        RunAnimation();

        if (zombify) {
            float distance = Mathf.Abs(targetVilliger.transform.position.x - transform.position.x);
            if (distance < 0.1f) {
                zombify = false;
                // TODO: Play eating animation

                eatingParticle.Play();
                eatingTimer = eatingTime;
                eating = true;
                spriteRenderer.enabled = false;
                Manager.ins.RemoveVilliger(targetVilliger);
                targetVilliger = null;
                CameraControl.ins.FocusCamera();

                for (int i = 0; i < eatSounds.Length; i++)
                    audioSource.PlayOneShot(eatSounds[i]);
            }
            else {
                Vector3 pos = transform.position;
                pos.x = Mathf.MoveTowards(pos.x, targetVilliger.transform.position.x, zombifyRunSpeed * Time.deltaTime);
                transform.position = pos;
            }
            return;
        }

        if (eating) {
            eatingTimer -= Time.deltaTime;
            if (eatingTimer <= 0) {
                eating = false;
                spriteRenderer.enabled = true;
                eatingParticle.Stop();
                Manager.ins.ResetWarning();
                CameraControl.ins.UnfocusCamera();
            }
            return;
        }

        zombieTimer -= Time.deltaTime;
        float percentage = zombieTimer / zombieTime;
        zombifyBar.transform.localScale = new Vector3(percentage, 1, 1);
        zombifyBar.color = Color.Lerp(emptyColor, fullColor, percentage);
        if (zombieTimer <= 0) {
            zombieTimer = zombieTime;

            Villiger[] villigers = FindObjectsOfType<Villiger>();
            float distance = -1;

            for (int i = 0; i < villigers.Length; i++)
            {
                if (villigers[i].CompareTag("Villiger")) {
                    float d = Mathf.Abs(villigers[i].transform.position.x - transform.position.x);
                    if (distance < 0 || d < distance) {
                        targetVilliger = villigers[i];
                        distance = d;
                    }
                }
            }

            if (targetVilliger.CompareTag("Villiger")) {
                transform.localScale = new Vector3(targetVilliger.transform.position.x > transform.position.x ? -1 : 1, 1, 1);
                zombify = true;
                Manager.ins.StartChasing();
                state = AnimState.Run;
                spriteRenderer.sprite = runSprites[0];
                animLoopIndex = 1;
                animLoopTimer = 0;
                turnParticle.Play();
            }
            else
                targetVilliger = null;
            return;
        }

        if (tryHiding) {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0) {
                tryHiding = false;
                hiding = true;
                spriteRenderer.sortingOrder = -1;
                spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f, 1);
                hidingParticle.Stop();
                rigidbody2D.velocity = Vector2.zero;
            }

            rigidbody2D.velocity = new Vector2(Mathf.MoveTowards(rigidbody2D.velocity.x, 0, walkDamp), 0);
            return;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            if (hiding) {
                hiding = false;
                spriteRenderer.sortingOrder = 2;
                spriteRenderer.color = Color.white;
            }
            else if (enteredTrees.Count > 0) {
                tryHiding = true;
                hideTimer = hideTime;
                hidingParticle.Play();
            }
        }

        bool inputed = false;
        float x = 0;
        if (Input.GetKey(KeyCode.A)) {
            x -= walkSpeed;
            inputed = true;
        }
        if (Input.GetKey(KeyCode.D)) {
            x += walkSpeed;
            inputed = true;
        }

        if (!inputed)
            x = Mathf.MoveTowards(rigidbody2D.velocity.x, 0, walkDamp);


        if (x == 0) {
            spriteRenderer.sprite = idleSprite;
            state = AnimState.Idle;
        }
        else {
            transform.localScale = new Vector3(x > 0? -1: 1, 1 ,1);

            if (state != AnimState.Walk) {
                state = AnimState.Walk;
                spriteRenderer.sprite = walkSprites[0];
                animLoopIndex = 1;
                animLoopTimer = 0;
            }
        }

        rigidbody2D.velocity = new Vector2(x, 0);
    }

    private void RunAnimation() {
        switch (state) {
            case AnimState.Walk:
                animLoopTimer += Time.deltaTime;
                if (animLoopTimer >= animSpeed) {
                    animLoopTimer = 0;
                    spriteRenderer.sprite = walkSprites[animLoopIndex];
                    if (++animLoopIndex >= walkSprites.Length)
                        animLoopIndex = 0;
                }
                break;
            case AnimState.Run:
                animLoopTimer += Time.deltaTime;
                if (animLoopTimer >= animSpeed) {
                    animLoopTimer = 0;
                    spriteRenderer.sprite = runSprites[animLoopIndex];
                    if (++animLoopIndex >= runSprites.Length)
                        animLoopIndex = 0;
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Tree")) {
            enteredTrees.Add(other);
            enterIndicator.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Tree")) {
            enteredTrees.Remove(other);
            if (enteredTrees.Count == 0) {
                hiding = false;
                spriteRenderer.sortingOrder = 2;
                spriteRenderer.color = Color.white;
                enterIndicator.gameObject.SetActive(false);

                if (tryHiding) {
                    tryHiding = false;
                    hidingParticle.Stop();
                }
            }
        }
    }

    public void GameOver() {
        hidingParticle.Stop();
        eatingParticle.Stop();
        rigidbody2D.velocity = Vector3.zero;
        enabled = false;
    }
}
