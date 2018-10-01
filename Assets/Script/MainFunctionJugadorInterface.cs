using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows.Speech;
using System.Linq;

public class MainFunctionJugadorInterface : MonoBehaviour {

    public GameObject textoJugador;
    /* Reconocedor de voz. */
   
  
    private bool detectarNombreJugador = false;
    private string playerName = "semento";

    /* Inicializo el diccionario con las únicas dos palabras que se van a utilizar. */
   public void iniciarDiccionario(string texto)
    {
        switch (texto)
        {
            case "my name":
                nombrarJugador();
                break;
            case "name player":
                nombrarJugador();
                break;

            case "player":
                nombrarJugador();
                break;

        }
    }

    // Use this for initialization
    void Start () {
        
    }


    // Update is called once per frame
    void Update () {
		
	}

    /* Función invocada cuando se quiere nombrar al jugador. */
    private void nombrarJugador()
    {
        detectarNombreJugador = true;
    }


    /* Función si ya nombre al jugador. */
    private void finalizarNombrar()
    {
        //PhraseRecognitionSystem.Shutdown();
        //this.dictationRecognizer.Stop();

        //this.keywordRecognizer.Start();
        detectarNombreJugador = false;
    }

   public void dictationResult(string text)
    {
        //dictationRecognizer.Start();
        Debug.Log(text);
        //texto.text = text;

        if (detectarNombreJugador)
        {
            obtenerNombreJugador(text);
        }
        else
        {
            iniciarDiccionario(text);
        }

 
    }

    private void obtenerNombreJugador(string text)
    {
        this.playerName = text;
        //nombre que se va a enviar
        textoJugador.GetComponent<TextMesh>().text = text;
        finalizarNombrar();
    }

    public string getPlayerName()
    {
        return this.playerName;
    }
   
}
