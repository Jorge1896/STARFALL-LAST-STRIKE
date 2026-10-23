
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossDreadnought : MonoBehaviour
{
    [Header("Entrada del jefe")]
    public float velocidadEntrada = 1.5f;

    [Range(0.1f, 0.9f)]
    public float posicionVerticalCombate = 0.75f;

    [Header("Movimiento horizontal")]
    public float velocidadHorizontal = 1.5f;
    public float margenHorizontal = 0.5f;

    [Header("Ataque: doble cañón")]
    public GameObject prefabBala;
    public Transform canonIzquierdo;
    public Transform canonDerecho;

    [Min(0.1f)]
    public float intervaloDisparo = 1.5f;

    [Min(0.1f)]
    public float velocidadBala = 5f;

    [Header("Ataque: ráfaga")]
    [Min(1)]
    public int cantidadDisparosRafaga = 3;

    [Min(0.05f)]
    public float intervaloRafaga = 0.2f;

    [Header("Ataque: abanico")]
    [Min(1)]
    public int cantidadBalasAbanico = 5;

    [Range(0f, 160f)]
    public float anguloAbanico = 90f;

    [Header("Segunda fase")]
    [Range(0.1f, 0.9f)]
    public float porcentajeActivacionFase2 = 0.5f;

    [Min(0.1f)]
    public float multiplicadorVelocidadMovimiento = 1.5f;

    [Min(0.1f)]
    public float multiplicadorVelocidadBalas = 1.4f;

    [Min(0.1f)]
    public float multiplicadorFrecuenciaDisparo = 1.5f;

    [Header("Sonidos de ataque")]
    public AudioClip sonidoDobleCanon;
    public AudioClip sonidoRafaga;
    public AudioClip sonidoAbanico;

    [Range(0f, 1f)]
    public float volumenAtaques = 0.65f;

    [Header("Destrucción del jefe")]
    public GameObject prefabExplosionPequena;
    public GameObject prefabExplosionFinal;
    public AudioClip sonidoDestruccion;

    [Range(0f, 1f)]
    public float volumenDestruccion = 0.85f;

    [Min(0.1f)]
    public float duracionDestruccion = 2.5f;

    [Min(1)]
    public int cantidadExplosiones = 10;

    private Rigidbody2D rb;
    private Camera camaraPrincipal;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth vidaJefe;
    private AudioSource audioSource;

    private bool entradaTerminada = false;
    private bool fase2Activada = false;
    private bool realizandoAtaque = false;
    private bool destruyendose = false;

    private int direccionHorizontal = 1;
    private int siguientePatron = 0;

    private float posicionYCombate;
    private float temporizadorDisparo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        camaraPrincipal = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        vidaJefe = GetComponent<EnemyHealth>();

        rb.gravityScale = 0f;

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        if (camaraPrincipal == null)
        {
            Debug.LogError(
                "BossDreadnought: no se encontró la cámara principal."
            );
            enabled = false;
            return;
        }

        if (vidaJefe == null)
        {
            Debug.LogError(
                "BossDreadnought: falta el componente EnemyHealth."
            );
            enabled = false;
            return;
        }

        float mitadAlto = camaraPrincipal.orthographicSize;

        float limiteInferior =
            camaraPrincipal.transform.position.y - mitadAlto;

        posicionYCombate =
            limiteInferior +
            (mitadAlto * 2f * posicionVerticalCombate);

        temporizadorDisparo = intervaloDisparo;
    }

    private void FixedUpdate()
    {
        if (destruyendose || camaraPrincipal == null)
        {
            return;
        }

        if (!entradaTerminada)
        {
            EntrarEnPantalla();
        }
        else
        {
            MoverHorizontalmente();
        }
    }

    private void Update()
    {
        if (destruyendose)
        {
            return;
        }

        ComprobarSegundaFase();

        if (!entradaTerminada || realizandoAtaque)
        {
            return;
        }

        temporizadorDisparo -= Time.deltaTime;

        if (temporizadorDisparo <= 0f)
        {
            switch (siguientePatron)
            {
                case 0:
                    DispararDobleCanon();
                    break;

                case 1:
                    StartCoroutine(DispararRafaga());
                    break;

                case 2:
                    DispararAbanico();
                    break;
            }

            siguientePatron = (siguientePatron + 1) % 3;
            temporizadorDisparo = ObtenerIntervaloDisparo();
        }
    }

    private void ComprobarSegundaFase()
    {
        if (fase2Activada || vidaJefe == null)
        {
            return;
        }

        if (vidaJefe.VidaActual <= 0)
        {
            return;
        }

        if (vidaJefe.PorcentajeVida <= porcentajeActivacionFase2)
        {
            fase2Activada = true;

            temporizadorDisparo = Mathf.Min(
                temporizadorDisparo,
                ObtenerIntervaloDisparo()
            );

            Debug.Log(
                "¡DREADNOUGHT HA ACTIVADO SU SEGUNDA FASE!"
            );
        }
    }

    private float ObtenerVelocidadMovimiento()
    {
        return fase2Activada
            ? velocidadHorizontal * multiplicadorVelocidadMovimiento
            : velocidadHorizontal;
    }

    private float ObtenerVelocidadBala()
    {
        return fase2Activada
            ? velocidadBala * multiplicadorVelocidadBalas
            : velocidadBala;
    }

    private float ObtenerIntervaloDisparo()
    {
        float intervalo = Mathf.Max(0.1f, intervaloDisparo);

        if (fase2Activada)
        {
            intervalo /= Mathf.Max(
                0.1f,
                multiplicadorFrecuenciaDisparo
            );
        }

        return Mathf.Max(0.1f, intervalo);
    }

    private void EntrarEnPantalla()
    {
        Vector2 nuevaPosicion = Vector2.MoveTowards(
            rb.position,
            new Vector2(rb.position.x, posicionYCombate),
            velocidadEntrada * Time.fixedDeltaTime
        );

        rb.MovePosition(nuevaPosicion);

        if (Mathf.Abs(nuevaPosicion.y - posicionYCombate) < 0.01f)
        {
            entradaTerminada = true;

            Debug.Log(
                "DREADNOUGHT ha llegado a su posición de combate."
            );
        }
    }

    private void MoverHorizontalmente()
    {
        float mitadAlto = camaraPrincipal.orthographicSize;
        float mitadAncho = mitadAlto * camaraPrincipal.aspect;

        float centroX = camaraPrincipal.transform.position.x;

        float mitadAnchoJefe = 0f;

        if (spriteRenderer != null)
        {
            mitadAnchoJefe = spriteRenderer.bounds.extents.x;
        }

        float limiteIzquierdo =
            centroX - mitadAncho +
            mitadAnchoJefe + margenHorizontal;

        float limiteDerecho =
            centroX + mitadAncho -
            mitadAnchoJefe - margenHorizontal;

        if (limiteIzquierdo >= limiteDerecho)
        {
            return;
        }

        float nuevaX =
            rb.position.x +
            direccionHorizontal *
            ObtenerVelocidadMovimiento() *
            Time.fixedDeltaTime;

        if (nuevaX >= limiteDerecho)
        {
            nuevaX = limiteDerecho;
            direccionHorizontal = -1;
        }
        else if (nuevaX <= limiteIzquierdo)
        {
            nuevaX = limiteIzquierdo;
            direccionHorizontal = 1;
        }

        rb.MovePosition(
            new Vector2(nuevaX, posicionYCombate)
        );
    }

    private void ReproducirSonido(AudioClip sonido)
    {
        if (audioSource != null && sonido != null)
        {
            audioSource.PlayOneShot(sonido, volumenAtaques);
        }
    }

    private void DispararDobleCanon(bool reproducirAudio = true)
    {
        if (destruyendose || prefabBala == null)
        {
            return;
        }

        if (reproducirAudio)
        {
            ReproducirSonido(sonidoDobleCanon);
        }

        DispararDesdeCanon(canonIzquierdo, Vector2.down);
        DispararDesdeCanon(canonDerecho, Vector2.down);
    }

    private IEnumerator DispararRafaga()
    {
        realizandoAtaque = true;

        ReproducirSonido(sonidoRafaga);

        int cantidad = Mathf.Max(1, cantidadDisparosRafaga);

        for (int i = 0; i < cantidad; i++)
        {
            if (destruyendose)
            {
                yield break;
            }

            DispararDobleCanon(false);

            if (i < cantidad - 1)
            {
                yield return new WaitForSeconds(
                    Mathf.Max(0.05f, intervaloRafaga)
                );
            }
        }

        realizandoAtaque = false;
    }

    private void DispararAbanico()
    {
        if (destruyendose || prefabBala == null)
        {
            return;
        }

        ReproducirSonido(sonidoAbanico);

        int cantidad = Mathf.Max(1, cantidadBalasAbanico);

        for (int i = 0; i < cantidad; i++)
        {
            float angulo = cantidad == 1
                ? 0f
                : Mathf.Lerp(
                    -anguloAbanico / 2f,
                    anguloAbanico / 2f,
                    i / (float)(cantidad - 1)
                );

            Vector2 direccion =
                Quaternion.Euler(0f, 0f, angulo) *
                Vector2.down;

            CrearBala(transform.position, direccion);
        }
    }

    private void DispararDesdeCanon(
        Transform canon,
        Vector2 direccion
    )
    {
        if (canon == null)
        {
            return;
        }

        CrearBala(canon.position, direccion);
    }

    private void CrearBala(
        Vector3 posicion,
        Vector2 direccion
    )
    {
        if (destruyendose || prefabBala == null)
        {
            return;
        }

        GameObject bala = Instantiate(
            prefabBala,
            posicion,
            Quaternion.identity
        );

        EnemyBullet proyectil = bala.GetComponent<EnemyBullet>();

        if (proyectil != null)
        {
            proyectil.velocidad = ObtenerVelocidadBala();
            proyectil.ConfigurarDireccion(direccion);
        }
        else
        {
            Debug.LogWarning(
                "La bala del jefe no tiene el script EnemyBullet."
            );
        }
    }

    // Se llama cuando la vida del jefe llega a cero.
    public void IniciarDestruccion()
    {
        if (destruyendose)
        {
            return;
        }

        destruyendose = true;

        // Detener cualquier ataque en curso.
        StopAllCoroutines();
        realizandoAtaque = false;

        // Desactivar las colisiones para evitar
        // impactos adicionales durante la destrucción.
        Collider2D[] colisionadores =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D colisionador in colisionadores)
        {
            colisionador.enabled = false;
        }

        // Detener la reproducción de ataques anteriores.
        if (audioSource != null)
        {
            audioSource.Stop();

            if (sonidoDestruccion != null)
            {
                audioSource.PlayOneShot(
                    sonidoDestruccion,
                    volumenDestruccion
                );
            }
        }

        StartCoroutine(SecuenciaDestruccion());
    }

    private IEnumerator SecuenciaDestruccion()
    {
        Debug.Log(
            "DREADNOUGHT: iniciando destrucción cinematográfica."
        );

        Color colorOriginal = Color.white;

        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }

        int totalExplosiones =
            Mathf.Max(1, cantidadExplosiones);

        float duracion =
            Mathf.Max(0.1f, duracionDestruccion);

        float intervalo =
            duracion / totalExplosiones;

        for (int i = 0; i < totalExplosiones; i++)
        {
            if (prefabExplosionPequena != null)
            {
                Vector3 posicionExplosion =
                    ObtenerPosicionExplosionAleatoria();

                Instantiate(
                    prefabExplosionPequena,
                    posicionExplosion,
                    Quaternion.identity
                );
            }

            // Alternar entre rojo y el color original.
            if (spriteRenderer != null)
            {
                spriteRenderer.color =
                    i % 2 == 0
                    ? Color.red
                    : colorOriginal;
            }

            yield return new WaitForSeconds(intervalo);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorOriginal;
        }

        // Explosión final en el centro del jefe.
        if (prefabExplosionFinal != null)
        {
            Instantiate(
                prefabExplosionFinal,
                transform.position,
                Quaternion.identity
            );
        }

        // Activar la victoria antes de destruir al jefe.
        if (GameManager.instancia != null)
        {
            GameManager.instancia.Victoria();
        }

        Destroy(gameObject);
    }

    private Vector3 ObtenerPosicionExplosionAleatoria()
    {
        if (spriteRenderer == null)
        {
            return transform.position;
        }

        Bounds limites = spriteRenderer.bounds;

        float x = Random.Range(
            limites.min.x,
            limites.max.x
        );

        float y = Random.Range(
            limites.min.y,
            limites.max.y
        );

        return new Vector3(
            x,
            y,
            transform.position.z
        );
    }
}
