

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicaZonas : MonoBehaviour
{
    [Header("Musica de cada zona")]
    public AudioClip musicaZona1;
    public AudioClip musicaZona2;
    public AudioClip musicaZona3;
    public AudioClip musicaZona4;
    public AudioClip musicaJefeFinal;

    [Header("Musica de pantallas finales")]
    public AudioClip musicaGameOver;
    public AudioClip musicaVictoria;

    [Header("Puntuacion para cambiar de zona")]
    public int puntosZona2 = 5000;
    public int puntosZona3 = 15000;
    public int puntosZona4 = 30000;
    public int puntosJefeFinal = 40000;

    [Header("Configuracion de audio")]
    [Range(0f, 1f)]
    public float volumenMusica = 0.35f;

    [Min(0f)]
    public float duracionFundido = 1.5f;

    private AudioSource reproductor;
    private int zonaActual;
    private Coroutine transicionActual;
    private bool musicaFinalActivada = false;

    private void Awake()
    {
        reproductor = GetComponent<AudioSource>();

        reproductor.playOnAwake = false;
        reproductor.loop = true;
        reproductor.spatialBlend = 0f;
        reproductor.volume = volumenMusica;
        reproductor.ignoreListenerPause = true;
    }

    private void Start()
    {
        zonaActual = ObtenerZonaActual();

        AudioClip cancionInicial = ObtenerMusica(zonaActual);

        if (cancionInicial != null)
        {
            reproductor.clip = cancionInicial;
            reproductor.Play();
        }
        else
        {
            Debug.LogWarning(
                "MusicaZonas: no hay musica asignada para la zona inicial."
            );
        }
    }

    private void Update()
    {
        if (musicaFinalActivada)
        {
            return;
        }

        int nuevaZona = ObtenerZonaActual();

        if (nuevaZona != zonaActual)
        {
            zonaActual = nuevaZona;

            AudioClip nuevaCancion = ObtenerMusica(zonaActual);

            if (nuevaCancion != null)
            {
                IniciarTransicion(nuevaCancion);
            }
        }
    }

    private int ObtenerZonaActual()
    {
        if (GameManager.instancia == null)
        {
            return 1;
        }

        int puntos = GameManager.instancia.puntuacion;

        if (puntos >= puntosJefeFinal)
            return 5;

        if (puntos >= puntosZona4)
            return 4;

        if (puntos >= puntosZona3)
            return 3;

        if (puntos >= puntosZona2)
            return 2;

        return 1;
    }

    private AudioClip ObtenerMusica(int zona)
    {
        switch (zona)
        {
            case 1: return musicaZona1;
            case 2: return musicaZona2;
            case 3: return musicaZona3;
            case 4: return musicaZona4;
            case 5: return musicaJefeFinal;
            default: return musicaZona1;
        }
    }

    public void ReproducirMusicaGameOver()
    {
        ActivarMusicaFinal(musicaGameOver, "Game Over");
    }

    public void ReproducirMusicaVictoria()
    {
        ActivarMusicaFinal(musicaVictoria, "Victoria");
    }

    private void ActivarMusicaFinal(
        AudioClip cancionFinal,
        string nombrePantalla
    )
    {
        if (musicaFinalActivada)
        {
            return;
        }

        musicaFinalActivada = true;

        if (cancionFinal == null)
        {
            Debug.LogWarning(
                "MusicaZonas: falta asignar musica de " +
                nombrePantalla
            );

            if (transicionActual != null)
            {
                StopCoroutine(transicionActual);
                transicionActual = null;
            }

            reproductor.Stop();
            return;
        }

        IniciarTransicion(cancionFinal);

        Debug.Log(
            "STARFALL: iniciando musica de " + nombrePantalla
        );
    }

    private void IniciarTransicion(AudioClip nuevaCancion)
    {
        if (transicionActual != null)
        {
            StopCoroutine(transicionActual);
            transicionActual = null;
        }

        transicionActual = StartCoroutine(
            CambiarMusicaSuavemente(nuevaCancion)
        );
    }

    private IEnumerator CambiarMusicaSuavemente(
        AudioClip nuevaCancion
    )
    {
        if (reproductor.clip == nuevaCancion)
        {
            reproductor.volume = volumenMusica;

            if (!reproductor.isPlaying)
            {
                reproductor.Play();
            }

            transicionActual = null;
            yield break;
        }

        float volumenInicial = reproductor.volume;
        float tiempo = 0f;

        if (duracionFundido > 0f)
        {
            while (tiempo < duracionFundido)
            {
                tiempo += Time.unscaledDeltaTime;

                float progreso = Mathf.Clamp01(
                    tiempo / duracionFundido
                );

                reproductor.volume = Mathf.Lerp(
                    volumenInicial,
                    0f,
                    progreso
                );

                yield return null;
            }
        }

        reproductor.volume = 0f;
        reproductor.Stop();
        reproductor.clip = nuevaCancion;
        reproductor.Play();

        tiempo = 0f;

        if (duracionFundido > 0f)
        {
            while (tiempo < duracionFundido)
            {
                tiempo += Time.unscaledDeltaTime;

                float progreso = Mathf.Clamp01(
                    tiempo / duracionFundido
                );

                reproductor.volume = Mathf.Lerp(
                    0f,
                    volumenMusica,
                    progreso
                );

                yield return null;
            }
        }

        reproductor.volume = volumenMusica;
        transicionActual = null;

        Debug.Log(
            "STARFALL: musica cambiada a " +
            nuevaCancion.name
        );
    }
}
