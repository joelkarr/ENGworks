namespace CDWKS.BXC.UserService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserService" in code, svc and config file together.
    public class UserService : IUserService
    {
        public bool IsUserValid(string username, string password, string role)
        {
            return (username == "imcgaw" && password == "u/A4yDrVWsQl1jky8Pus6g==");
        }
      }
}
