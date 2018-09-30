using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap {

    private List<Ficha> listaBuscados;
    private int cambiosRealizados;

    public Heap()
    {
        this.listaBuscados = new List<Ficha>();
        cambiosRealizados = 0;
    }

    /* Inserta una ficha dependiendo de su costo (menores primero). */
    private void insertElement(Ficha ficha)
    {
        for(int i = 0; i < this.listaBuscados.Count; i++)
        {
            Ficha fichaActual = this.listaBuscados[i];
            // Si la nueva ficha tiene un costo menor.
            if (fichaActual.getCostoTotal() >= ficha.getCostoTotal())
            {
                this.listaBuscados.Insert(i, ficha);
                return;
            }
        }

        this.listaBuscados.Add(ficha); 
    }

    /* Inserta un elemento, primero revise si ya se insertó en la lista de búsqueda previamente. */
    public void insert(Ficha ficha)
    {
        for(int i = 0; i < this.listaBuscados.Count; i++)
        {
            Ficha fichaTemp = listaBuscados[i];
            // Si las dos fichas tienen la misma posición.
            if (ficha.compararF(fichaTemp))
            {
                // Si su costo es menor a la previa.
                if (ficha.getCostoTotal() < ficha.getCostoTotal())
                {
                    cambiosRealizados++;
                    listaBuscados.Remove(fichaTemp);
                    this.insertElement(ficha); // Inserte si su costo es menor
                }
                return;
            }
        }
        this.insertElement(ficha); // Inserte si no tiene coincidencias.
    }

    /* Retorna el número de cambios que se han hecho. */
    public int getCambios()
    {
        return this.cambiosRealizados;
    }

    /* Obtenga la ficha en la posición 0. */
    public Ficha getFicha()
    {
        if (this.listaBuscados.Count > 0)
        {
            Ficha ficha = this.listaBuscados[0];
            this.listaBuscados.Remove(ficha);
            return ficha;
        }
        return null;
    }

    /* Obtenga el número de evaluaciones que realizó el algoritmo. */
    public int getCantidad()
    {
        return this.listaBuscados.Count;
    }

    public void print()
    {
        string text = "Lista de buscados: ";
        foreach(Ficha f in this.listaBuscados)
        {
            text += " (" + f.getX() + "," + f.getY() + ") = " + f.getCostoTotal();
        }
        Debug.Log(text);
    }
}
