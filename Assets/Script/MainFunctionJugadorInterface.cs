using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows.Speech;
using System.Linq;

public class MainFunctionJugadorInterface : MonoBehaviour {

    public GameObject textoJugador;
    /* Reconocedor de voz. */
    private KeywordRecognizer keywordRecognizer; // Diccionario.
    private Dictionary<string, Action> keywords = new Dictionary<string, Action>(); // Crea un diccionario.
    private DictationRecognizer dictationRecognizer; // Descifrador de texto.

    /* Inicializo el diccionario con las únicas dos palabras que se van a utilizar. */
    private void iniciarDiccionario()
    {
        /* Nombrar al jugador. */
        keywords.Add("nombrar jugador", () => { nombrarJugador(); });
        keywords.Add("jugador", () => { nombrarJugador(); });

        /* Volver a la anterior pantalla. */
        keywords.Add("atras", () => { volver(); });
        keywords.Add("cancelar", () => { volver(); });

        /* Salir del juego. */
        keywords.Add("salir", () => { salirJuego(); });

        /* Continuar el juego. */
        keywords.Add("listo", () => { continuar(); });
        keywords.Add("continuar", () => { continuar(); });
        keywords.Add("proseguir", () => { continuar(); });
    }

    // Use this for initialization
    void Start () {
        /* Cree el keyword Recognizer.*/
        iniciarDiccionario();
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPraseRecognized;
        keywordRecognizer.Start();

        /* Cree el dictation que detecta conforme avance. */
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;
    }

    /* El keyword detecta los argumentos que le paso. */
    private void KeywordRecognizerOnPraseRecognized(PhraseRecognizedEventArgs args)
    {
        Action keywordAction;

        print(args.text);
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    /* Función invocada cuando se quiere nombrar al jugador. */
    private void nombrarJugador()
    {

        //if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
        // Reproducir sonido.
        PhraseRecognitionSystem.Shutdown();
        keywordRecognizer.Stop();
       
        dictationRecognizer.Start();
    }

    /* Función invoca el siguiente Scene. */
    private void continuar()
    {
        eliminarReconocimiento();
    }

    /* Función usada para salir del juego. */
    private void salirJuego()
    {
        eliminarReconocimiento();
    }

    /* Función usada para volver a la anterior escena. */
    private void volver()
    {
        eliminarReconocimiento();
    }

    /* Función usada para eliminar el reconocimiento de voz. */
    private void eliminarReconocimiento()
    {
        keywordRecognizer.Stop();
        keywordRecognizer.Dispose();
        this.destroyDictator();
    }

    /*
    private IEnumerator RestartSpeechSystem()
    {
        while(dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            yield return null;
        }
    }
    */

    /* Función si ya nombre al jugador. */
    private void finalizarNombrar()
    {
        PhraseRecognitionSystem.Shutdown();
        this.dictationRecognizer.Stop();

        this.keywordRecognizer.Start();
    }

    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        dictationRecognizer.Start();
        Debug.Log(text);
        //texto.text = text;

        // Habría que usar un text builder en esta parte.

        if (text == "listo" || text == "ready" || text == "finished" || text == "completed")
        {
            finalizarNombrar(); // Finalice y proceda a usar el phrase recognition.
        }

        textoJugador.GetComponent<TextMesh>().text = text;
    }

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        // 
    }

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        //dictationRecognizer.Stop ();
    }

    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        // 
    }


    void destroyDictator()
    {
        dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
        dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
        dictationRecognizer.Dispose();
    }
}
