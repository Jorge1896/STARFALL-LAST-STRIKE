
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EfectoDanoPantalla : MonoBehaviour
{
    [Header("Imagen del destello")]
    public Image imagenDano;

    [Header("Configuracion del efecto")]
    [Range(0f, 1f)]
    public float intensidadMaxima = 0.35f;

    public float duracionAparicion = 0.08f;
    public float duracionDesaparicion = 0.35f;

    private Coroutine rutinaActual;

    private void Awake()
    {
        if (imagenDano == null)
        {
            imagenDano = GetComponent<Image>();
        }

        CambiarAlpha(0f);
    }

    public void MostrarDano()
    {
        if (imagenDano == null)
            return;

        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
        }

        rutinaActual = StartCoroutine(AnimarDestello());
    }

    private IEnumerator AnimarDestello()
    {
        float tiempo = 0f;

        // Aparicion rapida del destello
        while (tiempo < duracionAparicion)
        {
            tiempo += Time.unscaledDeltaTime;

            float progreso = Mathf.Clamp01(
                tiempo / Mathf.Max(0.001f, duracionAparicion)
            );

            CambiarAlpha(
                Mathf.Lerp(0f, intensidadMaxima, progreso)
            );

            yield return null;
        }

        CambiarAlpha(intensidadMaxima);

        tiempo = 0f;

        // Desaparicion gradual
        while (tiempo < duracionDesaparicion)
        {
            tiempo += Time.unscaledDeltaTime;

            float progreso = Mathf.Clamp01(
                tiempo / Mathf.Max(0.001f, duracionDesaparicion)
            );

            CambiarAlpha(
                Mathf.Lerp(intensidadMaxima, 0f, progreso)
            );

            yield return null;
        }

        CambiarAlpha(0f);
        rutinaActual = null;
    }

    private void CambiarAlpha(float alpha)
    {
        if (imagenDano == null)
            return;

        Color color = imagenDano.color;
        color.a = alpha;
        imagenDano.color = color;
    }

    private void OnDisable()
    {
        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
            rutinaActual = null;
        }

        CambiarAlpha(0f);
    }
}
