using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEstrella : MonoBehaviour {

    private int rows;
    private int cols;
    private int distanciaRecorrida;
    private System.TimeSpan tiempoAlgoritmo;
    private Heap listaBuscados;
    private List<Ficha> listaInsertados;
    private List<Ficha> listaCompletados;
    public GameObject tableroJuego;
    private Tablero campoJuego;
    public GameObject prefabFicha;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /* Función que me inicializa las variables y permite llamar al algoritmo varias veces. */
    public void inicializar()
    {
        this.campoJuego = tableroJuego.GetComponent<Tablero>();
        this.listaBuscados = new Heap();
        this.listaCompletados = new List<Ficha>();
        this.listaInsertados = new List<Ficha>();
        this.rows = campoJuego.obtenerFilas();
        this.cols = campoJuego.obtenerColumnas();
        this.distanciaRecorrida = 0;
    }

    /* Revise si ya había hecho ese movimiento previamente. */
    private bool compararPrevios(GameObject posActual, GameObject nuevoHijo)
    {
        Ficha temp = posActual.GetComponent<Ficha>().getPrevious();
        while (temp != null)
        {
            // Compare si la ficha está en la misma posición que la ficha anterior.
            if (temp.compararFicha(nuevoHijo))
            {
                return true;
            }
            temp = temp.getPrevious();
        }
        return false;
    }

    /* Compare si llegue a la meta. */
    private bool compararMeta(int x, int y)
    {
        GameObject meta = this.campoJuego.getMeta();
        Ficha fichaTemp = this.campoJuego.getFicha(x, y).GetComponent<Ficha>();

        // Si es la meta..
        if (fichaTemp.compararFicha(meta))
        {
            return true;
        }
        return false;
    }

    /* Mueva la ficha dependiendo de las posiciones X y Y. */
    private void moverFicha(int posX, int posY, Ficha posActual, Ficha nuevaFicha)
    {
        if (posX == posActual.getX() || posY == posActual.getY())
        {
            nuevaFicha.advanceNormal(campoJuego.getCostoLateral());
        }else
        {
            nuevaFicha.advanceDiagonal(campoJuego.getCostoDiagonal());
        }
    }

    /* Compare si un movimiento es válido. */
    private bool movimientoValidoDiagonal(GameObject posActual, int x, int y)
    {
        Ficha fichaActual = posActual.GetComponent<Ficha>();

        bool state = movimientoValido(posActual, x, y);

        if (state == false)
        {
            return false;
        }

        /* Revise si los vecinos no permiten el movimiento. */
        if (isDiagonalMovement(fichaActual.getX(), fichaActual.getY(), x, y))
        {
            state = diagonalValidMovement(posActual, x, y);
            if (state == false)
            {
                return false;
            }
        }

        return true; // Movimiento válido.
    }

    /* Compare si un movimiento es válido. */
    private bool movimientoValido(GameObject posActual, int x, int y)
    {
        GameObject[] enemigos = this.campoJuego.getEnemigos();
        GameObject fichaObj = this.campoJuego.getFicha(x, y);
        Ficha ficha = fichaObj.GetComponent<Ficha>();

        if (ficha.compararMuchasFichas(enemigos))
        {
            return false; // No es un movimiento válido.
        }

        if (ficha.compararListasFichas(listaInsertados.ToArray()))
        { 
            return false; // No es un movimiento válido.
        }

        // Si ya lo había insertado previamente.
        if (compararPrevios(posActual, fichaObj))
        {
            return false; // No es un movimiento válido.
        }

        return true; // Movimiento válido.
    }

    /* Verifique si es un movimiento diagonal. */
    private bool isDiagonalMovement(int x1, int y1, int x2, int y2)
    {
        int distOne = Mathf.Abs(x1 - x2);
        int distTwo = Mathf.Abs(y1 - y2);

        if (distOne == 1 && distTwo == 1)
        {
            return true;
        }
        return false;
    }

    /* Revisa si un movimiento en diagonal no es válido. */
    private bool diagonalValidMovement(GameObject posActual, int x, int y)
    {
        GameObject[] enemigos = this.campoJuego.getEnemigos();
        Ficha fuente = posActual.GetComponent<Ficha>();
        Ficha objetivo = campoJuego.getFicha(x, y).GetComponent<Ficha>();
        Ficha posibleEnemyOne = campoJuego.getFicha(fuente.getX(), objetivo.getY()).GetComponent<Ficha>();
        Ficha posibleEnemyTwo = campoJuego.getFicha(objetivo.getX(), fuente.getY()).GetComponent<Ficha>();

        // Si las dos son enemigos, no se tomarán en cuenta este camino...
        if (posibleEnemyOne.compararMuchasFichas(enemigos) && posibleEnemyTwo.compararMuchasFichas(enemigos))
        {
            return false;
        }

        return true;
    }

    /* *******************************************************
    * Obtengo todos los posibles movimientos de una ficha.
    * Se recibe la ficha a la que desea saber los posibles movimientos.
    * Solo se permiten movimientos laterales (arriba, abajo, derecha e izquierda).
    * Agrega a la lista de buscados.
    *********************************************************/
    private void obtenerAliadosLaterales(GameObject posActual)
    {
        GameObject meta = this.campoJuego.getMeta();
        Ficha fichaActual = posActual.GetComponent<Ficha>();
        int x = fichaActual.getX();
        int y = fichaActual.getY();
        int[,] positions = new int[4, 2]
        {
            { x-1, y },
            { x+1, y },
            { x, y-1 },
            { x, y+1 }};

        // Realice cuatro veces por las diagonales de las fichas.
        for(int pos = 0; pos < 4; pos++)
        {
            int posX = positions[pos, 0];
            int posY = positions[pos, 1];

            // Si no se sale del tablero y además no se evalua la misma ficha.
            if ((posX >= 0 && posX < this.rows) && (posY >= 0 && posY < this.cols) &&
                (fichaActual.getX() != posX || fichaActual.getY() != posY))
            {
                // Compare el tablero en [i, j];
                if (movimientoValido(posActual, posX, posY))
                {
                    // Tengo que crear una nueva ficha.
                    GameObject game = Instantiate(prefabFicha, transform.position, transform.rotation) as GameObject;
                    game.GetComponent<Renderer>().enabled = false; // Hagalo invisible.
                    game.transform.parent = transform;
                    Ficha nuevaFicha = game.GetComponent<Ficha>();
                    nuevaFicha.setLocation(posX, posY); // La nueva ficha tiene la misma locación.
                    nuevaFicha.setPrevious(fichaActual); // la nueva ficha precede a la posición actual.
                    nuevaFicha.setCostoManhattan(meta.GetComponent<Ficha>());
                    moverFicha(posX, posY, fichaActual, nuevaFicha);
                    listaBuscados.insert(nuevaFicha);
                }
            }
        }
    }

    /* *******************************************************
     * Obtengo todos los posibles movimientos de una ficha.
     * Se recibe la ficha a la que desea saber los posibles movimientos.
     * Se permiten movimientos laterales y diagonales.
     * Agrega a la lista de buscados.
    *********************************************************/
    private void obtenerAliadosDiagonal(GameObject posActual)
    {
        GameObject meta = this.campoJuego.getMeta();
        Ficha fichaActual = posActual.GetComponent<Ficha>();
        int x = fichaActual.getX() - 1;
        int y = fichaActual.getY() - 1;

        for(int posX = x; posX < (x + 3); posX++)
        {
            for(int posY = y; posY < (y + 3); posY++)
            {
                // Si no se sale del tablero y además no se evalua la misma ficha.
                if ((posX >= 0 && posX < this.rows) && (posY >= 0 && posY < this.cols)
                    && (fichaActual.getX() != posX || fichaActual.getY() != posY))
                {
                    // Si es un movimiento válido.
                    if (movimientoValidoDiagonal(posActual, posX, posY))
                    {
                        // Tengo que crear una nueva ficha.
                        GameObject game = Instantiate(prefabFicha, transform.position, transform.rotation) as GameObject;
                        game.GetComponent<Renderer>().enabled = false; // Hagalo invisible.
                        game.transform.parent = transform;
                        Ficha nuevaFicha = game.GetComponent<Ficha>();
                        nuevaFicha.setLocation(posX, posY); // La nueva ficha tiene la misma locación.
                        nuevaFicha.setPrevious(fichaActual); // la nueva ficha precede a la posición actual.
                        nuevaFicha.setCostoManhattan(meta.GetComponent<Ficha>());
                        moverFicha(posX, posY, fichaActual, nuevaFicha);
                        listaBuscados.insert(nuevaFicha);
                    }
                }
            }
        }
    }

    /* Inserto los valores de la ruta óptima. */
    private void seleccionarRutaOptima(GameObject posActual)
    {
        Ficha fichaActual = posActual.GetComponent<Ficha>();
        while(fichaActual != null)
        {
            this.listaCompletados.Insert(0, fichaActual);
            fichaActual = fichaActual.getPrevious();
        }
    }

    /* Obtener el tiempo de duración del algoritmo. */
    public System.TimeSpan getTime()
    {
        return this.tiempoAlgoritmo;
    }

    /* Obtener la ruta con los mejores resultados. */
    public List<Ficha> obtenerMejorRuta()
    {
        return this.listaCompletados;
    }

    /* Obtengo el número de evaluaciones pendientes que realizó el algoritmo. */
    public int obtengaPendientes()
    {
        return listaBuscados.getCantidad();
    }

    /* Obtenga el número de evaluaciones que realizó el algoritmo. */
    public int obtengaEvaluados()
    {
        return listaInsertados.Count;
    }

   /* Otenga el número de cambios de un mismo valor que realizó el algoritmo */
   public int getCambios()
    {
        return this.listaBuscados.getCambios();
    }

    /* Obtenga el costo total del algoritmo. */
    public int obtenerDistanciaTotal()
    {
        return this.distanciaRecorrida;
    }

    /* Realiza el algoritmo estrella. */
    public bool accion(bool diagonales)
    {
        GameObject jugador = this.campoJuego.getJugador();
        Ficha fichaJugador = jugador.GetComponent<Ficha>();
        listaInsertados.Insert(0, fichaJugador);
        
        System.DateTime tiempoInicial = System.DateTime.Now;
        /* Compare si llegue a la meta. O sea ¿Inicio == Final? */
        while(compararMeta(fichaJugador.getX(), fichaJugador.getY()) == false)
        {
            if (diagonales)
            {
                obtenerAliadosDiagonal(jugador);
            }else
            {
                obtenerAliadosLaterales(jugador);
            }

            Ficha mejorFicha = listaBuscados.getFicha();

            // Si no hay rutas óptimas
            if (mejorFicha == null)
            {
                return false;
            }

            jugador = campoJuego.getFicha(mejorFicha.getX(), mejorFicha.getY()); // Obtengo la ficha del tablero.
            fichaJugador = jugador.GetComponent<Ficha>(); // Obtengo su respectivo script de ficha
            // Lo reescribo con los datos almacenados
            fichaJugador.updateData(mejorFicha.getCostoRuta(), mejorFicha.getPrevious(), mejorFicha.getCostoManhattan());
            listaInsertados.Insert(0, fichaJugador);
        }

        System.DateTime tiempoFinal = System.DateTime.Now;

        tiempoAlgoritmo = new System.TimeSpan(tiempoFinal.Ticks - tiempoInicial.Ticks);

        this.distanciaRecorrida = fichaJugador.getCostoRuta();
        seleccionarRutaOptima(jugador);

        return true; // Todo salío bien.
    }
}
