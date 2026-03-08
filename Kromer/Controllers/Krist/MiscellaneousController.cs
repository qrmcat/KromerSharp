using Kromer.Models.Api.Krist.Misc;
using Kromer.Models.Api.Krist.Wallet;
using Kromer.Repositories;
using Kromer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Kromer.Controllers.Krist;

[Route("api/krist")]
[ApiController]
public class MiscellaneousController(WalletRepository walletRepository, MiscRepository miscRepository) : ControllerBase
{
    /// <summary>
    /// Authenticate a private key.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<KristResultAuthentication>> Authenticate(KristRequestPrivateKey request)
    {
        var authResult = await walletRepository.VerifyAddressAsync(request.PrivateKey);

        return new KristResultAuthentication
        {
            Ok = true,
            Address = authResult.Wallet?.Address,
            Authed = authResult.Authed,
        };
    }

    /// <summary>
    /// Get the message of the day.
    /// </summary>
    /// <returns></returns>
    [HttpGet("motd")]
    public ActionResult<KristMotdResponse> Motd()
    {
        return miscRepository.GetMotd();
    }

    
    /// <summary>
    /// Get the wallet version.
    /// </summary>
    /// <returns></returns>
    [HttpGet("walletversion")]
    public ActionResult<KristWalletVersionResponse> WalletVersion()
    {
        return new KristWalletVersionResponse
        {
            Ok = true,
            WalletVersion = miscRepository.GetWalletVersion(),
        };
    }

    /// <summary>
    /// Retrieve information about what's new.
    /// </summary>
    /// <returns>An object containing details about recent updates.</returns>
    [HttpGet("whatsnew")]
    public ActionResult<object> WhatsNew()
    {
        // lgtm 👍
        return new { };
    }

    /// <summary>
    /// Get the current network supply.
    /// </summary>
    /// <returns></returns>
    [HttpGet("supply")]
    public async Task<ActionResult<KristResultSupply>> Supply()
    {
        return new KristResultSupply
        {
            Ok = true,
            MoneySupply = await walletRepository.GetNetworkSupply(),
        };
    }

    /// <summary>
    /// Get a V2 address.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2")]
    public ActionResult<KristResultV2Address> GetV2Address(KristRequestPrivateKey request)
    {
        return new KristResultV2Address
        {
            Ok = true,
            Address = Crypto.MakeV2Address(request.PrivateKey, "k"),
        };
    }
}