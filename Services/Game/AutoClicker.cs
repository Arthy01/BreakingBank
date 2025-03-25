using BreakingBank.Models;
using BreakingBank.Models.SaveGame;

namespace BreakingBank.Services.Game
{
    public class AutoClicker
    {
        private readonly SessionService _sessionService;
        private readonly GameService _gameService;
        private readonly GameSettings _gameSettings;
        private readonly ILogger<GameService> _logger;

        private Cache _progressCache = new();

        private class Cache
        {
            private Dictionary<Session, Dictionary<GameService.Clickable, double>>  _progress = new();

            public void AddSession(Session session)
            {
                _progress[session] = new()
                {
                    { GameService.Clickable.Cartridge, 0 },
                    { GameService.Clickable.Paper, 0 },
                    { GameService.Clickable.Printer, 0 },
                    { GameService.Clickable.WashingMachine, 0 },
                    { GameService.Clickable.Dryer, 0 }
                };
            }

            public void RemoveSession(Session session)
            {
                _progress.Remove(session);
            }

            public ulong AddProgress(Session session, GameService.Clickable clickable, double amount)
            {
                if (!_progress.ContainsKey(session))
                    AddSession(session);

                _progress[session][clickable] += amount;

                ulong clicksToPerform = (ulong)Math.Max(0, _progress[session][clickable]);

                _progress[session][clickable] -= clicksToPerform;
                return clicksToPerform;
            }
        }

        private struct UpgradeData
        {
            public double EmployeeCount { get; private set; }
            public double EmployeeSpeed { get; private set; }
            public double EmployeeEfficiency { get; private set; }

            public UpgradeData(double empCount, double empSpeed, double empEfficiency)
            {
                EmployeeCount = empCount;
                EmployeeSpeed = empSpeed;
                EmployeeEfficiency = empEfficiency;
            }
        }

        public AutoClicker(SessionService sessionService, GameService gameService, GameSettings gameSettings, ILogger<GameService> logger)
        {
            _sessionService = sessionService;
            _gameService = gameService;
            _gameSettings = gameSettings;
            _logger = logger;
            
            _sessionService.OnSessionClosed += _progressCache.RemoveSession;
        }

        public void HandleAutoClicker()
        {
            foreach (Session session in _sessionService.GetAllActiveSessions())
            {
                foreach (GameService.Clickable clickable in Enum.GetValues(typeof(GameService.Clickable)))
                {
                    if (clickable == GameService.Clickable.Undefined)
                        continue;

                    UpgradeData upgradeData = GetUpgradesForClickable(session, clickable);

                    double clicksPerSecond = upgradeData.EmployeeCount * upgradeData.EmployeeSpeed * upgradeData.EmployeeEfficiency;

                    double clicksThisTick = clicksPerSecond / _gameSettings.TickRate;

                    ulong clicksToPerform = _progressCache.AddProgress(session, clickable, clicksThisTick);
                    _gameService.OnClickableClicked(session, clickable, clicksToPerform);
                }
            }
        }

        private UpgradeData GetUpgradesForClickable(Session session, GameService.Clickable clickable)
        {
            List<DirtyField<Upgrade>> upgrades = session.SaveGame.Upgrades.Upgrades;

            switch (clickable)
            {
                case GameService.Clickable.Paper:
                    return new UpgradeData(
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeCount_Paper, upgrades, true),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeSpeed_Paper, upgrades),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeEfficiency_Paper, upgrades));

                case GameService.Clickable.Cartridge:
                    return new UpgradeData(
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeCount_Cartridge, upgrades, true),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeSpeed_Cartridge, upgrades),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeEfficiency_Cartridge, upgrades));

                case GameService.Clickable.Printer:
                    return new UpgradeData(
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeCount_Printer, upgrades, true),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeSpeed_Printer, upgrades),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeEfficiency_Printer, upgrades));

                case GameService.Clickable.WashingMachine:
                    return new UpgradeData(
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeCount_WashingMachine, upgrades, true),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeSpeed_WashingMachine, upgrades),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeEfficiency_WashingMachine, upgrades));

                case GameService.Clickable.Dryer:
                    return new UpgradeData(
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeCount_Dryer, upgrades, true),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeSpeed_Dryer, upgrades),
                        GetUpgradeValueByID(Upgrade.UpgradeID.EmployeeEfficiency_Dryer, upgrades));

                default:
                    throw new NotImplementedException();
            }
        }

        private double GetUpgradeValueByID(Upgrade.UpgradeID id, List<DirtyField<Upgrade>> upgrades, bool asInt = false)
        {
            Upgrade upgrade = upgrades.Find(x => x.Value!.ID == id)!.Value!;

            return asInt ? upgrade.GetEffectInt() : upgrade.GetEffectDouble();
        }
    }
}
