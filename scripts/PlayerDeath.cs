using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            muerte();
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void muerte()
    {
        //en caso de querer que el personaje no se mueva al morir
        //rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("muerte"); 
    }

    private void resetearNivel()
    {
        // recargar el nivel con la escena que hay para volver a resetear
        //se usa '.name' por que el metodo LoadScene necesita el nombre de la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
