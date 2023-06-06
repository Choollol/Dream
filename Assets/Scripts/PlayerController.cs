using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Creature
    {
        Human, Fish, Duck, Leaf, Flame
    }

    public float speed;
    public Creature creature;
    public float movementSoundInterval;

    public bool canMove;
    public bool canMoveHorizontally;
    public bool canMoveVertically;

    private GameManager gameManager;
    private AudioManager audioManager;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private float horizontalInput;
    private float verticalInput;

    private bool doPlayMovementSound;
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        StartCoroutine(MovementSound());

        canMove = true;
    }
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (gameManager.world != GameManager.World.Doors)
        {
            verticalInput = Input.GetAxisRaw("Vertical");
        }
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        if (creature == Creature.Leaf)
        {
            animator.SetBool("isFalling", false);
        }

        if (gameManager.isGameActive)
        {
            MoveUpdate();
            BoundsUpdate();
            MovementSoundUpdate();
        }
    }
    private void MoveUpdate()
    {
        if (horizontalInput != 0)
        {
            if (horizontalInput < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        if (verticalInput != 0 || horizontalInput != 0)
        {
            animator.SetBool("isMoving", true);
        }
        else if (creature == Creature.Leaf)
        {
            animator.SetBool("isFalling", true);
        }
        if (canMove)
        {
            if (canMoveHorizontally)
            {
                rb.velocity = new Vector2(horizontalInput, rb.velocity.y);
            }
            if (canMoveVertically)
            {
                rb.velocity = new Vector2(rb.velocity.x, verticalInput);
            }
            rb.velocity = rb.velocity.normalized * speed;
        }
    }
    private void BoundsUpdate()
    {
        if (transform.position.x - boxCollider.size.x / 2 < gameManager.playerBounds[0])
        {
            transform.position = new Vector2(gameManager.playerBounds[0] + boxCollider.size.x / 2, transform.position.y);
        }
        else if (transform.position.x + boxCollider.size.x / 2 > gameManager.playerBounds[1])
        {
            transform.position = new Vector2(gameManager.playerBounds[1] - boxCollider.size.x / 2, transform.position.y);
        }
        if (transform.position.y - boxCollider.size.y / 2 < gameManager.playerBounds[2])
        {
            transform.position = new Vector2(transform.position.x, gameManager.playerBounds[2] + boxCollider.size.y / 2);
        }
        else if (transform.position.y + boxCollider.size.y / 2 > gameManager.playerBounds[3])
        {
            transform.position = new Vector2(transform.position.x, gameManager.playerBounds[3] - boxCollider.size.y / 2);
        }
    }
    private void MovementSoundUpdate()
    {
        if (Mathf.Abs(rb.velocity.x) > 0 || Mathf.Abs(rb.velocity.y) > 0)
        {
            doPlayMovementSound = true;
        }
        else
        {
            doPlayMovementSound = false;
        }
    }
    private IEnumerator MovementSound()
    {
        yield return new WaitForSeconds(movementSoundInterval);
        if (doPlayMovementSound && gameManager.isGameActive)
        {
            switch (creature)
            {
                case Creature.Human:
                    audioManager.PlaySound("Footstep", 0.95f, 1.05f);
                    break;
            }
        }
        StartCoroutine(MovementSound());
        yield break;
    }
    public IEnumerator FadeOut()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.g, 1);
        while (spriteRenderer.color.a >= 0)
        {
            spriteRenderer.color -= new Color(0, 0, 0, 2 * Time.deltaTime);
            yield return null;
        }
        yield break;
    }
    public IEnumerator FadeIn()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.g, 0);
        while (spriteRenderer.color.a <= 1)
        {
            spriteRenderer.color += new Color(0, 0, 0, 2 * Time.deltaTime);
            yield return null;
        }
        yield break;
    }
}
