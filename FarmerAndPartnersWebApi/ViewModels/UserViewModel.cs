using System.ComponentModel.DataAnnotations;

namespace FarmerAndPartnersWebApi.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя пользователя, не более 50 символов")]
        [StringLength(50, ErrorMessage = "Имя пользователя не должно превышать 50 символов")]
        [RegularExpression(@"^[A-я]*$", ErrorMessage = "Имя пользователя должно состоять только из букв")]
        public string Name { get; set; }
        public byte[] TimeStamp { get; set; }

        [Required(ErrorMessage = "Введите логин, не более 50 символов")]
        [StringLength(50, ErrorMessage = "Логин не должен превышать 50 символов")]
        [RegularExpression(@"^[A-z]\w*\W*.*$", ErrorMessage = "Логин должен начинаться с буквы")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль, не более 50 символов и не менее 5")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Пароль не должен быть меньше 5 символов и превышать 50 символов")]
        [RegularExpression(@"^[A-z]+[0-9]+.*\W+$|[0-9]+[A-z]+.*\W+$",
            ErrorMessage = "Слишком простой пароль. Следуйте следующему шаблону: \"комбинация букв и цифр или цифр и букв -> любое количество букв или цифр " +
                           "или знаков или их сочетание -> любое кол-во знаков\"")]
        public string Password { get; set; }

        [RegularExpression("^[1-9]+$", ErrorMessage = "Выберите компанию для указанного пользователя")]
        public int CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
    }
}
