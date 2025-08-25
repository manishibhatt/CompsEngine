using CompsEngine.Data;           // adjust to your generated namespaces
using CompsEngine.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompsEngine.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CompsEngineDbContext _db;
    private readonly JwtTokenService _jwt;

    public AuthController(CompsEngineDbContext db, JwtTokenService jwt)
    {
        _db = db; _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
    {
        // 1) email unique
        var exists = await _db.Users.AnyAsync(u => u.Email == req.Email);
        if (exists) return Conflict(new { message = "Email already registered." });

        // 2) hash+salt
        var (hash, salt) = PasswordHasher.Hash(req.Password);

        // 3) create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = req.Email,
            UserName = req.UserName,
            PasswordHash = hash,
            PasswordSalt = salt,
            EmailConfirmed = 0
        };
        _db.Users.Add(user);

        // 4) ensure BASIC plan exists (in case seed was missed)
        var plan = await _db.Plans.FirstOrDefaultAsync(p => p.Code == "BASIC");
        if (plan == null)
        {
            plan = new Plan
            {
                Id = Guid.NewGuid(),
                Code = "BASIC",
                Name = "Basic Plan",
                PriceCents = 0,
                Currency = "USD",
                BillInterval = "month",
                TrialDays = 7,
                IsActive = 1
            };
            _db.Plans.Add(plan);
        }

        // 5) create 7-day trial subscription
        var now = DateTime.UtcNow;
        var trialEnd = now.AddDays(plan.TrialDays);
        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PlanId = plan.Id,
            Status = "trialing",
            StartAt = now,
            CurrentPeriodStart = now,
            CurrentPeriodEnd = trialEnd,
            TrialEndAt = trialEnd,
            CancelAtPeriodEnd = 0
        };
        _db.Subscriptions.Add(sub);

        await _db.SaveChangesAsync();

        // 6) tokens
        var (access, exp) = _jwt.CreateAccessToken(user.Id.ToString());
        var (rtoken, rexp) = _jwt.CreateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = rtoken,
            ExpiresAt = rexp,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });
        await _db.SaveChangesAsync();

        return Ok(new AuthResponse(access, exp, rtoken, rexp));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user == null) return Unauthorized(new { message = "Invalid credentials." });

        if (!PasswordHasher.Verify(req.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized(new { message = "Invalid credentials." });

        var (access, exp) = _jwt.CreateAccessToken(user.Id.ToString());
        var (rtoken, rexp) = _jwt.CreateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = rtoken,
            ExpiresAt = rexp,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });
        await _db.SaveChangesAsync();

        return Ok(new AuthResponse(access, exp, rtoken, rexp));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] string refreshToken)
    {
        var rt = await _db.RefreshTokens.Include(r => r.User)
                 .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (rt == null || rt.RevokedAt != null || rt.ExpiresAt <= DateTime.UtcNow)
            return Unauthorized(new { message = "Invalid refresh token." });

        // rotate: revoke old + issue new
        rt.RevokedAt = DateTime.UtcNow;

        var (access, exp) = _jwt.CreateAccessToken(rt.UserId.ToString());
        var (newRt, newExp) = _jwt.CreateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = rt.UserId,
            Token = newRt,
            ExpiresAt = newExp,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        await _db.SaveChangesAsync();
        return Ok(new AuthResponse(access, exp, newRt, newExp));
    }
}
