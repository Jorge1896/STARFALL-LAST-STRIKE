
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBasic : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 2f;

    [Header("Eliminación")]
    public float margenEliminacion = 2f;

    [Header("Disparo enemigo")]
    public GameObject prefabBala;
    public float tiempoEntreDisparos = 2f;
    public float distanciaDisparo = 0.6f;

    [Header("Audio")]
    public AudioClip sonidoDisparo;

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Camera camaraPrincipal;
    private float temporizadorDisparo;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        camaraPrincipal = Camera.main;
        audioSource = GetComponent<AudioSource>();

        rb.gravityScale = 0f;
    }

    void Start()
    {
        temporizadorDisparo = tiempoEntreDisparos;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Vector2.down * velocidad;
    }

    void Update()
    {
        if (camaraPrincipal == null)
            return;

        float mitadAlto = camaraPrincipal.orthographicSize;
        float mitadAncho = mitadAlto * camaraPrincipal.aspect;

        Vector3 centro = camaraPrincipal.transform.position;

        bool dentroPantalla =
            transform.position.x > centro.x - mitadAncho &&
            transform.position.x < centro.x + mitadAncho &&
            transform.position.y > centro.y - mitadAlto &&
            transform.position.y < centro.y + mitadAlto;

        if (dentroPantalla)
        {
            temporizadorDisparo -= Time.deltaTime;

            if (temporizadorDisparo <= 0f)
            {
                Disparar();
                temporizadorDisparo =
                    Mathf.Max(0.1f, tiempoEntreDisparos);
            }
        }

        float limiteInferior = centro.y - mitadAlto;

        if (transform.position.y <
            limiteInferior - margenEliminacion)
        {
            Destroy(gameObject);
        }
    }

    void Disparar()
    {
        if (prefabBala == null)
            return;

        Vector3 posicionDisparo =
            transform.position +
            Vector3.down * distanciaDisparo;

        Instantiate(
            prefabBala,
            posicionDisparo,
            Quaternion.identity
        );

        // Reproducir sonido del láser enemigo
        if (audioSource != null && sonidoDisparo != null)
        {
            audioSource.PlayOneShot(sonidoDisparo);
        }
    }
}
