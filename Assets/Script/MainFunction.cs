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

    KeywordRecognizer keywordRecognizer;
    Dictionary<string, Action> keywords = new Dictionary<string, Action>(); // Crea un diccionario.


    private void iniciarDiccionario()
    {
   
        keywords.Add("up", () => { moverJugador("UP"); });
        keywords.Add("down", () => { moverJugador("DOWN"); });
        keywords.Add("right", () => { moverJugador("RIGHT"); });
        keywords.Add("left", () => { moverJugador("LEFT"); });
        keywords.Add("norte", () => { moverJugador("UP"); });
        keywords.Add("sur", () => { moverJugador("DOWN"); });
        keywords.Add("este", () => { moverJugador("RIGHT"); });
        keywords.Add("oeste", () => { moverJugador("LEFT"); });
        keywords.Add("noreste", () => { moverJugador("RIGHTUP"); });
        keywords.Add("sureste", () => { moverJugador("RIGHTDOWN"); });
        keywords.Add("noroeste", () => { moverJugador("LEFTUP"); });
        keywords.Add("suroeste", () => { moverJugador("LEFTDOWN"); });
        keywords.Add("mostrar ruta", () => { mostrarRuta(); });
        keywords.Add("ocultar ruta", () => { ocultarRuta(); });
        keywords.Add("activar diagonales", () => { activarDiagonales(); });
        keywords.Add("desactivar diagonales", () => { desactivarDiagonales(); });
        keywords.Add("diagonales", () => { setDiagonales(); });
        keywords.Add("automatico", () => { activarModoBerseck(); });
        keywords.Add("automático", () => { activarModoBerseck(); });
        keywords.Add("modo berseck", () => { activarModoBerseck(); });

        /* Diccionario y comandos de la cámara. */
        keywords.Add("encoger", () => { encoger(10); });
        keywords.Add("expandir", () => { expandir(10); });
        keywords.Add("agrandar", () => { expandir(10); });
        keywords.Add("alejate", () => { expandir(3); });
        keywords.Add("acercate", () => { encoger(3); });
        keywords.Add("zoom out", () => { expandir(2); });
        keywords.Add("zoom in", () => { encoger(2); });
        keywords.Add("camara derecha", () => { moverCamara("RIGHT"); });
        keywords.Add("camara izquierda", () => { moverCamara("LEFT"); });
        keywords.Add("camara abajo", () => { moverCamara("DOWN"); });
        keywords.Add("camara arriba", () => { moverCamara("UP"); });
        keywords.Add("cámara derecha", () => { moverCamara("RIGHT"); });
        keywords.Add("cámara izquierda", () => { moverCamara("LEFT"); });
        keywords.Add("cámara abajo", () => { moverCamara("DOWN"); });
        keywords.Add("cámara arriba", () => { moverCamara("UP"); });

        /* Crear tablero */
        keywords.Add("crear tablero", () => { preCreacion(); });
        keywords.Add("create chess", () => { preCreacion(); });
        keywords.Add("crear cuadricula", () => { preCreacion(); });
        keywords.Add("create box", () => { preCreacion(); });
    }
    
    /* Crear una nueva tabla... necesito procesar los comandos. */
    private void preCreacion()
    {
        // Llamo a una nueva scene. 
    }

    /* Mover la ficha a una posición nueva. */
    private void moverJugador()
    {
        
    }

    private void KeywordRecognizerOnPraseRecognized(PhraseRecognizedEventArgs args)
    {
        Action keywordAction;

        print(args.text);
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    /* Se configura la camara en el equipo. */
    private void configurarCamara()
    {
        Vector3 camaraPosition = tablero.ubicarCentro();
        //camaraPosition.x = camaraPosition.x - 15;
        camaraPosition.z = camara.transform.position.z;
        camara.transform.position = camaraPosition;
    }

	// Use this for initialization
	void Start () {
        iniciarDiccionario();

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPraseRecognized;
        keywordRecognizer.Start();

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
