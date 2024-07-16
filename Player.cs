// COLISÃO DE ATAQUE DO PLAYER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (player.GetComponent<PlayerController>().numeroCombo == 1)
            {
                collision.GetComponent<Character>().life--;
            }

            if (player.GetComponent<PlayerController>().numeroCombo == 2)
            {
                collision.GetComponent<Character>().life -= 2;
            }

        }

    }
}

// SOM DO PULO A COLISÃO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollider : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip groundedSound;

    public bool canJump;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            canJump = true;
            audioSource.PlayOneShot(groundedSound);
        }
    }
}

// CONTROLE DO PLAYER
// SUA FUNCIONALIDADE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 vel;

    public AudioSource audioSource;
    public AudioClip attack1Sound;
    public AudioClip attack2Sound;
   
    public AudioClip damageSound;
    public AudioClip dashSound;

    public Transform gameOverScreen;
    public Transform pauseScreen;

    public Transform floorCollider;
    public Transform skin;

    public int numeroCombo;
    public float tempoCombo;
    public float dashTime;


    public string currentLevel;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        currentLevel = SceneManager.GetActiveScene().name;
        DontDestroyOnLoad(transform.gameObject);
    }


    void Update()
    {

        if (!currentLevel.Equals(SceneManager.GetActiveScene().name))
        {
            currentLevel = SceneManager.GetActiveScene().name;
            transform.position = GameObject.Find("Spawn").transform.position;
        }

        if (GetComponent<Character>().life <= 0)
        {
            gameOverScreen.GetComponent<GameOver>().enabled = true;
            rb.simulated = false;
            this.enabled = false;
        }


        if (Input.GetButtonDown("Cancel"))
        {
            pauseScreen.GetComponent<Pause>().enabled = !pauseScreen.GetComponent<Pause>().enabled;
        }

        dashTime = dashTime + Time.deltaTime;

        if (Input.GetButtonDown("Fire2") && dashTime > 1)
        {
            audioSource.PlayOneShot(dashSound);

            dashTime = 0;
            skin.GetComponent<Animator>().Play("PlayerDash", -1);
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(skin.localScale.x * 600, 0));
        }


        tempoCombo = tempoCombo + Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && tempoCombo > 0.3f)
        {
            numeroCombo++;
            if (numeroCombo > 2)
            {
                numeroCombo = 1;
            }

            tempoCombo = 0;
            skin.GetComponent<Animator>().Play("PlayerAttack" + numeroCombo, -1);

            if(numeroCombo == 1)
            {
                audioSource.PlayOneShot(attack1Sound);
            }

            if (numeroCombo == 2)
            {
                audioSource.PlayOneShot(attack2Sound);
            }

        }

        if (tempoCombo >= 1)
        {
            numeroCombo = 0;
        }

        if (Input.GetButtonDown("Jump" ) && floorCollider.GetComponent<FloorCollider>().canJump == true)
        {
            skin.GetComponent<Animator>().Play("PlayerJump", -1);
            floorCollider.GetComponent<FloorCollider>().canJump = false;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0, 600));
        }

        vel = new Vector2(Input.GetAxisRaw("Horizontal") * 6.0f, rb.velocity.y);

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            skin.localScale = new Vector3(Input.GetAxisRaw("Horizontal"), 1, 1);
            skin.GetComponent<Animator>().SetBool("PlayerRun", true);
        }
        else
        {
            skin.GetComponent<Animator>().SetBool("PlayerRun", false);
        }

    }

    private void FixedUpdate()
    {
        if (dashTime > 0.5)
        {
            rb.velocity = vel;
        }
    }

    public void DestroyPlayer()
    {
        Destroy(transform.gameObject);
    }
}
