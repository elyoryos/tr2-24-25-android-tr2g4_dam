using System.Collections.Generic;
using System;

// ==========================================
// 🆕 NUEVAS ESTRUCTURAS PARA EL MENÚ (HOME)
// ==========================================
[Serializable]
public class RespuestaHome
{
    public List<ItemRanking> topEscuelas;
    public List<ItemRanking> ultimosPI;
}

[Serializable]
public class ItemRanking
{
    public string nombre; // Nombre escuela o alumno
    public string dato;   // "15 PIs" o la fecha
}

// ==========================================
// 🔍 ESTRUCTURAS DEL BUSCADOR (YA EXISTÍAN)
// ==========================================
[Serializable]
public class RespuestaAPI 
{
    public string tipo;      // "lista", "detalle" o "error"
    public string mensaje;   // Por si hay error
    
    // Si es "detalle" (una sola escuela encontrada)
    public string nombre;
    public List<Alumno> listaAlumnos;

    // Si es "lista" (muchas escuelas encontradas)
    public List<EscuelaSimple> resultados;
}

[Serializable]
public class EscuelaSimple 
{
    public string nombre;
    public string codigo;
}

[Serializable]
public class Escuela
{
    public string nombre;
    public string codi_centre;
    public string email;
    public List<Alumno> listaAlumnos;
}

[Serializable]
public class Alumno 
{
    public string iniciales;
    public int numDocumentos;
}