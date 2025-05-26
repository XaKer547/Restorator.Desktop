using System.ComponentModel;

namespace Restorator.Domain.Models.Enums
{
    public enum Roles
    {
        [Description("Пользователь")]
        User = 1,

        [Description("Менеджер ресторана")]
        Manager,

        [Description("Администратор")]
        Admin
    }
}