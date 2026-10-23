
using UnityEngine;

public class PlayerBullet1 : MonoBehaviour
{
    [Header("Daño del proyectil")]
    [Min(1)]
    public int danio = 1;

    [Header("Power-Ups")]
    public GameObject prefabPowerUp;
    public GameObject prefabPowerUpEscudo;
    public GameObject prefabPowerUpTriple;

    [Range(0f, 100f)]
    public float probabilidadPowerUp = 20f;

    [Header("Distribución de Power-Ups")]
    [Min(0f)]
    public float pesoDisparoDoble = 40f;

    [Min(0f)]
    public float pesoEscudo = 30f;

    [Min(0f)]
    public float pesoDisparoTriple = 30f;

    [Header("Audio")]
    public AudioClip sonidoExplosion;

    [Range(0f, 1f)]
    public float volumenExplosion = 0.5f;

    [Header("Efectos visuales")]
    public GameObject prefabExplosion;

    private bool impactoRegistrado = false;

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (!otro.CompareTag("Enemigo"))
        {
            return;
        }

        if (impactoRegistrado)
        {
            return;
        }

        impactoRegistrado = true;

        EnemyHealth vidaEnemigo =
            otro.GetComponent<EnemyHealth>();

        if (vidaEnemigo == null)
        {
            Debug.LogWarning(
                "El enemigo " + otro.name +
                " no tiene el componente EnemyHealth."
            );

            Destroy(gameObject);
            return;
        }

        BossDreadnought jefeFinal =
            vidaEnemigo.GetComponent<BossDreadnought>();

        bool esJefeFinal = jefeFinal != null;

        bool enemigoDestruido =
            vidaEnemigo.RecibirDanio(danio);

        Destroy(gameObject);

        if (!enemigoDestruido)
        {
            return;
        }

        Vector3 posicionImpacto =
            vidaEnemigo.transform.position;

        if (GameManager.instancia != null)
        {
            GameManager.instancia.SumarPuntos(
                vidaEnemigo.puntosAlDestruir
            );
        }

        // Mantener la destrucción especial del jefe.
        if (esJefeFinal)
        {
            Debug.Log(
                "¡DREADNOUGHT DERROTADO! " +
                "Iniciando secuencia de destrucción."
            );

            jefeFinal.IniciarDestruccion();
            return;
        }

        // Probabilidad general de generar un power-up.
        if (Random.Range(0f, 100f) < probabilidadPowerUp)
        {
            GenerarPowerUp(posicionImpacto);
        }

        // Sonido de explosión del enemigo normal.
        if (sonidoExplosion != null)
        {
            Vector3 posicionAudio =
                Camera.main != null
                ? Camera.main.transform.position
                : posicionImpacto;

            AudioSource.PlayClipAtPoint(
                sonidoExplosion,
                posicionAudio,
                volumenExplosion
            );
        }

        // Efecto visual de explosión.
        if (prefabExplosion != null)
        {
            Instantiate(
                prefabExplosion,
                posicionImpacto,
                Quaternion.identity
            );
        }

        Destroy(vidaEnemigo.gameObject);
    }

    private void GenerarPowerUp(Vector3 posicion)
    {
        // Calcular los pesos de los prefabs disponibles.
        float pesoDoble =
            prefabPowerUp != null
            ? Mathf.Max(0f, pesoDisparoDoble)
            : 0f;

        float pesoProteccion =
            prefabPowerUpEscudo != null
            ? Mathf.Max(0f, pesoEscudo)
            : 0f;

        float pesoTriple =
            prefabPowerUpTriple != null
            ? Mathf.Max(0f, pesoDisparoTriple)
            : 0f;

        float pesoTotal =
            pesoDoble + pesoProteccion + pesoTriple;

        if (pesoTotal <= 0f)
        {
            Debug.LogWarning(
                "No hay power-ups disponibles para generar."
            );
            return;
        }

        float seleccion = Random.Range(0f, pesoTotal);

        GameObject powerUpSeleccionado;

        if (seleccion < pesoDoble)
        {
            powerUpSeleccionado = prefabPowerUp;
        }
        else if (seleccion < pesoDoble + pesoProteccion)
        {
            powerUpSeleccionado = prefabPowerUpEscudo;
        }
        else
        {
            powerUpSeleccionado = prefabPowerUpTriple;
        }

        Instantiate(
            powerUpSeleccionado,
            posicion,
            Quaternion.identity
        );
    }
}
