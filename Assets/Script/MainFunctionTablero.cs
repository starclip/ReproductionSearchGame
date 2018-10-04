using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class MainFunctionTablero : MonoBehaviour
{

    public GameObject mValue;
    public GameObject nValue;
    public GameObject aValue;

    public GameObject mSound;
    public GameObject nSound;
    public GameObject aSound;
    public GameObject nextSound;
    public GameObject errorSound;

    private AudioSource mAudio;
    private AudioSource nAudio;
    private AudioSource aAudio;
    private AudioSource nextAudio;
    private AudioSource errorAudio;

    private int m;
    private int n;
    private int a;

    private bool stateOne;
    private bool stateTwo;
    private bool stateThree;

    private DictationRecognizer dictator;

    // Use this for initialization
    void Start()
    {
        awake();
    }

    void awake()
    {
        m = 0;
        n = 0;
        a = 0;
        stateOne = false;
        stateTwo = false;
        stateThree = false;

        mValue = Instantiate(mValue, mValue.transform.position, mValue.transform.rotation) as GameObject;
        nValue = Instantiate(nValue, nValue.transform.position, nValue.transform.rotation) as GameObject;
        aValue = Instantiate(aValue, aValue.transform.position, aValue.transform.rotation) as GameObject;

        mAudio = mSound.GetComponent<AudioSource>();
        nAudio = nSound.GetComponent<AudioSource>();
        aAudio = aSound.GetComponent<AudioSource>();
        nextAudio = nextSound.GetComponent<AudioSource>();
        errorAudio = errorSound.GetComponent<AudioSource>();
    }

    /* Reproducir audio... */
    public void playAudio(DictationRecognizer dictator, AudioSource audio)
    {
        this.dictator = dictator;
        if (audio == null)
        {
            StartCoroutine(play(mAudio));
        }
        else
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

    /* Convierte el número en válido. */
    private int convertNumber(string possible)
    {
        try
        {
            int numb = int.Parse(possible);
            return numb;
        }
        catch (Exception e)
        {
            return -1;
        }
    }

    /* A la hora de iniciar diccionario. */
    public void iniciarDiccionario(string texto)
    {
        if (texto == "one")
        {
            texto = "1";
        }
        if (stateOne == false)
        {
            int numb = convertNumber(texto);
            if (numb <= -1)
            {
                // Imprimir que el número no es válido.
                playAudio(this.dictator, errorAudio);
                return;
            }
            else if (numb > 1000)
            {
                // Imprimir que el número es demasiado alto.
                playAudio(this.dictator, errorAudio);
                return;
            }

            this.m = numb;
            stateOne = true;

            mValue.GetComponent<TextMesh>().text = texto;

            playAudio(this.dictator, nAudio);
            return;
        }

        if (stateTwo == false)
        {
            int numb = convertNumber(texto);
            if (numb <= -1)
            {
                // Imprimir que el número no es válido.
                playAudio(this.dictator, errorAudio);
                return;
            }
            else if (numb > 1000)
            {
                // Imprimir que el número es demasiado alto.
                playAudio(this.dictator, errorAudio);
                return;
            }

            this.n = numb;
            stateTwo = true;
            nValue.GetComponent<TextMesh>().text = texto;
            playAudio(this.dictator, aAudio);
            return;
        }

        if (stateThree == false)
        {
            int numb = convertNumber(texto);
            if (numb <= -1)
            {
                // Imprimir que el número no es válido.
                playAudio(this.dictator, errorAudio);
                return;
            }
            else if (numb > 15)
            {
                // Imprimir que el número es demasiado alto.
                playAudio(this.dictator, errorAudio);
                return;
            }

            this.a = numb;
            stateThree = true;
            // Reproducir el siguiente audio.

            playAudio(this.dictator, nextAudio);

            aValue.GetComponent<TextMesh>().text = texto;
            return;
        }
    }

    public int getA()
    {
        return this.a;
    }

    public int getM()
    {
        return this.m;
    }

    public int getN()
    {
        return this.n;
    }

    public void reset()
    {
        this.stateOne = false;
        this.stateTwo = false;
        this.stateThree = false;

        this.m = 0;
        this.n = 0;
        this.a = 0;

        this.aValue.GetComponent<TextMesh>().text = "0";
        this.mValue.GetComponent<TextMesh>().text = "0";
        this.nValue.GetComponent<TextMesh>().text = "0";
    }

    // Update is called once per frame
    void Update()
    {

    }
}

