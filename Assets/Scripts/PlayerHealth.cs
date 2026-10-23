
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Invulnerabilidad")]
    public float tiempoInvulnerabilidad = 1.5f;
    public float intervaloParpadeo = 0.12f;

    [Header("Efecto visual de daño")]
    public Color colorDanio = Color.red;
    public float duracionDestello = 0.15f;

    [Header("Destello de daño en pantalla")]
    public EfectoDanoPantalla efectoDanoPantalla;

    [Header("Escudo temporal")]
    public float duracionEscudo = 8f;
    public Color colorEscudo = Color.cyan;

    [Header("Explosión del jugador")]
    public GameObject prefabExplosionJugador;

    [Header("Explosión por choque con enemigos")]
    public GameObject prefabExplosionChoque;

    [Header("Audio")]
    public AudioClip sonidoDanio;

    [Range(0f, 1f)]
    public float volumenDanio = 0.6f;

    private bool esInvulnerable = false;
    private bool escudoActivo = false;
    private bool estaMuerto = false;
    private bool proteccionDashActiva = false;

    private Coroutine rutinaEscudo;
    private Coroutine rutinaInvulnerabilidad;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Collider2D colisionador;
    private NaveController controladorNave;

    private Color colorOriginal;

    public bool EscudoActivo
    {
        get { return escudoActivo; }
    }

    public bool ProteccionDashActiva
    {
        get { return proteccionDashActiva; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        colisionador = GetComponent<Collider2D>();
        controladorNave = GetComponent<NaveController>();

        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }

        if (controladorNave == null)
        {
            Debug.LogWarning(
                "PlayerHealth: no se encontró NaveController."
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (estaMuerto)
        {
            return;
        }

        if (otro.CompareTag("BalaEnemiga"))
        {
            RecibirDanio();
            Destroy(otro.gameObject);
        }
        else if (otro.CompareTag("Enemigo"))
        {
            ProcesarChoqueEnemigo(
                otro.gameObject,
                otro.transform.position
            );
        }
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        if (estaMuerto)
        {
            return;
        }

        if (colision.gameObject.CompareTag("Enemigo"))
        {
            Vector3 posicionChoque =
                colision.transform.position;

            if (colision.contactCount > 0)
            {
                posicionChoque =
                    colision.GetContact(0).point;
            }

            ProcesarChoqueEnemigo(
                colision.gameObject,
                posicionChoque
            );
        }
    }

    private void ProcesarChoqueEnemigo(
        GameObject enemigo,
        Vector3 posicionChoque
    )
    {
        if (enemigo == null)
        {
            return;
        }

        // Aplicar daño respetando las protecciones.
        RecibirDanio();

        // Nunca destruir al jefe por contacto.
        if (enemigo.GetComponentInParent<BossDreadnought>() != null)
        {
            return;
        }

        // Crear explosión visual del enemigo.
        if (prefabExplosionChoque != null)
        {
            posicionChoque.z = 0f;

            Instantiate(
                prefabExplosionChoque,
                posicionChoque,
                Quaternion.identity
            );
        }

        // Destruir únicamente al enemigo normal.
        Destroy(enemigo);
    }

    public void EstablecerProteccionDash(bool activar)
    {
        if (estaMuerto)
        {
            proteccionDashActiva = false;
            return;
        }

        proteccionDashActiva = activar;

        if (activar)
        {
            Debug.Log("Protección de Dash activada.");
        }
        else
        {
            Debug.Log("Protección de Dash desactivada.");
        }
    }

    public void ActivarEscudo()
    {
        if (estaMuerto)
        {
            return;
        }

        if (rutinaEscudo != null)
        {
            StopCoroutine(rutinaEscudo);
        }

        escudoActivo = true;

        if (rutinaInvulnerabilidad != null)
        {
            StopCoroutine(rutinaInvulnerabilidad);
            rutinaInvulnerabilidad = null;
            esInvulnerable = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = colorEscudo;
        }

        rutinaEscudo = StartCoroutine(
            TemporizadorEscudo()
        );

        Debug.Log("¡ESCUDO ACTIVADO!");
    }

    private IEnumerator TemporizadorEscudo()
    {
        yield return new WaitForSeconds(
            Mathf.Max(0.1f, duracionEscudo)
        );

        escudoActivo = false;
        rutinaEscudo = null;

        if (spriteRenderer != null && !estaMuerto)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = colorOriginal;
        }

        Debug.Log("Escudo desactivado.");
    }

    public void RecibirDanio()
    {
        // No aplicar daño si existe alguna protección.
        if (
            escudoActivo ||
            esInvulnerable ||
            proteccionDashActiva ||
            estaMuerto
        )
        {
            return;
        }

        if (GameManager.instancia == null)
        {
            Debug.LogWarning(
                "No se encontró el GameManager."
            );
            return;
        }

        // Descontar una vida.
        GameManager.instancia.PerderVida();

        // NUEVO: Reducir un nivel de disparo
        // únicamente cuando se pierde una vida.
        if (controladorNave != null)
        {
            controladorNave.ReducirNivelDisparo();
        }

        // Activar destello rojo de pantalla.
        if (efectoDanoPantalla != null)
        {
            efectoDanoPantalla.MostrarDano();
        }

        // Reproducir sonido de daño.
        if (audioSource != null && sonidoDanio != null)
        {
            audioSource.PlayOneShot(
                sonidoDanio,
                volumenDanio
            );
        }

        if (GameManager.instancia.vidas <= 0)
        {
            Morir();
        }
        else
        {
            rutinaInvulnerabilidad = StartCoroutine(
                ActivarInvulnerabilidad()
            );
        }
    }

    private IEnumerator ActivarInvulnerabilidad()
    {
        esInvulnerable = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = colorDanio;
        }

        float duracionRojo = Mathf.Clamp(
            duracionDestello,
            0f,
            tiempoInvulnerabilidad
        );

        yield return new WaitForSeconds(duracionRojo);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorOriginal;
        }

        float tiempoTranscurrido = duracionRojo;

        while (tiempoTranscurrido < tiempoInvulnerabilidad)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled =
                    !spriteRenderer.enabled;
            }

            float espera = Mathf.Min(
                intervaloParpadeo,
                tiempoInvulnerabilidad - tiempoTranscurrido
            );

            espera = Mathf.Max(0.01f, espera);

            yield return new WaitForSeconds(espera);

            tiempoTranscurrido += espera;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;

            spriteRenderer.color =
                escudoActivo ? colorEscudo : colorOriginal;
        }

        esInvulnerable = false;
        rutinaInvulnerabilidad = null;
    }

    private void Morir()
    {
        if (estaMuerto)
        {
            return;
        }

        estaMuerto = true;
        esInvulnerable = false;
        escudoActivo = false;
        proteccionDashActiva = false;

        if (rutinaEscudo != null)
        {
            StopCoroutine(rutinaEscudo);
            rutinaEscudo = null;
        }

        if (rutinaInvulnerabilidad != null)
        {
            StopCoroutine(rutinaInvulnerabilidad);
            rutinaInvulnerabilidad = null;
        }

        if (prefabExplosionJugador != null)
        {
            Instantiate(
                prefabExplosionJugador,
                transform.position,
                Quaternion.identity
            );
        }
        else
        {
            Debug.LogWarning(
                "No se asignó el prefab ExplosionJugador."
            );
        }

        if (controladorNave != null)
        {
            controladorNave.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (colisionador != null)
        {
            colisionador.enabled = false;
        }
    }
}
