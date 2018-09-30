using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void camaraMuevete(string Movement)
    {
        float posX = transform.position.x;
        float posY = transform.position.y;

        if (Movement == "RIGHT")
        {
            posX += 10;
        }

        if (Movement == "LEFT")
        {
            posX -= 10;
        }

        if (Movement == "DOWN")
        {
            posY -= 10;
        }

        if (Movement == "UP")
        {
            posY += 10;
        }

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
