using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Max Script
public class WanderingEnemy : BaseEnemy
{
    [Header("Wandering Area")]
    [SerializeField] Vector3 wanderingAreaCenter; //Runt den h�r punkten s� wanderar den - Max
    [SerializeField] float wanderingAreaSize; //Hur stor area som den wanderar - Max

    [Header("Wandering parameters")]
    [SerializeField] float wanderingSpeed; //Hur snabbt den g�r - Max
    [SerializeField] float targetDistance; //Hur n�ra den beh�ver vara till target f�r att det ska r�knas som att den �r framme - Max
    [SerializeField] float waitTimeToNewTarget; //Hur l�nge den ska st� still tills den tar en ny target att g� till - Max
    [SerializeField] Vector3 currentTarget; //Dit den �r p� v�g just nu - Max

    [Header("Checking For Player")]
    [SerializeField] float playerCheckRadius; //Hur stor radius som den kollar efter spelaren i - max
    [SerializeField] float tilCanCheckForPlayer = 1f; //Hur l�ng tid fienden m�ste v�nta mellan en avsutad chase tills den kan b�rja en ny - Max

    [Header("Chasing parameters")]
    [SerializeField] float chasingSpeed; //Hur snabbt den jagar spelaren - max
    [SerializeField] float rotationSpeed; //Hur snabbt den roterar n�r den r�r sig - max

    [Header("Other")]
    [SerializeField] bool dontStandStill = false; //Vissa enemies har ingen st� still animation - Max
    [SerializeField] float yPosToDie = -10f;

    [HideInInspector] public bool isChasingPlayer = false; //Jagar spelaren - Max
    bool isMoving = false; //R�r p� sig - Max
    bool canCheckForPlayer = true; 
    [HideInInspector] public bool overrideChasing = false; //�r public s� att andra skript kan accessa den - Max

    //References
    Transform player;
    Animator anim;

    public override void Awake()
    {
        base.Awake();

        player = FindObjectOfType<BallMovement>().transform;
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        wanderingAreaCenter = transform.position; //Den ska wandera runt d�r den f�rst placeras - max

        NewPos(); //Hittar en ny target pos att g� till - Max
    }

    private void Update()
    {
        if (hasDied) return;

        if(transform.position.y < yPosToDie)
        {
            DieNow();
        }

        if (canCheckForPlayer && !isChasingPlayer)
        {
            //Om spelaren �r inom radius ska man b�rja chasea den - Max
            if (Vector3.Distance(player.position, transform.position) < playerCheckRadius)
            {
                StartChasing();
            }
        }
        else if (!canCheckForPlayer)
        {
            //Om spelaren �r riktigt n�ra ska den b�rja chasea den oavsett om den ska v�nta lite till innan b�rjad chase igen - Max
            if (Vector3.Distance(player.position, transform.position) < playerCheckRadius / 3)
            {
                StartChasing();
            }
        }
    }

    void FixedUpdate()
    {
        //Man ska inte alltid kunna r�ra p� sig - Max
        if (!isMoving || hasDied) return;

        if (isChasingPlayer)
        {
            if (ObstructedCheck()) //Om det �r en v�gg i v�gen ska fienden sluta chasea - Max
            {
                Invoke(nameof(StopChasing), 1f);
                rb.velocity = Vector3.zero;
                canCheckForPlayer = false;
            }

            if (!overrideChasing) //Override anv�nds f�r att speciella fiender ska kunna g�ra laser och rocket-attacker - Max
            {
                ChasePlayer();
            }
        }
        else
        {
            if(Vector3.Distance(player.position, transform.position) < playerCheckRadius * 4)
                Wandering();
        }
    }

    void CanCheckForPlayer()
    {
        canCheckForPlayer = true;
    }

    void ChasePlayer()
    {
        if (GroundCheck()) //Kollar s� att den inte g�r utf�r ett stup - Max
        {
            MoveTowardsTarget(player.position, chasingSpeed);
        }
        else
        {
            //Annars slutar den chasea - Max
            Invoke(nameof(StopChasing), 0.5f);
            rb.velocity = Vector3.zero;
            canCheckForPlayer = false;
        }
    }

