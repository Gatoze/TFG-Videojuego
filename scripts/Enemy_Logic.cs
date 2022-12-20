using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Logic : MonoBehaviour
{
    #region Public Variables
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance; //distancia minima para el ataque 
    public float movementSpeed; //Velocidad de movimiento 
    public float timer; // tiempo de espera entre ataques
    public Transform leftlimit; //limite de movimiento a la izquierda
    public Transform rightlimit; //limite de movimiento a la derecha 
    #endregion

    #region Private variables
    private RaycastHit2D hit;
    private Transform target; //Objetivo, en este caso el componente que controla el moviemiento del personaje principal
    private Animator anim; // Animador
    private float distance;
    private float inTimer;
    private bool attackMode;
    private bool inRange;
    private bool cooling;
    #endregion

    private void Awake()
    {
      
        SelectTarget();
        inTimer = timer; // Guarda el valor inicial de timer
        anim = GetComponent<Animator>();
 
    }
    // Update is called once per frame
    void Update()
    {
        //Patrullar
        if (!attackMode)
        {
            Move();
        }
        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attacking"))
        {
            SelectTarget();
        }

        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, transform.right, rayCastLength, raycastMask);
            RaycastDebbugger();
        }

        //Cuando el jugador es detectado
        if(hit.collider != null)
        {
            EnemyLogic();
            
        }
        else if (hit.collider == null)
        {
            inRange = false;
        }

        if (inRange == false)
        {
            StopAttack();
        }
    }

    //Al colisionar con el personaje principal 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            target = collision.transform;
            inRange = true;
            Flip();
        }
    }
 
    // Movimiento y ataque
    private void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            StopAttack();
        }
        else if (attackDistance >= distance && cooling == false)
        {
            Attack();
        }

        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
        }
    }

    //Movimiento 
    void Move()
    {
        anim.SetBool("canWalk", true);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attacking"))
        {
            Vector2 targetedPosition = new Vector2(target.position.x, transform.position.y);

            Flip();

            transform.position = Vector2.MoveTowards(transform.position, targetedPosition, movementSpeed * Time.deltaTime);

        }

    }

    //Ataques
    void Attack()
    {
        timer = inTimer; //Resetea el timer cuando el jugadir entra en rango de ataque 
        attackMode = true; //Hace check para ver si el enemigo se puede atacar o no

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }
    
    
    void Cooldown() // Tiempo de espera entre ataque y ataque
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = inTimer;
        }
    }
    //Parar el ataque 
    void StopAttack()
    {
        cooling = false;
        timer = inTimer;
        attackMode = false;

        anim.SetBool("canWalk", true);
        anim.SetBool("Attack", false);
    }
  

   //Para comprobar donde se encuentra el enemigo 
    void RaycastDebbugger()
    {
        if ( distance > attackDistance)
        {
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.red);
        }
        else if(attackDistance > distance)
        {
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.green);
        }
    }

    public void TriggerCooling()
    {
        cooling = true;
    }

    private bool InsideOfLimits()
    {
        return transform.position.x > leftlimit.position.x && transform.position.x < rightlimit.position.x;
    }
    // Seleccionar al enemigo como objetivo 
    private void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftlimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightlimit.position);

        if(distanceToLeft > distanceToRight)
        {
            target = leftlimit;
        }
        else
        {
            target = rightlimit;
        }

        Flip();
    }
    //Dar la vuelta 
    void Flip()
    {

        Vector3 rotation = transform.eulerAngles;
        if(transform.position.x > target.position.x)
        {
            rotation.y = 0;

        }
        else
        {
            rotation.y = -180;
        }
        transform.eulerAngles = rotation;
    }
}
