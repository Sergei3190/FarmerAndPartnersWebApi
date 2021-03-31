using AutoMapper;
using FarmerAndPartnersModels;
using FarmerAndPartnersWebApi.Mappings;
using FarmerAndPartnersWebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmerAndPartnersWebApi.Tests.TestData
{
    internal static class UserTestData
    {
        private static readonly List<User> _users = new List<User>
        {
             new User { Id = 1, Name = "Сергей", TimeStamp = new byte[0], Login = "Serg_37", Password = "12322w@", CompanyId = 1,
                    Company = new Company { Id = 1, Name = "Лодочка", TimeStamp = new byte[0], ContractStatusId = 1,
                        ContractStatus = new ContractStatus { Id = 1, Definition = "Заключен" } } },

             new User { Id = 2, Name = "Михаил", TimeStamp = new byte[0], Login = "Mih_12", Password = "52622w@", CompanyId = 2,
                    Company = new Company { Id = 2, Name = "Зверушка", TimeStamp = new byte[0], ContractStatusId = 2,
                        ContractStatus = new ContractStatus { Id = 2, Definition = "Ещё не заключен" } } },

              new User { Id = 3, Name = "Екатерина", TimeStamp = new byte[0], Login = "Kat_31", Password = "78822w@", CompanyId = 3,
                    Company = new Company { Id = 3, Name = "Поедатель", TimeStamp = new byte[0], ContractStatusId = 3,
                        ContractStatus = new ContractStatus { Id = 3, Definition = "Расторгнут" } } }
        };

        private static readonly User _conflictUser = new User
        {
            Id = 2,
            Name = "Михаил",
            TimeStamp = new byte[1] { 1 },
            Login = "Mih_12",
            Password = "52622w@",
            CompanyId = 2
        };

        private static readonly UserViewModel _conflictEditUserViewModel = new UserViewModel
        {
            Id = 2,
            Name = "Мишаня",
            TimeStamp = new byte[0],
            Login = "Mihailo_12",
            Password = "5262wE@2w@",
            CompanyId = 3
        };

        private static readonly UserViewModel _newUserViewModel = new UserViewModel
        {
            Name = "Петросян",
            Login = "Pet_1215",
            Password = "526Petw@",
            CompanyId = 3
        };

        private static readonly UserViewModel _editUserViewModel = new UserViewModel
        {
            Id = 3,
            Name = "Катюха",
            TimeStamp = new byte[0],
            Login = "Katrin_31",
            Password = "5262wdeE2w@",
            CompanyId = 3
        };

        internal static List<User> Users => _users;
        internal static UserViewModel NewUserViewModel => _newUserViewModel;
        internal static UserViewModel EditUserViewModel => _editUserViewModel;
        internal static UserViewModel ConflictEditUserViewModel => _conflictEditUserViewModel;
        internal static (string Login, string Message) CustomError => ("Login", "Пользователь с таким логином уже существует");
        internal static string BadLogin => "Mih_12";
        internal static int BadId => -1;
        internal static int GoodId => 1;
        internal static int ConflictId => 2;

        internal static IMapper GetMapper() => new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<CompanyProfile>();
            cfg.AddProfile<ContractStatusProfile>();
        })
            .CreateMapper();

        internal static User GetUser(int id) => _users.FirstOrDefault(u => u.Id == id);
        internal static bool FindLoginUser(string login) => _users.Any(u => u.Login == login);
        internal static int GetIdLastUser() => _users.LastOrDefault().Id;

        internal static int AddUser(User user)
        {
            if (user is null)
                return -1;

            var count = _users.Count;

            _users.Add(user);

            if (count != _users.Count)
                return 1;

            return -1;
        }

        internal static int AddUserStatusCode500(User user) => -1;
        internal static Company GetCompany(int id) => CompanyTestData.Companies.FirstOrDefault(c => c.Id == id);
        internal static int CheckLoginUser(string login) => _users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();

        internal static int UpdateUser(User user)
        {
            var sourceUser = _users.FirstOrDefault(u => u.Id == user.Id && u.TimeStamp.SequenceEqual(user.TimeStamp));

            if (sourceUser is null)
                return -1;

            if (!sourceUser.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase))
                sourceUser.Name = user.Name;

            if (!sourceUser.Login.Equals(user.Login, StringComparison.OrdinalIgnoreCase))
                sourceUser.Login = user.Login;

            if (!sourceUser.Password.Equals(user.Password, StringComparison.OrdinalIgnoreCase))
                sourceUser.Password = user.Password;

            if (sourceUser.CompanyId != user.CompanyId)
                sourceUser.CompanyId = user.CompanyId;

            return 1;
        }

        internal static int UpdateConflictUser(User user)
        {
            if (_conflictUser.Id == user.Id && _conflictUser.TimeStamp != user.TimeStamp)
                return -1;

            return 1;
        }

        internal static int DeleteUser(User user)
        {
            if (!_users.Any(u => u.Id == user.Id && u.TimeStamp == user.TimeStamp))
                return -1;

            var result = _users.Remove(user);

            return result switch
            {
                false => -1,
                true => 1
            };
        }

        internal static int DeleteConflictUser(User user)
        {
            if (_conflictUser.Id == user.Id && _conflictUser.TimeStamp != user.TimeStamp)
                return -1;

            return 1;
        }
    }
}
