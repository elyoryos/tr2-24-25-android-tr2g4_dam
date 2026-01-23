using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ControladorDetalle : MonoBehaviour
{
    [Header("Configuración UI")]
    public VisualTreeAsset plantillaItemInforme;

    public void MostrarDatosEscuela(Escuela escuela, VisualElement root)
    {
        // 1. TÍTULO DINÁMICO (Esto sigue igual que antes)
        Label lblTitulo = root.Q<Label>("Txt_NombreEscuela");
        
        if (lblTitulo != null)
        {
            lblTitulo.text = string.IsNullOrEmpty(escuela.nombre) ? "Escuela Sin Nombre" : escuela.nombre;
        }

        // 2. LISTA DE ALUMNOS (AQUÍ ESTÁ EL CAMBIO IMPORTANTE)
        
        // CAMBIO 1: Le decimos al código que busque un 'ScrollView', no un VisualElement cualquiera
        ScrollView contenedorLista = root.Q<ScrollView>("Contenedor_Historial");

        if (contenedorLista != null)
        {
            // CAMBIO 2: ¡CRUCIAL! Usamos 'contentContainer.Clear()'
            // Si usas .Clear() a secas, borras las barras de scroll y se rompe.
            // Con esto, borramos solo los alumnos viejos, pero mantenemos el sistema de scroll.
            contenedorLista.contentContainer.Clear();

            if (escuela.listaAlumnos != null && escuela.listaAlumnos.Count > 0)
            {
                foreach (var alumno in escuela.listaAlumnos)
                {
                    if (plantillaItemInforme != null)
                    {
                        VisualElement nuevaFila = plantillaItemInforme.CloneTree();

                        // Lógica de datos (Igual que antes)
                        var lblIzq = nuevaFila.Q<Label>("Lbl_Fecha"); 
                        if(lblIzq != null) lblIzq.text = "Alumne: " + alumno.iniciales;

                        var lblDer = nuevaFila.Q<Label>("Lbl_Valor");
                        if(lblDer != null) lblDer.text = "PI's: " + alumno.numDocumentos;

                        // Añadimos la fila al ScrollView
                        contenedorLista.Add(nuevaFila);
                    }
                }
            }
            else
            {
                Label aviso = new Label("No hay alumnos con documentos.");
                aviso.style.alignSelf = Align.Center;
                contenedorLista.Add(aviso);
            }
        }
        else
        {
            Debug.LogError("❌ ERROR: No encuentro el ScrollView llamado 'Contenedor_Historial' en el UXML.");
        }
    }
}