using UnityEngine;

public class Coordenada {
    public int x;
    public int y;
    public Vector3 pos; // Posicion de las piezas en el espacio 3D.

    public Coordenada(int x, int y) {
        this.x = x;
        this.y = y;
        pos = new Vector3(0, 0, 0);
    }
}