using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows.Speech;
using System.Linq;

public class MainFunction : MonoBehaviour {

    /* Audios */
    public GameObject sonidoEmpezar;
    public GameObject xSound;
    public GameObject ySound;
    public GameObject errorSound;
    public GameObject readySound;
    public GameObject diagonalesActivadasSound;
    public GameObject diagonalesDesactivadasSound;
    public GameObject rutaNoSound;
    public GameObject berseckSound;
    public GameObject showRouteSound;
    public GameObject invalidSound;

    private AudioSource audioInicio;
    private AudioSource audioX;
    private AudioSource audioY;
    private AudioSource audioError;
    private AudioSource audioReady;
    private AudioSource audioDiagonalesAct;
    private AudioSource audioDiagonalesDes;
    private AudioSource audioNoRoute;
    private AudioSource audioBerseck;
    private AudioSource audioRoute;
    private AudioSource audioInvalid;

    public GameObject TableroFisico;
    public GameObject camara;
    public GameObject prefabCuadro;
    public int a;
    public int enemy;
    public bool hayDiagonales;
    public GameObject estrella;
    private AEstrella algoritmoEstrella;
    private GameObject cuadroVariables;
    private int filas;
    private int columnas;
    private int distancia;
    private Tablero tablero;
    private bool hayRutaMostrada;
    private bool modoBerseck;

    // State y valores referentes al modo de aparecer el cuadro en medio de la pantalla.
    private bool stateCuadro;
    private bool stateCambiarJugador; // Si es false ... cambiar meta.

    private int possiblePosX;
    private int possiblePosY;
   
    private bool assignedX;
    private bool assignedY;

    private DictationRecognizer director;

    public void defineDirector(DictationRecognizer recognizer)
    {
        this.director = recognizer;
    }

    /* Reproducir audio... */
    public void playAudio(AudioSource audio)
    {
        StartCoroutine(play(audio));
        
    }

    IEnumerator play(AudioSource audio)
    {
        this.director.Stop();
        //Debug.Log("Audio - Desactivado");
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
        this.director.Start();
        //Debug.Log("Audio - Activado");
    }

