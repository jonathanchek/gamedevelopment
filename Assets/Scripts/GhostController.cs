using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour {

    public float speed = 1.0f;
    public Vector2 direction = Vector2.up;
    public Color weakColor = Color.blue;
    public int points = 400;

    private float changeDirectionTime;
    private Vector2 originalPosition;
    private Color originalColor;
    private bool frozen = false;
    private bool weak = false;
    private bool eaten = false;

    private Rigidbody2D rb2d;
    private CircleCollider2D cc2d;
    private SpriteRenderer sr;
    private AudioSource ghostEatenSound;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        ghostEatenSound = GetComponent<AudioSource>();
        originalPosition = transform.position;
        originalColor = sr.color;
    }
	
	// Update is called once per frame
	void Update () {
        if (!frozen)
        {
            if (!eaten)
            {
                //Wall Bump Detection
                if (!openDirection(direction))
                {
                    if (canChangeDirection())
                    {
                        changeDirection();
                    }
                    else if (rb2d.velocity.magnitude < speed)
                    {
                        changeDirectionAtRandom();
                    }
                }
                //Come Across an Intersection
                else if (canChangeDirection() && Time.time > changeDirectionTime)
                {
                    changeDirectionAtRandom();
                }
                //Stuck on a non-wall
                else if (rb2d.velocity.magnitude < speed)
                {
                    changeDirectionAtRandom();
                }
            }
            else
            {
                //Check to see if it's arrived
                if (Vector2.Distance(originalPosition, transform.position) < 0.1f)
                {
                    transform.position = originalPosition;
                    setEaten(false);
                }
            }
            //Rotate Eyes
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t != transform)
                {
                    t.up = direction;
                }
            }
            //Move
            rb2d.velocity = direction * speed;
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

    private bool openDirection(Vector2 direction)
    {
        RaycastHit2D[] rch2ds = new RaycastHit2D[10];
        cc2d.Cast(direction, rch2ds, 1f, true);
        foreach (RaycastHit2D rch2d in rch2ds)
        {
            if (rch2d && rch2d.collider.gameObject.tag == "Level")
            {
                return false;
            }
        }
        return true;
    }

    private bool canChangeDirection()
    {
        Vector2 perpRight = Utility.PerpendicularRight(direction);
        bool openRight = openDirection(perpRight);
        Vector2 perpLeft = Utility.PerpendicularLeft(direction);
        bool openLeft = openDirection(perpLeft);
        return openRight || openLeft;
    }

    private void changeDirectionAtRandom()
    {
        changeDirectionTime = Time.time + 1;
        if (Random.Range(0, 2) == 0)
        {
            changeDirection();
        }
    }

    private void changeDirection()
    {
        changeDirectionTime = Time.time + 1;
        Vector2 perpRight = Utility.PerpendicularRight(direction);
        bool openRight = openDirection(perpRight);
        Vector2 perpLeft = Utility.PerpendicularLeft(direction);
        bool openLeft = openDirection(perpLeft);
        if (openRight || openLeft)
        {
            int choice = Random.Range(0, 2);
            if (!openLeft || (choice == 0 && openRight))
            {
                direction = perpRight;
            }
            else
            {
                direction = perpLeft;
            }
        }
        else
        {
            direction = -direction;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (weak)
            {
                coll.gameObject.GetComponent<PlayerController>().addPoints(points);
                setEaten(true);
            }
            else {
                GameManager.pacmanKilled();
            }
        }
    }

    public void reset()
    {
        transform.position = originalPosition;
        freeze(false);
    }

    public void freeze(bool freeze)
    {
        frozen = freeze;
        rb2d.velocity = Vector2.zero;
    }

    public void setweak(bool isweak)
    {
        weak = isweak;
        if (weak)
        {
            sr.color = weakColor;
        }
        else
        {
            sr.color = originalColor;
        }
    }

    public void setEaten(bool isEaten)
    {
        eaten = isEaten;
        if (eaten)
        {
            sr.color = new Color(0, 0, 0, 0);
            cc2d.enabled = false;
            direction = originalPosition - (Vector2)transform.position;
            ghostEatenSound.Play();
        }
        else
        {
            sr.color = originalColor;
            cc2d.enabled = true;
            direction = Vector2.up;
        }
    }

    public void blink()
    {
        if (sr.color == originalColor)
        {
            sr.color = weakColor;
        }
        else if (sr.color == weakColor)
        {
            sr.color = originalColor;
        }
    }
}
