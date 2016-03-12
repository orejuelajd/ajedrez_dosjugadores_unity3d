using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Tablero : MonoBehaviour {
	
    public int turno_actual = -1; // -1 = whites; 1 = blacks
    [SerializeField] List<Cuadrado> cuadrados = new List<Cuadrado>(); // List of all game squares (64) - ordered
    [SerializeField] List<Pieza> piezas = new List<Pieza>(); // List of all pieces in the game (32)

    void Start() {
		addCoordenadasCuadrado(); // Add "local" coordinates to all squares
		setCoordenadasInicioPiezas(); // Update all piece's coordinate
    }

    /*
    ---------------
    Squares related functions
    ---------------
    */ 
    // Returns closest square to the given position
    public Cuadrado getCuadradoMasCercano(Vector3 pos) {
		Cuadrado cuadrado = cuadrados[0];
		float masCercano = Vector3.Distance(pos, cuadrados[0].coor.pos);

		for (int i = 0; i < cuadrados.Count ; i++) {
			float distancia = Vector3.Distance(pos, cuadrados[i].coor.pos);

			if (distancia < masCercano) {
				cuadrado = cuadrados[i];
				masCercano = distancia;
            }
        }
		return cuadrado;
	}
	
	// Returns the square that is at the given coordinate (local position in the board)
	public Cuadrado getCuadradoDesdeCoordenada(Coordenada coor) {
		Cuadrado cuadrado = cuadrados[0];
		for (int i = 0; i < cuadrados.Count ; i++) {
			if (cuadrados[i].coor.x == coor.x && cuadrados[i].coor.y == coor.y) {
				return cuadrados[i];
            }
        }
        return cuadrado;
    }


    // Set start square's local coordinates & its current position
    private void addCoordenadasCuadrado() {
        int coor_x = 0;
        int coor_y = 0;
		for (int i = 0; i < cuadrados.Count ; i++) {
			cuadrados[i].coor = new Coordenada(coor_x, coor_y);
			cuadrados[i].coor.pos = new Vector3(cuadrados[i].transform.position.x - 0.5f, cuadrados[i].transform.position.y, cuadrados[i].transform.position.z - 0.5f);

            if (coor_y > 0 && coor_y % 7 == 0) {
                coor_x++;
                coor_y = 0;
            }
            else {
                coor_y++;
            }
        }
    }

    // Remove the given piece from the pieces list
    public void destruirPieza(Pieza piece) {
		piezas.Remove(piece);
    }

    // Update each piece's coordinates getting the closest square
    private void setCoordenadasInicioPiezas() {
		for (int i = 0; i < piezas.Count ; i++) {
			Cuadrado cuadradoMasCercano = getCuadradoMasCercano(piezas[i].transform.position);
			cuadradoMasCercano.sujetarPieza(piezas[i]);
			piezas[i].setCuadradoInicio(cuadradoMasCercano);
			piezas[i].tablero = this;
        }
    }
	
    public void cambiarTurno() {
		turno_actual = (turno_actual == -1) ? 1 : -1;
    }
}
