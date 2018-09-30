using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine.UI;
using System.Threading;

public class ProcesserVoice : MonoBehaviour
{

    private DictationRecognizer dictationRecognizer;
    public GameObject textObject;
    private Text texto;
    void Start()
    {
        dictationRecognizer = new DictationRecognizer();

        //dictationRecognizer.InitialSilenceTimeoutSeconds = 5;  

        //dictationRecognizer.AutoSilenceTimeoutSeconds = 5;

        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;

        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;

        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

        dictationRecognizer.DictationError += DictationRecognizer_DictationError;
    }

    private void activateDictationRecognizer()
    {
        dictationRecognizer.Start();
    }

    private void desactivateDictationRecognizer()
    {
        dictationRecognizer.Stop();
    }

    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        dictationRecognizer.Start();
        UnityEngine.Debug.Log(text);
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


    void OnDestroy()
    {
        dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
        dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
        dictationRecognizer.Dispose();
    }
}
