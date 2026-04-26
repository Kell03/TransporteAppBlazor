using Domain.Dto;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace TransporteWeb.Components.Pages
{
    public partial class PropietarioPage
    {
        private bool _success;
        private MudForm _form;
        private PropietarioDto _item = new PropietarioDto();
        List<PropietarioDto> list = new List<PropietarioDto>();
        private string[] _errors = [];
        public bool Disabled { get; set; }
        private MudTabs _tabs = null!;


        private IBrowserFile? selectedFile;
        private bool isLoading = false;
        private UploadResultDto? resultado;
        private string? _searchString;

        private Task ActivateAsync(int index)
        {
            return _tabs.ActivatePanelAsync(index);
        }

        protected override async Task OnInitializedAsync()
        {
            await GetData();
        }


        private void OnTabChanged(int newIndex)
        {
            if (newIndex == 0)
            {
                _item = new PropietarioDto();
            }
        }
        private async Task GetData()
        {
            list = await PropietarioService.GetAllAsync();
        }

        private async Task GetItemById(int id)
        {
            var item = await PropietarioService.GetById(id);
            if (item != null)
            {
                _item = item;

                await ActivateAsync(1);
            }
            else
            {
                Snackbar.Add("Error submitting the property.", Severity.Error);
            }


            // Do something with the data
        }

        private async Task DeleteItem(int id)
        {
            var delete = await PropietarioService.DeleteAsync(id);
            if (delete)
            {
                Snackbar.Add("Submitted!", Severity.Success);
                await OnInitializedAsync();
            }
            else
            {
                Snackbar.Add("Error deleting the property.", Severity.Error);
            }
        }

        private async Task Submit()
        {
            await _form.ValidateAsync();

            if (_form.IsValid)
            {


                var saveRol = (_item.Id == 0) ? await PropietarioService.SaveAsync(_item) : await PropietarioService.UpdateAsync(_item);
                if (saveRol != null)
                {
                    Snackbar.Add("Submitted!", Severity.Success);
                    await ActivateAsync(0);
                    await OnInitializedAsync();
                }
                else
                {
                    Snackbar.Add("Error submitting the user.", Severity.Error);
                }

                _item = new PropietarioDto();
            }
        }

        private Func<PropietarioDto, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Codigo?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                return true;

            if (x.Nombre?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
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
                resultado = await PropietarioService.UploadExcelAsync(stream, selectedFile.Name);

                // Mostrar mensaje al usuario
                if (resultado != null && resultado.RegistrosValidos > 0)
                {
                    Snackbar.Add("Propietarios cargados correctamente", Severity.Success);
                    await OnInitializedAsync();
                }
                else
                {
                    Snackbar.Add("Error al cargar propietarios", Severity.Error);
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
