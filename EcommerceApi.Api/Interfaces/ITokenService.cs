using EcommerceApi.Api.Models;

namespace EcommerceApi.Api.Interfaces;

public interface ITokenService 
{ 
    string CreateToken(AppUser user); // Ensure this says 'string', not 'object'
}