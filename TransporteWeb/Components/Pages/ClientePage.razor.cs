using Domain.Dto;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace TransporteWeb.Components.Pages
{
    public partial class ClientePage
    {
        private bool _success;
        private MudForm _form;
        private ClienteDto _item = new ClienteDto();
        List<ClienteDto> list = new List<ClienteDto>();
        private string[] _errors = [];
        public bool Disabled { get; set; }
        private MudTabs _tabs = null!;

        private IBrowserFile? selectedFile;
        private bool isLoading = false;
        private UploadResultDto? resultado;
        private int? empresaId;

        private string _searchString;



        private Task ActivateAsync(int index)
        {
            return _tabs.ActivatePanelAsync(index);
        }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            empresaId = Convert.ToInt32(user.FindFirst("EmpresaId")?.Value);
            await GetData();
        }

        private void OnTabChanged(int newIndex)
        {
            if (newIndex == 0)
            {
                _item = new ClienteDto();
            }
        }
        private async Task GetData()
        {
            list = await ClienteService.GetAllAsync();
        }


        private async Task GetItemById(int id)
        {
            var item = await ClienteService.GetById(id);
            if (item != null)
            {
                _item = item;

                await ActivateAsync(1);
            }
            else
            {
                Snackbar.Add("Error submitting the Centro distribucion.", Severity.Error);
            }


            // Do something with the data
        }

        private async Task DeleteItem(int id)
        {
            var delete = await ClienteService.DeleteAsync(id);
            if (delete)
            {
                Snackbar.Add("Submitted!", Severity.Success);
                await OnInitializedAsync();
            }
            else
            {
                Snackbar.Add("Error deleting the Centro distribucion.", Severity.Error);
            }
        }

        private async Task Submit()
        {
            await _form.ValidateAsync();

            if (_form.IsValid)
            {


                var saveRol = (_item.Id == 0) ? await ClienteService.SaveAsync(_item) : await ClienteService.UpdateAsync(_item);
                if (saveRol != null)
                {
                    Snackbar.Add("Submitted!", Severity.Success);
                    await ActivateAsync(0);
                    await OnInitializedAsync();
                }
                else
                {
                    Snackbar.Add("Error submitting the Centro distribucion.", Severity.Error);
                }

                _item = new ClienteDto();
            }
        }

        private Func<ClienteDto, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

          
            if (x.Nombre_comercial.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
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
                resultado = await ClienteService.UploadExcelAsync(stream, selectedFile.Name);

                // Mostrar mensaje al usuario
                if (resultado != null && resultado.RegistrosValidos > 0)
                {
                    Snackbar.Add("Centros de distribución cargados correctamente", Severity.Success);
                    await OnInitializedAsync();
                }
                else
                {
                    Snackbar.Add("Error al cargar centros de distribución", Severity.Error);
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
