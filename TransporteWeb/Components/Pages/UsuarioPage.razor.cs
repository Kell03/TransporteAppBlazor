using Domain.Dto;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;

namespace TransporteWeb.Components.Pages
{
    public  partial class UsuarioPage
    {
        private bool _success;
        private MudForm _form;
        private UsuarioDto _item = new UsuarioDto();
        List<UsuarioDto> list = new List<UsuarioDto>();
        List<RolDto> roles = new List<RolDto>();
        private string[] _errors = [];
        public bool Disabled { get; set; }
        private MudTabs _tabs = null!;
        private string _password;

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
                _item = new UsuarioDto();
            }
        }
        private async Task GetData()
        {
            list = await UsuarioService.GetAllAsync();
            roles = await RolService.GetAllAsync();
            // Do something with the data
        }


        private async Task GetItemById(int id)
        {
            var item = await UsuarioService.GetById(id);
            if (item != null)
            {
                _item = item;
               
                await ActivateAsync(1);
            }
            else
            {
                Snackbar.Add("Error submitting the role.", Severity.Error);
            }


            // Do something with the data
        }


        private async Task DeleteItem(int id)
        {
            var delete = await UsuarioService.DeleteAsync(id);
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

        private async Task Submit()
        {
            await _form.ValidateAsync();

            if (_form.IsValid)
            {

                
                var saveRol = (_item.Id == 0) ? await UsuarioService.SaveAsync(_item) : await UsuarioService.UpdateAsync(_item);
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

                _item = new UsuarioDto();

            }
        }

        private static IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Password is required!";
                yield break;
            }
        }

        private string PasswordMatch(string arg) => _item.Password  != arg ? "Passwords don't match" : null;
    }

}

