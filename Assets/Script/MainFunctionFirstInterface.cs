using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows.Speech;
using System.Linq;

public class MainFunctionFirstInterface : MonoBehaviour {

    /* Reconocedor de voz. */
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, Action> keywords = new Dictionary<string, Action>(); // Crea un diccionario.

    /* Inicializo el diccionario con las únicas dos palabras que se van a utilizar. */
    private void iniciarDiccionario()
    {
        keywords.Add("iniciar", () => { iniciarJuego(); });
        keywords.Add("salir", () => { salirJuego(); });
    }

    // Use this for initialization
    void Start () {
        iniciarDiccionario();
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPraseRecognized;
        keywordRecognizer.Start();
    }
	
	// Update is called once per frame
	void Update () {
		
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

    /* Inicia el juego... saltó a la siguiente Scene. */
    private void iniciarJuego()
    {
        eliminarReconocimiento();
        Debug.Log("Iniciar Juego");
    }

    /* Salgo del juego... eliminó la actual scene. */
    private void salirJuego()
    {
        eliminarReconocimiento();
        Application.Quit(); // Salga del juego.
    }

    private void eliminarReconocimiento()
    {
        keywordRecognizer.Stop();
        keywordRecognizer.Dispose();
    }
}
