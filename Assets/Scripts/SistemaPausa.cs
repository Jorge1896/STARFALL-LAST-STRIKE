
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SistemaPausa : MonoBehaviour
{
    [Header("Interfaz de pausa")]
    public GameObject panelPausa;

    [Header("Pantallas de resultado")]
    public GameObject panelGameOver;
    public GameObject panelVictoria;

    private bool juegoPausado = false;

    private void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        juegoPausado = false;

        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (juegoPausado)
            {
                ContinuarMision();
            }
            else if (!HayPantallaDeResultado())
            {
                PausarJuego();
            }
        }
    }

    private bool HayPantallaDeResultado()
    {
        bool derrota = panelGameOver != null
            && panelGameOver.activeInHierarchy;

        bool victoria = panelVictoria != null
            && panelVictoria.activeInHierarchy;

        return derrota || victoria;
    }

    public void PausarJuego()
    {
        if (juegoPausado || HayPantallaDeResultado())
            return;

        juegoPausado = true;

        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (panelPausa != null)
            panelPausa.SetActive(true);
    }

    public void ContinuarMision()
    {
        if (!juegoPausado)
            return;

        juegoPausado = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    public void AbandonarMision()
    {
        juegoPausado = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene("MenuPrincipal");
    }

    private void OnDisable()
    {
        if (juegoPausado)
        {
            juegoPausado = false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}
