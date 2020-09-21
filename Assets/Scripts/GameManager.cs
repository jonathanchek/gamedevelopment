using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public enum GameState
    {
        PLAY, PACMAN_DYING, PACMAN_DEAD, GAME_OVER, GAME_WON
    };
    public GameState gameState = GameState.PLAY;
    [Range(1,10)]
    public float ghostweakDuration = 7.0f;
    [Range(1,5)]
    public float ghostweakEndWarningDuration = 2.0f;
    
    public Image gameWonScreen;
    public Image gameOverScreen;

    public GameObject pacman;
    public AnimationClip pacmanDeathAnimation;
    public List<GameObject> ghosts;
    public List<GameObject> pellets;

    public AudioSource pacmanKilledAudio;
    public AudioSource gameWonAudio;
    public AudioSource gameOverAudio;

    private static GameManager instance;
    private float respawnTime;
    private float weakTime = 0;

	// Use this for initialization
	void Start () {
		if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        gameOverScreen.enabled = false;
        gameWonScreen.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		switch (gameState)
        {
            case GameState.PLAY:
                bool foundpellet = false;
                foreach (GameObject pellets in pellets)
                {
                    if (pellets.activeSelf)
                    {
                        foundpellet = true;
                        break;
                    }
                }
                if (!foundpellet)
                {
                    gameState = GameState.GAME_WON;
                }
                break;
            case GameState.PACMAN_DYING:
                if (Time.time > respawnTime)
                {
                    gameState = GameState.PACMAN_DEAD;
                    respawnTime = Time.time + 1;
                    pacman.SetActive(false);
                }
                break;
            case GameState.PACMAN_DEAD:
                if (Time.time > respawnTime)
                {
                    gameState = GameState.PLAY;
                    pacman.SetActive(true);
                    PlayerController playerController = pacman.GetComponent<PlayerController>();
                    playerController.setLivesLeft(playerController.livesLeft - 1);
                    if (playerController.livesLeft >= 0)
                    {
                        playerController.setAlive(true);
                    }
                    else
                    {
                        gameState = GameState.GAME_OVER;
                    }
                    pacman.transform.position = Vector2.zero;
                    foreach (GameObject ghost in ghosts)
                    {
                        ghost.GetComponent<GhostController>().reset();
                    }
                }
                break;
            case GameState.GAME_OVER:
                gameOverScreen.enabled = true;
                gameWonScreen.enabled = false;
                if (!gameOverAudio.isPlaying)
                {
                    gameOverAudio.Play();
                }
                if (Input.anyKeyDown)
                {
                    resetGame();
                    gameState = GameState.PLAY;
                    gameOverScreen.enabled = false;
                    gameWonScreen.enabled = false;
                }
                break;
            case GameState.GAME_WON:
                gameOverScreen.enabled = false;
                gameWonScreen.enabled = true;
                if (!gameWonAudio.isPlaying)
                {
                    gameWonAudio.Play();
                }
                if (Input.anyKeyDown)
                {
                    resetGame();
                    gameState = GameState.PLAY;
                    gameOverScreen.enabled = false;
                    gameWonScreen.enabled = false;
                }
                break;
        }
        
        if (weakTime > 0)
        {
            if (Time.time > weakTime)
            {
                weakTime = 0;
                foreach (GameObject ghost in ghosts)
                {
                    ghost.GetComponent<GhostController>().setweak(false);
                }
            }
            else if (Time.time > weakTime - ghostweakEndWarningDuration
                && (Time.time *10)%2 < 0.1f)
            {
                foreach (GameObject ghost in ghosts)
                {
                    ghost.GetComponent<GhostController>().blink();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    public static void pacmanKilled()
    {
        instance.pacman.GetComponent<PlayerController>().setAlive(false);
        instance.gameState = GameState.PACMAN_DYING;
        instance.respawnTime = Time.time + instance.pacmanDeathAnimation.length;
        instance.pacmanKilledAudio.Play();
        foreach (GameObject ghost in instance.ghosts)
        {
            ghost.GetComponent<GhostController>().freeze(true);
        }
    }

    public void resetGame()
    {
        pacman.transform.position = Vector2.zero;
        PlayerController playerController = pacman.GetComponent<PlayerController>();
        playerController.setLivesLeft(2);
        playerController.setAlive(true);
        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostController>().reset();
        }
        foreach (GameObject pellets in pellets)
        {
            pellets.SetActive(true);
        }
    }

    public static void makeGhostsweak()
    {
        instance.weakTime = Time.time + instance.ghostweakDuration;
        foreach (GameObject ghost in instance.ghosts)
        {
            ghost.GetComponent<GhostController>().setweak(true);
        }
    }
}
