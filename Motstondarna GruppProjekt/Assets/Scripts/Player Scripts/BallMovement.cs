using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallMovement : MonoBehaviour //av K-J (utom där det står max)
{
    [Header("X-Z Movement")]
    [SerializeField] float topSpeed;//högsta hastigheten
    [SerializeField] float acceleration;//hur snabbt man accelererar
    [SerializeField] float extraAccelerationFactor;//hur mycket gånger mer man ska accelerera när man tvärsvänger
    [SerializeField] Transform orientationTransform;//en transform som används för att kalkulera vilket håll bollen ska åka baserat på kamerans rotation
    [SerializeField] Slider speedBar;//en bar som visar hur snabbt man åker
    [SerializeField] Image fillImage;
    [SerializeField] Gradient speedColors;//röd / gul = inte tillräckligt snabbt för att döda käglor, grön = tillräckligt snabbt

    float inputX;
    float inputZ;

    Vector3 targetSpeed;//hastigheten som klotet ska accelerera mot


    [Header("Jumping")]
    [SerializeField] float jumpStrength;//hur högt man hoppar
    [SerializeField] float jumpHoldTime;//hur länge man kan holla in knappen för ett högre hopp
    [SerializeField] float extraGravityFactor;//hur mycket gånger mer man ska ha gravitation när man inte håller in hoppknappen
    [SerializeField] float terminalVelocity;//Den snabbaste hastighet klotet kan falla
    [SerializeField] LayerMask groundLayers;//vilka layers som räknas som ground
    [SerializeField] LayerMask slipparyLayer;//vilka layers som räknas som slippary

    float holdTimer;

    
    [Header("Dashing")]
    [SerializeField] float dashTime;//hur länge man är fast i dashen
    [SerializeField] float dashHoldTime;
    [SerializeField] GameObject dashTrail;//referense till trailobjektet
    [SerializeField] Color dashBarColor;


    float dashTimer;

    [Header("Other")]
    [SerializeField] ParticleSystem chargeParticle;//referense till partiklesystemet som sätts på när man håller in dashknappen
    [SerializeField] float slideExtraGravity;//hur mycket gånger mer gravitation man ska ha när man åker i en slide(ränna)
    [SerializeField] bool inHub;
    [SerializeField] GameObject ballUI;

    bool onGround;
    bool onSlippary;
    bool isJumping;
    bool canDash;

    bool noInput;

    public bool aboveKillSpeed = false; //För att se om spelaren åker tillräckligt snabbt för att döda en simple bowling pin - Max

    public Vector3 currentSpeed;

    [HideInInspector] public Rigidbody rb;
    public PlayerState state;

    

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();


        if(inHub)
        {
            state = PlayerState.Off;
            ballUI.SetActive(false);

        }

        
        
    }


    public virtual void Update()
    {
        if (Pause.gamePaused) return;
        switch (state)
        {
            case PlayerState.Free:
                #region

                //hämtar inputs för vilket håll man ska gå
                inputX = Input.GetAxisRaw("Horizontal");
                inputZ = Input.GetAxisRaw("Vertical");

                noInput = inputX == 0 && inputZ == 0;//variabel som kollar om man tryckt åt något håll alls

                if(inputX != 0 && inputZ != 0)
                {
                    inputX *= Mathf.Sqrt(0.5f);
                    inputZ *= Mathf.Sqrt(0.5f);
                }
                
                //Den speed som bollen ska accelerera mot
                targetSpeed = new Vector3(inputX * topSpeed * orientationTransform.right.x + inputZ * topSpeed * orientationTransform.forward.x, 0f, inputX * topSpeed * orientationTransform.right.z + inputZ * topSpeed * orientationTransform.forward.z);


                //Hopp
                if (Input.GetButtonDown("Jump") && onGround)
                {
                    rb.velocity = new Vector3(rb.velocity.x, jumpStrength, rb.velocity.z);//ger en y velocity
                    isJumping = true;
                    holdTimer = jumpHoldTime;
                }

                //Om man släpper hoppknappen, eller att jumptimern går ut, blir isJumping false
                if (isJumping && !onGround)
                {
                    if (Input.GetButton("Jump"))
                    {
                        holdTimer -= Time.deltaTime;
                        if (holdTimer <= 0)
                        {
                            isJumping = false;
                        }
                    }
                    else
                    {
                        isJumping = false;
                    }
                }

                //Dash
                if(Input.GetButtonDown("Fire1") && canDash)
                {
                    state = PlayerState.ChargeDash;//ändrar state
                    chargeParticle.Play();//Sätter igång particles
                    canDash = false;//gör att man inte kan dasha (föräns variabeln blir true igen)
                    dashTimer = dashHoldTime;

                    rb.useGravity = false;//stänger av gravitation

                    dashTrail.transform.position = transform.position;
                    dashTrail.SetActive(false);
                }

                #endregion
                break;
            case PlayerState.ChargeDash:
                #region

                currentSpeed = Vector3.Lerp(currentSpeed, Vector3.zero, 0.05f);//ändrar bollens hastighet långsamt till 0

                //kollar om man släpper dashKnappen
                if(!Input.GetButton("Fire1"))
                {
                    SoundManagerScript.PlaySound("Dash");

                    chargeParticle.Stop();//Stänger av particle systemet
                    state = PlayerState.Dash;//ändrar state

                    rb.velocity = orientationTransform.forward * topSpeed;//ändrar hastigheten (i den riktingen kameran kollar)
                    currentSpeed = rb.velocity;
                    dashTimer = dashTime;

                    dashTrail.SetActive(true);
                }
                else
                {
                    dashTimer -= Time.deltaTime;
                    if(dashTimer <= 0)
                    {
                        state = PlayerState.Free;
                        chargeParticle.Stop();
                    }
                }
                #endregion
                break;
            case PlayerState.Dash:
                #region
                dashTimer -= Time.deltaTime;

                if(dashTimer <= 0f)//när dashtimern är slut är man tillbaks till vanligt movement
                {
                    state = PlayerState.Free;
                    rb.useGravity = true;
                }

                #endregion
                break;
        }
    }

    
    void FixedUpdate()
    {
        switch(state)
        {
            case PlayerState.Free:
                #region
                if (Physics.Raycast(transform.position, Vector3.down, 0.60f, groundLayers))
                {
                    onGround = true;
                    canDash = true;
                }//kollar om man är på ground
                else
                {
                    onGround = false;
                }
                if (Physics.Raycast(transform.position, Vector3.down, 0.52f, slipparyLayer))
                {
                    onSlippary = true;
                }//kollar om man är på slippary
                else
                {
                    onSlippary = false;
                }


                //är vinkeln mellan vectorerna av den nuvarnade hastigheten och target hastigheten mer än 90 grader räknar jag det som tvärsvägning
                //det ska även bli extra acceleration om spelaren är över top hastigheten
                float accModifier = 1f;
                if(currentSpeed.magnitude > topSpeed)
                {
                    accModifier = extraAccelerationFactor * 2f;
                }
                else if(Vector3.Angle(targetSpeed.normalized, currentSpeed.normalized) > 90f)
                {
                    accModifier = extraAccelerationFactor;
                }

                if ((onGround && !onSlippary) || !noInput)//Är man i luften eller på halt golv OCH inte trycker åt något håll behåller man den hastighet man hade
                {

                    currentSpeed = Vector3.MoveTowards(new Vector3(rb.velocity.x, 0f, rb.velocity.z), targetSpeed, acceleration * Time.fixedDeltaTime * accModifier);


                    rb.velocity = new Vector3(currentSpeed.x, rb.velocity.y, currentSpeed.z);
                }

                if (!onGround && !isJumping)//lägger till extra gravitation för ett bättre hopp
                {
                    rb.velocity += new Vector3(0f, Physics.gravity.y * (extraGravityFactor - 1) * Time.fixedDeltaTime, 0f);
                }

                float yVelocityClamped = Mathf.Clamp(rb.velocity.y, terminalVelocity, 69420f);//clampar y så att man inte går över terminal velocity
                rb.velocity = new Vector3(rb.velocity.x, yVelocityClamped, rb.velocity.z);

                chargeParticle.transform.position = transform.position;//gör att charge particlen följer med spelaren

                #endregion
                break;
            case PlayerState.ChargeDash:
                #region
                rb.velocity = new Vector3(currentSpeed.x, currentSpeed.y, currentSpeed.z);
                chargeParticle.transform.position = transform.position;
                #endregion
                break;
            case PlayerState.Renn:
            case PlayerState.End:
                #region
                rb.velocity += new Vector3(0f, Physics.gravity.y * (slideExtraGravity - 1) * Time.fixedDeltaTime, 0f);
                #endregion
                break;
            case PlayerState.Dash:
                #region
                dashTrail.transform.position = transform.position;
                break;
            #endregion
            case PlayerState.Hub:
                #region
                rb.velocity += new Vector3(0f, Physics.gravity.y * (slideExtraGravity - 1) * Time.fixedDeltaTime, 0f);
                currentSpeed = rb.velocity;
                #endregion
                break;
            case PlayerState.Off:
                #region
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                #endregion
                break;
            case PlayerState.UnControllable:
                #region
                if (Physics.Raycast(transform.position, Vector3.down, 0.52f, groundLayers))
                {
                    state = PlayerState.Free;
                }

                currentSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.useGravity = true;
                rb.velocity += new Vector3(0f, Physics.gravity.y * (extraGravityFactor - 1) * Time.fixedDeltaTime, 0f);

                #endregion
                break;
        }

        rb.angularVelocity = new Vector3(rb.velocity.z,0f,-rb.velocity.x);//ändrar rotationSpeeden baserat på hastigheten bollen har

        float _value = new Vector3(rb.velocity.x,0f,rb.velocity.z).magnitude / (topSpeed * 0.95f);//_value är mellan 0 och 1 och är hur mycket procent av topspeed man har

        if(state == PlayerState.ChargeDash)
        {
            speedBar.value = dashTimer / dashHoldTime;

            fillImage.color = dashBarColor;
        }
        else
        {
            speedBar.value = Mathf.Lerp(speedBar.value, _value, 0.3f);//ändrar valuen på slidern så att baren fylls upp

            fillImage.color = speedColors.Evaluate(_value);//ändar färgen på baren baserat på om man har tillräckligt med fart(för att döda käglor) med hjälp av en gradient
        }

        //Kollar bollens hastighet och ändrar vissa bowling pins till 
        //triggers så att det ser bättre ut när man får en strike - Max 
        if (new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude >= 8)
        {
            if (!aboveKillSpeed) //boolen behövs så det här inte görs varje frame - Max
            {
                aboveKillSpeed = true;
                ChangeBowlingPinLayers(9);
            }
        }
        else
        {
            if (aboveKillSpeed)
            {
                aboveKillSpeed = false;
                ChangeBowlingPinLayers(0);
            }
        }

    }

    void ChangeBowlingPinLayers(int layer)
    {
        //FindGameObjectsWithTag är tydligen bättre för performance än FindObjectsOfType
        //och eftersom det kanske finns väldigt många pins på banan så använder jag det - Max
        GameObject[] allSimpleBowlingPins = GameObject.FindGameObjectsWithTag("SimpleBowlingPin");

        //ändrar lagret på varenda bowling pin den hittar - Max
        for (int i = 0; i < allSimpleBowlingPins.Length; i++)
        {
            allSimpleBowlingPins[i].layer = layer;
        }
    }

    public void UpdateRotation(Vector3 rotation)
    {
        orientationTransform.eulerAngles = rotation;
        chargeParticle.transform.eulerAngles = rotation;
        if(state == PlayerState.ChargeDash)dashTrail.transform.eulerAngles = rotation;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ränna") && state == PlayerState.Free)
        {
            state = PlayerState.Renn;
            currentSpeed = Vector3.zero;
        }
        else if(other.gameObject.CompareTag("Coin"))
        {

            other.gameObject.GetComponent<CollectableCoin>().CollectCoin();

            SoundManagerScript.PlaySound("Coins");
        }
        else if(other.gameObject.CompareTag("Finish"))
        {
            transform.position = other.transform.position;
            rb.velocity = Vector3.right * 2f;
            state = PlayerState.End;
            rb.useGravity = true;
            chargeParticle.Stop();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ränna") && state == PlayerState.Free)
        {
            state = PlayerState.Renn;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ränna"))
        {
            state = PlayerState.Free;
            currentSpeed = rb.velocity;
            currentSpeed.y = 0f;
        }
    }

    public void BossDamaged(Vector3 bossKnockback)
    {
        state = PlayerState.UnControllable;

        rb.velocity = bossKnockback;
    }
    
}





public enum PlayerState
{
    Free,
    ChargeDash,
    Dash,
    Renn,
    Off,
    Hub,
    End,
    UnControllable,
}