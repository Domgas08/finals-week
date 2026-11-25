namespace FinalsWeek.Web;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public class CurrentUser
{
    private readonly AuthenticationStateProvider authentication;

    public CurrentUser(AuthenticationStateProvider authenticationStateProvider)
    {
        this.authentication = authenticationStateProvider;
    }

    public async Task<string?> GetIdAsync()
    {
        var state = await this.authentication.GetAuthenticationStateAsync();
        return state.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}