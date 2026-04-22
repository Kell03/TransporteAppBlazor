using Domain.Dto;
using MudBlazor;

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

        private async Task OnOrigenChangedAsync()

        {

            destino = await DestinoService.GetAllAsync();
            destino = destino.Where(x => x.Id != _item.Origen_id).ToList();
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
    }
}
