
using UnityEngine;

public class PowerUpDisparoTriple : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 1.5f;

    [Header("Eliminación")]
    public float margenEliminacion = 2f;

    [Header("Audio")]
    public AudioClip sonidoRecarga;

    [Range(0f, 1f)]
    public float volumenRecarga = 0.7f;

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
        transform.Translate(
            Vector3.down * velocidad * Time.deltaTime,
            Space.World
        );

        // Eliminarlo cuando salga de la pantalla.
        if (camaraPrincipal != null)
        {
            float limiteInferior =
                camaraPrincipal.transform.position.y -
                camaraPrincipal.orthographicSize;

            if (transform.position.y <
                limiteInferior - margenEliminacion)
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

        // Buscar el controlador de la nave.
        NaveController nave =
            otro.GetComponentInParent<NaveController>();

        if (nave == null || !nave.isActiveAndEnabled)
        {
            return;
        }

        recogido = true;

        // Activar el disparo triple.
        nave.ActivarDisparoTriple();

        // Reproducir sonido de recarga.
        if (sonidoRecarga != null)
        {
            Vector3 posicionAudio =
                camaraPrincipal != null
                ? camaraPrincipal.transform.position
                : transform.position;

            AudioSource.PlayClipAtPoint(
                sonidoRecarga,
                posicionAudio,
                volumenRecarga
            );
        }

        Debug.Log(
            "¡Power-Up recogido! Disparo triple activado."
        );

        // Eliminar la cápsula.
        Destroy(gameObject);
    }
}
