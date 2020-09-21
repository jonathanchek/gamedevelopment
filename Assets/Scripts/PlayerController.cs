using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed;
    public int score = 0;
    public int livesLeft = 2;

    public Text scoreText;
    public Image life1;
    public Image life2;

    private Vector2 direction;
    private bool alive = true;

    Rigidbody2D rb2d;
    Animator animator;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (alive)
        {
            animator.SetFloat("currentSpeed", rb2d.velocity.magnitude);
            if (Input.GetAxis("Horizontal") < 0)
            {
                direction = Vector2.left;
            }
            if (Input.GetAxis("Horizontal") > 0)
            {
                direction = Vector2.right;
            }
            if (Input.GetAxis("Vertical") < 0)
            {
                direction = Vector2.down;
            }
            if (Input.GetAxis("Vertical") > 0)
            {
                direction = Vector2.up;
            }
            rb2d.velocity = direction * speed;
            transform.up = direction;
            if (rb2d.velocity.x == 0)
            {
                transform.position = new Vector2(Mathf.Round(transform.position.x), transform.position.y);
            }
            if (rb2d.velocity.y == 0)
            {
                transform.position = new Vector2(transform.position.x, Mathf.Round(transform.position.y));
            }
        }
    }

    public void addPoints(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = ""+score;
    }

    public void setAlive(bool isAlive)
    {
        alive = isAlive;
        animator.SetBool("alive", alive);
        rb2d.velocity = Vector2.zero;
    }

    public void setLivesLeft(int lives)
    {
        livesLeft = lives;
        life1.enabled = livesLeft >= 1;
        life2.enabled = livesLeft >= 2;
    }
}
