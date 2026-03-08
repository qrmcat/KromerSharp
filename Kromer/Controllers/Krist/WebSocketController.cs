using Kromer.Models.Api.Krist.Wallet;
using Kromer.Models.Api.Krist.WebSocket;
using Kromer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kromer.Controllers.Krist;

[ApiController]
[Route("api/krist/ws")]
public class WebSocketController(SessionService sessionService, SessionManager.SessionManager sessionManager) : ControllerBase
{
    /// <summary>
    /// Initializes a new WebSocket connection by creating a session and generating
    /// a WebSocket URL for the client to connect.
    /// </summary>
    /// <param name="request">
    /// An optional parameter containing a private key. If provided, the session will
    /// be initialized using this private key.
    /// </param>
    /// <returns>
    /// A response containing the WebSocket URL, the connection expiration time, and
    /// a success flag indicating whether the initialization was successful.
    /// </returns>
    [HttpPost("start")]
    public async Task<ActionResult<KristResponseWebSocketInitiate>> InitConnection(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
        KristRequestOptionalPrivateKey? request = null)
    {
        var sessionId = await sessionService.InstantiateSession(request?.PrivateKey);
        var url = $"wss://{HttpContext.Request.Host}/api/krist/ws/gateway/{sessionId}";

        return new KristResponseWebSocketInitiate
        {
            Ok = true,
            Url = new Uri(url),
            Expires = (int)SessionManager.SessionManager.ConnectionExpireTime.TotalSeconds,
        };
    }

    /// <summary>
    /// Handles an incoming WebSocket connection request. Validates the session associated
    /// with the specified session ID and establishes a WebSocket connection. Manages the
    /// WebSocket session lifecycle, including listening for messages and handling
    /// disconnection events.
    /// </summary>
    /// <param name="sessionId">
    /// A unique identifier for the WebSocket session. This parameter is used to retrieve
    /// and validate the session associated with the client initiating the connection.
    /// </param>
    /// <returns>
    /// An action result indicating the outcome of the WebSocket connection attempt. Will
    /// return a bad request response if the request is not a valid WebSocket request, or
    /// an empty result upon successful connection setup and session handling.
    /// </returns>
    [Route("gateway/{sessionId:guid}")]
    public async Task<ActionResult> Gateway(Guid sessionId)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            return BadRequest("Invalid WebSocket request");
        }

        var session = sessionService.ValidateSession(sessionId);
        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        session.WebSocket = webSocket;
        await sessionManager.HandleWebSocketSessionAsync(session, HttpContext.RequestAborted);

        return new EmptyResult();
    }
}