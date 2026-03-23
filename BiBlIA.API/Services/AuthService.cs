using BíblIA.Api.Data;
using BíblIA.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace BíblIA.Api.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Validar dados
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email e senha são obrigatórios"
            };
        }

        if (request.Password != request.PasswordConfirm)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Senhas não conferem"
            };
        }

        // Verificar se usuário já existe
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (existingUser != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email já cadastrado"
            };
        }

        // Criar novo usuário
        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            PasswordHash = HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Name);

        return new AuthResponse
        {
            Success = true,
            Message = "Registro realizado com sucesso",
            Token = token,
            User = new UserAuthDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email ou senha incorretos"
            };
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Name);

        return new AuthResponse
        {
            Success = true,
            Message = "Login realizado com sucesso",
            Token = token,
            User = new UserAuthDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            }
        };
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}