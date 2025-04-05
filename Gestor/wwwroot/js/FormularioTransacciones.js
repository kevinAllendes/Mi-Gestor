function inicializarFormularioTransacciones(urlObtenerCategorias)
{
    /** #region  Creamos el metodo que genera el cambio de categoria dependiendo
    del tipo de operacion */
    $("#TipoOperacionId").change(async function(){
        const valorSeleccionado =  $(this).val(); 
        const respuesta = await fetch(urlObtenerCategorias, {
            method: 'POST',
            body: valorSeleccionado,
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const json = await respuesta.json();
        /**En este sector ya poseo las categorias falta cargarlas*/
        const opciones = json.map(categoria => `<option value=${categoria.value}>${categoria.text}</option>`);
        $("#CategoriaId").html(opciones);
        
    })
}