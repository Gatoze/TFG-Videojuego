using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class vida : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private float jumpforce = 10f;
    public float currentHealth { get; private set;  }

    private Animator anim;
    private Rigidbody2D rb;
    private bool death;


    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void takesDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        
        if (currentHealth > 0f)
        {
            //jugador herido
            anim.SetTrigger("danio");
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        }
        else
        {
            //jugador muerto 
            if (!death)
            {
               
                anim.SetTrigger("muerte");
                GetComponent<PlayerMovement>().enabled = false;
                death = true;

                // ver como hacer que haya un tiempo de espera hasta el respawn
                /*
                new WaitForSecondsRealtime(4);
                resetLevcel();
                */
            }
            
        }
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    private void resetLevcel()
    {
        // recargar el nivel con la escena que hay para volver a resetear
        //se usa '.name' por que el metodo LoadScene necesita el nombre de la escena

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
