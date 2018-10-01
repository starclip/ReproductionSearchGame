using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows.Speech;
using System.Linq;

public class MainFunction : MonoBehaviour {

    public GameObject TableroFisico;
    public GameObject camara;
    public int largo;
    public int ancho;
    public int a;
    public int enemy;
    public bool hayDiagonales;
    public GameObject estrella;
    private AEstrella algoritmoEstrella;
    private int filas;
    private int columnas;
    private int distancia;
    private Tablero tablero;
    private bool hayRutaMostrada;
    private bool modoBerseck;



    private void iniciarDiccionario(string text)
    {
        switch (text)
        {
            case "up":
                moverJugador("UP");
                break;

            case "down":
                moverJugador("DOWN");
                break;

            case "right":
                moverJugador("RIGHT");
                break;

            case "left":
                moverJugador("LEFT");
                break;

            case "north":
                moverJugador("UP");
                break;

            case "south":
                moverJugador("DOWN");
                break;

            case "east":
                moverJugador("RIGHT");
                break;

            case "west":
                moverJugador("LEFT");
                break;

            case "north east":
                moverJugador("RIGHTUP");
                break;

            case "south east":
                moverJugador("RIGHTDOWN");
                break;

            case "north west":
                moverJugador("LEFTUP");
                break;

            case "south west":
                moverJugador("LEFTDOWN");
                break;

            case "show route":
                mostrarRuta();
                break;

            case "hide route":
                ocultarRuta();
                break;

            case "show":
                mostrarRuta();
                break;

            case "activate diagonals":
                activarDiagonales();
                break;

            case "desactivate diagonals":
                desactivarDiagonales();
                break;

            case "diagonals":
                setDiagonales();
                break;

            case "automatic":
                activarModoBerseck();
                break;

            case "berseck mode":
                activarModoBerseck();
                break;

            /* Diccionario y comandos de la cámara. */
            case "shrink":
                encoger(10);
                break;

            case "expand":
                expandir(10);
                break;

            case "magnify":
                expandir(10);
                break;

            case "zoom in":
                encoger(2);
                break;

            case "zoom out":
                expandir(2);
                break;

            case "camera right":
                moverCamara("RIGHT");
                break;

            case "camera left":
                moverCamara("LEFT");
                break;

            case "camera down":
                moverCamara("DOWN");
                break;

            case "camera up":
                moverCamara("UP");
                break;


            /* Crear tablero */
            case "create board":
                preCreacion();
                break;

            case "create grid":
                preCreacion();
                break;

            case "create box":
                preCreacion();
                break;

            default:
                UnityEngine.Debug.Log("PALABRA NO RECONOCIBLE");
                break;
        }

     
    }
    
    /* Crear una nueva tabla... necesito procesar los comandos. */
    private void preCreacion()
    {
        // Llamo a una nueva scene.
        crearTablero();
    }

    /* Mover la ficha a una posición nueva. */
    private void moverJugador()
    {
        
    }

    /*private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        //dictationRecognizer.Start();
        Debug.Log(text);
        //texto.text = text;

        iniciarDiccionario(text);


    }*/

   

 

    /* Se configura la camara en el equipo. */
    private void configurarCamara()
    {
        Vector3 camaraPosition = tablero.ubicarCentro();
        //camaraPosition.x = camaraPosition.x - 15;
        camaraPosition.z = camara.transform.position.z;
        camara.transform.position = camaraPosition;
    }

    public void crearTablero()
    {
        tablero = TableroFisico.GetComponent<Tablero>();
        this.filas = obtenerFilas(largo, a);
        this.columnas = obtenerColumnas(ancho, a);
        tablero.inicializar(this.filas, this.columnas, this.a, 10, this.enemy);
        tablero.crearTablero();
        tablero.seleccionarMeta();
        tablero.seleccionarInicio();
        tablero.seleccionarEnemigos(this.enemy);
        configurarCamara();
        this.modoBerseck = false;
    }

	// Use this for initialization
	void Start () {

        

    }
	
	// Update is called once per frame
	void Update () {
        
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
            tablero.printTablero();
            hayRutaMostrada = true;
        }
        else
        {
            print("No se pudo encontrar la ruta óptima");
            // Se avisa por medio de voz que no se encontró una ruta óptima... no hay ruta.
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
        this.tablero.getJugador().GetComponent<Jugador>().obtenerPosiblesMovimientos(rutaOptima);
    }

    private void activarDiagonales()
    {
        this.hayDiagonales = true;
        print("True");
    }

    private void desactivarDiagonales()
    {
        this.hayDiagonales = false;
        print("False");
    }

    private void setDiagonales()
    {
        this.hayDiagonales = !this.hayDiagonales;
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
