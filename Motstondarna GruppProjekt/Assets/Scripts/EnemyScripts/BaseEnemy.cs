using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Max Script
public class BaseEnemy : MonoBehaviour
{
    [Header("Base Enemy")]

    [SerializeField] float playerVelocityForDeath; //Hur snabbt spelaren ska åka för att döda fienden - Max
    [SerializeField] float dieForce; //Den force fienden skjuts iväg från spelaren med när den dör - Max
    [SerializeField] float fadeSpeed; //Hur snabbt fienden fadear iväg efter död - Max

    [SerializeField] bool canDieFromOtherPins = false; //Om de kan dö av att andra fiender åker in i dem - Max
    [SerializeField] bool canKillPlayer = false; //Basic pins kan inte göra skada mot spelaren - Max

    public bool hasDied = false;

    //Referenser
    MeshRenderer rend;
    [HideInInspector] public Rigidbody rb; //Är public för att skript som ärver av detta också ska kunna accessa rigidbodyn - Max
    [SerializeField] GameObject deathParticle; //Man dör Particle som skapas när man dör - Max

    public virtual void Awake()
    {
        rend = GetComponentInChildren<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        OnAnyCollisionEnter(collision.collider, collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnAnyCollisionEnter(other);
    }

    //Den här funktionen callas både om spelaren collidar med en collider eller en trigger - Max
    void OnAnyCollisionEnter(Collider other, Collision collision = null)
    {
        if (hasDied) return;

        BallMovement ball = other.transform.GetComponent<BallMovement>();

        if (ball != null) //Kollar om det är spelaren man collidat med - Max
        {
            if (ball.currentSpeed.magnitude >= playerVelocityForDeath)
            {
                Die(collision != null ? collision.GetContact(0).point : other.transform.position, ball.currentSpeed);
            }
            else if (ball.currentSpeed.magnitude < playerVelocityForDeath)
            {
                rb.isKinematic = true; //Så att spelaren inte kan putta på fienden - Max

                //Använder formeln angle = point1 - point2 för att ta fram riktningen från spelaren till fienden - Max
                Vector3 dir = collision != null ? collision.GetContact(0).point : other.transform.position - transform.position;
                dir = dir.normalized; //Normalizar vinkeln den så att jag bara får vinkeln av vektorn - Max

                ball.currentSpeed = dir * 5; //Lägger på en force i direction - Max

                //De flesta käglor gör damage till spelaren om man åker in i dem för långsamt - Max
                if (canKillPlayer)
                    other.transform.GetComponent<BallHealth>().TakeDamage(Vector3.zero, 1);
            }
        }
        else //Om man inte collidade med spelaren
        {
            if (canDieFromOtherPins && other.transform.GetComponent<BaseEnemy>()) //Basic käglor kan dö av varandra när de studsar på varnadra - Max
            {
                Vector3 dir = collision.GetContact(0).point - transform.position; //Får riktingen mot contact point - Max 
                dir = -dir.normalized;  //Normalizar vinkeln så att jag bara får vinkeln av vektorn - Max

                Die();
            }
        }       
    }

    private void OnCollisionExit(Collision collision)
    {
        //Man ska kunna fortsätta röra sig efter man inte längre rör spelaren
        if (collision.transform.GetComponent<BallMovement>())
            rb.isKinematic = false;
    }

    public virtual void Die(Vector3 contactPoint, Vector3 speed) //Dör och får en dieforce (flyger iväg från spelaren) - Max
    {
        PrepareDeath();

        //Använder formeln angle = point1 - point2 för att ta fram riktningen från spelaren till fienden - Max
        Vector3 dir = contactPoint - transform.position;
        dir = speed.normalized; //Använder spelarens riktning för att skapa forcen - Max

        GetComponent<Rigidbody>().AddForceAtPosition(dir * dieForce, contactPoint); //Lägger på en force i riktingen - Max

        StartCoroutine(Fade());
    }

    public void Die() //Dör utan att få en dieforce - Max
    {
        PrepareDeath();
        StartCoroutine(Fade());
    }

    public void DieNow() //Dör direkt utan att fadea - Max
    {
        PrepareDeath();
        Destroy(gameObject);
    }

    void PrepareDeath()
    {
        hasDied = true; //Så att man inte kan dö flera gånger - Max

        SoundManagerScript.PlaySound("KägglaDamage"); //Ljudeffekt - Max
        if(deathParticle != null)
            SpawnParticles(); //Spawnar particles - Max
    }

    void SpawnParticles()
    {
        //Skapar particle - Max
        GameObject newParticle = Instantiate(deathParticle, transform.position, Quaternion.identity);

        //Förstör den efter viss tid - Max
        Destroy(newParticle, 1);
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(2); //Först väntar 2 sekunder - Max

        Color color;
        while (rend.material.color.a > 0) //Uppdaterar materialet varje frame och den får mindre och mindre alpha - Max
        {
            color = rend.material.color;
            rend.material.color = new Color(color.r, color.g, color.b, color.a - (Time.deltaTime * fadeSpeed));
            yield return null;
        }

        Destroy(gameObject); //När den är osynlig så förstörs objektet på riktigt - Max
    }
}
