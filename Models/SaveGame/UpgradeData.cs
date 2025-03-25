namespace BreakingBank.Models.SaveGame
{
    public class UpgradeData : SaveGameData
    {
        public List<DirtyField<Upgrade>> Upgrades { get; } = new List<DirtyField<Upgrade>>()
        {
            // EmployeeCount
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeCount_Paper, "Paper-Mitarbeiter", "Erhöht die Anzahl an Papier-Mitarbeitern", 0, 100, 50, 0, 1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeCount_Cartridge, "Toner-Mitarbeiter", "Erhöht die Anzahl an Toner-Mitarbeitern", 0, 100, 50, 0, 1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeCount_Printer, "Druck-Mitarbeiter", "Erhöht die Anzahl an Druck-Mitarbeitern", 0, 100, 50, 1, 0) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeCount_WashingMachine, "Wasch-Mitarbeiter", "Erhöht die Anzahl an Wasch-Mitarbeitern", 0, 100, 50, 0, 1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeCount_Dryer, "Trocken-Mitarbeiter", "Erhöht die Anzahl an Trocken-Mitarbeitern", 0, 100, 50, 0, 1) },

            // EmployeeSpeed
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeSpeed_Paper, "Papiergeschwindigkeit", "Erhöht die Klickgeschwindigkeit von Papier-Mitarbeitern", 0, 100, 50, 0.2, 0.2) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeSpeed_Cartridge, "Tonergeschwindigkeit", "Erhöht die Klickgeschwindigkeit von Toner-Mitarbeitern", 0, 100, 50, 0.2, 0.2) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeSpeed_Printer, "Druckgeschwindigkeit", "Erhöht die Klickgeschwindigkeit von Druck-Mitarbeitern", 0, 100, 50, 0.2, 0.2) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeSpeed_WashingMachine, "Waschgeschwindigkeit", "Erhöht die Klickgeschwindigkeit von Wasch-Mitarbeitern", 0, 100, 50, 0.2, 0.2) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeSpeed_Dryer, "Trockengeschwindigkeit", "Erhöht die Klickgeschwindigkeit von Trocken-Mitarbeitern", 0, 100, 50, 0.2, 0.2) },

            // EmployeeEfficiency
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeEfficiency_Paper, "Papiereffizienz", "Erhöht den Wert eines Klicks von Papier-Mitarbeitern", 0, 100, 50, 1, 0.1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeEfficiency_Cartridge, "Toner-Effizienz", "Erhöht den Wert eines Klicks von Toner-Mitarbeitern", 0, 100, 50, 1, 0.1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeEfficiency_Printer, "Druck-Effizienz", "Erhöht den Wert eines Klicks von Druck-Mitarbeitern", 0, 100, 50, 1, 0.1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeEfficiency_WashingMachine, "Wasch-Effizienz", "Erhöht den Wert eines Klicks von Wasch-Mitarbeitern", 0, 100, 50, 1, 0.1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeEfficiency_Dryer, "Trocken-Effizienz", "Erhöht den Wert eines Klicks von Trocken-Mitarbeitern", 0, 100, 50, 1, 0.1) },

            // Player-Effizienz
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.Player_Efficiency, "Spieler-Effizienz", "Erhöht den Wert deiner eigenen Klicks", 0, 200, 100, 1, 1) }
        };

        public UpgradeData()
        {
            RegisterEvents();
        }

        public UpgradeData(List<DirtyField<Upgrade>> upgrades)
        {
            Upgrades = upgrades;

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            foreach (DirtyField<Upgrade> upgradeField in Upgrades)
            {
                upgradeField.OnDirtyStateChanged += () => HandleDirtyStateChanged(upgradeField, upgradeField.Value!.Name);
                upgradeField.Value!.OnDirtyStateChanged += () => { if (upgradeField.Value!.Level.IsDirty) upgradeField.SetDirty(); };
            }

            /*
            Upgrades[0].OnDirtyStateChanged += () => HandleDirtyStateChanged(Upgrades[0], Upgrades[0].Value!.Name);
            Upgrades[0].Value!.OnDirtyStateChanged += () => { if (Upgrades[0].Value!.Level.IsDirty) Upgrades[0].SetDirty(); };
            */
        }

        public override void ClearDirtyData()
        {
            Upgrades[0].ClearDirty();
            Upgrades[0].Value!.ClearDirtyData();

            base.ClearDirtyData();
        }
    }
}