    // Use this for initialization
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        tablero = null;
        modoBerseck = false;
        possiblePosX = 0;
        possiblePosY = 0;
        stateCuadro = false;
        audioInicio = sonidoEmpezar.GetComponent<AudioSource>();
        audioX = xSound.GetComponent<AudioSource>();
        audioY = ySound.GetComponent<AudioSource>();
        audioError = errorSound.GetComponent<AudioSource>();
        audioReady = readySound.GetComponent<AudioSource>();
        audioDiagonalesAct = diagonalesActivadasSound.GetComponent<AudioSource>();
        audioDiagonalesDes = diagonalesDesactivadasSound.GetComponent<AudioSource>();
        audioNoRoute = rutaNoSound.GetComponent<AudioSource>();
        audioBerseck = berseckSound.GetComponent<AudioSource>();
        audioInvalid = invalidSound.GetComponent<AudioSource>();
        audioRoute = showRouteSound.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        // Si está activado el modo Berseck.
        if (modoBerseck)
        {
            // Si hay un tablero en juego.
            if (this.tablero)
            {
                // Si hay un jugador en el tablero creado.
                if (this.tablero.getJugador())
                {
                    // Si el estado actual del jugador ya finalizo de movilizarme
                    if (!this.tablero.getJugador().GetComponent<Jugador>().getActualState())
                    {
                        bool state = recorrerRuta();
                        // Llegue al final.
                        if (!state)
                        {
                            this.modoBerseck = false;
                        }
                    }
                }
            }
        }

    }

    /* Detecta las palabras en modo normal. */
    private void modoDetectarPalabras(string text)
    {
        switch (text)
        {
            case "up":
                moverJugador("UP");
                return;

            case "down":
                moverJugador("DOWN");
                return;

            case "right":
                moverJugador("RIGHT");
                return;

            case "left":
                moverJugador("LEFT");
                return;

            case "north":
                moverJugador("UP");
                return;

            case "south":
                moverJugador("DOWN");
                return;

            case "east":
                moverJugador("RIGHT");
                return;

            case "west":
                moverJugador("LEFT");
                return;

            case "north east":
                moverJugador("RIGHTUP");
                return;

            case "south east":
                moverJugador("RIGHTDOWN");
                return;

            case "north west":
                moverJugador("LEFTUP");
                return;

            case "south west":
                moverJugador("LEFTDOWN");
                return;

            case "northeast":
                moverJugador("RIGHTUP");
                return;

            case "southeast":
                moverJugador("RIGHTDOWN");
                return;

            case "northwest":
                moverJugador("LEFTUP");
                return;

            case "southwest":
                moverJugador("LEFTDOWN");
                return;

            case "show route":
                mostrarRuta();
                return;

            case "hide route":
                ocultarRuta();
                return;

            case "hide":
                ocultarRuta();
                return;

            case "show":
                mostrarRuta();
                return;

            case "activate diagonals":
                activarDiagonales();
                return;

            case "disactivate diagonals":
                desactivarDiagonales();
                return;

            case "diagonals":
                setDiagonales();
                return;

            case "automatic":
                activarModoBerseck();
                return;

            case "berseck mode":
                activarModoBerseck();
                return;

            /* Diccionario y comandos de la cámara. */
            case "shrink":
                encoger(10);
                return;

            case "expand":
                expandir(10);
                return;

            case "magnify":
                expandir(10);
                return;

            case "zoom in":
                encoger(2);
                return;

            case "zoom out":
                expandir(2);
                return;

            case "camera right":
                moverCamara("RIGHT");
                return;

            case "camera left":
                moverCamara("LEFT");
                return;

            case "camera down":
                moverCamara("DOWN");
                return;

            case "camera up":
                moverCamara("UP");
                return;

            case "camera east":
                moverCamara("RIGHT");
                return;

            case "camera west":
                moverCamara("LEFT");
                return;

            case "camera south":
                moverCamara("DOWN");
                return;

            case "camera north":
                moverCamara("UP");
                return;

            /* Cambiar posicion de jugador */
            case "change player":
                cambiarPosicionJugador();
                return;

            case "change objective":
                cambiarPosicionMeta();
                return;

            case "position player":
                cambiarPosicionJugador();
                return;

            case "position pleasure":
                cambiarPosicionJugador();
                return;

            case "position goal":
                cambiarPosicionMeta();
                return;

            case "position go":
                cambiarPosicionMeta();
                return;
        }

        playAudio(audioInvalid);
        
        // Sonido de error movimiento.
    }

    /* Convierte el número en válido. */
    private int convertNumber(string possible)
    {
        try
        {
            int numb = int.Parse(possible);
            return numb;
        }
        catch (Exception e)
        {
            return -1;
        }
    }

    /* Función que procesa la posición x y y.*/
    private void processesNumber(string texto)
    {
        if (texto == "one")
        {
            texto = "1";
        }

        if (assignedX == false)
        {
            int numb = convertNumber(texto) - 1; 
            int numFilas = this.tablero.obtenerFilas();
            if ((numb < 0) || (numb >= numFilas))
            {
                playAudio(audioError);
                Debug.Log("La posición es inválida. Supera el número de filas.");
                return;
            }

            this.possiblePosX = numb;
            assignedX = true;

            GameObject xValue = cuadroVariables.transform.GetChild(4).gameObject;
            xValue.GetComponent<TextMesh>().text = texto;

            audioY.Play();
            return;
        }

        if (assignedY == false)
        {
            int numb = convertNumber(texto) - 1;
            int numColumnas = this.tablero.obtenerColumnas();
            if ((numb < 0) || (numb >= numColumnas))
            {
                playAudio(audioError);
                Debug.Log("La posición es inválida. Supera el número de columnas");
                return;
            }

            this.possiblePosY = numb;
            assignedY = true;

            GameObject yValue = cuadroVariables.transform.GetChild(5).gameObject;
            yValue.GetComponent<TextMesh>().text = texto;

            audioReady.Play();
            return;
        }

        audioReady.Play();
        // Recuerde el audio de que debe decir listo.
    }

    /* Verifica la nueva posición del jugador. */
    private void verificarNuevaPosicion()
    {
        if (this.possiblePosX == -1 || this.possiblePosY == -1)
        {
            // Imprimir que no ha dicho las nuevas coordenadas.
            return;
        }

        bool stateValid = tablero.movimientoValido(this.possiblePosX, this.possiblePosY);

        // Si el movimiento es válido.
        if (stateValid)
        {
            // Si es el jugador el que hay que mover.
            if (this.stateCambiarJugador)
            {
                tablero.transportPlayer(this.possiblePosX, this.possiblePosY);
            }else
            {
                tablero.transportGoal(this.possiblePosX, this.possiblePosY);
            }
        }else
        {
            Debug.Log("Movimiento inválido.");
            playAudio(audioError);
            // Indicar que no se puede mover a esa posición.
        }

    }

    /* Procesa los datos que el jugador le de con la intención de almacenar los números. */
    private void modoCambiarPosicion(string text)
    {
        switch (text)
        {
            case "ready":
                // Procesa los datos datos por el jugador.
                verificarNuevaPosicion(); // Comprueba si hay fichas en esa casilla.
                reestablecerCuadro();
                return;
            case "cancel":
                // Detiene el procedimiento y restaura el juego.
                reestablecerCuadro();
                return;
        }

        processesNumber(text);
    }

    /* Realice una acción dependiendo de la palabra que el usuario pronuncie. */
    public void iniciarDiccionario(string text)
    {
        // Si no quiere cambiar las posiciones del jugador o la meta.
        if (stateCuadro)
        {
            modoCambiarPosicion(text);
        }else
        {
            modoDetectarPalabras(text);
        }
    }

    /* Configure las filas inmediatamente. */
    public void setFilas(int filas, int columnas, int a)
    {
        this.filas = filas;
        this.columnas = columnas;
        this.a = a;
    }

    /* Crea el cuadro con las variables x y y. */
    private void aparecerCuadro()
    {
        stateCuadro = true;
        audioX.Play();
        Vector3 pos = tablero.ubicarCentro();
        pos.z = prefabCuadro.transform.position.z - 5;
        cuadroVariables = Instantiate(prefabCuadro, pos, prefabCuadro.transform.rotation);
    }

    /* Destruye el cuadro que almaceno anteriormente. */
    private void desaparecerCuadro()
    {
        Destroy(cuadroVariables);
        cuadroVariables = null;
    }

    /* Se reestablecen los valores cuando el usuario decide volver al modo de detectar palabras normales. */
    private void reestablecerCuadro()
    {
        this.stateCuadro = false;
        this.possiblePosX = 0;
        this.possiblePosY = 0;
        this.assignedX = false;
        this.assignedY = false;
        desaparecerCuadro();
    }

    /* Mover la ficha a una posición nueva. */
    private void cambiarPosicionJugador()
    {
        this.stateCambiarJugador = true;
        if (!stateCuadro)
        {
            aparecerCuadro();
        }
    }

    /* Mover la posición de la ficha de la meta. */
    private void cambiarPosicionMeta()
    {
        this.stateCambiarJugador = false;
        if (!stateCuadro)
        {
            aparecerCuadro();
        }
    }


    /* Se configura la camara en el equipo. */
    private void configurarCamara()
    {
        Vector3 camaraPosition = tablero.ubicarCentro();
        camaraPosition.z = camara.transform.position.z;
        camara.transform.position = camaraPosition;
    }

    /* Realice la creación del tablero. */
    public void crearTablero()
    {
        tablero = TableroFisico.GetComponent<Tablero>();
        tablero.inicializar(this.filas, this.columnas, this.a, 10);
        tablero.crearTablero();
        tablero.seleccionarMeta();
        tablero.seleccionarInicio();
        tablero.seleccionarEnemigos();
        configurarCamara();
    }

    /* Recorrer ruta. */
    private bool recorrerRuta()
    {
        if (this.tablero == null)
        {
            return false;
        }

        if (this.tablero.getJugador() == null)
        {
            return false;
        }

        String siguienteMovimiento = this.tablero.getJugador().GetComponent<Jugador>().obtenerMovimiento();
        if (siguienteMovimiento != null)
        {
            moverJugador(siguienteMovimiento);
            return true;
        }

        return false;
    }

    private void moverJugador(string movimiento)
    {
        GameObject jugador = tablero.getJugador();
        Jugador juego = jugador.GetComponent<Jugador>();

        bool state = this.tablero.verificarMovimientoJugador(movimiento, this.hayDiagonales);

        Debug.Log("Jugador state: " + state);

        juego.mover(movimiento, state); // Mueva el jugador...
    }

    private int obtenerFilas(int largo, int distancia)
    {
        float numeroFilas = largo / distancia;
        return Mathf.RoundToInt(numeroFilas);
    }

    private int obtenerColumnas(int ancho, int distancia)
    {
        float numeroFilas = ancho / distancia;
        return Mathf.RoundToInt(numeroFilas);
    }

    private void mostrarRuta()
    {
        if (tablero == null)
        {
            print("El tablero no ha sido creado");
        }

        if (hayRutaMostrada)
        {
            this.ocultarRuta();
        }

        algoritmoEstrella = estrella.GetComponent<AEstrella>();
        algoritmoEstrella.inicializar();
        bool state = algoritmoEstrella.accion(this.hayDiagonales); // Se realiza el algoritmo de estrella.

        if (state)
        {
            tablero.setRuta(algoritmoEstrella.obtenerMejorRuta());
            tablero.dibujarRutaOptima();
            hayRutaMostrada = true;
            playAudio(audioRoute);
        }
        else
        {
            audioNoRoute.Play();
        }
    }

    private void ocultarRuta()
    {
        this.tablero.eliminarRutaOptima();
    }

    private void activarModoBerseck()
    {
        this.mostrarRuta();
        this.modoBerseck = true;
        List<Ficha> rutaOptima = this.tablero.obtenerRuta();
        bool state = this.tablero.getJugador().GetComponent<Jugador>().obtenerPosiblesMovimientos(rutaOptima);
        if (!state)
        {
            audioNoRoute.Play();
        }else
        {
            // Reproducir modo berseck...
            playAudio(audioBerseck);
        }
    }

    private void activarDiagonales()
    {
        this.hayDiagonales = true;
        playAudio(audioDiagonalesAct);
    }

    private void desactivarDiagonales()
    {
        this.hayDiagonales = false;
        playAudio(audioDiagonalesDes);
    }

    private void setDiagonales()
    {
        if (this.hayDiagonales)
        {
            desactivarDiagonales();
        }else
        {
            activarDiagonales();
        }
    }

    private void expandir(int val)
    {
       float sizeCamera = this.camara.GetComponent<Camera>().orthographicSize;
        if (sizeCamera >= 1000)
        {
            print("No puedo agrandar más la cámara.");
            return;
        }else
        {
            //sizeCamera += 10;
            this.camara.GetComponent<Camera>().orthographicSize = sizeCamera + val;
        }
    }

    private void encoger(int val)
    {
        float sizeCamera = this.camara.GetComponent<Camera>().orthographicSize;
        if (sizeCamera <= 10)
        {
            print("No puedo encoger más la cámara.");
            return;
        }
        else
        {
            this.camara.GetComponent<Camera>().orthographicSize = sizeCamera - val;
        }
    }

    private void moverCamara(string movement)
    {
        this.camara.GetComponent<CamaraController>().camaraMuevete(movement);
    }


}
