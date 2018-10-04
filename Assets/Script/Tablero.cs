using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablero : MonoBehaviour {

    public GameObject prefab;
    public GameObject meta;
    public GameObject jugadorPrefab;
    public GameObject prefabEnemigos;
    public GameObject huellas;
    public GameObject soundWin;
    public GameObject movePlayerSound;
    private AudioSource audioWin;
    private AudioSource audioPlayer;
    private int filas;
    private int columnas;
    private float distanciaX;
    private float distanciaY;
    private int costoLateral;
    private int costoDiagonal;
    private int numeroFichas;
    private int A;
    private float scaleNumb;
    private GameObject jugador;
    private GameObject[] enemigos;
    private List<Ficha> rutaOptima;
    private GameObject[,] tablero;
    private GameObject[] rutaHuellasOptimas;
    private bool stateReproducir;
    
	// Use this for initialization
	void Start () {
        audioWin = soundWin.GetComponent<AudioSource>();
        audioPlayer = movePlayerSound.GetComponent<AudioSource>();
    }

    public void inicializar(int filas, int columnas, int A, int distancia)
    {
        this.filas = filas;
        this.columnas = columnas;
        this.rutaOptima = new List<Ficha>();
        this.tablero = new GameObject[filas, columnas];
        int numEnemies = (int) ((this.filas * this.columnas) / 4);
        int enemigos = numEnemies;
        Debug.Log("Enemigos: " + enemigos);
        this.enemigos = new GameObject[enemigos];
        this.numeroFichas = filas * columnas;
        this.rutaOptima = new List<Ficha>();
        this.costoLateral = distancia;
        this.costoDiagonal = (int)Math.Sqrt(Math.Pow(distancia, 2) + Math.Pow(distancia, 2));
        this.A = A;
        setDistancias(A); // Configuro las distancias de A.
        expandirImagen();
        stateReproducir = false;
    }

    /* Ubique el centro de los cuadros. */
    public Vector3 ubicarCentro()
    {
        Vector2 puntoX4 = new Vector2(transform.position.x + this.distanciaX * this.columnas, transform.position.y + this.distanciaY * this.filas);

        float posX = (puntoX4.x / 2);
        float posY = (puntoX4.y / 2);

        Vector3 resultado = new Vector3(posX , -posY, 50);
        return resultado;
    }

    /* Expandir la imagen de acuerdo con el a. */
    private void expandirImagen()
    {
        float dateScale = (float)0.15;
        double scaleX = (0.05 + dateScale * (this.A - 1)) * this.filas;
        double scaleY = (0.07 + dateScale * (this.A - 1)) * this.columnas;
        float scX = (float) Math.Ceiling( (decimal) scaleX);
        float scY = (float) Math.Ceiling( (decimal) scaleY);
        GameObject pantallaRoja = GameObject.Find("fondoRojo");
        pantallaRoja.transform.localScale = new Vector3(scX, scY, 10);
        pantallaRoja.transform.position = ubicarCentro();
    }

    /* Establecer las coordenadas del jugador. */
    private void setJugadorCoord(int x, int y)
    {
        this.jugador.GetComponent<Ficha>().setLocation(x, y);
    }

    /* Mover al jugador a las coordenadas X y Y. */
    public void transportPlayer(int x, int y)
    {
        GameObject fichaObj = getFicha(x, y);
        this.jugador.transform.position = fichaObj.transform.position;
        setJugadorCoord(x, y);
    }

    /* Mover la meta a las coordenadas X y Y. */
    public void transportGoal(int x, int y)
    {
        GameObject fichaObj = getFicha(x, y);
        this.meta.transform.position = fichaObj.transform.position;
        this.meta.GetComponent<Ficha>().setLocation(x, y);
    }
    
    /* *******************************************************
         * Se selecciona la meta a seguir.
         * No se deben haber generado enemigos.
         *********************************************************/
    public void seleccionarMeta()
    {
        GameObject objeto = obtenerficha();
        Ficha fichaTemp = objeto.gameObject.GetComponent<Ficha>();
        // Si la ficha de la meta no está en el mismo lugar que el jugador.
        while (fichaTemp.compararFicha(this.jugador))
        {
            objeto = obtenerficha();
            fichaTemp = objeto.gameObject.GetComponent<Ficha>();
        }

        Vector3 pos = objeto.transform.position;
        meta = Instantiate(meta, pos, transform.rotation) as GameObject;
        meta.GetComponent<Ficha>().setLocation(fichaTemp.getX(), fichaTemp.getY());
    }

    /* Compare si un movimiento es válido. */
    private bool movimientoValido(GameObject posActual, int x, int y, bool diagonal)
    {
        GameObject[] enemigos = this.getEnemigos();
        GameObject fichaObj = this.getFicha(x, y);
        Ficha ficha = fichaObj.GetComponent<Ficha>();

        if (ficha.compararMuchasFichas(enemigos))
        {
            return false; // Es un enemigo.
        }

        if (diagonal)
        {
            Ficha fuente = this.jugador.GetComponent<Ficha>();
            Ficha objetivo = this.getFicha(x, y).GetComponent<Ficha>();
            Ficha posibleEnemyOne = this.getFicha(fuente.getX(), objetivo.getY()).GetComponent<Ficha>();
            Ficha posibleEnemyTwo = this.getFicha(objetivo.getX(), fuente.getY()).GetComponent<Ficha>();

            // Si las dos son enemigos, no se tomarán en cuenta este camino...
            if (posibleEnemyOne.compararMuchasFichas(enemigos) && posibleEnemyTwo.compararMuchasFichas(enemigos))
            {
                return false; // Hay dos posibles enemigos que bloquean mi camino.
            }
        }

        Debug.Log("It's easy to me....");
        audioPlayer.Play();
        return true; // Movimiento válido.
    }

    /* Verifica si el movimiento indicado por el jugador es viable... */
    public bool verificarMovimientoJugador(String movement, bool hayDiagonales)
    {
        bool state;
        int posX = this.jugador.GetComponent<Ficha>().getX();
        int posY = this.jugador.GetComponent<Ficha>().getY();
        
        if (movement.Equals("RIGHT"))
        {
            posY = posY + 1;
        }

        if (movement.Equals("LEFT"))
        {
            posY = posY - 1;
        }

        if (movement.Equals("UP"))
        {
            posX = posX - 1;
        }

        if (movement.Equals("DOWN"))
        {
            posX = posX + 1;
        }

        if (movement.Equals("RIGHTUP"))
        {
            posX = posX - 1;
            posY = posY + 1;
        }

        if (movement.Equals("LEFTUP"))
        {
            posX = posX - 1;
            posY = posY - 1;
        }

        if (movement.Equals("RIGHTDOWN"))
        {
            posX = posX + 1;
            posY = posY + 1;
        }

        if (movement.Equals("LEFTDOWN"))
        {
            posX = posX + 1;
            posY = posY - 1;
        }

        if ((movement == "RIGHTUP" || movement == "LEFTUP" || movement == "RIGHTDOWN" || movement == "LEFTDOWN") && hayDiagonales == false)
        {
            return false; // No se permite el movimiento en diagonales.
        }

            // Si no se sale del tablero.
            if ((posX >= 0 && posX < this.filas) && (posY >= 0 && posY < this.columnas))
        {
            // Si es un movimiento en diagonal.
            if (movement == "RIGHTUP" || movement == "LEFTUP" || movement == "RIGHTDOWN" || movement == "LEFTDOWN")
            {
                state = movimientoValido(this.jugador, posX, posY, true);
            }else
            {
                state = movimientoValido(this.jugador, posX, posY, false);
            }

            if (state)
            {
                setJugadorCoord(posX, posY);
            }
            return state;
        }

        return false; // Me salgo del cuadrado....
    }

    /* Obtenga el costo lateral total. */
    public int getCostoLateral()
    {
        return this.costoLateral;
    }

    /* Obtenga el costo lateral diagonal. */
    public int getCostoDiagonal()
    {
        return this.costoDiagonal;
    }

    /* *******************************************************
     * Se selecciona el jugador por primera vez.
     * No se deben haber generado enemigos.
     *********************************************************/
    public void seleccionarInicio()
    {
        GameObject objeto = obtenerficha();
        Ficha fichaTemp = objeto.gameObject.GetComponent<Ficha>();
        // Si la ficha del jugador no está en el mismo lugar que la meta.
        while (fichaTemp.compararFicha(this.meta))
        {
            objeto = obtenerficha();
            fichaTemp = objeto.gameObject.GetComponent<Ficha>();
        }

        Vector3 pos = objeto.transform.position;
        jugador = Instantiate(jugadorPrefab, pos, transform.rotation) as GameObject;
        jugador.GetComponent<Ficha>().setLocation(fichaTemp.getX(), fichaTemp.getY());
        jugador.GetComponent<Jugador>().setDistances(this.distanciaX, this.distanciaY); // Distancias a avanzar.
    }

    /* Compare si un movimiento es válido. */
    public bool movimientoValido(int x, int y)
    {
        GameObject fichaObj = getFicha(x, y);
        Ficha ficha = fichaObj.GetComponent<Ficha>();

        // Si el movimiento al que se quiere ir está en un enemigo.
        if (ficha.compararMuchasFichas(this.enemigos))
        {
            return false; // No es un movimiento válido.
        }

        if (ficha.compararFicha(this.jugador))
        {
            return false;
        }

        if (ficha.compararFicha(this.meta))
        {
            return false;
        }

        return true; // Movimiento válido.
    }

    /* *******************************************************
     * Se seleccionan las posiciones para asignar los enemigos.
     * Se deben haber seleccionado previamente la meta y el inicio.
     *********************************************************/
    public void seleccionarEnemigos()
    {
        //this.enemigos = new Enemigo[numeroEnemigos];

        // Creo N número de enemigos.
        for (int i = 0; i < this.enemigos.Length; i++)
        {
            GameObject objeto = obtenerficha();
            Ficha fichaTemp = objeto.gameObject.GetComponent<Ficha>();
            // La ficha no puede ser la meta o el jugador.
            while (fichaTemp.compararFicha(this.meta) || fichaTemp.compararFicha(this.jugador)
                || fichaTemp.compararMuchasFichas(this.enemigos))
            {
                objeto = obtenerficha();
                fichaTemp = objeto.gameObject.GetComponent<Ficha>();
            }
            Vector3 pos = objeto.transform.position;
            GameObject enemy = Instantiate(prefabEnemigos, pos, transform.rotation) as GameObject;
            enemy.GetComponent<Ficha>().setLocation(fichaTemp.getX(), fichaTemp.getY());
            enemy.GetComponent<Enemy>().inicializar();
            this.enemigos[i] = enemy;
        }
    }

    /* Se crea el tablero. */
    public void crearTablero()
    {
        int cont = 0;
        for (int i = 0; i < this.filas; i++)
        {
            float posY = transform.position.y - (i * (float) this.distanciaY);
            for (int j = 0; j < this.columnas; j++)
            {;
                float posX = transform.position.x + (j * this.distanciaX);
                float posZ = transform.position.z;

                Vector3 pos = new Vector3(posX, posY, posZ);
  
                GameObject fichaNueva = Instantiate(prefab, pos, transform.rotation) as GameObject;
                Ficha ficha = fichaNueva.gameObject.GetComponent<Ficha>();
                ficha.setLocation(i, j);
                ficha.setScale(this.A); // Haga la ficha con la escala que tenga A.
                fichaNueva.transform.parent = transform;
                transform.GetChild(cont + 1).transform.localScale = 
                    new Vector3(ficha.getScale()[0], ficha.getScale()[1], fichaNueva.transform.localScale.z);
                tablero[i, j] = fichaNueva;
                cont++;
            }
        }
    }

    /* Obtengo una ficha del tablero. */
    public GameObject getFicha(int x, int y)
    {
        return this.tablero[x, y];
    }

    /* Obtener la meta */
    public GameObject getMeta()
    {
        return this.meta;
    }

    /* Obtener el jugador */
    public GameObject getJugador()
    {
        return this.jugador;
    }

    /* Obtener los enemigos */
    public GameObject[] getEnemigos()
    {
        return this.enemigos;
    }

    /* Obtener las filas */
    public int obtenerFilas()
    {
        return this.filas;
    }

    /* Obtener las columnas */
    public int obtenerColumnas()
    {
        return this.columnas;
    }

    /* Configure la ruta más óptima para el tablero. */
    public void setRuta(List<Ficha> ruta)
    {
        this.rutaOptima = ruta;
    }

    /* Dibujar huellas... */
    public void dibujarRutaOptima()
    {
        int tamaño = this.rutaOptima.Count;
        rutaHuellasOptimas = new GameObject[tamaño];

        for(int i = 1; i < tamaño - 1; i++)
        {
            Ficha ficha = rutaOptima[i];
            GameObject posicion = getFicha(ficha.getX(), ficha.getY());
            rutaHuellasOptimas[i] = Instantiate(huellas, posicion.transform.position, posicion.transform.rotation) as GameObject;
        }
    }

    /* Eliminar la ruta óptima. */
    public void eliminarRutaOptima()
    {
        for(int i = 0; i < this.rutaOptima.Count; i++)
        {
            Destroy(rutaHuellasOptimas[i]);
        }
        rutaHuellasOptimas = null;
        rutaOptima.Clear();
    }

    /* Obtengo una ficha al azar del tablero. */
    public GameObject obtenerficha()
    {
        System.Random r = new System.Random();
        int x = r.Next(0, this.filas);
        int y = r.Next(0, this.columnas);
        GameObject fichaSeleccionada = tablero[x, y];
        return fichaSeleccionada;
    }

    // Update is called once per frame
    void Update () {
		if (jugador != null && meta != null)
        {
            Ficha jugadorFicha = jugador.GetComponent<Ficha>();
            if (jugadorFicha != null)
            {
                if (jugadorFicha.compararFicha(meta) && stateReproducir == false)
                {
                    audioWin.Play();
                    stateReproducir = true;
                }
            }
        }
	}

    /* Configuramos las distancias con respecto a nuestro a. */
    public void setDistancias(int a)
    {
        float distanceX = 7;
        float distanceY = 0;
        if (a == 1)
        { 
            distanceY = 6;
        }else
        {
            distanceY = (float) 5.5;
        }

        float disX = 2 * (a - 1);
        float disY = (float) 1.5 * (a - 1);

        this.distanciaX = distanceX + disX;
        this.distanciaY = distanceY + disY;
    }

    /* Obtener la ruta óptima de A estrella. */
    public List<Ficha> obtenerRuta()
    {
        return this.rutaOptima;
    }

    /* Destruye el tablero de juego. */
    public void eliminarTablero()
    {
        for(int i = 0; i < this.filas; i++)
        {
            for(int j = 0; j < this.columnas; j++)
            {
                Destroy(tablero[i, j]);
            }
        }
    }

}
