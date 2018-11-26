using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.LocalDB.Model;

namespace XF.LocalDB.ViewModel
{
    public class AlunoViewModel : INotifyPropertyChanged
    {
        #region Propriedades

        public Aluno AlunoModel { get; set; }
        public string Nome { get { return App.UsuarioVM.Nome; } }

        private Aluno selecionado;
        public Aluno Selecionado
        {
            get { return selecionado; }
            set
            {
                selecionado = value as Aluno;
                EventPropertyChanged();
            }
        }

        private string pesquisaPorNome;
        public string PesquisaPorNome
        {
            get { return pesquisaPorNome; }
            set
            {
                if (value == pesquisaPorNome) return;

                pesquisaPorNome = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PesquisaPorNome)));
                AplicarFiltro();
            }
        }

        public List<Aluno> CopiaListaAlunos;
        public ObservableCollection<Aluno> Alunos { get; set; } = new ObservableCollection<Aluno>();

        // UI Events
        public OnAdicionarAlunoCMD OnAdicionarAlunoCMD { get; }
        public OnEditarAlunoCMD OnEditarAlunoCMD { get; }
        public OnDeleteAlunoCMD OnDeleteAlunoCMD { get; }
        public ICommand OnSairCMD { get; private set; }
        public ICommand OnNovoCMD { get; private set; }

        #endregion

        public AlunoViewModel()
        {
            AlunoRepository repository = AlunoRepository.Instance;

            OnAdicionarAlunoCMD = new OnAdicionarAlunoCMD(this);
            OnEditarAlunoCMD = new OnEditarAlunoCMD(this);
            OnDeleteAlunoCMD = new OnDeleteAlunoCMD(this);
            OnSairCMD = new Command(OnSair);
            OnNovoCMD = new Command(OnNovo);

            CopiaListaAlunos = new List<Aluno>();
            Carregar();
        }

        public void Carregar()
        {
            CopiaListaAlunos = AlunoRepository.GetAlunos().ToList();
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            if (pesquisaPorNome == null)
                pesquisaPorNome = "";

            var resultado = CopiaListaAlunos.Where(n => n.Nome.ToLowerInvariant()
                                .Contains(PesquisaPorNome.ToLowerInvariant().Trim())).ToList();

            var removerDaLista = Alunos.Except(resultado).ToList();
            foreach (var item in removerDaLista)
            {
                Alunos.Remove(item);
            }

            for (int index = 0; index < resultado.Count; index++)
            {
                var item = resultado[index];
                if (index + 1 > Alunos.Count || !Alunos[index].Equals(item))
                    Alunos.Insert(index, item);
            }
        }

        public void Adicionar(Aluno paramAluno)
        {
            if ((paramAluno == null) || (string.IsNullOrWhiteSpace(paramAluno.Nome)))
                App.Current.MainPage.DisplayAlert("Atenção", "O campo nome é obrigatório", "OK");
            else if (AlunoRepository.SalvarAluno(paramAluno) > 0)
                App.Current.MainPage.Navigation.PopAsync();
            else
                App.Current.MainPage.DisplayAlert("Falhou", "Desculpe, ocorreu um erro inesperado =(", "OK");
        }

        public async void Editar()
        {
            await App.Current.MainPage.Navigation.PushAsync(
                new View.Aluno.NovoAlunoView() { BindingContext = App.AlunoVM });
        }

        public async void Remover()
        {
            if (await App.Current.MainPage.DisplayAlert("Atenção?",
                string.Format("Tem certeza que deseja remover o {0}?", Selecionado.Nome), "Sim", "Não"))
            {
                if (AlunoRepository.RemoverAluno(Selecionado.Id) > 0)
                {
                    CopiaListaAlunos.Remove(Selecionado);
                    Carregar();
                }
                else
                    await App.Current.MainPage.DisplayAlert(
                            "Falhou", "Desculpe, ocorreu um erro inesperado =(", "OK");
            }
        }

        private async void OnSair()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        private void OnNovo()
        {
            App.AlunoVM.Selecionado = new Model.Aluno();
            App.Current.MainPage.Navigation.PushAsync(
                new View.Aluno.NovoAlunoView() { BindingContext = App.AlunoVM });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void EventPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class OnAdicionarAlunoCMD : ICommand
    {
        private AlunoViewModel alunoVM;
        public OnAdicionarAlunoCMD(AlunoViewModel paramVM)
        {
            alunoVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void AdicionarCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            alunoVM.Adicionar(parameter as Aluno);
        }
    }

    public class OnEditarAlunoCMD : ICommand
    {
        private AlunoViewModel alunoVM;
        public OnEditarAlunoCMD(AlunoViewModel paramVM)
        {
            alunoVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void EditarCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => (parameter != null);
        public void Execute(object parameter)
        {
            App.AlunoVM.Selecionado = parameter as Aluno;
            alunoVM.Editar();
        }
    }

    public class OnDeleteAlunoCMD : ICommand
    {
        private AlunoViewModel alunoVM;
        public OnDeleteAlunoCMD(AlunoViewModel paramVM)
        {
            alunoVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void DeleteCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => (parameter != null);
        public void Execute(object parameter)
        {
            App.AlunoVM.Selecionado = parameter as Aluno;
            alunoVM.Remover();
        }
    }
}
