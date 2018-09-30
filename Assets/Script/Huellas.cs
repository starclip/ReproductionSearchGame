using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Huellas : MonoBehaviour {

    public GameObject huella0;
    public GameObject huella1;
    public GameObject huella2;
    public GameObject huella3;
    public GameObject huella4;
    private GameObject[] huellas;
    private int position;
    private bool state;
    private int cont;
	// Use this for initialization
	void Start () {
        huellas = new GameObject[5] { huella0, huella1, huella2, huella3, huella4 };
        position = 0;
        cont = 0;
        state = true;
        foreach(GameObject huella in huellas)
        {
            huella.GetComponent<Renderer>().enabled = false;
        }
	}

    private void changeState()
    {
        cont++;

        if (cont == 100)
        {
            cont = 0;
            state = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (state)
        {
            GameObject selected = huellas[position];
            selected.GetComponent<Renderer>().enabled = true;
            state = false;
            position++;

            if (position == huellas.Length)
            {
                position = 0;
                foreach (GameObject huella in huellas)
                {
                    huella.GetComponent<Renderer>().enabled = false;
                }
            }
        }

        changeState();
	}
}
