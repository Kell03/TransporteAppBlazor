using Domain.Dto;
using Microsoft.AspNetCore.Components.Forms;
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

        private IBrowserFile? selectedFile;
        private bool isLoading = false;
        private UploadResultDto? resultado;

        private string _searchString;

        private Task ActivateAsync(int index)
        {
            return _tabs.ActivatePanelAsync(index);
        }

        private void OnTabChanged(int newIndex)
        {
            if (newIndex == 0)
            {
                _item = new ConductorDto();
            }
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


        private Func<ConductorDto, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.NombreCompleto.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Cedula.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            if (x.Propietario.Nombre.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };


        private void OnFileSelected(IBrowserFile file)
        {
            selectedFile = file;

        }

        private async Task UploadFile()
        {
            if (selectedFile == null) return;

            isLoading = true;
            try
            {
                using var stream = selectedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);

                // Llamar al servicio
                resultado = await ConductorService.UploadExcelAsync(stream, selectedFile.Name);

                // Mostrar mensaje al usuario
                if (resultado != null && resultado.RegistrosValidos > 0)
                {
                    Snackbar.Add("Conductores cargados correctamente", Severity.Success);
                    await OnInitializedAsync();
                }
                else
                {
                    Snackbar.Add("Error al cargar conductores", Severity.Error);
                    await OnInitializedAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Mostrar error al usuario
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
