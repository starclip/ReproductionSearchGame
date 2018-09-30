using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador : MonoBehaviour {

    public GameObject player;
    public Animator animator;
    private float posStart;
    public float speed;
    public float turnSpeed;
    private double distanceX;
    private double distanceY;
    private string name;
    private Boolean change;
    private Boolean isRotating;
    private double posToMove;
    private double secondPosToMove;
    private String moveMent;
    private String[] posiblesMovimientos;
    private int pos;

	// Use this for initialization
	void Start () {
        this.change = false;
        setAnimation(false);
        this.isRotating = true;
        this.posStart = player.transform.rotation.eulerAngles.z;
        this.pos = 0;
    }

    /* Almacena en una lista los movimientos que guían a la ruta óptima. */
    public void obtenerPosiblesMovimientos(List<Ficha> rutaOptima)
    {
        if (rutaOptima == null)
        {
            Debug.Log("No hay ruta optima");
            return;
        }

        if (rutaOptima.Count == 0)
        {
            Debug.Log("No hay posibles movimientos");
            return;
        }

        posiblesMovimientos = new String[rutaOptima.Count - 1];

        Ficha puntoPartida = rutaOptima[0];
        for(int i = 1; i < rutaOptima.Count; i++)
        {
            Ficha puntoFinal = rutaOptima[i];
            // Obtengo movimiento de acuerdo a sus coordenadas.
            posiblesMovimientos[i - 1] = 
                almacenarMovimiento(puntoPartida.getX(), puntoPartida.getY(), puntoFinal.getX(), puntoFinal.getY());
            puntoPartida = puntoFinal;
        }
        this.pos = 0;
        String text = "";
        foreach(String name in posiblesMovimientos)
        {
            text += name + " - ";
        }
        Debug.Log(text);
    }

    /* Dados dos fichas encuentre cual es el movimiento apto. */
    private String almacenarMovimiento(int x1, int y1, int x2, int y2)
    {
        int difOne = x2 - x1;
        int difTwo = y2 - y1;
        String movement = "";

        if (difOne == -1 && difTwo == -1)
        {
            movement = "LEFTUP";
        }else if (difOne == 0 && difTwo == -1)
        {
            movement = "LEFT";
        }
        else if (difOne == 1 && difTwo == -1)
        {
            movement = "LEFTDOWN";
        }
        else if (difOne == -1 && difTwo == 0)
        {
            movement = "UP";
        }
        else if (difOne == 1 && difTwo == 0)
        {
            movement = "DOWN";
        }
        else if (difOne == -1 && difTwo == 1)
        {
            movement = "RIGHTUP";
        }
        else if (difOne == 0 && difTwo == 1)
        {
            movement = "RIGHT";
        }
        else if (difOne == 1 && difTwo == 1)
        {
            movement = "RIGHTDOWN";
        }

        return movement;
    }

    /* Se obtiene el siguiente movimiento de posibles movimientos. */
    public String obtenerMovimiento()
    {
        if (this.pos < this.posiblesMovimientos.Length)
        {
            String nextMove = this.posiblesMovimientos[pos];
            this.pos++;
            return nextMove;
        }
        return null;
    }

    /* Verifique si es un movimiento diagonal. */
    private bool isDiagonalMovement(int x1, int y1, int x2, int y2)
    {
        int distOne = Mathf.Abs(x1 - x2);
        int distTwo = Mathf.Abs(y1 - y2);

        if (distOne == 1 && distTwo == 1)
        {
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        decideMovement();
    }

    /* Cambia el estado de animación .. si hay una traslación. */
    private void setAnimation(Boolean state)
    {
        animator.SetBool("state", state);
    }

    /* Retorna el estado actual de cambio. */
    public bool getActualState()
    {
        return this.change;
    }

    /* Función que me permite decidir el movimiento del jugador. */
    private void decideMovement()
    {
        if (change)
        {
            // Primero rote..
            if (isRotating)
            {
                rotate();
            }
            else // Muevase...
            {
                movePlayer();
            }
        }
    }

    // Configura el nombre del agente.
    public void setName(string pName)
    {
        this.name = pName;
    }

    // Se obtiene el nombre del agente.
    public string getName()
    {
        return this.name;
    }

    /* Configure la distancia. */
    public void setDistances(float distanceX, float distanceY)
    {
        this.distanceX = (double)distanceX;
        this.distanceY = (double)distanceY;
    }

    /* Función que permite rotar al espermatozoide antes de moverse. */
    private void rotate()
    {
        float posFinal = posStart; // El punto final sería la posición inicial del espermatozoide.

        if (moveMent == "LEFT")
        {
           posFinal = posStart + 90; // 90 Grados...
        }

        if (moveMent == "RIGHT")
        {
            posFinal = posStart + 270; // 270 grados...
        }

        if (moveMent == "UP")
        {
            posFinal = posStart; // 0 Grados...
        }

        if (moveMent == "DOWN")
        {
            posFinal = posStart + 180; // 180 Grados...
        }

        if (moveMent == "RIGHTUP")
        {
            posFinal = posStart + 315; // 315 Grados (Noreste)
        }

        if (moveMent == "RIGHTDOWN")
        {
            posFinal = posStart + 225; // 225 Grados (Sureste)
        }

        if (moveMent == "LEFTUP")
        {
            posFinal = posStart + 45; // 45 Grados (Noroeste)
        }

        if (moveMent == "LEFTDOWN")
        {
            posFinal = posStart + 135; // 135 Grados (Suroeste)
        }

        // ya llegue a la posición indicada.
        if (player.transform.rotation.eulerAngles.z > (posFinal - 1) && player.transform.rotation.eulerAngles.z < (posFinal + 1))
        {
            player.transform.Rotate(Vector3.zero);
            isRotating = false;
        }
        else
        {
            // Gire hacia la derecha.
            if (player.transform.rotation.eulerAngles.z < (posFinal - 1))
            {
                player.transform.Rotate(Vector3.forward * (float)turnSpeed);
            }
            else // Gire hacia la derecha.
            {
                player.transform.Rotate(Vector3.forward * (float)-turnSpeed);
            }
        }
    }

    /* Permite el movimiento del jugador. */
    public void mover(string movement, bool state)
    {
        float posX = GetComponent<Rigidbody2D>().position.x;
        float posY = GetComponent<Rigidbody2D>().position.y;

        /* Se da el comando de dirigirse a la izquierda.  */
        if (movement == "LEFT")
        {
            this.moveMent = "LEFT";               // Muevase a la izquierda.
            change = true;                   // Se pasa a movimiento
            isRotating = true;               // Se activa la rotación previa.
            if (state)
            {
                setAnimation(true);              // Cambie la animación.
                this.posToMove = posX - distanceX;   // Posición final a llegar.
            }
            else
            {
                this.posToMove = posX;
            }
        }

        /* Se da el comando de dirigirse a la derecha. */
        if (movement == "RIGHT")
        {
            this.moveMent = "RIGHT";             // Muevase a la derecha.
            change = true;                  // Se pasa a movimiento
            isRotating = true;              // Se activa la rotación previa.
            if (state)
            {
                setAnimation(true);              // Cambie la animación.
                this.posToMove = posX + distanceX;   // Posición final a llegar.
            }
            else
            {
                this.posToMove = posX;
            }
        }

        /* Se da el comando de dirigirse arriba. */
        if (movement == "UP")
        {
            this.moveMent = "UP";                // Muevase hacia arriba.
            change = true;                  // Se pasa a movimiento
            isRotating = true;              // Se activa la rotación previa.
            if (state)
            {
                setAnimation(true);              // Cambie la animación.
                this.posToMove = posY + distanceY;   // Posición final a llegar.
            }
            else
            {
                this.posToMove = posY;
            }
        }

        /* Se da el comando de dirigirse hacia abajo. */
        if (movement == "DOWN")
        {
            this.moveMent = "DOWN";              // Muevase hacia abajo.
            change = true;                  // Se pasa a movimiento
            isRotating = true;              // Se activa la rotación previa.
            if (state)
            {
                setAnimation(true);              // Cambie la animación.
                this.posToMove = posY - distanceY;   // Posición final a llegar.
            }
            else
            {
                this.posToMove = posY;
            }
        }

        /* Se da el comando de dirigirse hacia la diagonal arriba - derecha. */
        if (movement == "RIGHTUP")
        {
            this.moveMent = "RIGHTUP";              // Muevase hacia abajo.
            change = true;                  // Se pasa a movimiento
            isRotating = true;              // Se activa la rotación previa.
            if (state)
            {
                setAnimation(true);              // Cambie la animación.
                this.posToMove = posX + distanceX;   // Posición final a llegar.
                this.secondPosToMove = posY + distanceY;
            }
            else
            {
                this.posToMove = posX;
                this.secondPosToMove = posY;
            }
        }

        /* Se da el comando de dirigirse hacia la diagonal abajo - derecha. */
        if (movement == "RIGHTDOWN")
        {
            this.moveMent = "RIGHTDOWN";              // Muevase hacia abajo.
            change = true;                  // Se pasa a movimiento
            isRotating = true;              // Se activa la rotación previa.
            if (state)
            {
                setAnimation(true);              // Cambie la animación.
                this.posToMove = posX + distanceX;   // Posición final a llegar.
                this.secondPosToMove = posY - distanceY;
            }
            else
            {
                this.posToMove = posX;
                this.secondPosToMove = posY;
            }
        }

        /* Se da el comando de dirigirse hacia la diagonal arriba - izquierda. */
        if (movement == "LEFTUP")
        {
            this.moveMent = "LEFTUP";              // Muevase en diagonal
            change = true;                  // Se pasa a movimiento
            isRotating = true;              // Se activa la rotación previa.
            if (state)
            {
                setAnimation(true);              // Cambie la animación.
                this.posToMove = posX - distanceX;   // Posición final a llegar.
                this.secondPosToMove = posY + distanceY;
            }
            else
            {
                this.posToMove = posX;
                this.secondPosToMove = posY;
            }
        }

        /* Se da el comando de dirigirse hacia la diagonal arriba - izquierda. */
        if (movement == "LEFTDOWN")
        {
            this.moveMent = "LEFTDOWN";              // Muevase en diagonal
            change = true;                  // Se pasa a movimiento
            isRotating = true;              // Se activa la rotación previa.
            if (state)
            {
                setAnimation(true);              // Cambie la animación.
                this.posToMove = posX - distanceX;   // Posición final a llegar.
                this.secondPosToMove = posY - distanceY;
            }
            else
            {
                this.posToMove = posX;
                this.secondPosToMove = posY;
            }
        }
    }

    /* Movimiento del jugador...  */
    private void movePlayer()
    {
        if (moveMent == "LEFT")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, GetComponent<Rigidbody2D>().velocity.y);

            // Si he alcanzado la posición a la que me quería mover en el eje Xfinal = Xstart.
            if (posToMove >= GetComponent<Rigidbody2D>().position.x)
            {
                change = false; // Se debe decidir tomar una nueva decisión.
                setAnimation(false);
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y); // Esté estático.
            }
        }

        if (moveMent == "RIGHT")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);

            // Si he alcanzado la posición a la que me quería mover en el eje Xstart = Xfinal.
            if (posToMove <= GetComponent<Rigidbody2D>().position.x)
            {
                change = false; // Se debe decidir tomar una nueva decisión.
                setAnimation(false);
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
            }
        }

        if (moveMent == "UP")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, speed);

            // Si he alcanzado la posición a la que me quería mover en el eje Ystart = Yfinal.
            if (posToMove <= GetComponent<Rigidbody2D>().position.y)
            {
                change = false;
                setAnimation(false);
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
            }
        }

        if (moveMent == "DOWN")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, -speed);

            // Si he alcanzado la posición a la que me quería mover en el eje Yfinal = Ystart.
            if (posToMove >= GetComponent<Rigidbody2D>().position.y)
            {
                change = false;
                setAnimation(false);
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
            }
        }

        if (moveMent == "RIGHTUP")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(speed, speed);

            // Si he alcanzado la posición a la que me quería mover en el eje Xfinal = Xstart.
            if (posToMove <= GetComponent<Rigidbody2D>().position.x || secondPosToMove <= GetComponent<Rigidbody2D>().position.y)
            {

                if (posToMove <= GetComponent<Rigidbody2D>().position.x && secondPosToMove > GetComponent<Rigidbody2D>().position.y)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
                }
                else if(secondPosToMove <= GetComponent<Rigidbody2D>().position.y && posToMove > GetComponent<Rigidbody2D>().position.x) 
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
                }else
                {
                    Debug.Log("He llegado");
                    change = false; // Se debe decidir tomar una nueva decisión.
                    setAnimation(false);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); // Esté estático.
                }
            }
        }

        if (moveMent == "RIGHTDOWN")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(speed, -speed);

            // Si he alcanzado la posición a la que me quería mover en el eje Xfinal = Xstart.
            if (posToMove <= GetComponent<Rigidbody2D>().position.x || secondPosToMove >= GetComponent<Rigidbody2D>().position.y)
            {

                if (posToMove <= GetComponent<Rigidbody2D>().position.x && secondPosToMove < GetComponent<Rigidbody2D>().position.y)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, -speed);
                }
                else if (secondPosToMove >= GetComponent<Rigidbody2D>().position.y && posToMove > GetComponent<Rigidbody2D>().position.x)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
                }
                else
                {
                    Debug.Log("He llegado");
                    change = false; // Se debe decidir tomar una nueva decisión.
                    setAnimation(false);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); // Esté estático.
                }
            }
        }

        if (moveMent == "LEFTUP")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, speed);

            // Si he alcanzado la posición a la que me quería mover en el eje Xfinal = Xstart.
            if (posToMove >= GetComponent<Rigidbody2D>().position.x || secondPosToMove <= GetComponent<Rigidbody2D>().position.y)
            {

                if (posToMove >= GetComponent<Rigidbody2D>().position.x && secondPosToMove > GetComponent<Rigidbody2D>().position.y)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
                }
                else if (secondPosToMove <= GetComponent<Rigidbody2D>().position.y && posToMove < GetComponent<Rigidbody2D>().position.x)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
                }
                else
                {
                    Debug.Log("He llegado");
                    change = false; // Se debe decidir tomar una nueva decisión.
                    setAnimation(false);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); // Esté estático.
                }
            }
        }

        if (moveMent == "LEFTDOWN")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, -speed);

            // Si he alcanzado la posición a la que me quería mover en el eje Xfinal = Xstart.
            if (posToMove >= GetComponent<Rigidbody2D>().position.x || secondPosToMove >= GetComponent<Rigidbody2D>().position.y)
            {

                if (posToMove >= GetComponent<Rigidbody2D>().position.x && secondPosToMove < GetComponent<Rigidbody2D>().position.y)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, -speed);
                }
                else if (secondPosToMove >= GetComponent<Rigidbody2D>().position.y && posToMove < GetComponent<Rigidbody2D>().position.x)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
                }
                else
                {
                    Debug.Log("He llegado");
                    change = false; // Se debe decidir tomar una nueva decisión.
                    setAnimation(false);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); // Esté estático.
                }
            }
        }
    }
}
