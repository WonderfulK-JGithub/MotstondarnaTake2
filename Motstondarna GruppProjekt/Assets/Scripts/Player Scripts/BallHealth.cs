using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public MeshRenderer rend;
    [SerializeField] float dissolveSpeed;

    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] float characterTime;
    [SerializeField] float typeWriterTime;
    int healthPoints;

    float invinceTimer;

    bool invinceable;

    


    public override void Awake()
    {
        base.Awake();

        current = this;

        healthPoints = maxHealth;
        //rend = GetComponent<MeshRenderer>();
        defaultColor = rend.material.GetColor("_Color");

        NewHealth();

        
    }

    public override void Update()
    {
        base.Update();

        if(invinceable)//ger bollen en annan f�rg n�r den tar skada
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
        if(knockBack != Vector3.zero)rb.velocity = new Vector3(currentSpeed.x, rb.velocity.y, currentSpeed.z);//om det inte �r n�gon knockback ska bollens hastighet inte �ndras

        healthPoints -= damage;//tar bort hp

        //uppdaterar health UI
        NewHealth();
        healthImageAnim.Play("HP_Image_Hurt");

        if(healthPoints <= 0)//om man har noll hp �r det game over
        {
            GameOver();
            invinceable = true;
            enabled = false;
            StartCoroutine(Dissolve());
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            
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
        if (Pause.gamePaused) return;

        enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        SoundManagerScript.PlaySound("Game Over");
        CameraController.current.enabled = false;

        Invoke("Transition", deathTime);
        Pause.gamePaused = true;
        Pause.source.Stop();
        FindObjectOfType<Pause>().enabled = false;
    }

    void Transition()
    {
        SceneTransition.current.ReLoadScene();
    }

    private new void OnTriggerEnter(Collider other)
    {
        //imagine anv�nda interface
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

    IEnumerator Dissolve()
    {
        float i = 1f;
        while(i > -1f)
        {
            i -= dissolveSpeed * Time.deltaTime;
            rend.material.SetFloat("_DissolveTime", i);
            
            yield return null;
        }
    }


    public void ShowTextMessage(string message)
    {
        StartCoroutine(TypeWriter(message));
    }
    IEnumerator TypeWriter(string message)
    {
        WaitForSeconds daWaitTime = new WaitForSeconds(characterTime);
        for (int i = 0; i < message.Length; i++)
        {
            infoText.text = message.Substring(0, i + 1);
            yield return daWaitTime;
        }

        yield return new WaitForSeconds(typeWriterTime);

        infoText.text = null;
    }
}
