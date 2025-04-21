    using ShakeMe.Core.Models;
    using ShakeMe.Core.Dtos;

    namespace ShakeMe.Core.Services;

    public class UserService : IUserService
    {
        private readonly List<UserModel> _users;

        public UserService()
        {
            Console.WriteLine("🛠️ Constructeur UserService appelé");
            _users = new List<UserModel>();

            // Création d’un utilisateur fictif
            _users.Add(new UserModel
            {
                FirstName = "Killian",
                LastName = "Carvalho",
                Email = "test@shake.me",
                Pseudo = "0kiw4n",
                PasswordHash = "Pa/5842697130",
                DateOfBirth = new DateTime(1998, 1, 19),
                LastActive = DateTime.UtcNow
            });
        }

        public async Task<UserModel> CreateUser(CreateUserDto dto)
        {
            Console.WriteLine("📥 Dans UserService.CreateUser");
            var user = new UserModel
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Pseudo = string.IsNullOrWhiteSpace(dto.Pseudo)
                    ? GeneratePseudo(dto.FirstName, dto.LastName)
                    : dto.Pseudo,
                LastActive = DateTime.UtcNow,
                PasswordHash = "to be encrypted"
            };
            Console.WriteLine($"🧪 dto.Pseudo: {dto.Pseudo}, dto.FirstName: {dto.FirstName}, dto.LastName: {dto.LastName}");
            Console.WriteLine($"📦 Email = {dto.Email}, DateOfBirth = {dto.DateOfBirth}");

            if (_users == null)
            {
                Console.WriteLine("❌ La liste _users est NULL !");
                throw new Exception("_users est null !");
            }


            _users.Add(user);

            // Simuler une tâche asynchrone (par exemple appel API)
            await Task.Delay(10); 

            return user;
        }
        
        public UserModel? UpdateUser(int id, UpdateUserDto dto)
        {
            var user = GetUserById(id);
            if (user == null) return null;

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Email = dto.Email ?? user.Email;
            user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
            user.Pseudo = dto.Pseudo ?? user.Pseudo;
            user.LastActive = DateTime.UtcNow;

            return user;
        }

        public UserModel? GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            return _users;
        }

        public bool DeleteUser(int id)
        {
            var user = GetUserById(id);
            if (user == null) return false;

            return _users.Remove(user);
        }
        
        public async Task<UserModel?> AuthenticateUserAsync(string identifier, string password)
        {
            var user = _users.FirstOrDefault(u =>
                (u.Email.Equals(identifier, StringComparison.OrdinalIgnoreCase)
                 || u.Pseudo.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                && u.PasswordHash == password);

            return await Task.FromResult(user);
        }

        private string GeneratePseudo(string firstName, string lastName)
        {
            Console.WriteLine($"🧪 dto.FirstName: {firstName}, dto.LastName: {lastName}");
            return $"{firstName.ToLowerInvariant()}_{lastName.ToLowerInvariant()}_{Guid.NewGuid().ToString()[..4]}";
        }
    }