using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallHealth : BallMovement // av K-J
{
    public static BallHealth current;

    [Header("Health")]
    [SerializeField] int maxHealth;
    [SerializeField] float invinceTime;//hur l�nge man �r od�dlig efter att man blivit skadad av en k�ggla
    [SerializeField] float deathTime;//hur mycket scenetransitionen �r delayed n�r man �r d�d

    [SerializeField] Image healthImage;
    [SerializeField] Sprite[] healthSprites;
    [SerializeField] Animator healthImageAnim;
    [SerializeField] ParticleSystem waterSplashPS;

    public float lowestLevel = -4;

    public Color invinceColor;//vilken f�rg man har n�r man �r od�dlig (�ndras av en animation som bollen har)
    Color defaultColor;

    int healthPoints;

    float invinceTimer;

    bool invinceable;

    public MeshRenderer rend;


    public override void Awake()
    {
        base.Awake();

        current = this;

        healthPoints = maxHealth;
        //rend = GetComponent<MeshRenderer>();
        defaultColor = rend.material.color;

        NewHealth();

        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if(invinceable)
        {
            float _time = 1f - (invinceTimer / invinceTime);

            invinceTimer -= Time.deltaTime;
            if(invinceTimer <= 0)
            {
                invinceable = false;
            }

            rend.material.color = invinceColor;
            
        }
        else
        {
            rend.material.color = defaultColor;
        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            TakeDamage(Vector3.zero, 1);
        }

        if (transform.position.y < lowestLevel) // om man �r under lowestLevel - Anton
        {
            GameOver();
        }
    }

    public void TakeDamage(Vector3 knockBack,int damage)//tar bort hp och �ndrar hastigheten/ "ge en knockback"
    { 
        if (invinceable) return;

        CameraController.current.ScreenShake();

        currentSpeed = knockBack;
        if(knockBack != Vector3.zero)rb.velocity = new Vector3(currentSpeed.x, rb.velocity.y, currentSpeed.z);

        healthPoints -= damage;
        NewHealth();

        healthImageAnim.Play("HP_Image_Hurt");

        if(healthPoints <= 0)
        {
            GameOver();
            invinceable = true;
            this.enabled = false;
        }
        else
        {
            invinceTimer = invinceTime;
            invinceable = true;
            SoundManagerScript.PlaySound("Skada");
        }
    }

    void NewHealth()
    {
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealth);

        healthImage.sprite = healthSprites[healthPoints];

    }

    public void GameOver()
    {
        enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        SoundManagerScript.PlaySound("Game Over");
        CameraController.current.enabled = false;

        Invoke("Transition", deathTime);
        Pause.gamePaused = true;
        FindObjectOfType<Pause>().enabled = false;
    }

    void Transition()
    {
        SceneTransition.current.ReLoadScene();
    }

    private new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.CompareTag("Heart"))
        {
            healthPoints = maxHealth;
            NewHealth();

            Destroy(other.gameObject);

            SoundManagerScript.PlaySound("PowerUp");

            
        }
        else if(other.gameObject.CompareTag("ShockWave"))
        {
            TakeDamage(Vector3.zero, 1);
        }
        else if(other.gameObject.CompareTag("Water"))
        {
            //SoundManagerScript.PlaySound("WaterSplash");

            //waterSplashPS.Play();
            //waterSplashPS.transform.position = transform.position;

            GameOver();
        }
    }
}
