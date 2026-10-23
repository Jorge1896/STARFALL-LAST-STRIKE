
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Transicion cinematografica")]
    public Image pantallaNegra;

    [Min(0f)]
    public float duracionTransicion = 1f;

    private bool iniciandoPartida = false;

    private void Start()
    {
        if (pantallaNegra != null)
        {
            Color color = pantallaNegra.color;
            color.a = 0f;
            pantallaNegra.color = color;

            pantallaNegra.raycastTarget = false;
        }
    }

    // Inicia el primer nivel con una transicion.
    public void Jugar()
    {
        if (iniciandoPartida)
            return;

        iniciandoPartida = true;

        Time.timeScale = 1f;

        if (pantallaNegra != null)
        {
            StartCoroutine(TransicionAlNivel());
        }
        else
        {
            Debug.LogWarning(
                "No se asigno la pantalla negra. Cargando Nivel1 directamente."
            );

            SceneManager.LoadScene("Nivel1");
        }
    }

    private IEnumerator TransicionAlNivel()
    {
        pantallaNegra.raycastTarget = true;

        Color color = pantallaNegra.color;
        color.a = 0f;
        pantallaNegra.color = color;

        float tiempo = 0f;

        while (tiempo < duracionTransicion)
        {
            tiempo += Time.unscaledDeltaTime;

            float progreso = duracionTransicion > 0f
                ? Mathf.Clamp01(tiempo / duracionTransicion)
                : 1f;

            color.a = Mathf.SmoothStep(0f, 1f, progreso);
            pantallaNegra.color = color;

            yield return null;
        }

        color.a = 1f;
        pantallaNegra.color = color;

        SceneManager.LoadScene("Nivel1");
    }

    // Cierra el juego.
    public void Salir()
    {
        Debug.Log("Saliendo de STARFALL: LAST STRIKE...");
        Application.Quit();
    }
}
