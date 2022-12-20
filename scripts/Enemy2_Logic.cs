using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy2_Logic : MonoBehaviour
{
    #region Variables
    private Transform target;
    private bool inRange;
    private float distance;
    public float movementSpeed;
    private Animator anim;
    public Transform leftlimit;
    public Transform rightlimit;
    #endregion


    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            target = collision.transform;
            inRange = true;
            Flip();
        }
    }
    private void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);
    }

    void Move()
    {
        anim.SetBool("canWalk", true);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy2_Hurt"))
        {
            Vector2 targetedPosition = new Vector2(target.position.x, transform.position.y);

            Flip();

            transform.position = Vector2.MoveTowards(transform.position, targetedPosition, movementSpeed * Time.deltaTime);

        }

    }
    void Flip()
    {

        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x)
        {
            rotation.y = 0;

        }
        else
        {
            rotation.y = -180;
        }
        transform.eulerAngles = rotation;
    }
    private void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftlimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightlimit.position);

        if (distanceToLeft > distanceToRight)
        {
            target = leftlimit;
        }
        else
        {
            target = rightlimit;
        }

        Flip();
    }
}
