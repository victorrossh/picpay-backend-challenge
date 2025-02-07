namespace Teste.Application.Services.Implementations;

public interface ICryptographyImp
{
    string EncryptPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}