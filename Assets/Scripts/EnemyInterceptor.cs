
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyInterceptor : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 2.5f;
    public float amplitudZigzag = 1.2f;
    public float frecuenciaZigzag = 2f;

    [Header("Eliminación")]
    public float margenEliminacion = 2f;

    [Header("Disparo doble")]
    public GameObject prefabBala;
    public float tiempoEntreDisparos = 1.8f;
    public float distanciaDisparo = 0.6f;
    public float separacionBalas = 0.5f;

    [Header("Audio")]
    public AudioClip sonidoDisparo;

    private Rigidbody2D rb;
    private Camera camaraPrincipal;
    private AudioSource audioSource;

    private float temporizadorDisparo;
    private float tiempoMovimiento;
    private float posicionXInicial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        camaraPrincipal = Camera.main;
        audioSource = GetComponent<AudioSource>();

        rb.gravityScale = 0f;
    }

    private void Start()
    {
        posicionXInicial = rb.position.x;
        temporizadorDisparo = tiempoEntreDisparos;
    }

    private void FixedUpdate()
    {
        // Movimiento horizontal en zigzag.
        tiempoMovimiento += Time.fixedDeltaTime;

        float desplazamientoX = Mathf.Sin(
            tiempoMovimiento * frecuenciaZigzag
        ) * amplitudZigzag;

        // Movimiento vertical hacia abajo.
        float nuevaY =
            rb.position.y - velocidad * Time.fixedDeltaTime;

        Vector2 nuevaPosicion = new Vector2(
            posicionXInicial + desplazamientoX,
            nuevaY
        );

        rb.MovePosition(nuevaPosicion);
    }

    private void Update()
    {
        if (camaraPrincipal == null)
        {
            return;
        }

        float mitadAlto = camaraPrincipal.orthographicSize;
        float mitadAncho = mitadAlto * camaraPrincipal.aspect;

        Vector3 centro = camaraPrincipal.transform.position;

        bool dentroPantalla =
            transform.position.x > centro.x - mitadAncho &&
            transform.position.x < centro.x + mitadAncho &&
            transform.position.y > centro.y - mitadAlto &&
            transform.position.y < centro.y + mitadAlto;

        // Disparar solamente cuando está en pantalla.
        if (dentroPantalla)
        {
            temporizadorDisparo -= Time.deltaTime;

            if (temporizadorDisparo <= 0f)
            {
                Disparar();

                temporizadorDisparo = Mathf.Max(
                    0.1f,
                    tiempoEntreDisparos
                );
            }
        }

        // Eliminar enemigo al salir de la pantalla.
        float limiteInferior = centro.y - mitadAlto;

        if (transform.position.y <
            limiteInferior - margenEliminacion)
        {
            Destroy(gameObject);
        }
    }

    private void Disparar()
    {
        if (prefabBala == null)
        {
            return;
        }

        Vector3 posicionCentral =
            transform.position +
            Vector3.down * distanciaDisparo;

        Vector3 posicionIzquierda =
            posicionCentral +
            Vector3.left * (separacionBalas / 2f);

        Vector3 posicionDerecha =
            posicionCentral +
            Vector3.right * (separacionBalas / 2f);

        // Primera bala.
        Instantiate(
            prefabBala,
            posicionIzquierda,
            Quaternion.identity
        );

        // Segunda bala.
        Instantiate(
            prefabBala,
            posicionDerecha,
            Quaternion.identity
        );

        // Sonido del disparo enemigo.
        if (audioSource != null && sonidoDisparo != null)
        {
            audioSource.PlayOneShot(sonidoDisparo);
        }
    }
}
