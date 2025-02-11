using Teste.Application.DTOs.Requests;
using Teste.Application.DTOs.Responses;
using Teste.Application.Services.Implementations;
using Teste.Application.UseCases.Implementations;
using Teste.Application.UseCases.Validators;
using Teste.Domain.Repositories;
using Teste.Shared.Constants;
using Teste.Shared.Exceptions;

namespace Teste.Application.UseCases;

public class SignInUseCase(
    IAccountRepository repository,
    ITokenizationImp tokenization,
    ICryptographyImp cryptography)
    : ISignInImp
{
    public async Task<TokenRes> ExecuteSignInAsync(SignInReq request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validation = await new SignInValidator().ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new BadRequestException(validation.Errors.Select(er => er.ErrorMessage).ToArray());

            var account = await repository.GetByEmailAsync(request.email, cancellationToken);
            if (account is null)
                throw new NotFoundException([AccountMessages.EMAIL_NOT_FOUND]);

            if (!cryptography.VerifyPassword(request.password, account.Password))
                throw new BadRequestException([AccountMessages.PASSWORD_INCORRECT]);

            var (token, expiry) = await tokenization.GenerateTokenAsync(account.Id);

            return new TokenRes(token, expiry);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new UnknownException([UnknownMessages.UNEXPECTED_ERROR]);
        }
    }
}