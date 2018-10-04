using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Windows.Speech;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    private GameObject mainFunctionFirstInterface;
    private GameObject mainFunctionJugadorInterface;
    private GameObject mainFunction;
    private GameObject mainFunctionTablero;

    private int scene; //numero de la escena en la que estoy.
    private DictationRecognizer dictationRecognizer; // Descifrador de texto.

    private AssetBundle loader;
    private string[] scenes;

    MainFunctionFirstInterface escena1;
    MainFunctionJugadorInterface escena2;
    MainFunction escena4;
    MainFunctionTablero escena3;

    private bool definidaEscena1;
    private bool definidaEscena2;
    private bool definidaEscena3;
    private bool definidaEscena4;

    /* Variables que van a ser usadas para cambiar datos en la interfaz más adelante. */
    private string playerName; // Nombre del jugador.
    private int filas; // Número de filas.
    private int columnas; // Número de columnas;
    private int a;

    // Use this for initialization
    void Start() {
        this.scene = 1;
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;
        dictationRecognizer.Start();

        definidaEscena1 = false;
        definidaEscena2 = false;
        definidaEscena3 = false;
        definidaEscena4 = false;
        scenes = new string[4] { "PrimerEscena", "EscenaJugador", "EscenaTablero", "EscenaPrincipal" };
        seleccionarEscena(); // Seleccionó la primera escena del juego.
    }

    // Update is called once per frame
    void Update() {
 
    }

    /* Evite que el juego reinicie el dictation. */
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    /* Selecciona el objeto que desea cargar a la escena. */
    private void seleccionarEscena()
    {
        if (scene == 1 && definidaEscena1 == false)
        {
            this.mainFunctionFirstInterface = GameObject.Find("MainObjectFirstInterface");
            this.escena1 = mainFunctionFirstInterface.GetComponent<MainFunctionFirstInterface>();
            this.escena1.playAudio(this.dictationRecognizer);
            this.definidaEscena1 = true;
        }
        else if (scene == 2 && definidaEscena2 == false)
        {
            this.mainFunctionJugadorInterface = GameObject.Find("MainObjectInt");
            this.escena2 = mainFunctionJugadorInterface.GetComponent<MainFunctionJugadorInterface>();
            this.escena2.playAudio(this.dictationRecognizer, null);
            this.definidaEscena2 = true;
        } else if (scene == 3 && definidaEscena3 == false)
        {
            this.mainFunctionTablero = GameObject.Find("MainFunctionTableroInterface");
            this.escena3 = mainFunctionTablero.GetComponent<MainFunctionTablero>();
            this.escena3.playAudio(this.dictationRecognizer, null);
            this.definidaEscena3 = true;
        }else if (scene == 4 && definidaEscena4 == false)
        {
            this.mainFunction = GameObject.Find("MainObject");
            this.escena4 = mainFunction.GetComponent<MainFunction>();
            this.escena4.defineDirector(this.dictationRecognizer);
            this.escena4.setFilas(this.filas, this.columnas, this.a);
            this.escena4.crearTablero();
            this.definidaEscena4 = true;
        }
    }

    /* Crea el diccionario para distinguir las palabras que envía. */
    private void crearDiccionarioEscena1(string text)
    {
        switch (text)
        {
            case "start":
                this.escena1.iniciarJuego();
                cargarEscena();
                break;

            case "next":
                this.escena1.iniciarJuego();
                cargarEscena();
                break;

            case "leave":
                salirJuego();
                break;
        }
    }

    /* Crea el diccionario para distinguir las palabras que envía. */
    private void crearDiccionarioEscena2(string text)
    {
        if (text.Equals("next") || text.Equals("continue") || text.Equals("go ahead"))
        {
            this.playerName = this.escena2.getPlayerName();
            cargarEscena();
        }
        else
        {
            escena2.dictationResult(text);
        }
    }


    /* Crea el diccionario para distinguir las palabras que envía hacia la escena 3.*/
    private void crearDiccionarioEscena3(string text)
    {
        switch (text)
        {
            case "next":
                comprobarVariable();
                cargarEscena();
                this.escena3.reset();
                return;

            case "leave":
                salirJuego();
                return;

            case "reset":
                this.escena3.reset();
                return;
        }

        // Si no es ni next... ni leave... significo que puede haber sido una palabra.
        this.escena3.iniciarDiccionario(text);
    }

    /* Crea el diccionario para la escena 4 donde está el juego en progreso. */
    private void crearDiccionarioEscena4(string text)
    {
        switch (text)
        {
            case "create box":
                cargarEscena();
                return;

            case "leave":
                salirJuego();
                return;
        }

        this.escena4.iniciarDiccionario(text);
    }

    /* Función que comprueba si las variables que se procesaron tienen valores reales. */
    private void comprobarVariable()
    {
        int m = this.escena3.getM();
        int n = this.escena3.getN();
        int a = this.escena3.getA();

        if (a == null || a == 0)
            this.a = 3;
        else
            this.a = a;

        if (n == null || n == 0)
            this.columnas = 10;
        else
            this.columnas = n;

        if (m == null || m == 0)
            this.filas = 10;
        else
            this.filas = m;
        
    }

    /* Carga la escena del juego. */

    private void cargarEscena()
    {
        // Escena del inicio del juego salta a seleccionar jugador.
        if (this.scene == 1)
        {
            definidaEscena1 = false;
            SceneManager.LoadScene(this.scenes[1]);
            this.scene = 2;
            StartCoroutine(seleccionar());
            
        }
        // Escena de seleccionar salta a crear tablero.
        else if (this.scene == 2)
        {
            definidaEscena2 = false;
            SceneManager.LoadScene(this.scenes[2]);
            this.scene = 3;
            StartCoroutine(seleccionar());
        }
        // Escena de crear tablero salta a jugar... (juego).
        else if (this.scene == 3)
        {
            definidaEscena3 = false;
            SceneManager.LoadScene(this.scenes[3]);
            this.scene = 4;
            StartCoroutine(seleccionar());
        }
        else if (this.scene == 4)
        {
            definidaEscena4 = false;
            SceneManager.LoadScene(this.scenes[2]);
            this.scene = 3;
            StartCoroutine(seleccionar());
        }

            
    }
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        UnityEngine.Debug.Log(text);

        if (scene == 1)
        {
            crearDiccionarioEscena1(text);
        }
        else if (scene == 2)
        {
            crearDiccionarioEscena2(text);
        }
        else if (scene == 3)
        {
            crearDiccionarioEscena3(text);
        }
        else
        {
            crearDiccionarioEscena4(text);
        }
    }

    /* Cargue la pantalla, realice un delay de 1 segundo para que cargue la nueva escena.*/
    private IEnumerator seleccionar()
    {
        yield return new WaitForSeconds(1);
        seleccionarEscena();
    }

 

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        // 
    }

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        //Debug.Log("Cause: " + cause.ToString());
        StartCoroutine(esperarDictation());
    }

    IEnumerator esperarDictation()
    {
        //Debug.Log("Desactivado");
        dictationRecognizer.Stop();
        yield return new WaitForSeconds(1);
        dictationRecognizer.Start();
        //Debug.Log("Activado");
    }

    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        // 
        Debug.Log("Error: " + error);
    }

    void OnDestroy()
    {

        dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
        dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
        dictationRecognizer.Dispose();
    }

    private void salirJuego()
    {
        OnDestroy(); // Destruya el detector de audio.
        Application.Quit(); // Cierre el juego.
    }
}
