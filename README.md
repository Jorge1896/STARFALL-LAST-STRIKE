# STARFALL: LAST STRIKE

### Videojuego 2D Shoot 'em up | Unity 6.5

**STARFALL: LAST STRIKE** es un videojuego de acción y disparos espaciales en 2D, desarrollado con Unity y C# como parte de un proyecto académico de INACAP.

El jugador controla una nave de combate que debe sobrevivir a oleadas de enemigos, mejorar su armamento y avanzar a través de diferentes zonas hasta enfrentarse al jefe final: **Dreadnought**.

## Características principales

- Combate espacial 2D con desplazamiento vertical.
- Movimiento de la nave mediante teclado.
- Disparo continuo y mejoras de armamento: disparo doble y triple.
- Sistema de Dash para realizar maniobras evasivas.
- Escudo protector e invulnerabilidad temporal al recibir daño.
- Sistema de tres vidas y recuperación de vidas mediante bonificaciones.
- Enemigos de distintos tipos, incluyendo naves básicas e interceptoras.
- Dificultad progresiva según la puntuación.
- Cuatro zonas visuales con escenarios diferentes.
- Jefe final **Dreadnought**, con múltiples patrones de ataque y fases de combate.
- Sistema de puntuación e interfaz HUD.
- Menú principal, pausa, pantalla de Game Over y pantalla de Victoria.
- Efectos visuales, explosiones y efectos de sonido.
- Banda sonora de ocho pistas con cambios musicales según el progreso de la partida.

## Controles

| Acción | Tecla |
|---|---|
| Mover la nave | W, A, S, D |
| Disparar | Click izquierdo del mouse |
| Dash hacia la izquierda | Espacio + A |
| Dash hacia la derecha | Espacio + D |
| Pausar / reanudar | Esc |

## Progresión del videojuego

El juego aumenta progresivamente su dificultad según la puntuación del jugador.

| Etapa | Puntuación |
|---|---|
| Zona 1 — Bases enemigas | 0–4.999 |
| Zona 2 — Complejo industrial | 5.000–14.999 |
| Zona 3 — Cañones y montañas | 15.000–29.999 |
| Zona 4 — Instalaciones orbitales | 30.000–39.999 |
| Jefe final — Dreadnought | Desde 40.000 |

## Banda sonora

El videojuego incorpora ocho pistas musicales para acompañar las diferentes etapas:

- OST 00 — Before the Last Strike (menú principal).
- OST 01 — Last Outpost.
- OST 02 — Iron Offensive.
- OST 03 — Ashes of War.
- OST 04 — Beyond the Atmosphere.
- OST 05 — Dreadnought Last Strike.
- OST 06 — Fallen Star (Game Over).
- OST 07 — Dawn of Victory (Victoria).

## 🛠️ Tecnologías utilizadas

- **Motor:** Unity 6.5
- **Lenguaje:** C#
- **Sistema de entrada:** Unity Input System
- **Renderizado:** Universal Render Pipeline (2D)
- **Interfaz:** TextMesh Pro
- **Control de versiones:** Git y GitHub

## Estructura del proyecto

- `Assets/` — Escenas, scripts, sprites, prefabs, animaciones y audio.
- `Packages/` — Dependencias y paquetes de Unity.
- `ProjectSettings/` — Configuración del proyecto.

## Cómo abrir el proyecto

1. Instalar Unity Hub y una versión compatible de Unity 6.5.
2. Descargar el repositorio o clonarlo mediante Git:

   `git clone https://github.com/Jorge1896/STARFALL-LAST-STRIKE.git`

3. Abrir Unity Hub.
4. Seleccionar **Add project from disk**.
5. Elegir la carpeta descargada del proyecto.
6. Esperar a que Unity importe los recursos y dependencias.
7. Abrir la escena `Assets/Scenes/MenuPrincipal.unity`.
8. Presionar **Play** para comenzar.

## Contexto académico

Proyecto desarrollado para una evaluación académica de INACAP, aplicando conceptos de programación en C#, desarrollo de videojuegos 2D, diseño de mecánicas, gestión de escenas, interfaces y control de versiones.

---

**STARFALL: LAST STRIKE — La última batalla comienza entre las estrellas.**
