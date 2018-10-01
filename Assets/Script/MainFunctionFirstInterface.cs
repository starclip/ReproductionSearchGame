using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows.Speech;
using System.Linq;
using System.Threading;
using System.Text;
using UnityEngine.UI;


public class MainFunctionFirstInterface : MonoBehaviour {

    /* Reconocedor de voz. */
    //KeywordRecognizer keywordRecognizer;
    private DictationRecognizer dictationRecognizer;
    Dictionary<string, Action> keywords = new Dictionary<string, Action>(); // Crea un diccionario.

    public ConfidenceLevel confidence = ConfidenceLevel.Medium;
    private Text texto;

    /* Inicializo el diccionario con las únicas dos palabras que se van a utilizar. */
    private void iniciarDiccionario()
    {
        keywords.Add("start", () => { iniciarJuego(); });
        keywords.Add("out", () => { salirJuego(); });
    }

    // Use this for initialization
    void Start () {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;

        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;

        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

        dictationRecognizer.DictationError += DictationRecognizer_DictationError;

        dictationRecognizer.Start();
        //iniciarDiccionario();
        //keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), confidence);
        //keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPraseRecognized;
        //keywordRecognizer.Start();
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
       // dictationRecognizer.Start();
        //Application.OpenURL("https://www.google.com/search?q=" + text);
        UnityEngine.Debug.Log(text);

        switch(text)
        {
            case "start":
                iniciarJuego();
                break;

            case "leave":
                salirJuego();
                break;
        }
        //texto.text = text;
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
    /* El keyword detecta los argumentos que le paso. */
    /*private void KeywordRecognizerOnPraseRecognized(PhraseRecognizedEventArgs args)
    {
        Action keywordAction;
        print(args.text);

        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }*/

    /* Inicia el juego... saltó a la siguiente Scene. */
    private void iniciarJuego()
    {
        //eliminarReconocimiento();
        Debug.Log("Iniciar Juego");
    }

    /* Salgo del juego... eliminó la actual scene. */
    private void salirJuego()
    {
       // eliminarReconocimiento();
        Debug.Log("Salir juego");
        Application.Quit(); // Salga del juego.
    }

    private void eliminarReconocimiento()
    {
        //keywordRecognizer.Stop();
        //keywordRecognizer.Dispose();
    }

    void OnDestroy()
    {

        dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
        dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
        dictationRecognizer.Dispose();
    }
}
