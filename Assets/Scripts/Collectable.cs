using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {

    public int points = 100;
    public AudioClip collectSound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            collected(coll);
        }
    }

    protected virtual void collected(Collider2D coll)
    {
        coll.gameObject.GetComponent<PlayerController>().addPoints(points);
        AudioSource.PlayClipAtPoint(collectSound, transform.position);
        gameObject.SetActive(false);
    }
}
