using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Max Script
public class RocketEnemy : MonoBehaviour
{
    bool rocketOn = false; //Faktiskt �ker med raketet - Max
    bool alerted = false; //Att fienden bara �r alerted - Max

    [Header("Parameters")]
    [SerializeField] float rocketRotatingSpeed; //Hur snabbt fienden roterar n�r den �ker med raketen - Max
    [SerializeField] float rocketSpeed; //Hur snabbt fienden �ker med raketen - Max
    [SerializeField] float rocketRotSpeedWhileActivating; //Hur snabbt den roterar n�r den blir alerted och kastar sig p� marken f�r att b�rja �ka - Max

    [SerializeField] float rocketActivateRadius; //Radius f�r att den ska aktiveras - Max
    [SerializeField] float rocketExplosionTime; //Hur l�nge den �ker innan automatisk explosion - Max

    [SerializeField] float rocketExplodeRadius; //Radius f�r att kolla hur n�ra spelaren ska vara f�r att den ska explodera - Max

    [SerializeField] float knockBackForce; //Explosionens knockback p� spelaren - Max

    [Header("References")]
    Transform player;
    [SerializeField] ParticleSystem rocketParticles; //Partiklarna fr�n raketen
    [SerializeField] GameObject explosion; //Explosionen
    WanderingEnemy wanderingScript;
    Rigidbody rb;

    Animator anim;


    private void Awake()
    {
        wanderingScript = GetComponent<WanderingEnemy>();
        player = FindObjectOfType<BallMovement>().transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        rocketParticles.Stop(); //S� att inte raketen �r p� n�r den bara g�r runt - Max
    }

    private void Update()
    {
        if (wanderingScript.isChasingPlayer && !wanderingScript.overrideChasing && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(player.position.x, 0, player.position.z)) < rocketActivateRadius)
        {
            StartRocket(); //N�r spelaren �r tillr�ckligt n�ra s� ska den starta raketen - Max
        }

        if (rocketOn && !wanderingScript.hasDied)
        {
            RocketInUpdate();
        }
        else if(alerted)
        {
            //Roterar fienden mot spelaren medan den g�r startanimationen, annars kan den �ka �t fel h�ll - Max
            RotateTowardsPlayer(rocketRotSpeedWhileActivating);
        }

        //Om tillr�ckligt n�ra spelaren s� ska den explodera - Max
        if (rocketOn && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(player.position.x, 0, player.position.z)) < rocketExplodeRadius)
        {
            Explode();
        }
    }

    void StartRocket()
    {
        //Ser till s� att det inte blir konstigt med wanderingscript - Max
        wanderingScript.overrideChasing = true;

        anim.Play("RocketStart");
        alerted = true; //ist�llet f�r att starta raketen direkt s� alertar vi den - Max
        Invoke(nameof(StartMovingRocket), 1.05f);
        StartCoroutine(nameof(tilExplode)); //Tills den exploderar automatiskt - Max
    }

    void StartMovingRocket()
    {
        //Raketen s�tts faktiskt ig�ng - Max
        rocketOn = true;
        rocketParticles.Play();
        SoundManagerScript.PlaySound("RocketFiende");
    }

    void RocketInUpdate()
    {
        RotateTowardsPlayer();

        //R�r sig fram�t
        Vector3 newVel = transform.forward * rocketSpeed;
        rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
    }

    

    void RotateTowardsPlayer()
    {
        RotateTowardsPlayer(rocketRotatingSpeed);
    }

    void RotateTowardsPlayer(float rotSpeed)
    {
        //Roterar
        Vector3 lookAt = transform.InverseTransformPoint(player.position);
        lookAt.y = 0;
        lookAt = transform.TransformPoint(lookAt);

        Quaternion rotation = transform.rotation;
        transform.LookAt(lookAt, transform.up);
        Quaternion lookRotation = transform.rotation;
        transform.rotation = Quaternion.RotateTowards(rotation, lookRotation, rotSpeed * Time.deltaTime);
    }

    //Exploderar automatiskt efter ett tag - Max
    IEnumerator tilExplode()
    {
        yield return new WaitForSeconds(rocketExplosionTime);
        Explode();
    }

    void Explode()
    {
        //Kollar om tillr�ckligt n�ra spelaren f�r att skada den - Max
        if (Vector3.Distance(transform.position, player.position) < rocketExplodeRadius)
        {
            //f�r fram vector riktning och l�ngd
            Vector3 dir = player.position - transform.position;

            //Normalizar s� att det �r bara riktning
            dir = dir.normalized;

            //Ger knockback till spelaren, 
            //TakeDamage har en knockback variabel men den kan inte g�ra p� y-axeln, 
            //s� d�rf�r g�r jag det p� rigidbodyns velocity ist�llet - Max
            player.GetComponent<Rigidbody>().velocity = new Vector3(0,20,0) + dir * knockBackForce;

            //Skadar spelaren - Max
            FindObjectOfType<BallHealth>().TakeDamage(Vector3.zero, 1);
        }

        Transform newExplosion = Instantiate(explosion, transform.position, Quaternion.identity).transform;
        Destroy(newExplosion.GetChild(0).gameObject, 0.2f);
        Destroy(newExplosion.gameObject, 1);

        //Ingen fade s� anv�nnder die now - Max
        wanderingScript.DieNow();
    }
}
