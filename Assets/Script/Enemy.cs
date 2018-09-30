using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject bebe;
    private double turnSpeed;
    private bool direction;

	// Use this for initialization
	void Start () {
        System.Random r = new System.Random();
        turnSpeed = r.NextDouble() + 0.2;
        direction = getDirectionRandom(r);
	}

    public void inicializar()
    {
        System.Random r = new System.Random();
        turnSpeed = r.NextDouble() + 0.2;
        direction = getDirectionRandom(r);
    }
	
	// Update is called once per frame
	void Update () {
		if (direction)
        {
            bebe.transform.Rotate(Vector3.forward * (float)turnSpeed);
        }else
        {
            bebe.transform.Rotate(Vector3.forward * (float) - turnSpeed);
        }
	}

    // Obtenga de manera random si la dirección es hacia la derecha o izquierda.
    private bool getDirectionRandom(System.Random r)
    {
        int rand = r.Next(0, 2);
        if (rand == 0)
        {
            direction = false;
        }
        else
        {
            direction = true;
        }
        return direction;
    }
}
