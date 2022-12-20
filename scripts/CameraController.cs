using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Vector3 normal = new Vector3(0f, 0f, -15f);
    private float tiempoSuave = 0.25f;
    private Vector3 velocidad = Vector3.zero;

    [SerializeField]private Transform player;

    void Update()
    {
        Vector3 targetedPosition = player.position + normal;
        transform.position = Vector3.SmoothDamp(transform.position, targetedPosition, ref velocidad, tiempoSuave);
    }
}
