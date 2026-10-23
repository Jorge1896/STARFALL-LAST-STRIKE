
using UnityEngine;
using TMPro;

public class TextoVidaFlotante : MonoBehaviour
{
    [Header("Animación")]
    public float duracion = 1f;
    public float velocidadSubida = 1.5f;

    private TMP_Text texto;
    private Color colorOriginal;
    private float tiempoTranscurrido = 0f;

    private void Awake()
    {
        texto = GetComponent<TMP_Text>();

        if (texto != null)
        {
            colorOriginal = texto.color;
        }
    }

    private void Update()
    {
        tiempoTranscurrido += Time.deltaTime;

        // Mover el mensaje suavemente hacia arriba.
        transform.position +=
            Vector3.up * velocidadSubida * Time.deltaTime;

        // Desvanecer el texto progresivamente.
        if (texto != null)
        {
            float progreso = Mathf.Clamp01(
                tiempoTranscurrido / Mathf.Max(0.01f, duracion)
            );

            Color nuevoColor = colorOriginal;
            nuevoColor.a = 1f - progreso;

            texto.color = nuevoColor;
        }

        // Eliminar el mensaje al finalizar.
        if (tiempoTranscurrido >= duracion)
        {
            Destroy(gameObject);
        }
    }
}
