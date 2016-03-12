using UnityEngine;

class DragAndDrop : MonoBehaviour {
    private bool arrastrando = false;
    private float distancia;
    private Pieza esta_pieza;

    [SerializeField]
    private Tablero tablero;

    void Start() {
		esta_pieza = GetComponent<Pieza>(); // Obtener los componetes de una pieza.
    }

    void Update() {
		if (arrastrando) {
			Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 puntoRayo = rayo.GetPoint(distancia);

			// Actualizar la posicion de arrastrado de la pieza, se intenta ponerlo lo mas cercano posible al puntero del mouse.
			transform.position = new Vector3(puntoRayo.x - 0.5f, 2.7f, puntoRayo.z);
			transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

    void OnMouseDown() {
        // Si el turno es el de la pieza seleccionada.
		if (tablero.turno_actual == esta_pieza.equipo) {
            GetComponent<Rigidbody>().isKinematic = true;
			// Seteo de la distancia entre el mouse y la pieza.
            distancia = Vector3.Distance(transform.position, Camera.main.transform.position);
			arrastrando = true; // Iniciar arrastrado.
        }
    }
 
    void OnMouseUp() {
		if (arrastrando) {
			GetComponent<Rigidbody>().isKinematic = false;
			// Obtener el cuadro mas cercano e intentar mover la pieza hacia el.
            Cuadrado cuadradoMasCercano = tablero.getCuadradoMasCercano(transform.position);
			esta_pieza.moverPieza(cuadradoMasCercano);
			arrastrando = false; // Parar el arrastrado.
        }
    }
}