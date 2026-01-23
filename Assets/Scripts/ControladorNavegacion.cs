using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections; 

public class ControladorNavegacion : MonoBehaviour
{
    [Header("Referencias de la Escena")]
    public GameObject loginAntiguo;
    public UIDocument documentoUI;
    public ConectorAPI miApi;
    public ControladorDetalle controladorDetalle;

    [Header("Tus Pantallas (Archivos UXML)")]
    public VisualTreeAsset archivoLoginP1; // <--- ¡NUEVO! Arrastra aquí Login_P1
    public VisualTreeAsset archivoMenuP2;
    public VisualTreeAsset archivoBuscadorP3;
    public VisualTreeAsset archivoDetalleP4;
    
    [Header("Plantillas")]
    public VisualTreeAsset plantillaItemEscuela; 

    // --- NUEVO: AL ARRANCAR EL JUEGO ---
    void Start()
    {
        // 1. Apagamos el Canvas antiguo por si acaso se quedó encendido
        if (loginAntiguo != null) loginAntiguo.SetActive(false);
        
        // 2. Mostramos directamente la pantalla de Login Nueva
        MostrarLogin();
    }

    // --- LÓGICA DEL LOGIN (NUEVA) ---
    void MostrarLogin()
    {
        // Cargamos el diseño del Login
        if (archivoLoginP1 != null) documentoUI.visualTreeAsset = archivoLoginP1;
        var root = documentoUI.rootVisualElement;

        // Buscamos los componentes que has creado
        var inputUser = root.Q<TextField>("Input_Usuario");
        var inputPass = root.Q<TextField>("Input_Contra");
        var btnAcceder = root.Q<Button>("Btn_Acceder");
        var lblError = root.Q<Label>("Lbl_Error"); // (Opcional, si lo pusiste)

        // Si pulsamos el botón...
        if (btnAcceder != null)
        {
            btnAcceder.clicked += () => 
            {
                // Obtenemos lo que ha escrito el usuario
                string usuario = (inputUser != null) ? inputUser.value : "";
                string contra = (inputPass != null) ? inputPass.value : "";

                // COMPROBACIÓN DE CONTRASEÑA
                if (usuario == "admin" && contra == "admin")
                {
                    Debug.Log("✅ Login Correcto");
                    MostrarMenuPrincipal(); // ¡Vamos al menú!
                }
                else
                {
                    Debug.Log("❌ Login Incorrecto");
                    // Si pusiste una etiqueta de error, la mostramos
                    if (lblError != null) lblError.style.display = DisplayStyle.Flex;
                }
            };
        }
    }

    // --- A PARTIR DE AQUÍ TODO SIGUE IGUAL QUE ANTES ---

    public void EntrarEnLaApp()
    {
        if (loginAntiguo != null) loginAntiguo.SetActive(false);
        MostrarMenuPrincipal();
    }

    void MostrarMenuPrincipal()
    {
        documentoUI.visualTreeAsset = archivoMenuP2;
        var root = documentoUI.rootVisualElement;

        var botonBuscar = root.Q<Button>("Btn_IrBuscador");
        if (botonBuscar != null) botonBuscar.clicked += () => MostrarBuscador();

        StartCoroutine(CargarDatosMenu(root));
    }

    IEnumerator CargarDatosMenu(VisualElement root)
    {
        string url = "http://localhost:3001/api/unity/home"; 
        
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var json = webRequest.downloadHandler.text;
                RespuestaHome datosHome = JsonUtility.FromJson<RespuestaHome>(json);

                // 1. LLENAR TOP ESCUELAS
                var listaTop = root.Q<ScrollView>("Lista_TopEscuelas");
                if (listaTop != null && plantillaItemEscuela != null)
                {
                    listaTop.Clear();
                    int posicion = 1; 

                    foreach (var item in datosHome.topEscuelas)
                    {
                        var fila = plantillaItemEscuela.CloneTree();
                        var lbl = fila.Q<Label>("Lbl_Nombre");
                        
                        if (lbl != null) 
                        {
                            lbl.text = posicion + ". " + item.nombre + " (" + item.dato + ")"; 
                        }
                        
                        listaTop.Add(fila);
                        posicion++; 
                    }
                }

                // 2. LLENAR ÚLTIMOS MOVIMIENTOS
                var listaUltimos = root.Q<ScrollView>("Lista_UltimosPI");
                if (listaUltimos != null && plantillaItemEscuela != null)
                {
                    listaUltimos.Clear();
                    foreach (var item in datosHome.ultimosPI)
                    {
                        var fila = plantillaItemEscuela.CloneTree();
                        
                        var lbl = fila.Q<Label>("Lbl_Nombre");
                        if (lbl != null) lbl.text = item.nombre;
                        
                        listaUltimos.Add(fila);
                    }
                }
            }
            else
            {
                Debug.Log("⚠️ No se pudo cargar el menú (¿Servidor apagado?): " + webRequest.error);
            }
        }
    }

    void MostrarBuscador()
    {
        documentoUI.visualTreeAsset = archivoBuscadorP3;
        var root = documentoUI.rootVisualElement;

        var botonAtras = root.Q<Button>("Btn_Atras");
        if (botonAtras != null) botonAtras.clicked += () => MostrarMenuPrincipal();

        var btnBuscar = root.Q<Button>("Btn_Buscar");
        if(btnBuscar != null) btnBuscar.clicked += () => RealizarBusqueda();
    }

    void RealizarBusqueda()
    {
        var root = documentoUI.rootVisualElement;
        var input = root.Q<TextField>("Input_Busqueda");
        
        var listaResultados = root.Q<ScrollView>("Lista_Resultados"); 

        if (input != null && !string.IsNullOrEmpty(input.value))
        {
            if (listaResultados != null) listaResultados.Clear();

            if (miApi != null)
            {
                miApi.BuscarEscuela(input.value, (respuesta) => 
                {
                    if (respuesta.tipo == "detalle")
                    {
                        Escuela escuela = new Escuela();
                        escuela.nombre = respuesta.nombre;
                        escuela.listaAlumnos = respuesta.listaAlumnos;
                        MostrarDetalleEscuela(escuela);
                    }
                    else if (respuesta.tipo == "lista")
                    {
                        if (listaResultados == null) return;

                        foreach(var esc in respuesta.resultados)
                        {
                            if (plantillaItemEscuela != null)
                            {
                                var fila = plantillaItemEscuela.CloneTree();

                                var label = fila.Q<Label>("Lbl_Nombre");
                                if(label != null) label.text = esc.codigo + " - " + esc.nombre;

                                var btnFila = fila.Q<VisualElement>(); 
                                btnFila.RegisterCallback<ClickEvent>(evt => 
                                {
                                    input.value = esc.codigo;
                                    RealizarBusqueda();
                                });

                                listaResultados.Add(fila);
                            }
                        }
                    }
                    else 
                    {
                        Debug.Log("⚠️ No se encontraron resultados o hubo error.");
                    }
                });
            }
        }
    }

    void MostrarDetalleEscuela(Escuela datos)
    {
        documentoUI.visualTreeAsset = archivoDetalleP4;
        var root = documentoUI.rootVisualElement;

        if (controladorDetalle != null)
        {
            controladorDetalle.MostrarDatosEscuela(datos, root);
        }

        var btnAtras = root.Q<Button>("Btn_Atras");
        if (btnAtras != null) btnAtras.clicked += () => MostrarBuscador();
    }
}