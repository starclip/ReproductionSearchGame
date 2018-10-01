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

    private int scene; //numero de la escena en la que estoy.
    private DictationRecognizer dictationRecognizer; // Descifrador de texto.

    private AssetBundle loader;
    private string[] scenes;

    MainFunctionJugadorInterface escena2;

    private string playerName;

    // Use this for initialization
    void Start () {
        this.scene = 1;
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;
        dictationRecognizer.Start();




        scenes = new string[3] { "PrimerEscena", "EscenaJugador", "EscenaPrincipal" };
     

        seleccionarEscena();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void printScenes()
    {
        for (int i = 0; i < this.scenes.Length; i++)
        {
            Debug.Log(scenes[i] + "\n");
        }
    }

    private void seleccionarEscena()
    {
        if (scene == 1)
        {
            mainFunctionFirstInterface = GameObject.Find("MainObjectFirstInterface");
        }
        else if (scene == 2)
        {
            mainFunctionJugadorInterface = GameObject.Find("MainObjectInt");
        }
        else
        {
            mainFunction = GameObject.Find("MainObject");
        }
    }

    private void crearDiccionarioEscenea1(string text)
    {
        MainFunctionFirstInterface escena1 = mainFunctionFirstInterface.GetComponent<MainFunctionFirstInterface>();
 
        switch (text)
        {
            case "start":
                escena1.iniciarJuego();
                cargarEscena();
                break;

            case "leave":
                escena1.salirJuego();
                break;
        }
    }

    private void crearDiccionarioEscenea2(string text)
    {
        this.escena2 = mainFunctionJugadorInterface.GetComponent<MainFunctionJugadorInterface>();
        if (text.Equals("next"))
        {
            this.playerName = this.escena2.getPlayerName();
            cargarEscena();
        }

        else
            escena2.dictationResult(text);
    }

    private void cargarEscena()
    {
        if (this.scene == 1)
        {
            SceneManager.LoadScene(this.scenes[1]);
            this.scene = 2;
        }

        else if (this.scene == 2)
        {
            SceneManager.LoadScene(this.scenes[2]);
            this.scene = 3;
        }

            
    }
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        // dictationRecognizer.Start();
        //Application.OpenURL("https://www.google.com/search?q=" + text);
        UnityEngine.Debug.Log(text);

        if (scene == 1)
        {
            crearDiccionarioEscenea1(text);
        }
        else if (scene == 2)
        {
            seleccionarEscena();
            crearDiccionarioEscenea2(text);
        }
        else
        {
            seleccionarEscena();
        }



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

    void OnDestroy()
    {

        dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
        dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
        dictationRecognizer.Dispose();
    }
}
