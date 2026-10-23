
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 5f;
    public float tiempoVida = 5f;

    [Header("Dirección del disparo")]
    public Vector2 direccion = Vector2.down;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direccion.normalized * velocidad;
    }

    // Permite que otros enemigos configuren
    // la dirección del proyectil.
    public void ConfigurarDireccion(Vector2 nuevaDireccion)
    {
        if (nuevaDireccion.sqrMagnitude > 0.001f)
        {
            direccion = nuevaDireccion.normalized;
        }
        else
        {
            direccion = Vector2.down;
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            Debug.Log("¡La bala enemiga impactó al jugador!");

            Destroy(gameObject);
        }
    }
}
