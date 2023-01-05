using SmartCalcApp.Deals;
using SmartCalcApp.Info;
using System;
using System.Configuration;
using System.IO;

namespace SmartCalcApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = ConfigurationManager.AppSettings["DataDirectory"].ToString();

            if (dir.IsAbsolutePath())
            {
                Context.DealsFilePath = Path.Combine(dir, "deals.xlsx");
                Context.InfoFilePath = Path.Combine(dir, "info.xlsx");
            }
            else
            {
                dir = Path.Combine(Environment.CurrentDirectory, dir);
                dir = Path.GetFullPath(dir);

                Context.DealsFilePath = Path.Combine(dir, "deals.xlsx");
                Context.InfoFilePath = Path.Combine(dir, "info.xlsx");
            }          

            //AddSymbol();
            //AddSymbolData();
            //UpdateInfo();
            //return;

            switch (args[0])
            {
                case "SymbolAdded":
                    AddSymbol();
                    AddSymbolData();
                    UpdateInfo();
                    break;

                case "DealsAdded":
                    AddSymbolData();
                    UpdateInfo();
                    break;

                case "UdpateData":
                    UpdateInfo();
                    break;

                case "UdpateBuyInfo":
                    UdpateBuyInfo();
                    break;
            }
        }

        private static void AddSymbol()
        {
            DealsHandler.SortWorksheets();
            InfoHandler.UpdateIdMapping();
            InfoHandler.RebuildPortfolioList();
        }

        private static void AddSymbolData()
        {
            DealsHandler.CalculateDeals();
            InfoHandler.ApplyDealsData();
        }

        private static void UpdateInfo()
        {
            InfoHandler.UpdatePortfolio();
        }

        private static void UdpateBuyInfo()
        {
            InfoHandler.UpdateBuyInfo();
        }
    }
}
