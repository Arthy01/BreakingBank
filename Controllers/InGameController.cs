using BreakingBank.Models;
using BreakingBank.Models.SaveGame;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BreakingBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InGameController : ControllerBase
    {
        private readonly SessionService _sessionService;
        private readonly ILogger<InGameController> _logger;

        public InGameController(SessionService sessionService, ILogger<InGameController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet("upgrades")]
        public ActionResult<List<dynamic>> GetUpgrades(Upgrade.UpgradeID id = Upgrade.UpgradeID.Undefined)
        {
            User user = Models.User.GetByClaims(User);

            Session? session = _sessionService.GetSessionByUser(user);

            if (session == null)
                return BadRequest("User is not connected to a session!");

            List<dynamic> result = new();

            if (id == Upgrade.UpgradeID.Undefined)
            {
                foreach (DirtyField<Upgrade> upgrade in session.SaveGame.Upgrades.Upgrades)
                {
                    if (upgrade.Value != null)
                        result.Add(new { Upgrade = upgrade.Value, CurrentCost = upgrade.Value.GetCost() });
                }
            }
            else
            {
                Upgrade? upgrade = session.SaveGame.Upgrades.Upgrades.Find(x => x.Value.ID == id).Value;

                if (upgrade != null)
                    result.Add(new { Upgrade = upgrade, CurrentCost = upgrade.GetCost(), CurrentEffectInt = upgrade.GetEffectInt(), CurrentEffectDbl = upgrade.GetEffectDouble() });
            }

            if (result.Count == 0)
                return NotFound("No Upgrade with ID " + id + " has been found!");

            return Ok(result);
        }

        [HttpGet("upgradeCost")]
        public ActionResult<ulong> GetUpgradeCost(Upgrade.UpgradeID id)
        {
            User user = Models.User.GetByClaims(User);

            Session? session = _sessionService.GetSessionByUser(user);

            if (session == null)
                return BadRequest("User is not connected to a session!");

            if (id == Upgrade.UpgradeID.Undefined)
                return BadRequest("The UpgradeID cant be undefined!");

            Upgrade? upgrade = session.SaveGame.Upgrades.Upgrades.Find(x => x.Value.ID == id).Value;

            if (upgrade == null)
                return NotFound("No Upgrade with ID " + id + " has been found!");

            return Ok(upgrade.GetCost());
        }

        [HttpGet("upgradeEffect")]
        public ActionResult<dynamic> GetUpgradeEffect(Upgrade.UpgradeID id)
        {
            User user = Models.User.GetByClaims(User);

            Session? session = _sessionService.GetSessionByUser(user);

            if (session == null)
                return BadRequest("User is not connected to a session!");

            if (id == Upgrade.UpgradeID.Undefined)
                return BadRequest("The UpgradeID cant be undefined!");

            Upgrade? upgrade = session.SaveGame.Upgrades.Upgrades.Find(x => x.Value.ID == id).Value;

            if (upgrade == null)
                return NotFound("No Upgrade with ID " + id + " has been found!");

            return Ok(new { CurrentEffectInt = upgrade.GetEffectInt(), CurrentEffectDbl = upgrade.GetEffectDouble() });
        }
    }
}
