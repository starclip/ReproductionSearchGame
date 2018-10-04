using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows.Speech;
using System.Linq;
using System.Collections;

public class MainFunctionJugadorInterface : MonoBehaviour {

    public GameObject textoJugador;
    /* Reconocedor de voz. */
   
  
    private bool detectarNombreJugador = false;
    private string playerName = "semento";

    public GameObject nameSound;
    public GameObject sayNameSound;

    private AudioSource nameAudio;
    private AudioSource sayNameAudio;

    private AudioSource[] audios;

    private DictationRecognizer dictator;

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
        nameAudio = nameSound.GetComponent<AudioSource>();
        sayNameAudio = sayNameSound.GetComponent<AudioSource>();
    }

    /* Reproducir audio... */
    public void playAudio(DictationRecognizer dictator, AudioSource audio)
    {
        this.dictator = dictator;
        if (audio == null)
        {
            StartCoroutine(play(nameAudio));
        }else
        {
            StartCoroutine(play(audio));
        }
    }

    IEnumerator play(AudioSource audio)
    {
        dictator.Stop();
        //Debug.Log("Audio - Desactivado");
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
        dictator.Start();
        //Debug.Log("Audio - Activado");
    }


    // Update is called once per frame
    void Update () {
		
	}

    /* Función invocada cuando se quiere nombrar al jugador. */
    private void nombrarJugador()
    {
        detectarNombreJugador = true;
        //sayNameAudio.Play();
        playAudio(this.dictator, sayNameAudio);
    }


    /* Función si ya nombre al jugador. */
    private void finalizarNombrar()
    {
        
        detectarNombreJugador = false;
        //nameAudio.Play();
        playAudio(this.dictator, nameAudio);

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
