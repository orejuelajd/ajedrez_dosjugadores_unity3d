using UnityEngine;

public class Cuadrado : MonoBehaviour {

    public Coordenada coor; // La posicion del cuadrado en el tablero.
    public Pieza pieza_sujetada = null; // Pieza actual parada en el cuadrado.
	
    void Start() {
    }

    public void sujetarPieza(Pieza piece) {
		pieza_sujetada = piece;
    }
}