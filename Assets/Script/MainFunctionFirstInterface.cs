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
   


    // Use this for initialization
    void Start () {
        //iniciarDiccionario();
        //keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), confidence);
        //keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPraseRecognized;
        //keywordRecognizer.Start();
    }
	
	// Update is called once per frame
	void Update () {
		
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
    public void iniciarJuego()
    {
        //eliminarReconocimiento();
        Debug.Log("Iniciar Juego");
  
    }

    /* Salgo del juego... eliminó la actual scene. */
    public void salirJuego()
    {
       // eliminarReconocimiento();
        Debug.Log("Salir juego");
        Application.Quit(); // Salga del juego.
    }


}
