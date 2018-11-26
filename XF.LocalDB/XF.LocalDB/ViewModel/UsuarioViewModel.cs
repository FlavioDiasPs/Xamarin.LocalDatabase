using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XF.LocalDB.Model;

namespace XF.LocalDB.ViewModel
{
    public class UsuarioViewModel : INotifyPropertyChanged
    {
        #region Propriedade
        public Usuario UsuarioModel { get; set; }
        public string Nome { get; set; }
        public string Stream { get; set; }
        private bool isloading;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnAtualizarPropriedade([CallerMemberName] string propriedade = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propriedade));
        }

        public bool IsLoading
        {
            get { return isloading; }
            set
            {
                isloading = value;
                OnAtualizarPropriedade();
            }
        }


        // UI Events
        public IsAutenticarCMD IsAutenticarCMD { get; }
        #endregion

        public UsuarioViewModel()
        {
            UsuarioModel = new Usuario();
            IsAutenticarCMD = new IsAutenticarCMD(this);
            this.GetUsuarios("https://apiaplicativofiap.azurewebsites.net/content/xml/usuarios.xml");
        }

        public void IsAutenticar(Usuario paramUser)
        {
            IsLoading = true;
            this.Nome = paramUser.Username;
            if (UsuarioRepository.IsAutorizado(paramUser))
            {
                App.Current.MainPage.Navigation.PushAsync(
                    new View.Aluno.MainPage() { BindingContext = App.AlunoVM });
                IsLoading = false;
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Atenção", "Usuário não autorizado", "Ok");
                IsLoading = false;
            }
        }

        private async void GetUsuarios(string paramURL)
        {
            var httpRequest = new HttpClient();
            Stream = await httpRequest.GetStringAsync(paramURL);
        }
    }

    public class IsAutenticarCMD : ICommand
    {
        private UsuarioViewModel usuarioVM;
        public IsAutenticarCMD(UsuarioViewModel paramVM)
        {
            usuarioVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void DeleteCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter)
        {
            if (parameter != null) return true;

            return false;
        }
        public void Execute(object parameter)
        {
            usuarioVM.IsAutenticar(parameter as Usuario);
        }
    }
}
