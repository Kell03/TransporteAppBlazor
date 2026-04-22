using Domain.Dto;
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



    }
}
