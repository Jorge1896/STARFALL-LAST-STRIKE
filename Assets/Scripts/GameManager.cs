
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Puntuación")]
    public int puntuacion = 0;

    [Header("Interfaz")]
    public TMP_Text textoPuntuacion;

    [Header("Sistema de vidas")]
    public int vidas = 3;
    public TMP_Text textoVidas;

    [Min(1)]
    public int maximoVidas = 3;

    [Header("Game Over")]
    public GameObject panelGameOver;

    [Header("Explosión final")]
    public float retrasoGameOver = 0.65f;

    [Header("Victoria")]
    public GameObject panelVictoria;

    [Min(0f)]
    public float retrasoVictoria = 1.5f;

    [Header("Sistema musical")]
    public MusicaZonas musicaZonas;

    private bool juegoTerminado = false;
    private bool finalizandoJuego = false;
    private bool victoriaEnProceso = false;

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        juegoTerminado = false;
        finalizandoJuego = false;
        victoriaEnProceso = false;

        maximoVidas = Mathf.Max(1, maximoVidas);
        vidas = Mathf.Clamp(vidas, 0, maximoVidas);

        if (musicaZonas == null)
        {
            musicaZonas = FindAnyObjectByType<MusicaZonas>();
        }

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(false);
        }

        ActualizarInterfaz();
    }

    public void SumarPuntos(int puntos)
    {
        if (juegoTerminado || finalizandoJuego)
        {
            return;
        }

        puntuacion += puntos;
        ActualizarInterfaz();
    }

    public void PerderVida()
    {
        if (juegoTerminado || finalizandoJuego)
        {
            return;
        }

        vidas = Mathf.Max(0, vidas - 1);

        ActualizarInterfaz();

        if (vidas <= 0)
        {
            finalizandoJuego = true;
            StartCoroutine(EsperarExplosionFinal());
        }
    }

    // Recuperar una vida sin superar el máximo.
    public bool RecuperarVida()
    {
        if (juegoTerminado ||
            finalizandoJuego ||
            victoriaEnProceso ||
            vidas <= 0)
        {
            return false;
        }

        if (vidas >= maximoVidas)
        {
            Debug.Log("Ya tienes el máximo de vidas.");
            return false;
        }

        vidas = Mathf.Min(vidas + 1, maximoVidas);

        ActualizarInterfaz();

        Debug.Log(
            "¡VIDA RECUPERADA! Vidas actuales: " +
            vidas + "/" + maximoVidas
        );

        return true;
    }

    private IEnumerator EsperarExplosionFinal()
    {
        yield return new WaitForSeconds(retrasoGameOver);

        if (!victoriaEnProceso)
        {
            GameOver();
        }
    }

    private void ActualizarInterfaz()
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = "SCORE: " + puntuacion;
        }

        if (textoVidas != null)
        {
            textoVidas.text = "VIDAS: " + vidas;
        }
    }

    public void GameOver()
    {
        if (juegoTerminado || victoriaEnProceso)
        {
            return;
        }

        juegoTerminado = true;
        finalizandoJuego = true;

        Debug.Log("GAME OVER");

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }

        if (musicaZonas != null)
        {
            musicaZonas.ReproducirMusicaGameOver();
        }
        else
        {
            Debug.LogWarning(
                "GameManager: no se encontró MusicaZonas para Game Over."
            );
        }

        Time.timeScale = 0f;
    }

    public void Victoria()
    {
        if (juegoTerminado ||
            finalizandoJuego ||
            victoriaEnProceso)
        {
            return;
        }

        victoriaEnProceso = true;
        finalizandoJuego = true;

        Debug.Log(
            "¡DREADNOUGHT DESTRUIDO! MISIÓN COMPLETADA."
        );

        StartCoroutine(EsperarVictoria());
    }

    private IEnumerator EsperarVictoria()
    {
        yield return new WaitForSeconds(retrasoVictoria);

        if (juegoTerminado)
        {
            yield break;
        }

        juegoTerminado = true;

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "GameManager: falta asignar PanelVictoria."
            );
        }

        if (musicaZonas != null)
        {
            musicaZonas.ReproducirMusicaVictoria();
        }
        else
        {
            Debug.LogWarning(
                "GameManager: no se encontró MusicaZonas para Victoria."
            );
        }

        Debug.Log("¡MISIÓN COMPLETADA!");

        Time.timeScale = 0f;
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene("MenuPrincipal");
    }
}
