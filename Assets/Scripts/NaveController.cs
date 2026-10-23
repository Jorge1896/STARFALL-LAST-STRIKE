
using UnityEngine;
using UnityEngine.InputSystem;

public class NaveController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;

    [Header("Dash lateral")]
    public float velocidadDash = 18f;
    public float duracionDash = 0.18f;
    public float recargaDash = 1.5f;

    [Header("Efecto visual del Dash")]
    public TrailRenderer estelaDash;

    [Header("Audio del Dash")]
    public AudioClip sonidoDash;

    [Range(0f, 1f)]
    public float volumenDash = 0.7f;

    [Header("Disparo")]
    public GameObject prefabBala;
    public Transform puntoDisparo;
    public float tiempoEntreDisparos = 0.2f;

    [Header("Power-Up de disparo doble")]
    public bool disparoDoble = false;
    public float separacionBalas = 0.35f;

    [Header("Power-Up de disparo triple")]
    public bool disparoTriple = false;
    public float separacionBalasTriple = 0.45f;

    [Header("Texto flotante de vida")]
    public GameObject prefabTextoVidaFlotante;
    public Vector3 desplazamientoTextoVida =
        new Vector3(0f, 1f, 0f);

    [Header("Audio")]
    public AudioClip sonidoDisparo;

    private AudioSource audioSource;
    private float proximoDisparo;

    private Rigidbody2D rb;
    private Vector2 movimiento;
    private Camera camaraPrincipal;
    private SpriteRenderer spriteRenderer;
    private PlayerHealth vidaJugador;

    private bool realizandoDash = false;
    private float direccionDash = 0f;
    private float tiempoFinDash = 0f;
    private float proximoDash = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        camaraPrincipal = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        vidaJugador = GetComponent<PlayerHealth>();

        if (estelaDash == null)
        {
            estelaDash = GetComponent<TrailRenderer>();
        }

        if (estelaDash != null)
        {
            estelaDash.emitting = false;
            estelaDash.Clear();
        }

        if (vidaJugador == null)
        {
            Debug.LogWarning(
                "La nave no tiene el componente PlayerHealth."
            );
        }
    }

    public void OnMover(InputValue valor)
    {
        movimiento = valor.Get<Vector2>().normalized;
    }

    public void OnDash(InputValue valor)
    {
        if (!valor.isPressed ||
            realizandoDash ||
            Time.time < proximoDash ||
            Mathf.Abs(movimiento.x) < 0.01f)
        {
            return;
        }

        direccionDash = Mathf.Sign(movimiento.x);
        realizandoDash = true;

        tiempoFinDash = Time.time + duracionDash;
        proximoDash = tiempoFinDash + recargaDash;

        if (vidaJugador != null)
        {
            vidaJugador.EstablecerProteccionDash(true);
        }

        if (estelaDash != null)
        {
            estelaDash.Clear();
            estelaDash.emitting = true;
        }

        if (audioSource != null && sonidoDash != null)
        {
            audioSource.PlayOneShot(
                sonidoDash,
                volumenDash
            );
        }

        Debug.Log(
            direccionDash < 0f
                ? "¡Dash protegido hacia la izquierda!"
                : "¡Dash protegido hacia la derecha!"
        );
    }

    public void OnDisparar(InputValue valor)
    {
        if (!valor.isPressed ||
            Time.time < proximoDisparo)
        {
            return;
        }

        if (prefabBala == null || puntoDisparo == null)
        {
            Debug.LogWarning(
                "Falta asignar la bala o el punto de disparo."
            );
            return;
        }

        if (disparoTriple)
        {
            DispararTriple();
        }
        else if (disparoDoble)
        {
            DispararDoble();
        }
        else
        {
            DispararNormal();
        }

        if (audioSource != null &&
            sonidoDisparo != null)
        {
            audioSource.PlayOneShot(sonidoDisparo);
        }

        proximoDisparo =
            Time.time + tiempoEntreDisparos;
    }

    private void DispararNormal()
    {
        Instantiate(
            prefabBala,
            puntoDisparo.position,
            puntoDisparo.rotation
        );
    }

    private void DispararDoble()
    {
        Instantiate(
            prefabBala,
            puntoDisparo.position +
                Vector3.left * separacionBalas,
            puntoDisparo.rotation
        );

        Instantiate(
            prefabBala,
            puntoDisparo.position +
                Vector3.right * separacionBalas,
            puntoDisparo.rotation
        );
    }

    private void DispararTriple()
    {
        Instantiate(
            prefabBala,
            puntoDisparo.position +
                Vector3.left * separacionBalasTriple,
            puntoDisparo.rotation
        );

        Instantiate(
            prefabBala,
            puntoDisparo.position,
            puntoDisparo.rotation
        );

        Instantiate(
            prefabBala,
            puntoDisparo.position +
                Vector3.right * separacionBalasTriple,
            puntoDisparo.rotation
        );
    }

    private void RecompensarMejoraRepetida()
    {
        if (GameManager.instancia == null)
        {
            Debug.LogWarning(
                "No se encontró GameManager."
            );
            return;
        }

        bool vidaRecuperada =
            GameManager.instancia.RecuperarVida();

        if (vidaRecuperada)
        {
            MostrarTextoVida();

            Debug.Log(
                "¡MEJORA REPETIDA! +1 VIDA."
            );
        }
        else
        {
            Debug.Log(
                "Mejora repetida: no se recuperó vida."
            );
        }
    }

    private void MostrarTextoVida()
    {
        if (prefabTextoVidaFlotante == null)
        {
            Debug.LogWarning(
                "Falta asignar TextoVidaFlotante."
            );
            return;
        }

        Vector3 posicionTexto =
            transform.position + desplazamientoTextoVida;

        Instantiate(
            prefabTextoVidaFlotante,
            posicionTexto,
            Quaternion.identity
        );
    }

    public void ActivarDisparoDoble()
    {
        if (disparoDoble || disparoTriple)
        {
            RecompensarMejoraRepetida();
            return;
        }

        disparoDoble = true;

        Debug.Log(
            "¡Power-Up activado! Disparo doble."
        );
    }

    public void ActivarDisparoTriple()
    {
        if (disparoTriple)
        {
            RecompensarMejoraRepetida();
            return;
        }

        disparoTriple = true;
        disparoDoble = false;

        Debug.Log(
            "¡Power-Up activado! Disparo triple."
        );
    }

    // NUEVO: Reduce un nivel de armamento al perder una vida.
    public void ReducirNivelDisparo()
    {
        if (disparoTriple)
        {
            disparoTriple = false;
            disparoDoble = true;

            Debug.Log(
                "¡NAVE DAÑADA! Disparo triple reducido a doble."
            );
        }
        else if (disparoDoble)
        {
            disparoDoble = false;

            Debug.Log(
                "¡NAVE DAÑADA! Disparo doble reducido a normal."
            );
        }
        else
        {
            Debug.Log(
                "¡NAVE DAÑADA! Se mantiene el disparo normal."
            );
        }
    }

    private void FixedUpdate()
    {
        if (realizandoDash)
        {
            if (Time.time >= tiempoFinDash)
            {
                FinalizarDash();
            }
            else
            {
                rb.linearVelocity = new Vector2(
                    direccionDash * velocidadDash,
                    0f
                );

                return;
            }
        }

        rb.linearVelocity = movimiento * velocidad;
    }

    private void FinalizarDash()
    {
        realizandoDash = false;

        if (vidaJugador != null)
        {
            vidaJugador.EstablecerProteccionDash(false);
        }

        if (estelaDash != null)
        {
            estelaDash.emitting = false;
        }
    }

    private void OnDisable()
    {
        realizandoDash = false;

        if (vidaJugador != null)
        {
            vidaJugador.EstablecerProteccionDash(false);
        }

        if (estelaDash != null)
        {
            estelaDash.emitting = false;
            estelaDash.Clear();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void LateUpdate()
    {
        if (camaraPrincipal == null ||
            spriteRenderer == null)
        {
            return;
        }

        Vector3 posicion = transform.position;

        float mitadAlto =
            camaraPrincipal.orthographicSize;

        float mitadAncho =
            mitadAlto * camaraPrincipal.aspect;

        Vector3 centroCamara =
            camaraPrincipal.transform.position;

        Vector3 tamanoNave =
            spriteRenderer.bounds.extents;

        float minimoX =
            centroCamara.x - mitadAncho + tamanoNave.x;

        float maximoX =
            centroCamara.x + mitadAncho - tamanoNave.x;

        float minimoY =
            centroCamara.y - mitadAlto + tamanoNave.y;

        float maximoY =
            centroCamara.y + mitadAlto - tamanoNave.y;

        if (minimoX > maximoX ||
            minimoY > maximoY)
        {
            return;
        }

        posicion.x = Mathf.Clamp(
            posicion.x,
            minimoX,
            maximoX
        );

        posicion.y = Mathf.Clamp(
            posicion.y,
            minimoY,
            maximoY
        );

        transform.position = posicion;
    }
}
