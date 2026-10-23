
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 10f;

    [Header("Tiempo máximo de vida")]
    public float tiempoDeVida = 3f;

    [Header("Límite de pantalla")]
    public float margenSuperior = 0f;

    private Rigidbody2D rb;
    private Camera camaraPrincipal;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        camaraPrincipal = Camera.main;

        // Mantener el movimiento original.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.up * velocidad;
        }

        // Seguridad: destruir la bala si permanece
        // demasiado tiempo en la escena.
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        if (camaraPrincipal == null)
        {
            return;
        }

        // Obtener el límite superior visible
        // de la cámara, considerando su posición.
        float limiteSuperior =
            camaraPrincipal.transform.position.y +
            camaraPrincipal.orthographicSize;

        // Calcular la mitad de la altura de la bala.
        float mitadAlturaBala = 0f;

        if (spriteRenderer != null)
        {
            mitadAlturaBala =
                spriteRenderer.bounds.extents.y;
        }

        // Destruir cuando la punta de la bala
        // alcanza el borde superior visible.
        float puntaBala =
            transform.position.y + mitadAlturaBala;

        if (puntaBala >= limiteSuperior - margenSuperior)
        {
            Destroy(gameObject);
        }
    }
}
