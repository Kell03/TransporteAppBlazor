using Domain.Dto;
using MudBlazor;

namespace TransporteWeb.Components.Pages
{
    public partial class ConductorPage
    {

        private bool _success;
        private MudForm _form;
        private ConductorDto _item = new ConductorDto();
        List<ConductorDto> list = new List<ConductorDto>();
        List<PropietarioDto> propietarios = new List<PropietarioDto>();
        List<CamionDto> camiones = new List<CamionDto>();

        private string[] _errors = [];
        public bool Disabled { get; set; }
        private MudTabs _tabs = null!;
        private DateTime? _date = DateTime.Today;


        private Task ActivateAsync(int index)
        {
            return _tabs.ActivatePanelAsync(index);
        }

        protected override async Task OnInitializedAsync()
        {
            await GetData();
        }

        private async Task GetData()
        {
            list = await ConductorService.GetAllAsync();
            propietarios = await PropietarioService.GetAllAsync();
            camiones = await CamionService.GetAllAsync();
        }

        private async Task GetItemById(int id)
        {
            var item = await ConductorService.GetById(id);
           
            if (item != null)
            {
                _item = item;
                _date = _item.Fecha_alta.ToLocalTime();
                await ActivateAsync(1);
            }
            else
            {
                Snackbar.Add("Error submitting the conductor.", Severity.Error);
            }

        }

        private async Task DeleteItem(int id)
        {
            var delete = await ConductorService.DeleteAsync(id);
            if (delete)
            {
                Snackbar.Add("Submitted!", Severity.Success);
                await OnInitializedAsync();
            }
            else
            {
                Snackbar.Add("Error deleting the conductor.", Severity.Error);
            }
        }

        private async Task Submit()
        {
            await _form.ValidateAsync();

            if (_form.IsValid)
            {

                _item.Fecha_alta = _date.Value.ToLocalTime();
                _item.Camion_Id = _item.Camion?.Id;
                var saveRol = (_item.Id == 0) ? await ConductorService.SaveAsync(_item) : await ConductorService.UpdateAsync(_item);
                if (saveRol != null)
                {
                    Snackbar.Add("Submitted!", Severity.Success);
                    await ActivateAsync(0);
                    await OnInitializedAsync();
                }
                else
                {
                    Snackbar.Add("Error submitting the conductor.", Severity.Error);
                }

                _item = new ConductorDto();

            }
        }

        private async Task<IEnumerable<CamionDto>> SearchCamiones(string value, CancellationToken token)
        {
            await Task.Delay(5, token); // Simula latencia de API

            if (string.IsNullOrEmpty(value))
                return camiones;

            return camiones.Where(x =>
                x.Placa1.Contains(value, StringComparison.InvariantCultureIgnoreCase)
            );
        }
    }
}