    void Wandering()
    {
        Vector3 target = currentTarget;

        //Om den �r tillr�ckligt n�ra target s� ska fienden stanna och efter ett tag f� en ny target - Max
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.x, 0, target.z)) < targetDistance)
        {
            isMoving = false;

            if (anim != null && !dontStandStill)
                anim.SetBool("isWalking", false); //St� still - Max

            Invoke(nameof(NewPos), waitTimeToNewTarget); //Skaffar ny target efter ett tag - Max
            return;
        }

        MoveTowardsTarget(target, wanderingSpeed);
    }

    void StopChasing()
    {
        isChasingPlayer = false;
        Invoke(nameof(CanCheckForPlayer), tilCanCheckForPlayer); //Efter 1 sekund s� kan fienden b�rja kolla efter spelaren igen - Max
    }

    public void StartChasing()
    {
        //Kollar om det finns en v�gg som �r i v�gen - Max
        if (Physics.Linecast(transform.position, player.position, LayerMask.GetMask("Ground", "Slippery"))) return;

        if(anim != null && !dontStandStill)
            anim.SetBool("isWalking", true); //B�rja g� igen - Max

        isChasingPlayer = true;
    }

    void MoveTowardsTarget(Vector3 target, float speed)
    {
        //Kollar mot target - Max
        var lookPos = target - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        //R�r sig mot target - Max
        if(Physics.Raycast(transform.position + transform.forward, Vector2.down, LayerMask.GetMask("Ground", "Slippery")))
        {
            Vector3 newVel = transform.forward * speed;
            rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
        }
    }

    bool GroundCheck()
    {
        Debug.DrawRay(transform.position + transform.forward * 1, Vector3.down, Color.red, 4);

        //Raycast ner�t och returnar true om den tr�ffar ground
        if (Physics.Raycast(transform.position + transform.forward * 1, Vector3.down, 4, LayerMask.GetMask("Ground", "Slippery")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool ObstructedCheck()
    {
        if (Physics.Linecast(transform.position, player.position, LayerMask.GetMask("Ground", "Slippery")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void NewPos()
    {
        bool foundPos = false;
        int count = 0; //failsafe //S� att unity inte fryser om man placerar fiender p� ett s� d�ligt s�tt att det inte finns n�n mark under dem - Max
        while (!foundPos)
        {
            //failsafe
            count++;
            if(count > 100)
            {
                Debug.LogError("S�tt fienden p� marken >:("); //Ger error - Max
                break;
            }

            //Random position inom radius - Max
            Vector3 newTarget = wanderingAreaCenter +
            new Vector3
                (
                Random.Range(-wanderingAreaSize, wanderingAreaSize),
                0,
                Random.Range(-wanderingAreaSize, wanderingAreaSize)
                );

            //Den testar om den nya positionen har mark under sig, annars m�ste den hitta en ny - Max
            if (Physics.Raycast(newTarget + new Vector3(0,5,0), Vector3.down, 10, LayerMask.GetMask("Ground", "Slippery")))
            {
                foundPos = true;

                currentTarget = newTarget;
                isMoving = true;
            }
        }

        if (anim != null && !dontStandStill)
            anim.SetBool("isWalking", true); //B�rja g� igen - Max
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        //Ganska on�dig kod efter att chase state las till - Max
        if (collision.transform.GetComponent<BallMovement>())
        {
            NewPos();
        }
    }

    public override void Die(Vector3 contactPoint, Vector3 speed)
    {
        //G�r s� att den kan p�verkas av forces - Max
        isMoving = false;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        
        //Sen d�r
        base.Die(contactPoint, speed);
    }

    private void OnDrawGizmosSelected()
    {
        //Visar bara upp fiendens radius i sceneview s� det �r enkelt att bygga banor - Max

        Gizmos.color = new Color(0,255,0,0.4f);

        //Gizmos.DrawSphere(transform.position, playerCheckRadius);

        if (Application.isPlaying)
        {
            Gizmos.DrawCube(wanderingAreaCenter, new Vector3(wanderingAreaSize * 2, wanderingAreaSize * 2, wanderingAreaSize * 2));
        }
        else
        {
            Gizmos.DrawCube(transform.position, new Vector3(wanderingAreaSize * 2, wanderingAreaSize * 2, wanderingAreaSize * 2));
        }
    }
}
