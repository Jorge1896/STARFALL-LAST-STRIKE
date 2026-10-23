
using UnityEngine;

public class PowerUpDisparoDoble : MonoBehaviour
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
        // Mover el Power-Up hacia abajo.
        transform.Translate(
            Vector3.down * velocidad * Time.deltaTime,
            Space.World
        );

        // Eliminarlo si sale por debajo de la pantalla.
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
        // Evitar recoger el mismo Power-Up varias veces.
        if (recogido)
        {
            return;
        }

        // Comprobar si el objeto que lo recoge
        // es la nave del jugador.
        NaveController nave =
            otro.GetComponent<NaveController>();

        if (nave != null)
        {
            recogido = true;

            // Activar el disparo doble.
            nave.ActivarDisparoDoble();

            // Reproducir el sonido de recarga.
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
                "¡Power-Up recogido! Disparo doble activado."
            );

            // Destruir la cápsula.
            Destroy(gameObject);
        }
    }
}
