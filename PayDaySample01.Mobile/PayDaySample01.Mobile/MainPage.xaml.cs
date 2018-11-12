using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PayDaySample01.Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Email.Text))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    "Debes ingresar un email.", 
                    "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(this.Password.Text))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    "Debes ingresar un password.", 
                    "Aceptar");
                return;
            }

            this.ActivityIndicator.IsRunning = true;
            var apiService = new ApiService();
            var token = await apiService.GetToken(
                "https://paydaysample01api.azurewebsites.net", 
                this.Email.Text, 
                this.Password.Text);
            this.ActivityIndicator.IsRunning = false;


            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {

                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Usuario o contraseña incorrectos.",
                    "Aceptar");
                this.Password.Text = string.Empty;
                return;
            }

            this.Email.Text = string.Empty;
            this.Password.Text = string.Empty;
            await Application.Current.MainPage.Navigation.PushAsync(new EmployeesPage(token));
        }
    }
}
