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

    public GameObject sonidoBienvenida;
    private AudioSource audio;
    private DictationRecognizer dictation;

    // Use this for initialization
    void Start () {
        audio = sonidoBienvenida.GetComponent<AudioSource>();
    }

    IEnumerator play()
    {
        dictation.Stop();
        //Debug.Log("Desactivado");
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
        dictation.Start();
        //Debug.Log("Activado");
    }

    public void playAudio(DictationRecognizer dictation)
    {
        this.dictation = dictation;
        StartCoroutine(play());
    }

    // Update is called once per frame
    void Update () {
		
	}

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
