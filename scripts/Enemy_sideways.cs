using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_sideways : MonoBehaviour
{
    [SerializeField] private float danno;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<vida>().takesDamage(danno);
        }
    }
}
