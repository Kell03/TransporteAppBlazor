using Domain.Dto;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace TransporteWeb.Components.Pages
{
    public partial class CamionPage
    {
        private bool _success;
        private MudForm _form;
        private CamionDto _item = new CamionDto();
        List<CamionDto> list = new List<CamionDto>();
        List<PropietarioDto> propietarios = new List<PropietarioDto>();
        private string[] _errors = [];
        public bool Disabled { get; set; }
        private MudTabs _tabs = null!;

        private IBrowserFile? selectedFile;
        private bool isLoading = false;
        private UploadResultDto? resultado;

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
            list = await CamionService.GetAllAsync();
            propietarios = await PropietarioService.GetAllAsync();
        }

        private async Task GetItemById(int id)
        {
            var item = await CamionService.GetById(id);
            if (item != null)
            {
                _item = item;

                await ActivateAsync(1);
            }
            else
            {
                Snackbar.Add("Error submitting the camion.", Severity.Error);
            }

        }

        private async Task DeleteItem(int id)
        {
            var delete = await CamionService.DeleteAsync(id);
            if (delete)
            {
                Snackbar.Add("Submitted!", Severity.Success);
                await OnInitializedAsync();
            }
            else
            {
                Snackbar.Add("Error deleting the camion.", Severity.Error);
            }
        }

        private async Task Submit()
        {
            await _form.ValidateAsync();

            if (_form.IsValid)
            {


                var saveRol = (_item.Id == 0) ? await CamionService.SaveAsync(_item) : await CamionService.UpdateAsync(_item);
                if (saveRol != null)
                {
                    Snackbar.Add("Submitted!", Severity.Success);
                    await ActivateAsync(0);
                    await OnInitializedAsync();
                }
                else
                {
                    Snackbar.Add("Error submitting the camion.", Severity.Error);
                }

                _item = new CamionDto();

            }
        }

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
                resultado = await CamionService.UploadExcelAsync(stream, selectedFile.Name);

                // Mostrar mensaje al usuario
                if (resultado != null && resultado.RegistrosValidos > 0)
                {
                    Snackbar.Add("Camiones cargados correctamente", Severity.Success);
                    await OnInitializedAsync();
                }
                else
                {
                    Snackbar.Add("Error al cargar camiones", Severity.Error);
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
