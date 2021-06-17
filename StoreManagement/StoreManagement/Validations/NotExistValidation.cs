using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StoreManagement.Validations
{
    public class NotExistValidation : ValidationRule
    {
        public string ErrorMessage { get; set; }
        public string ErrorMessageNull { get; set; }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            if (value == null)
            {
                return new ValidationResult(true, null);
            }
            if (value.ToString() == "")
            {
                return new ValidationResult(false, this.ErrorMessageNull);
            }
            if (!isExistUserName(value.ToString()))
            {
                result = new ValidationResult(false, this.ErrorMessage);
            }
            return result;
        }

        bool isExistUserName(string username)
        {
            var isExistUserName = DataProvider.Instance.DB.Accounts.Where(p => p.Username == username).Count();

            return isExistUserName > 0;
        }
    }
}
