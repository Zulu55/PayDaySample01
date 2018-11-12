using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PayDaySample01.Mobile
{
    public partial class EmployeesPage : ContentPage
    {
        private TokenResponse token;

        public EmployeesPage(TokenResponse token)
        {
            InitializeComponent();
            this.token = token;
            this.LoadEmployees();
        }

        private async void LoadEmployees()
        {
            var apiService = new ApiService();
            var response = await apiService.GetList<Employee>(
                "https://paydaysample01api.azurewebsites.net", 
                "/api", "/Employees", 
                this.token.TokenType, 
                this.token.AccessToken);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "No pude traer empleados.",
                    "Aceptar");
                return;
            }


            this.EmployeesList.ItemsSource = (List<Employee>)response.Result;
        }
    }
}
