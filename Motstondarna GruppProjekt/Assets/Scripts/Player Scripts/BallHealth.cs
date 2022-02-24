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
    [SerializeField] float invinceTime;//hur länge man är odödlig efter att man blivit skadad av en käggla
    [SerializeField] float deathTime;//hur mycket scenetransitionen är delayed när man är död

    [SerializeField] Image healthImage;
    [SerializeField] Sprite[] healthSprites;
    [SerializeField] Animator healthImageAnim;

    public float lowestLevel = -4;

    public Color invinceColor;//vilken färg man har när man är odödlig (ändras av en animation som bollen har)
    Color defaultColor;

    public MeshRenderer rend;
    [SerializeField] float dissolveSpeed;

    [Header("Text")]
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

        if(invinceable)//ger bollen en annan färg när den tar skada
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

        if (transform.position.y < lowestLevel) // om man är under lowestLevel - Anton
        {
            GameOver();
        }
    }

    public void TakeDamage(Vector3 knockBack,int damage)//tar bort hp och ändrar hastigheten/ "ge en knockback"
    { 
        if (invinceable) return;

        CameraController.current.ScreenShake();

        currentSpeed = knockBack;
        if(knockBack != Vector3.zero)rb.velocity = new Vector3(currentSpeed.x, rb.velocity.y, currentSpeed.z);//om det inte är någon knockback ska bollens hastighet inte ändras

        healthPoints -= damage;//tar bort hp

        //uppdaterar health UI
        NewHealth();
        healthImageAnim.Play("HP_Image_Hurt");

        if(healthPoints <= 0)//om man har noll hp är det game over
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

    void NewHealth()//uppdaterar health UI
    {
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealth);

        healthImage.sprite = healthSprites[healthPoints];

    }

    public void GameOver()
    {
        if (Pause.gamePaused) return;//om spelet har pausats kan man inte dö

        enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        SoundManagerScript.PlaySound("Game Over");
        CameraController.current.enabled = false;//kameran slutar följa spelaren

        Invoke("Transition", deathTime);//börjar scenetransitiona efter viss tid
        Pause.gamePaused = true;
        Pause.source.Stop();//stoppar musiken
        FindObjectOfType<Pause>().enabled = false;
    }

    void Transition()
    {
        SceneTransition.current.ReLoadScene();
    }

    private new void OnTriggerEnter(Collider other)
    {
        //imagine använda interface
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
            GameOver();
        }
    }

    IEnumerator Dissolve()
    {
        float i = 1f;
        while(i > -1f)
        {
            i -= dissolveSpeed * Time.deltaTime;
            rend.material.SetFloat("_DissolveTime", i);//ändrar variabel i shadergraph koden
            
            yield return null;
        }
    }


    public void ShowTextMessage(string message)
    {
        StartCoroutine(TypeWriter(message));
    }
    IEnumerator TypeWriter(string message)//skriver text en character i taget
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
