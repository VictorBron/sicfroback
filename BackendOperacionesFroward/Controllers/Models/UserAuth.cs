namespace BackendOperacionesFroward.Controllers.Models
{
    public class UserAuth
    {
        private string Login;
        private string Password;
        private string Email;

        public UserAuth()
        { }

        public UserAuth(string login, string password, string email)
        {
            Login = login;
            Password = password;
            Email = email;
        }

        public string GetLogin()
        {
            return Login;
        }

        public UserAuth WithLogin(string login)
        {
            Login = login;
            return this;
        }

        public string GetPassword()
        {
            return Password;
        }

        public UserAuth WithPass(string password)
        {
            Password = password;
            return this;
        }
        public string GetEmail()
        {
            return Email;
        }

        public UserAuth WithEmail(string email)
        {
            Email = email;
            return this;
        }

    }
}
