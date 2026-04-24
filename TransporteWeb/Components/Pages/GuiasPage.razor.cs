using Domain.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using static MudBlazor.Colors;

namespace TransporteWeb.Components.Pages
{
    public partial class GuiasPage
    {
        private bool _success;
        private MudForm _form;
        private GuiaDto _item = new GuiaDto();
        List<GuiaDto> list = new List<GuiaDto>();
        List<ConductorDto> conductores = new List<ConductorDto>();
        List<CentroDistribucionDto> origen = new List<CentroDistribucionDto>();
        List<CentroDistribucionDto> destino = new List<CentroDistribucionDto>();
        List<CamionDto> camiones = new List<CamionDto>();
        private string[] _errors = [];
        public bool Disabled { get; set; }
        private MudTabs _tabs = null!;
        private DateTime? _date = DateTime.Today;

        private string _searchString;

        private Task ActivateAsync(int index)
        {
            return _tabs.ActivatePanelAsync(index);
        }
        private void OnTabChanged(int newIndex)
        {
            if (newIndex == 0)
            {
                _item = new GuiaDto();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await GetData();
        }

        private async Task GetData()
        {
            list = await GuiaService.GetAllAsync();
            conductores = await ConductorService.GetAllAsync();
            camiones = await CamionService.GetAllAsync();
            origen = await OrigenService.GetAllAsync();
            //destino = await DestinoService.GetAllAsync();
        }


        private async Task GetItemById(int id)
        {
            var item = await GuiaService.GetById(id);

            if (item != null)
            {
                _item = item;
                _date = _item.Fecha.ToLocalTime();
                await ActivateAsync(1);
            }
            else
            {
                Snackbar.Add("Error submitting the guia.", Severity.Error);
            }

        }

       

        private async Task DeleteItem(int id)
        {
            var delete = await GuiaService.DeleteAsync(id);
            if (delete)
            {
                Snackbar.Add("Submitted!", Severity.Success);
                await OnInitializedAsync();
            }
            else
            {
                Snackbar.Add("Error deleting the guia.", Severity.Error);
            }
        }

        private async Task Submit()
        {
            await _form.ValidateAsync();

            if (_form.IsValid)
            {

                _item.Fecha = _date.Value.ToLocalTime();
                _item.Origen_id = _item.Origen.Id;
                _item.Destino_id = _item.Destino.Id;
                _item.Conductor_id = _item.Conductor.Id;
                _item.camion_id = _item.Camion.Id;
                var saveRol = (_item.Id == 0) ? await GuiaService.SaveAsync(_item) : await GuiaService.UpdateAsync(_item);
                if (saveRol != null)
                {
                    Snackbar.Add("Submitted!", Severity.Success);
                    await ActivateAsync(0);
                    await OnInitializedAsync();
                }
                else
                {
                    Snackbar.Add("Error submitting the guia.", Severity.Error);
                }

                _item = new GuiaDto();

            }
        }


        private Func<GuiaDto, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Numero_guia.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Conductor.NombreCompleto.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Origen.Nombre.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Destino.Nombre.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };


        private async Task<IEnumerable<CamionDto>> SearchCamiones(string value, CancellationToken token)
        {
            await Task.Delay(5, token); // Simula latencia de API

            if (string.IsNullOrEmpty(value))
                return camiones;

            return camiones.Where(x =>
                x.Placa1.Contains(value, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        private async Task<IEnumerable<ConductorDto>> SearchConductores(string value, CancellationToken token)
        {
            await Task.Delay(5, token); // Simula latencia de API

            if (string.IsNullOrEmpty(value))
                return conductores;

           
            return conductores.Where(x =>
                x.NombreCompleto.Contains(value, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        private async Task<IEnumerable<CentroDistribucionDto>> SearchCentros(string value, CancellationToken token)
        {
            await Task.Delay(5, token); // Simula latencia de API

            if(_item.Origen_id > 0)
            {
                return origen.Where(x =>
               x.Nombre.Contains(value, StringComparison.InvariantCultureIgnoreCase)
               ).OrderByDescending(x => x.Id == _item.Origen_id);
            }

            if (string.IsNullOrEmpty(value))
                return origen;

            
            return origen.Where(x =>
                x.Nombre.Contains(value, StringComparison.InvariantCultureIgnoreCase)
            );
        }


        private async Task<IEnumerable<CentroDistribucionDto>> SearchCentrosDestino(string value, CancellationToken token)
        {

            if (_item.Origen != null)
            {
                await Task.Delay(5, token); // Simula latencia de API

                if (string.IsNullOrEmpty(value))
                    return origen.Where(x => x.Id != _item.Origen.Id);

                return origen.Where(x =>
                    x.Nombre.Contains(value, StringComparison.InvariantCultureIgnoreCase) && x.Id != _item.Origen.Id
                );
            }
            else
            {
                return new List<CentroDistribucionDto>();
            }
        }
    }
}
