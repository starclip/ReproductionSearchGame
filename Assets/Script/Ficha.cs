using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ficha : MonoBehaviour {

    private int x, y;
    private int costoRuta;
    private Ficha previous;
    private float scaleX = (float) 0.05;
    private float scaleY = (float) 0.07;
    private int costoManhattan;
    private GameObject fichaSelected;

    // Use this for initialization
    void Start () {
        // Obtengo mi posicion actual.
        costoManhattan = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setScale(int distance)
    {
        float dateScale = (float) 0.015;
        this.scaleX = this.scaleX + dateScale * (distance - 1);
        this.scaleY = this.scaleY + dateScale * (distance - 1);
    }

    public float[] getScale()
    {
        float[] result = { this.scaleX, this.scaleY };
        return result;
    }


    public void updateData(int costoRuta, Ficha previous, int costoManhattan)
    {
        this.costoRuta = costoRuta;
        this.previous = previous;
        this.costoManhattan = costoManhattan;
    }

    /* Configurar la locación. */
    public void setLocation(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /* Asigne el valor previo. */
    public void setPrevious(Ficha previous)
    {
        this.previous = previous;
        this.costoRuta += previous.getCostoRuta();
    }

    /* Obtenga el valor previo. */
    public Ficha getPrevious()
    {
        return this.previous;
    }

    /* Se configura el costo de Manhattan basado con respecto al punto final. */
    public void setCostoManhattan(Ficha puntoFinal)
    {
        int cost1 = Math.Abs(puntoFinal.getX() - this.x);
        int cost2 = Math.Abs(puntoFinal.getY() - this.y);
        this.costoManhattan = cost1 + cost2;
    }

    /* Obtenga la coordenada X */
    public int getX()
    {
        return x;
    }

    /* Obtenga la coordenada Y */
    public int getY()
    {
        return y;
    }

    /* Avance en una diagonal. Aumente el valor de la diagonal. */
    public void advanceDiagonal(int costoDiagonal)
    {
        this.costoRuta += costoDiagonal;
    }

    /* Avance en una lateral. Aumente el valor de la lateral. */
    public void advanceNormal(int costoLateral)
    {
        this.costoRuta += costoLateral;
    }

    /* Compare si las coordenadas entre las fichas son equivalentes. */
    public Boolean compararFicha(GameObject f)
    {
        if (f == null)
        {
            return false;
        }

        Ficha ficha = f.GetComponent<Ficha>();

        if (this.x == ficha.getX() && this.y == ficha.getY())
        {
            return true;
        }
        return false;
    }

    /* Compare si las coordenadas entre las fichas son equivalentes. */
    public Boolean compararF(Ficha ficha)
    {
        if (ficha == null)
        {
            return false;
        }

        if (this.x == ficha.getX() && this.y == ficha.getY())
        {
            return true;
        }
        return false;
    }

    /* Compare las coordenadas de la ficha con los enemigos. */
    public Boolean compararMuchasFichas(GameObject[] fichas)
    {
        if (fichas == null)
        {
            return false;
        }

        // Por cada enemigo o bloque en el campo.
        foreach (GameObject individual in fichas)
        {
            if (individual == null)
            {
                return false; // No está en una casilla de un enemigo.
            }
            Ficha littleFicha = individual.GetComponent<Ficha>();
            if (this.x == littleFicha.getX() && this.y == littleFicha.getY())
            {
                return true; // Es un enemigo.
            }
        }

        return false; // No está en una casilla de un enemigo.
    }

    /* Compare las coordenadas con listas de fichas */
    public Boolean compararListasFichas(Ficha[] fichas)
    {
        if (fichas == null)
        {
            return false;
        }

        // Por cada ficha de la lista comparela a esta ficha actual.
        foreach (Ficha individual in fichas)
        {
            if (individual == null)
            {
                return false; // No son iguales :v
            }
            if (this.x == individual.getX() && this.y == individual.getY())
            {
                return true; // Son iguales
            }
        }
        return false; // No son las mismas casillas.
    }

    /* Obtener el costo de la ruta. */
    public int getCostoRuta()
    {
        return this.costoRuta;
    }

    /* Obtener el costo de manhattan. */
    public int getCostoManhattan()
    {
        return this.costoManhattan;
    }

    /* Obtener el costo total. f(n) = g(n) + h(n) */
    public int getCostoTotal()
    {
        return this.costoRuta + this.costoManhattan;
    }

}
