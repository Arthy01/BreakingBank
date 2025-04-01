using System.Runtime.Versioning;

namespace BreakingBank.Models.SaveGame
{
    public class UpgradeData : SaveGameData
    {
        public List<DirtyField<Upgrade>> Upgrades { get; } = new List<DirtyField<Upgrade>>()
        {
            // EmployeeCount
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeCount_Paper, "Paper-Mitarbeiter", "Erhöht die Anzahl an Papier-Mitarbeitern", 0, 100, 50, 0, 1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeCount_Cartridge, "Toner-Mitarbeiter", "Erhöht die Anzahl an Toner-Mitarbeitern", 0, 100, 50, 0, 1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.EmployeeCount_Printer, "Druck-Mitarbeiter", "Erhöht die Anzahl an Druck-Mitarbeitern", 0, 100, 50, 0, 1) },
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

            // ProcessingCounts
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.ProcessingCount_Printer, "Druckeranzahl", "Erhöht die Anzahl an Druckern", 0, 100, 50, 1, 1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.ProcessingCount_WashingMachine, "Waschmaschinenanzahl", "Erhöht die Anzahl an Waschmaschinen", 0, 100, 50, 1, 1) },
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.ProcessingCount_Dryer, "Trockneranzahl", "Erhöht die Anzahl an Trocknern", 0, 100, 50, 1, 1) },

            // Player-Effizienz
            new DirtyField<Upgrade>{ Value = new Upgrade(Upgrade.UpgradeID.Player_Efficiency, "Spieler-Effizienz", "Erhöht den Wert deiner eigenen Klicks", 0, 200, 100, 1, 1) }
        };

        public List<DirtyField<Investment>> Investments { get; } = new()
        {
            // Investments
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.Laundromat, "Waschsalon", "Saubere Wäsche, saubere Gewinne.", false, 10000, 10) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.Pizzeria, "Pizzeria", "Pizza ist nicht nur lecker – sie ist auch perfekt, um andere, weniger legale Einnahmequellen unauffällig zu verstecken.", false, 50000, 50) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.SelfStorage, "Self-Storage-Anlage", "Die Leute haben zu viele Sachen und brauchen Platz. Vermiete leere Räume für volle Profite!", false, 250000, 250) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.Gym, "Fitnessstudio", "Die meisten zahlen, aber erscheinen nie. Perfektes Geschäftsmodell.", false, 1000000, 1000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.FranchiseRestaurant, "Franchise-Restaurant", "Frittierte Gewinne – ein Erfolgsrezept, das immer funktioniert.", false, 5000000, 5000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.LuxuryHotel, "Luxushotel", "Exklusive Suiten für zahlungskräftige Gäste – und Minibar-Preise, die astronomisch sind.", false, 20000000, 20000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.SoccerClub, "Fußballverein", "Ticketverkäufe, TV-Rechte und Trikot-Deals – ein profitabler Spielzug!", false, 100000000, 100000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.PrivateBank, "Privatbank", "Zinsen, Kredite und Kontogebühren – Geld mit Geld verdienen.", false, 500000000, 500000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.StreamingPlatform, "Streaming-Plattform", "Filme, Serien und jede Menge fragwürdige Eigenproduktionen – das Abo-Geld fließt trotzdem!", false, 2000000000, 2000000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.TelecommunicationsNetwork, "Telekommunikationsnetzwerk", "Jeder braucht Internet und Handys. Du kassierst mit jedem Anruf und jeder Nachricht.", false, 10000000000, 10000000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.ChipFactory, "Chipfabrik", "Ohne Mikrochips läuft nichts – und deine Fabriken liefern die unverzichtbare Technologie.", false, 50000000000, 50000000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.SpaceCompany, "Raumfahrtunternehmen", "Satellitenstarts, Marskolonien – die Zukunft ist interplanetar.", false, 250000000000, 250000000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.MultinationalHolding, "Multinationale Holding", "Beteiligungen an den größten Konzernen der Welt – wahre Marktmacht.", false, 1000000000000, 1000000000) },
            new DirtyField<Investment>(){ Value = new Investment(Investment.InvestmentID.BuyTheWorld, "Die Welt kaufen", "Herzlichen Glückwunsch, du besitzt jetzt alles. Dein Wort ist Gesetz!", false, 1000000000000000, 1000000000000) },
        };

        public UpgradeData(EconomyData economyData, ProcessingData processingData)
        {
            SetDatasForUpgradesAndInvestments(economyData, processingData);
            RegisterEvents();
        }

        public UpgradeData(List<DirtyField<Upgrade>> upgrades, List<DirtyField<Investment>> investmentList, EconomyData economyData, ProcessingData processingData)
        {
            Upgrades = upgrades;
            Investments = investmentList;

            SetDatasForUpgradesAndInvestments(economyData, processingData);
            RegisterEvents();
        }

        private void SetDatasForUpgradesAndInvestments(EconomyData economyData, ProcessingData processingData)
        {
            foreach (DirtyField<Upgrade> upgrade in Upgrades)
            {
                upgrade.Value!.SetEconomyData(economyData);
                upgrade.Value!.SetProcessingData(processingData);
            }

            foreach (DirtyField<Investment> investment in Investments)
            {
                investment.Value!.SetEconomyData(economyData);
            }
        }

        private void RegisterEvents()
        {
            foreach (DirtyField<Upgrade> upgradeField in Upgrades)
            {
                upgradeField.OnDirtyStateChanged += () => HandleDirtyStateChanged(upgradeField, "upgrade_" + upgradeField.Value!.Name + "_" + (int)upgradeField.Value!.ID);
                upgradeField.Value!.OnDirtyStateChanged += () => { if (upgradeField.Value!.Level.IsDirty) upgradeField.SetDirty(); };
            }

            foreach (DirtyField<Investment> investmentField in Investments)
            {
                investmentField.OnDirtyStateChanged += () => HandleDirtyStateChanged(investmentField, "investment_" + investmentField.Value!.Name + "_" + (int)investmentField.Value!.ID);
                investmentField.Value!.OnDirtyStateChanged += () => { if (investmentField.Value!.IsPurchased.IsDirty) investmentField.SetDirty(); };
            }
        }

        public override void ClearDirtyData()
        {
            Upgrades[0].ClearDirty();
            Upgrades[0].Value!.ClearDirtyData();

            base.ClearDirtyData();
        }
    }
}
