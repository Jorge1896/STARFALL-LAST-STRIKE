
using UnityEngine;

public class PowerUpEscudo : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadCaida = 2f;

    [Header("Destrucción fuera de pantalla")]
    public float margenInferior = 1f;

    [Header("Audio de activación")]
    public AudioClip sonidoEscudo;

    [Range(0f, 1f)]
    public float volumenEscudo = 0.7f;

    private Camera camaraPrincipal;
    private bool recogido = false;

    private void Start()
    {
        camaraPrincipal = Camera.main;
    }

    private void Update()
    {
        if (recogido)
        {
            return;
        }

        // Mover el power-up hacia abajo.
        transform.position += Vector3.down *
                              velocidadCaida *
                              Time.deltaTime;

        // Eliminarlo cuando salga de la pantalla.
        if (camaraPrincipal != null)
        {
            float limiteInferior =
                camaraPrincipal.transform.position.y -
                camaraPrincipal.orthographicSize -
                margenInferior;

            if (transform.position.y < limiteInferior)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (recogido)
        {
            return;
        }

        PlayerHealth jugador =
            otro.GetComponentInParent<PlayerHealth>();

        if (jugador == null || !jugador.isActiveAndEnabled)
        {
            return;
        }

        recogido = true;

        // Activar los 8 segundos de protección.
        jugador.ActivarEscudo();

        // Reproducir sonido de activación.
        if (sonidoEscudo != null)
        {
            Vector3 posicionAudio =
                camaraPrincipal != null
                ? camaraPrincipal.transform.position
                : transform.position;

            AudioSource.PlayClipAtPoint(
                sonidoEscudo,
                posicionAudio,
                volumenEscudo
            );
        }

        Debug.Log("¡Power-Up de escudo recogido!");

        Destroy(gameObject);
    }
}
