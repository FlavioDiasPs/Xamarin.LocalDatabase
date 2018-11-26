using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XF.LocalDB.ViewModel;

namespace XF.LocalDB
{
	public partial class App : Application
	{
        #region ViewModels
        public static AlunoViewModel AlunoVM { get; set; }
        public static UsuarioViewModel UsuarioVM { get; set; }
        #endregion

        public App()
        {
            InitializeComponent();
            InitializeApplication();

            MainPage = new NavigationPage(new View.Login.LoginView() { BindingContext = App.UsuarioVM });
        }

        private void InitializeApplication()
        {
            if (AlunoVM == null) AlunoVM = new AlunoViewModel();
            if (UsuarioVM == null) UsuarioVM = new UsuarioViewModel();
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
