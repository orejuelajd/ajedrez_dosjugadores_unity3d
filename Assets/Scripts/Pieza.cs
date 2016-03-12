using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pieza : MonoBehaviour
{
	private List<Movimiento> movimientos_permitidos = new List<Movimiento> (); // Lista de movimientos con los cuales la pieza se puede mover desde su posicion actual.
	private TipoMovimiento tipo_movimiento; // Tipo de movimiento con el cual la pieza se va a mover.
	public List<Coordenada> puntos_nopermitidos = new List<Coordenada> (); // Coordenadas en las cuales la ficha no va a poder posicionarse.
	public bool inicio; // Se vuelve true cuando la pieza se mueve por primera vez
	public Cuadrado actual_Cuadrado; // En que cuadro esta actualmente la pieza.
	public Tablero tablero;
	public string nombre_pieza;
	public int equipo; // Blancas = -1, Negras = 1

	void Start ()
	{
		// Inicializar los movimientos permitidos de la pieza.
		switch (nombre_pieza) {
		case "Peon":
			addMovimientosPermitidosPeon ();
			break;
		case "Torre":
			addMovimientosPermitidosLineal ();
			break;
		case "Caballo":
			addMovimientosPermitidosCaballo ();
			break;
		case "Alfil":
			addMovimientosPermitidosDiagonal ();
			break;
		case "Reina":
			addMovimientosPermitidosLineal ();
			addMovimientosPermitidosDiagonal ();
			break;
		case "Rey":
			addMovimientosPermitidosRey ();
			break;
		}
	}

	// Una vez el usuario arraste la pieza, se tratara de mover, y si ha sido arrastrado en un cuadro no valido,
	// la pieza sera retornada a su posicion.
	public void moverPieza (Cuadrado Cuadrado)
	{
		if (validarMovimientoPermitido (Cuadrado)) {
			// Cambiar los casos  por el actual tipo de movimiento.
			switch (tipo_movimiento) {
				case TipoMovimiento.InicioSolo:
					break;
				case TipoMovimiento.Comer:
				case TipoMovimiento.ComerMover:
				case TipoMovimiento.ComerMoverSaltar:
                    // Si se puede comer la pieza, se destruye la pieza.
					comerPieza (Cuadrado.pieza_sujetada);
					break;
			}

			// Actualizar el cuadrado actual de la pieza
			actual_Cuadrado.sujetarPieza (null);
			Cuadrado.sujetarPieza (this);
			actual_Cuadrado = Cuadrado;
			if (!inicio)
				inicio = true;

			// Cambiar el turno del juego
			tablero.cambiarTurno ();
		}

		// Limpiar los puntos no permitidos y actualizar la posicion de la pieza.
		puntos_nopermitidos.Clear ();
		transform.position = new Vector3 (actual_Cuadrado.coor.pos.x, transform.position.y, actual_Cuadrado.coor.pos.z);
		transform.rotation = new Quaternion (0, 0, 0, 0);
	}

	// Obtener la coordenada arrancando desde la posicion de la pieza (0, 0)
	public Coordenada getCoordenadaMovimiento (Cuadrado Cuadrado)
	{ 
		int coor_x = (Cuadrado.coor.x - actual_Cuadrado.coor.x) * equipo;
		int coor_y = (Cuadrado.coor.y - actual_Cuadrado.coor.y) * equipo;

		return new Coordenada (coor_x, coor_y);
	}

	// Mirar si la pieza si se puede desplazar al cuadrado que el usuario la quiere desplazar.
	public bool validarMovimientoPermitido (Cuadrado Cuadrado)
	{
		Coordenada coor_move = getCoordenadaMovimiento (Cuadrado);

		for (int i = 0; i < movimientos_permitidos.Count; i++) {
			if (coor_move.x == movimientos_permitidos [i].x && coor_move.y == movimientos_permitidos [i].y) {
				tipo_movimiento = movimientos_permitidos [i].tipo;
				switch (tipo_movimiento) {
				case TipoMovimiento.InicioSolo:
					// Si la pieza no ha sido movida antes, puede moverse al cuadro.
					if (!inicio && validarPuedeMover (Cuadrado)) 
						return true;
					break;
				case TipoMovimiento.Mover:
					if (validarPuedeMover (Cuadrado)) {
						return true;
					} 
					break;
				case TipoMovimiento.Comer:
					if (validarPuedeComer (Cuadrado)) 
						return true;
					break;
				case TipoMovimiento.ComerMover:
				case TipoMovimiento.ComerMoverSaltar:
					if (validarPuedeComerMover (Cuadrado)) {
						return true;
					}
					break;
				}
			}
		}
		return false;
	}
	
	// Retornar si la pieza no se puede colocar en el cuadro seleccionado.
	private bool validarPuedeMover (Cuadrado Cuadrado)
	{
		// Si el cuadro esta libre, se desplaza la pieza.
		if (Cuadrado.pieza_sujetada == null)
			return true;
		return false;
	}
	
	// Retornar si la pieza puede comer una pieza del enemigo que esta puesta en el cuadro seleccionado.
	private bool validarPuedeComer (Cuadrado Cuadrado)
	{
		// Si el cuadro esta sosteniendo una pieza del enemigo, la pieza se puede desplazar.
		if (Cuadrado.pieza_sujetada != null && Cuadrado.pieza_sujetada.equipo != equipo)
			return true;
		return false;
	}
	
	// Retornar si la pieza puede comer o moverse a el cuadrado seleccionado.
	private bool validarPuedeComerMover (Cuadrado Cuadrado)
	{
		if (validarPuedeComer (Cuadrado) || validarPuedeMover (Cuadrado))
			return true; 
		return false;
	}
	
	// Añadir la posicion no permitida de la pieza, cuadrados a los cuales la pieza no puede desplazarse.
	public void addPuntoNoPermitido (Cuadrado Cuadrado)
	{
		Coordenada coor_movimiento = getCoordenadaMovimiento (Cuadrado);

		for (int j = 0; j < movimientos_permitidos.Count; j++) {
			if (coor_movimiento.x == movimientos_permitidos [j].x && coor_movimiento.y == movimientos_permitidos [j].y) {
				switch (movimientos_permitidos [j].tipo) {
				case TipoMovimiento.InicioSolo:
				case TipoMovimiento.Mover:
				case TipoMovimiento.Comer:
				case TipoMovimiento.ComerMover:
						// Si el cuadrado tiene una pieza.
					if (Cuadrado.pieza_sujetada != null) {
						puntos_nopermitidos.Add (coor_movimiento);
					} 
					break;
				}
			}
		}   
	}

	// Aladir a la pieza un movimiento permitido.
	private void addMovimientoPermitido (int coor_x, int coor_y, TipoMovimiento tipo)
	{
		Movimiento nuevo_movimiento = new Movimiento (coor_x, coor_y, tipo);
		movimientos_permitidos.Add (nuevo_movimiento);
	}

	// Movimientos permitidos de los peones.
	private void addMovimientosPermitidosPeon ()
	{
		addMovimientoPermitido (0, 1, TipoMovimiento.Mover);
		addMovimientoPermitido (0, 2, TipoMovimiento.InicioSolo);
		addMovimientoPermitido (1, 1, TipoMovimiento.Comer);
		addMovimientoPermitido (-1, 1, TipoMovimiento.Comer);
	}

	// Movimientos lineales permitidos para las torres y la reina.
	private void addMovimientosPermitidosLineal ()
	{
		for (int coor_x = 1; coor_x < 8; coor_x++) {
			addMovimientoPermitido (coor_x, 0, TipoMovimiento.ComerMover);
			addMovimientoPermitido (0, coor_x, TipoMovimiento.ComerMover);
			addMovimientoPermitido (-coor_x, 0, TipoMovimiento.ComerMover);
			addMovimientoPermitido (0, -coor_x, TipoMovimiento.ComerMover);
		}
	}

	// Movimientos diagonales permitidos para la reina y el alfil.
	private void addMovimientosPermitidosDiagonal ()
	{
		for (int coor_x = 1; coor_x < 8; coor_x++) {
			addMovimientoPermitido (coor_x, -coor_x, TipoMovimiento.ComerMover);
			addMovimientoPermitido (-coor_x, coor_x, TipoMovimiento.ComerMover);
			addMovimientoPermitido (coor_x, coor_x, TipoMovimiento.ComerMover);
			addMovimientoPermitido (-coor_x, -coor_x, TipoMovimiento.ComerMover);
		}
	}

	// Movimientos permitidos para el caballo.
	private void addMovimientosPermitidosCaballo ()
	{
		for (int coor_x = 1; coor_x < 3; coor_x++) {
			for (int coor_y = 1; coor_y < 3; coor_y++) {
				if (coor_y != coor_x) {
					addMovimientoPermitido (coor_x, coor_y, TipoMovimiento.ComerMoverSaltar);
					addMovimientoPermitido (-coor_x, -coor_y, TipoMovimiento.ComerMoverSaltar);
					addMovimientoPermitido (coor_x, -coor_y, TipoMovimiento.ComerMoverSaltar);
					addMovimientoPermitido (-coor_x, coor_y, TipoMovimiento.ComerMoverSaltar);
				}
			}
		}
	}

	// Movimientos permitidos para el rey.
	private void addMovimientosPermitidosRey ()
	{
		addMovimientoPermitido (0, 1, TipoMovimiento.ComerMover);
		addMovimientoPermitido (1, 1, TipoMovimiento.ComerMover);
		addMovimientoPermitido (1, 0, TipoMovimiento.ComerMover);
		addMovimientoPermitido (1, -1, TipoMovimiento.ComerMover);
		addMovimientoPermitido (0, -1, TipoMovimiento.ComerMover);
		addMovimientoPermitido (-1, -1, TipoMovimiento.ComerMover);
		addMovimientoPermitido (-1, 0, TipoMovimiento.ComerMover);
		addMovimientoPermitido (-1, 1, TipoMovimiento.ComerMover);
	}

	public void setCuadradoInicio (Cuadrado Cuadrado)
	{
		actual_Cuadrado = Cuadrado;
	} 

	// Funcion llamada cuando se come esta pieza. Se destruye el gameobject
	public void comeme ()
	{
		tablero.destruirPieza (this);
		Destroy (this.gameObject);
	}

	// Llamada cuando se come una pieza
	private void comerPieza (Pieza pieza)
	{
		if (pieza != null && pieza.equipo != equipo)
			pieza.comeme ();
	}
}