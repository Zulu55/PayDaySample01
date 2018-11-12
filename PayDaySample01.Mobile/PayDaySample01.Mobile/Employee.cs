using System;
namespace PayDaySample01.Mobile
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string Document { get; set; }

        public int CityId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PicturePath { get; set; }

        public DateTime HireIn { get; set; }

        public decimal Salary { get; set; }

        public bool HasChildren { get; set; }

        public string Email { get; set; }

        public string FullName { get { return $"{this.FirstName} {this.LastName}"; } }

        public string PictureFullPath { 
            get { 
                return $"https://paydayclock.azurewebsites.net{this.PicturePath.Substring(1)}"; 
            } 
        }
    }
}
