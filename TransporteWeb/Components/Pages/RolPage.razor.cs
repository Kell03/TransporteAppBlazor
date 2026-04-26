using Domain.Dto;
using MudBlazor;
using System.Text.RegularExpressions;
using static MudBlazor.CategoryTypes;

namespace TransporteWeb.Components.Pages
{
    public partial class RolPage
    {

        private bool _success;
        private MudForm _form;
        private RolDto rol = new RolDto();
        List<RolDto> roles = new List<RolDto>();
        private string[] _errors = [];
        public bool Crear { get; set; } = false;
        public bool Editar { get; set; } = false;
        public bool Leer { get; set; } = true;
        public bool Eliminar { get; set; } = false;
        public bool Disabled { get; set; }
        private MudTabs _tabs = null!;

        private Task ActivateAsync(int index)
        {
            return _tabs.ActivatePanelAsync(index);
        }

        protected override async Task OnInitializedAsync()
        {
            ResetPermisos();
            await GetData();
        }

        private void OnTabChanged(int newIndex)
        {
            if (newIndex == 0)
            {
                rol = new RolDto();
                ResetPermisos();

            }
        }
        private async Task GetData()
        {
            roles = await RolService.GetAllAsync();
            // Do something with the data
        }


        private async Task GetItemById(int id)
        {
            ResetPermisos();
            var item = await RolService.GetById(id);
            if (item != null)
            {
                rol = item;
                foreach (var permiso in rol.RolesPermisos)
                {
                    switch (permiso.Permiso.ToLower())
                    {
                        case "crear":
                            Crear = true;
                            break;
                        case "editar":
                            Editar = true;
                            break;
                        case "leer":
                            Leer = true;
                            break;
                        case "eliminar":
                            Eliminar = true;
                            break;
                    }
                }
                await ActivateAsync(1);
            }
            else
            {
                Snackbar.Add("Error submitting the role.", Severity.Error);
            }

       
            // Do something with the data
        }


        private async Task Submit()
        {
            await _form.ValidateAsync();

            if (_form.IsValid)
            {
                var permisosSeleccionados = new List<string>();

                if (Crear) permisosSeleccionados.Add("Crear");
                if (Editar) permisosSeleccionados.Add("Editar");
                if (Leer) permisosSeleccionados.Add("Leer");
                if (Eliminar) permisosSeleccionados.Add("Eliminar");


                foreach (var permiso in permisosSeleccionados)
                {
                    rol.RolesPermisos.Add(new RolPermisoDto { Permiso = permiso });
                }

                var saveRol = (rol.Id == 0) ? await RolService.SaveAsync(rol) : await RolService.UpdateAsync(rol);
                if (saveRol != null)
                {
                    Snackbar.Add("Submitted!", Severity.Success);
                        await ActivateAsync(0);
                        await OnInitializedAsync();
                    }
                else
                {
                    Snackbar.Add("Error submitting the role.", Severity.Error);
                }

                rol = new RolDto();

            }


        }

        private async Task DeleteItem(int id)
        {
            var delete = await RolService.DeleteAsync(id);
            if (delete)
            {
                Snackbar.Add("Submitted!", Severity.Success);
                await OnInitializedAsync();
            }
            else
            {
                Snackbar.Add("Error deleting the role.", Severity.Error);
            }
        }

       private void ResetPermisos()
        {
            Crear = false;
            Leer = true;
            Editar = false;
            Eliminar = false;
        }

    }
}
