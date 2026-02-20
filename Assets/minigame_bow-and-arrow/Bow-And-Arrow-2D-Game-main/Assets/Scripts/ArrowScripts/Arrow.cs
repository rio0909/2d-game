using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Arrow : MonoBehaviour
{

    public float minShotForce = 0.0f;
    public float maxShotForce = 10000.0f;
    public bool shot = false;
    private bool hit = false;
    public int shotStrength = 0;
    public float shootingAngleIncrement = 0.5f;
    public float shootingAngle = 0;
    public float maxShootingAngle = 90;
    public float minShootingAngle = -90;
    public int minVal = -2;

    // --- TWEAKED: Dialed up to 95! ---
    public float chargeSpeed = 95f; 
    private float exactPower = 0f;
    // ---------------------------------
    
    // --- NEW: A rule to make sure we don't fire when clicking UI! ---
    private bool isCharging = false; 
    // ---------------------------------------------------------------

    public GameObject target;
    public TargetGameManager gameManager;
    
    public AudioManager audiomanager;
    private new Rigidbody2D rigidbody2D;

    public UnityEvent HitTarget;
    public UnityEvent MissTarget;


    private Vector3 position;
    private Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        HitTarget = new UnityEvent();
        MissTarget = new UnityEvent();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
        position = transform.position;
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            gameManager.GameOver();
            StartCoroutine(gameManager.QuitGame());
            
        }
        else
        {
            if (hit)
            {
                rigidbody2D.linearVelocity = Vector2.zero;
                rigidbody2D.angularVelocity = 0;
                rigidbody2D.gravityScale = 0;
                return;
            }

            if (transform.position.y < minVal)
            {
                hit = true;
                MissTarget.Invoke();
            }

            if (!shot)
            {
                transform.rotation = Quaternion.Euler(0, 0, shootingAngle);
            }

            if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && shootingAngle < maxShootingAngle)
            {
                shootingAngle += shootingAngleIncrement;
            } 
            
            else if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && shootingAngle > minShootingAngle)
            {
                shootingAngle -= shootingAngleIncrement;
            }        

            float velocity = Mathf.Abs(rigidbody2D.linearVelocity.x);

            // --- FIXED: Only start charging if the mouse clicks DOWN while the Game Over panel is NOT active ---
            if (Input.GetMouseButtonDown(0) && !gameManager.GOPanel.activeInHierarchy)
            {
                isCharging = true;
            }

            // Build up strength while the mouse button is held AND we have a legit charge going
            if (Input.GetMouseButton(0) && isCharging)
            {
                if (exactPower < 100)
                {
                    exactPower += chargeSpeed * Time.deltaTime;
                    shotStrength = Mathf.FloorToInt(exactPower);
                }
            }

            // When the button is released AND we were legitimately charging, fire the arrow!
            if (!shot && Input.GetMouseButtonUp(0) && isCharging)
            {
                isCharging = false; // Turn off the charge rule until we click again
                
                audiomanager.Audio.PlayOneShot(audiomanager.shootingsound);
                rigidbody2D.gravityScale = 1;
                float force = shotStrength / 150.0f * maxShotForce;
                float forceX = force * Mathf.Cos(shootingAngle * Mathf.Deg2Rad);
                float forceY = force * Mathf.Sin(shootingAngle * Mathf.Deg2Rad);
                rigidbody2D.AddForce(new Vector2(forceX, forceY));
                shot = true;
            }
        }
        
    }


    public void reset()
    {
        hit = false;
        shot = false;
        transform.position = position;
        transform.rotation = rotation;
        rigidbody2D.linearVelocity = Vector2.zero;
        rigidbody2D.angularVelocity = 0;
        rigidbody2D.gravityScale = 0;
        
        shotStrength = 0;
        exactPower = 0f;
        isCharging = false; // Make sure the rule is reset!
        
        shootingAngle = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        hit = true;
        if (collision.gameObject == target)
        {
            HitTarget.Invoke();
        } else
        {
            MissTarget.Invoke();
        }
    }
}