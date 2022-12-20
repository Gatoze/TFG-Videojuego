using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    [SerializeField] private vida playerLife;
    [SerializeField] private Image fullHealth;
    [SerializeField] private Image actualHealth;
    // Start is called before the first frame update
    void Start()
    {
        fullHealth.fillAmount = playerLife.currentHealth / 10;
    }

    // Update is called once per frame
    void Update()
    {
        actualHealth.fillAmount = playerLife.currentHealth / 10;
    }
}
