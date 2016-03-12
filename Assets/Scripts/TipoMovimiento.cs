public enum TipoMovimiento {
    InicioSolo, // Movimientos permitidos para la pieza solo para el turno inicial.
    Mover, // Movimiento estandar de la pieza, pero no puede comer.
    Comer, // Movimiento permitido de la pieza, en el cual puede comer.
    ComerMover, // Movimiento en el cual la pieza puede comer o moverse sin comer.
    ComerMoverSaltar // No hay restricciones, la pieza se puede mover con sus movimientos estandar, solo si no hay una ficha del mismo equipo.
}