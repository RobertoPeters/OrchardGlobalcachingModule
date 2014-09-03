using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Globalcaching.Services
{
    public interface IMacroService : IDependency
    {
        List<GCEuGeocacheFilterMacro> SaveMacro(string name, string macro);
        List<GCEuGeocacheFilterMacro> DeleteMacro(long ID);
        List<GCEuGeocacheFilterMacro> SaveAndRunMacro(string name, string macro);
        GCEuGeocacheFilterMacro GetMacro(long ID);
        GCEuGeocacheFilterMacro GetMacro(int UserID, string name);
        List<GCEuGeocacheFilterMacro> GetMacrosOfUser(int UserID);
        MacroFunctionInfo[] GetFunctions();
    }

    public class MacroService : IMacroService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public MacroService(IGCEuUserSettingsService gcEuUserSettingsService,
            IWorkContextAccessor workContextAccessor)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _workContextAccessor = workContextAccessor;
        }

        public MacroFunctionInfo[] GetFunctions()
        {
            MacroFunctionInfo[] result = new MacroFunctionInfo[]
            {
                new MacroFunctionInfo
                {
                    Name = "Beschikbaar",
                    ProtoType = "Beschikbaar()",
                    Description = "Caches die beschikbaar zijn",
                    Examples = "Beschikbaar()",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietBeschikbaar",
                    ProtoType = "NietBeschikbaar()",
                    Description = "Caches die niet beschikbaar zijn",
                    Examples = "NietBeschikbaar()",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "Gearchiveerd",
                    ProtoType = "Gearchiveerd()",
                    Description = "Caches die gearchiveerd zijn",
                    Examples = "Gearchiveerd()",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietGearchiveerd",
                    ProtoType = "NietGearchiveerd()",
                    Description = "Caches die niet gearchiveerd zijn",
                    Examples = "NietGearchiveerd()",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "CacheType",
                    ProtoType = "CacheType(type1, type2, ...typen)",
                    Description = "Caches van de types die zijn opgegeven\r\nTraditional Cache = 2\r\nMulti-cache = 3\r\nVirtual Cache = 4\r\nLetterbox Hybrid = 5\r\nEvent Cache = 6\r\nUnknown (Mystery) Cache = 8\r\nProject APE Cache = 9\r\nWebcam Cache = 11\r\nLocationless (Reverse) Cache = 12\r\nCache In Trash Out Event = 13\r\nEarthcache = 137\r\nMega-Event Cache = 453\r\nWherigo Cache = 1858\r\nLost and Found Event Cache = 3653",
                    Examples = "CacheType(2)\r\nCacheType(2,4,6)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietCacheType",
                    ProtoType = "NietCacheType(type1, type2, ...typen)",
                    Description = "Caches die niet van de opgegeven types zijn\r\nTraditional Cache = 2\r\nMulti-cache = 3\r\nVirtual Cache = 4\r\nLetterbox Hybrid = 5\r\nEvent Cache = 6\r\nUnknown (Mystery) Cache = 8\r\nProject APE Cache = 9\r\nWebcam Cache = 11\r\nLocationless (Reverse) Cache = 12\r\nCache In Trash Out Event = 13\r\nEarthcache = 137\r\nMega-Event Cache = 453\r\nWherigo Cache = 1858\r\nLost and Found Event Cache = 3653",
                    Examples = "NietCacheType(2)\r\nNietCacheType(2,4,6)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "Container",
                    ProtoType = "Container(type1, type2, ...typen)",
                    Description = "Caches waarvan de container van de opgegeven types zijn\r\nNot chosen = 1\r\nMicro = 2\r\nRegular = 3\r\nLarge = 4\r\nVirtual = 5\r\nOther = 6\r\nSmall = 8",
                    Examples = "Container(2)\r\nContainer(2,4,6)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietContainer",
                    ProtoType = "NietContainer(type1, type2, ...typen)",
                    Description = "Caches waarvan de container niet van de opgegeven types zijn\r\nNot chosen = 1\r\nMicro = 2\r\nRegular = 3\r\nLarge = 4\r\nVirtual = 5\r\nOther = 6\r\nSmall = 8",
                    Examples = "NietContainer(2)\r\nNietContainer(2,4,6)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "LandCode",
                    ProtoType = "LandCode(code1, code2, ...coden)",
                    Description = "Caches die in 1 van de opgegeven landen liggen\r\nBelgie = 4, Luxemburg = 8, Nederland = 141",
                    Examples = "LandCode(141)\r\nLandCode(4,8)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietLandCode",
                    ProtoType = "NietLandCode(code1, code2, ...coden)",
                    Description = "Caches die in geen van de opgegeven landen liggen\r\nBelgie = 4, Luxemburg = 8, Nederland = 141",
                    Examples = "NietLandCode(141)\r\nNietLandCode(4,8)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "AanmaakDatumVoor",
                    ProtoType = "AanmaakDatumVoor(dag,maand,jaar)",
                    Description = "Caches die aangemaakt zijn voor de opgegeven datum",
                    Examples = "AanmaakDatumVoor(22,4,2014)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietAanmaakDatumVoor",
                    ProtoType = "NietAanmaakDatumVoor(dag,maand,jaar)",
                    Description = "Caches die niet aangemaakt zijn voor de opgegeven datum",
                    Examples = "NietAanmaakDatumVoor(22,4,2014)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "AanmaakDatumNa",
                    ProtoType = "AanmaakDatumNa(dag,maand,jaar)",
                    Description = "Caches die aangemaakt zijn na de opgegeven datum",
                    Examples = "AanmaakDatumNa(22,4,2014)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietAanmaakDatumNa",
                    ProtoType = "NietAanmaakDatumNa(dag,maand,jaar)",
                    Description = "Caches die niet aangemaakt zijn na de opgegeven datum",
                    Examples = "NietAanmaakDatumNa(22,4,2014)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "AangepastNaDatum",
                    ProtoType = "AangepastNaDatum(dag,maand,jaar)",
                    Description = "Caches die aangepast zijn na de opgegeven datum",
                    Examples = "AangepastNaDatum(22,4,2014)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietAangepastNaDatum",
                    ProtoType = "NietAangepastNaDatum(dag,maand,jaar)",
                    Description = "Caches die niet aangepast zijn na de opgegeven datum",
                    Examples = "NietAangepastNaDatum(22,4,2014)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "GepubliceerdNaDatum",
                    ProtoType = "GepubliceerdNaDatum(dag,maand,jaar)",
                    Description = "Caches die gepubliceerd zijn na de opgegeven datum",
                    Examples = "GepubliceerdNaDatum(22,4,2014)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietGepubliceerdNaDatum",
                    ProtoType = "NietGepubliceerdNaDatum(dag,maand,jaar)",
                    Description = "Caches die niet gepubliceerd zijn na de opgegeven datum",
                    Examples = "NietGepubliceerdNaDatum(22,4,2014)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "MoeilijkheidGroterDan",
                    ProtoType = "MoeilijkheidGroterDan(waarde)",
                    Description = "Caches met een moeilijkheid groter dan de opgegeven waarde",
                    Examples = "MoeilijkheidGroterDan(2)\r\nMoeilijkheidGroterDan(1.5)",
                    PMOnly = true
                }
                , new MacroFunctionInfo
                {
                    Name = "NietMoeilijkheidGroterDan",
                    ProtoType = "NietMoeilijkheidGroterDan(waarde)",
                    Description = "Caches met een moeilijkheid kleiner of gelijk aan de opgegeven waarde",
                    Examples = "NietMoeilijkheidGroterDan(2)\r\nMoeilijkheidGroterDan(1.5)",
                    PMOnly = true
                }
                , new MacroFunctionInfo
                {
                    Name = "TerreinGroterDan",
                    ProtoType = "TerreinGroterDan(waarde)",
                    Description = "Caches met een terrein groter dan de opgegeven waarde",
                    Examples = "TerreinGroterDan(2)\r\nTerreinGroterDan(1.5)",
                    PMOnly = true
                }
                , new MacroFunctionInfo
                {
                    Name = "NietTerreinGroterDan",
                    ProtoType = "NietTerreinGroterDan(waarde)",
                    Description = "Caches met een terrein kleiner of gelijk aan de opgegeven waarde",
                    Examples = "NietTerreinGroterDan(2)\r\nNietTerreinGroterDan(1.5)",
                    PMOnly = true
                }
                , new MacroFunctionInfo
                {
                    Name = "CacheAfstandGroterDan",
                    ProtoType = "CacheAfstandGroterDan(waarde)",
                    Description = "Caches met een afstand groter dan de opgegeven waarde in kilometers",
                    Examples = "CacheAfstandGroterDan(2)\r\nCacheAfstandGroterDan(1.5)",
                    PMOnly = true
                }
                , new MacroFunctionInfo
                {
                    Name = "NietCacheAfstandGroterDan",
                    ProtoType = "NietCacheAfstandGroterDan(waarde)",
                    Description = "Caches met een afstand kleiner of gelijk aan de opgegeven waarde in kilometers",
                    Examples = "NietCacheAfstandGroterDan(2)\r\nNietCacheAfstandGroterDan(1.5)",
                    PMOnly = true
                }
                , new MacroFunctionInfo
                {
                    Name = "AantalPlaatjesGroterDan",
                    ProtoType = "AantalPlaatjesGroterDan(waarde)",
                    Description = "Caches met meer plaatjes in de cachebeschrijving dan de opgegeven waarde",
                    Examples = "AantalPlaatjesGroterDan(2)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietAantalPlaatjesGroterDan",
                    ProtoType = "NietAantalPlaatjesGroterDan(waarde)",
                    Description = "Caches met minder of gelijk aantal plaatjes in de cachebeschrijving dan de opgegeven waarde",
                    Examples = "NietAantalPlaatjesGroterDan(2)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "HintsBevatTekst",
                    ProtoType = "HintsBevatTekst(tekst)",
                    Description = "Caches waarvan de hint de opgegeven tekst bevat",
                    Examples = "HintsBevatTekst(\"boom\")\r\nHintsBevatTekst(\"zeg eens \\\"hallo\\\"\")",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "FavorietenMeerDan",
                    ProtoType = "FavorietenMeerDan(waarde)",
                    Description = "Caches met meer dan het opgegeven aantal Favorites",
                    Examples = "FavorietenMeerDan(2)",
                    PMOnly = true
                }
                , new MacroFunctionInfo
                {
                    Name = "NietFavorietenMeerDan",
                    ProtoType = "NietFavorietenMeerDan(waarde)",
                    Description = "Caches met niet meer dan het opgegeven aantal Favorites",
                    Examples = "NietFavorietenMeerDan(2)",
                    PMOnly = true
                }
                , new MacroFunctionInfo
                {
                    Name = "IsGeblokkeerd",
                    ProtoType = "IsGeblokkeerd()",
                    Description = "Caches die geblokkeerd zijn",
                    Examples = "IsGeblokkeerd()",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietIsGeblokkeerd",
                    ProtoType = "NietIsGeblokkeerd()",
                    Description = "Caches die niet geblokkeerd zijn",
                    Examples = "NietIsGeblokkeerd()",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "IsPremiumCache",
                    ProtoType = "IsPremiumCache()",
                    Description = "Caches die alleen voor Premium Mebers zijn",
                    Examples = "IsPremiumCache()",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietIsPremiumCache",
                    ProtoType = "NietIsPremiumCache()",
                    Description = "Caches die niet alleen voor Premium Mebers zijn",
                    Examples = "NietIsPremiumCache()",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "LatitudeTussen",
                    ProtoType = "LatitudeTussen(min, max)",
                    Description = "Caches met een lengtegraad tussen min en max",
                    Examples = "LatitudeTussen(55.5, 56.0)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietLatitudeTussen",
                    ProtoType = "NietLatitudeTussen(min, max)",
                    Description = "Caches met een lengtegraad die niet tussen min en max ligt",
                    Examples = "NietLatitudeTussen(55.5, 56.0)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "LongitudeTussen",
                    ProtoType = "LongitudeTussen(min, max)",
                    Description = "Caches met een breedtegraad tussen min en max",
                    Examples = "LongitudeTussen(5.5, 6.0)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietLongitudeTussen",
                    ProtoType = "NietLongitudeTussen(min, max)",
                    Description = "Caches met een breedtegraad die niet tussen min en max ligt",
                    Examples = "NietLongitudeTussen(5.5, 6.0)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NaamBevatTekst",
                    ProtoType = "NaamBevatTekst(tekst)",
                    Description = "Caches waarvan de naam de opgegeven tekst bevat",
                    Examples = "NaamBevatTekst(\"boom\")\r\nNaamBevatTekst(\"zeg eens \\\"hallo\\\"\")",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietNaamBevatTekst",
                    ProtoType = "NietNaamBevatTekst(tekst)",
                    Description = "Caches waarvan de naam niet de opgegeven tekst bevat",
                    Examples = "NietNaamBevatTekst(\"boom\")\r\nNaamBevatTekst(\"zeg eens \\\"hallo\\\"\")",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NaamBegintMetTekst",
                    ProtoType = "NaamBegintMetTekst(tekst)",
                    Description = "Caches waarvan de naam begint met de opgegeven tekst",
                    Examples = "NaamBegintMetTekst(\"boom\")\r\nNaamBegintMetTekst(\"zeg eens \\\"hallo\\\"\")",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietNaamBegintMetTekst",
                    ProtoType = "NietNaamBegintMetTekst(tekst)",
                    Description = "Caches waarvan de naam niet begint met de opgegeven tekst",
                    Examples = "NietNaamBegintMetTekst(\"boom\")\r\nNietNaamBegintMetTekst(\"zeg eens \\\"hallo\\\"\")",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NaamEindigtMetTekst",
                    ProtoType = "NaamEindigtMetTekst(tekst)",
                    Description = "Caches waarvan de naam eindigt met de opgegeven tekst",
                    Examples = "NaamEindigtMetTekst(\"boom\")\r\nNaamBegintMetTekst(\"zeg eens \\\"hallo\\\"\")",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietNaamEindigtMetTekst",
                    ProtoType = "NietNaamEindigtMetTekst(tekst)",
                    Description = "Caches waarvan de naam niet eindigt met de opgegeven tekst",
                    Examples = "NietNaamEindigtMetTekst(\"boom\")\r\nNietNaamEindigtMetTekst(\"zeg eens \\\"hallo\\\"\")",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "VanEigenaar",
                    ProtoType = "VanEigenaar(naam1, naam2,...naamx)",
                    Description = "Caches van de opgegeven eigenaren",
                    Examples = "VanEigenaar(\"pietje\")\r\nNaamBegintMetTekst(\"pietje \\\"puk\\\"\")\r\nVanEigenaar(\"pietje\",\"puk\")",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietVanEigenaar",
                    ProtoType = "NietVanEigenaar(naam1, naam2,...naamx)",
                    Description = "Caches niet van de opgegeven eigenaren",
                    Examples = "NietVanEigenaar(\"pietje\")\r\nNietVanEigenaar(\"pietje \\\"puk\\\"\")\r\nNietVanEigenaar(\"pietje\",\"puk\")",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "BinnenStraal",
                    ProtoType = "BinnenStraal(coordinaat, straal)",
                    Description = "Caches die binnen de opgegeven cirkel liggen. De straal is in kilometers",
                    Examples = "BinnenStraal(\"N 53° 08.065 E 6° 26.096\", 5)\r\nBinnenStraal(\"N53 08.065 E6 26.096\", 2.5)\r\nBinnenStraal(\"53.12345, 6.154343\", 7.5)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietBinnenStraal",
                    ProtoType = "NietBinnenStraal(coordinaat, straal)",
                    Description = "Caches die buiten de opgegeven cirkel liggen. De straal is in kilometers",
                    Examples = "NietBinnenStraal(\"N 53° 08.065 E 6° 26.096\", 5)\r\nBinnenStraal(\"N53 08.065 E6 26.096\", 2.5)\r\nBinnenStraal(\"53.12345, 6.154343\", 7.5)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "AantalKeerGevondenMeerDan",
                    ProtoType = "AantalKeerGevondenMeerDan(waarde)",
                    Description = "Caches welke al meer dan opgegeven waarde gevonden zijn",
                    Examples = "AantalKeerGevondenMeerDan(100)",
                    PMOnly = false
                }
                , new MacroFunctionInfo
                {
                    Name = "NietAantalKeerGevondenMeerDan",
                    ProtoType = "NietAantalKeerGevondenMeerDan(waarde)",
                    Description = "Caches welke niet meer dan opgegeven waarde gevonden zijn",
                    Examples = "NietAantalKeerGevondenMeerDan(100)",
                    PMOnly = false
                }
            };

            return result.OrderBy(x => x.Name).ToArray();
        }

        public List<GCEuGeocacheFilterMacro> GetMacrosOfUser(int UserID)
        {
            List<GCEuGeocacheFilterMacro> result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Fetch<GCEuGeocacheFilterMacro>("where UserID=@0", UserID);
            }
            return result;
        }

        public GCEuGeocacheFilterMacro GetMacro(long ID)
        {
            GCEuGeocacheFilterMacro result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.FirstOrDefault<GCEuGeocacheFilterMacro>("where ID=@0", ID);
            }
            return result;
        }

        public GCEuGeocacheFilterMacro GetMacro(int UserID, string name)
        {
            GCEuGeocacheFilterMacro result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.FirstOrDefault<GCEuGeocacheFilterMacro>("where UserID=@0 and Name=@1", UserID, name);
            }
            return result;
        }

        public List<GCEuGeocacheFilterMacro> SaveMacro(string name, string macro)
        {
            List<GCEuGeocacheFilterMacro> result = null;
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 0)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    var m = db.FirstOrDefault<GCEuGeocacheFilterMacro>("where UserID=@0 and Name=@1", settings.YafUserID, name);
                    if (m != null)
                    {
                        m.CleanMacro = "";
                        m.FinishedTime = null;
                        m.Name = name;
                        m.RawMacro = macro;
                        m.ProcessInfo = "";
                        m.StartTime = null;
                        m.ProcessedSteps = null;
                        m.TotalSteps = null;
                        db.Update(m);
                    }
                    else
                    {
                        m = new GCEuGeocacheFilterMacro();
                        m.UserID = settings.YafUserID;

                        m.CleanMacro = "";
                        m.FinishedTime = null;
                        m.Name = name;
                        m.RawMacro = macro;
                        m.ProcessInfo = "";
                        m.StartTime = null;
                        m.ProcessedSteps = null;
                        m.TotalSteps = null;
                        db.Save(m);
                    }
                    result = db.Fetch<GCEuGeocacheFilterMacro>("where UserID=@0", settings.YafUserID);
                }
            }
            return result;
        }

        public List<GCEuGeocacheFilterMacro> DeleteMacro(long ID)
        {
            List<GCEuGeocacheFilterMacro> result = null;
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 0)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    var m = db.FirstOrDefault<GCEuGeocacheFilterMacro>("where UserID=@0 and ID=@1", settings.YafUserID, ID);
                    if (m != null)
                    {
                        db.Delete(m);
                    }
                    result = db.Fetch<GCEuGeocacheFilterMacro>("where UserID=@0", settings.YafUserID);
                }
            }
            return result;
        }

        public List<GCEuGeocacheFilterMacro> SaveAndRunMacro(string name, string macro)
        {
            List<GCEuGeocacheFilterMacro> result = SaveMacro(name, macro);
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings != null && settings.YafUserID > 0)
            {
                var m = GetMacro(settings.YafUserID, name);
                if (m != null)
                {
                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                    {
                        m.ProcessedSteps = 0;
                        m.StartTime = DateTime.Now;
                        db.Save(m);
                    }                    

                    System.Threading.Thread t = new System.Threading.Thread(() => ProcessMacroThreadMethod(m.ID));
                    t.IsBackground = true;
                    t.Start();
                }
            }
            return result;
        }

        private void ProcessMacroThreadMethod(long ID)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var m = GetMacro(ID);
                if (m != null)
                {
                    try
                    {
                        m.ProcessInfo = "Macro is gestart";
                        db.Save(m);

                        List<string> tables = db.Fetch<string>(string.Format("SELECT name FROM GCEuMacroData.sys.tables WHERE name like 'macro_{0}_%'", m.UserID));
                        foreach (string t in tables)
                        {
                            db.Execute(string.Format("drop table GCEuMacroData.dbo.{0}", t));
                        }

                        //parse macro to get clean macro
                        List<string> lines = getCleanMacro(m);
                        db.Save(m);

                        m.TotalSteps = lines.Count;
                        m.ProcessedSteps = 0;
                        List<string> variables = new List<string>();

                        if (lines.Count <= 30)
                        {
                            for (int i = 0; i < m.TotalSteps; i++)
                            {
                                if (processMacroLine(db, m, variables, lines[i]))
                                {
                                    m.ProcessedSteps++;
                                    db.Save(m);
                                }
                                else
                                {
                                    m.ProcessInfo = string.Format("{0}\r\nFout in regel: {1}", m.ProcessInfo, lines[i]);
                                    m.ProcessedSteps = -1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            m.ProcessInfo = string.Format("{0}\r\nMacro mag niet meer dan 30 regels bevatten", m.ProcessInfo);
                            m.ProcessedSteps = -1;
                        }
                        if (m.ProcessedSteps < 0)
                        {
                            if (!variables.Contains("resultaat"))
                            {
                                m.ProcessInfo = string.Format("{0}\r\nMacro heeft geen resultaat", m.ProcessInfo);
                                m.ProcessedSteps = -1;
                            }
                        }
                        m.FinishedTime = DateTime.Now;
                        db.Save(m);
                    }
                    catch
                    {
                        m.ProcessedSteps = -1; //meaning error
                        db.Save(m);
                    }
                }
            }
        }

        private List<string> getCleanMacro(GCEuGeocacheFilterMacro m)
        {
            List<string> result = new List<string>();
            StringBuilder sb = new StringBuilder();
            string[] lines = (m.RawMacro ?? "").Split(new char[]{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string l in lines)
            {
                string s = l.Trim();
                if (!s.StartsWith("#") && s!="")
                {
                    sb.AppendLine(s);
                    result.Add(s);
                }
            }
            m.CleanMacro = sb.ToString();
            return result;
        }

        private bool processMacroLine(PetaPoco.Database db, GCEuGeocacheFilterMacro m, List<string> variables, string line)
        {
            //scheme: variable = variable|func <en|of variable|func>
            string[] parts = line.Split(new char[] { '=' }, 2);
            if (parts.Length != 2) return false;

            string v = parts[0].Trim().ToLower();
            if (variables.Contains(v)) return false;
            if (!checkVariableName(v)) return false;

            variables.Add(v);
            db.Execute(string.Format("create table GCEuMacroData.dbo.macro_{0}_{1} (ID bigint not null)", m.UserID, v));

            //variable EN .. => inner join
            //variabele OF .. => union
            /*
             * select GcComData.GCComGeocache.ID from GcComData.GCComGeocache
             */
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("insert into GCEuMacroData.dbo.macro_{0}_{1} (ID) select distinct ID from (", m.UserID, v);

            //test
            //sql.Append("select GcComData.dbo.GCComGeocache.ID from GcComData.dbo.GCComGeocache where GcComData.dbo.GCComGeocache.Available=1 and GcComData.dbo.GCComGeocache.CountryID=141");

            List<string> innerJoins = new List<string>();
            List<string> whereClauses = new List<string>();

            sql.Append("select GcComData.dbo.GCComGeocache.ID from GcComData.dbo.GCComGeocache with (nolock) ");
            string textLeft = parts[1].Trim();
            while (!string.IsNullOrEmpty(textLeft))
            {
                //step 1: add query
                int pos1 = textLeft.IndexOf(' ');
                int pos2 = textLeft.IndexOf('(');
                if ((pos1>0 && pos1 < pos2) || pos2 < 0)
                {
                    //variable
                    string v1;
                    if (pos1 < 0)
                    {
                        v1 = textLeft.ToLower();
                        textLeft = "";
                    }
                    else
                    {
                        v1 = textLeft.Substring(0, pos1).ToLower();
                        textLeft = textLeft.Substring(pos1).Trim();
                    }
                    if (!variables.Contains(v1)) return false;
                    innerJoins.Add(string.Format(" inner join GCEuMacroData.dbo.macro_{0}_{1} on GCEuMacroData.dbo.macro_{0}_{1}.ID = GcComData.dbo.GCComGeocache.ID ", m.UserID, v1));
                }
                else
                {
                    //function
                    if (!processFunction(db, innerJoins, whereClauses, ref textLeft))
                    {
                        return false;
                    }
                }

                //step 2: check: en or of
                textLeft = textLeft.Trim();
                if (!string.IsNullOrEmpty(textLeft))
                {
                    pos1 = textLeft.IndexOf(' ');
                    string c = textLeft.Substring(0, pos1).ToLower();
                    textLeft = textLeft.Substring(pos1).Trim();
                    if (c == "en" || c=="*" || c=="x")
                    {
                        //just continue with where clause
                    }
                    else if (c == "of" || c=="+" || c=="plus")
                    {
                        //union
                        foreach (string s in innerJoins)
                        {
                            sql.Append(s);
                        }
                        innerJoins.Clear();
                        if (whereClauses.Count > 0)
                        {
                            sql.AppendFormat(" where {0} ", whereClauses[0]);
                            for (int i = 1; i < whereClauses.Count; i++)
                            {
                                sql.AppendFormat(" and {0} ", whereClauses[i]);
                            }
                        }
                        whereClauses.Clear();

                        sql.Append("union select GcComData.dbo.GCComGeocache.ID from GcComData.dbo.GCComGeocache with (nolock) ");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            foreach (string s in innerJoins)
            {
                sql.Append(s);
            }
            innerJoins.Clear();
            if (whereClauses.Count > 0)
            {
                sql.AppendFormat(" where {0} ", whereClauses[0]);
                for (int i = 1; i < whereClauses.Count; i++)
                {
                    sql.AppendFormat(" and {0} ", whereClauses[i]);
                }
            }
            whereClauses.Clear();

            sql.Append(") as t");

            try
            {
                int cnt = db.Execute(sql.ToString().Replace("@","@@"));
                m.ProcessInfo = string.Format("{0}\r\n{1} => bevat {2} caches", m.ProcessInfo, line, cnt);
            }
            catch
            {
                m.ProcessInfo = string.Format("{0}\r\nEr is een fout opgetreden in regel: {1}", m.ProcessInfo, line);
                return false;
            }
            return true;
        }

        private bool processFunction(PetaPoco.Database db, List<string> innerJoins, List<string> whereClauses, ref string textLeft)
        {
            int pos1 = textLeft.IndexOf('(');
            string f = textLeft.Substring(0, pos1).ToLower();
            textLeft = textLeft.Substring(pos1+1);
            List<string> parameters = getParameters(ref textLeft);
            switch (f)
            {
                case "beschikbaar":
                    whereClauses.Add(" GcComData.dbo.GCComGeocache.Available = 1 ");
                    break;
                case "nietbeschikbaar":
                    whereClauses.Add(" GcComData.dbo.GCComGeocache.Available = 0 ");
                    break;
                case "gearchiveerd":
                    whereClauses.Add(" GcComData.dbo.GCComGeocache.Archived = 1 ");
                    break;
                case "nietgearchiveerd":
                    whereClauses.Add(" GcComData.dbo.GCComGeocache.Archived = 0 ");
                    break;
                case "cachetype":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.GeocacheTypeId in ({0}) ", string.Join(", ", (from a in parameters select int.Parse(a)).ToArray())));
                    break;
                case "nietcachetype":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.GeocacheTypeId not in ({0}) ", string.Join(", ", (from a in parameters select int.Parse(a)).ToArray())));
                    break;
                case "container":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.ContainerTypeId in ({0}) ", string.Join(", ", (from a in parameters select int.Parse(a)).ToArray())));
                    break;
                case "nietcontainer":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.ContainerTypeId not in ({0}) ", string.Join(", ", (from a in parameters select int.Parse(a)).ToArray())));
                    break;
                case "landcode":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.CountryID in ({0}) ", string.Join(", ", (from a in parameters select int.Parse(a)).ToArray())));
                    break;
                case "nietlandcode":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.CountryID not in ({0}) ", string.Join(", ", (from a in parameters select int.Parse(a)).ToArray())));
                    break;
                case "aanmaakdatumvoor":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.DateCreated < '{0}-{1}-{2}' ", int.Parse(parameters[2]), int.Parse(parameters[1]), int.Parse(parameters[0])));
                    break;
                case "nietaanmaakdatumvoor":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.DateCreated >= '{0}-{1}-{2}' ", int.Parse(parameters[2]), int.Parse(parameters[1]), int.Parse(parameters[0])));
                    break;
                case "aanmaakdatumna":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.DateCreated > '{0}-{1}-{2}' ", int.Parse(parameters[2]), int.Parse(parameters[1]), int.Parse(parameters[0])));
                    break;
                case "nietaanmaakdatumna":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.DateCreated <= '{0}-{1}-{2}' ", int.Parse(parameters[2]), int.Parse(parameters[1]), int.Parse(parameters[0])));
                    break;
                case "aangepastnadatum":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.DateLastUpdate > '{0}-{1}-{2}' ", int.Parse(parameters[2]), int.Parse(parameters[1]), int.Parse(parameters[0])));
                    break;
                case "nietaangepastnadatum":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.DateLastUpdate <= '{0}-{1}-{2}' ", int.Parse(parameters[2]), int.Parse(parameters[1]), int.Parse(parameters[0])));
                    break;
                case "gepubliceerdnadatum":
                    addInnerJoin(" inner join GcEuData.dbo.GCEuGeocache with (nolock) on GcComData.dbo.GCComGeocache.ID = GcEuData.dbo.GCEuGeocache.ID ", innerJoins);
                    whereClauses.Add(string.Format(" GcEuData.dbo.GCEuGeocache.PublishedAtDate > '{0}-{1}-{2}' ", int.Parse(parameters[2]), int.Parse(parameters[1]), int.Parse(parameters[0])));
                    break;
                case "nietgepubliceerdnadatum":
                    addInnerJoin(" inner join GcEuData.dbo.GCEuGeocache with (nolock) on GcComData.dbo.GCComGeocache.ID = GcEuData.dbo.GCEuGeocache.ID ", innerJoins);
                    whereClauses.Add(string.Format(" GcEuData.dbo.GCEuGeocache.PublishedAtDate <= '{0}-{1}-{2}' ", int.Parse(parameters[2]), int.Parse(parameters[1]), int.Parse(parameters[0])));
                    break;
                case "moeilijkheidgroterdan":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Difficulty > {0} ", getDouble(parameters[0])));
                    break;
                case "nietmoeilijkheidgroterdan":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Difficulty <= {0} ", getDouble(parameters[0])));
                    break;
                case "terreingroterdan":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Terrain > {0} ", getDouble(parameters[0])));
                    break;
                case "nietterreingroterdan":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Terrain <= {0} ", getDouble(parameters[0])));
                    break;
                case "cacheafstandgroterdan":
                    addInnerJoin(" inner join GcEuData.dbo.GCEuGeocache with (nolock) on GcComData.dbo.GCComGeocache.ID = GcEuData.dbo.GCEuGeocache.ID ", innerJoins);
                    whereClauses.Add(string.Format(" GcEuData.dbo.GCEuGeocache.Distance > {0} ", getDouble(parameters[0])));
                    break;
                case "nietcacheafstandgroterdan":
                    addInnerJoin(" inner join GcEuData.dbo.GCEuGeocache with (nolock) on GcComData.dbo.GCComGeocache.ID = GcEuData.dbo.GCEuGeocache.ID ", innerJoins);
                    whereClauses.Add(string.Format(" GcEuData.dbo.GCEuGeocache.Distance <= {0} ", getDouble(parameters[0])));
                    break;
                case "aantalplaatjesgroterdan":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.ImageCount > {0} ", int.Parse(parameters[0])));
                    break;
                case "nietaantalplaatjesgroterdan":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.ImageCount <= {0} ", int.Parse(parameters[0])));
                    break;
                case "hintsbevattekst":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.EncodedHints like '%{0}%' ", parameters[0].Replace("'","''")));
                    break;
                case "favorietenmeerdan":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.FavoritePoints > {0} ", int.Parse(parameters[0])));
                    break;
                case "nietfavorietenmeerdan":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.FavoritePoints <= {0} ", int.Parse(parameters[0])));
                    break;
                case "isgeblokkeerd":
                    whereClauses.Add(" GcComData.dbo.GCComGeocache.IsLocked = 1 ");
                    break;
                case "nietisgeblokkeerd":
                    whereClauses.Add(" GcComData.dbo.GCComGeocache.IsLocked = 0 ");
                    break;
                case "ispremiumcache":
                    whereClauses.Add(" GcComData.dbo.GCComGeocache.IsPremium = 1 ");
                    break;
                case "nietispremiumcache":
                    whereClauses.Add(" GcComData.dbo.GCComGeocache.IsPremium = 0 ");
                    break;
                case "latitudetussen":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Latitude between {0} and {1} ", getDouble(parameters[0]), getDouble(parameters[1])));
                    break;
                case "nietlatitudetussen":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Latitude not between {0} and {1} ", getDouble(parameters[0]), getDouble(parameters[1])));
                    break;
                case "longitudetussen":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Longitude between {0} and {1} ", getDouble(parameters[0]), getDouble(parameters[1])));
                    break;
                case "nietlongitudetussen":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Longitude not between {0} and {1} ", getDouble(parameters[0]), getDouble(parameters[1])));
                    break;
                case "naambevattekst":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Name like '%{0}%' ", parameters[0].Replace("'", "''")));
                    break;
                case "nietnaambevattekst":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Name not like '%{0}%' ", parameters[0].Replace("'", "''")));
                    break;
                case "naambegintmettekst":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Name like '{0}%' ", parameters[0].Replace("'", "''")));
                    break;
                case "nietnaambegintmettekst":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Name not like '{0}%' ", parameters[0].Replace("'", "''")));
                    break;
                case "naameindigtmetTekst":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Name like '%{0}' ", parameters[0].Replace("'", "''")));
                    break;
                case "nietnaameindigtmetTekst":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.Name not like '%{0}' ", parameters[0].Replace("'", "''")));
                    break;
                case "vaneigenaar":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.OwnerId in ({0}) ", getGCComIDArray(db, parameters)));
                    break;
                case "nietvaneigenaar":
                    whereClauses.Add(string.Format(" GcComData.dbo.GCComGeocache.OwnerId not in ({0}) ", getGCComIDArray(db, parameters)));
                    break;
                case "binnenstraal":
                    {
                        LatLon ll = LatLon.FromString(parameters[0]);
                        whereClauses.Add(string.Format(" GcComData.dbo.F_GREAT_CIRCLE_DISTANCE(GcComData.dbo.GCComGeocache.Latitude, GcComData.dbo.GCComGeocache.Longitude, {0}, {1}) < {2}", ll.lat.ToString().Replace(',', '.'), ll.lon.ToString().Replace(',', '.'), getDouble(parameters[1])));
                    }
                    break;
                case "nietbinnenstraal":
                    {
                        LatLon ll = LatLon.FromString(parameters[0]);
                        whereClauses.Add(string.Format(" GcComData.dbo.F_GREAT_CIRCLE_DISTANCE(GcComData.dbo.GCComGeocache.Latitude, GcComData.dbo.GCComGeocache.Longitude, {0}, {1}) >= {2}", ll.lat.ToString().Replace(',', '.'), ll.lon.ToString().Replace(',', '.'), getDouble(parameters[1])));
                    }
                    break;
                case "aantalkeergevondenmeerdan":
                    addInnerJoin(" inner join GcEuData.dbo.GCEuGeocache with (nolock) on GcComData.dbo.GCComGeocache.ID = GcEuData.dbo.GCEuGeocache.ID ", innerJoins);
                    whereClauses.Add(string.Format(" GcEuData.dbo.GCEuGeocache.FoundCount > {0} ", int.Parse(parameters[0])));
                    break;
                case "nietaantalkeergevondenmeerdan":
                    addInnerJoin(" inner join GcEuData.dbo.GCEuGeocache with (nolock) on GcComData.dbo.GCComGeocache.ID = GcEuData.dbo.GCEuGeocache.ID ", innerJoins);
                    whereClauses.Add(string.Format(" GcEuData.dbo.GCEuGeocache.FoundCount <= {0} ", int.Parse(parameters[0])));
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void addInnerJoin(string j, List<string> innerJoins)
        {
            if (!innerJoins.Contains(j, StringComparer.OrdinalIgnoreCase))
            {
                innerJoins.Add(j);
            }
        }

        private List<string> getParameters(ref string textLeft)
        {
            //textLest: ')' or '1,2,3)' or "test",1)
            List<string> result = new List<string>();

            var r = new Regex("\"([^\"\\\\]*(\\\\.)*)*\"");
            var m = r.Matches(textLeft);
            //m contains blocks of text. we skip this while searching for ',' and ')'
            //first look for the end of the function
            int endPos = -1;
            int startScan = 0;
            while (endPos<0)
            {
                endPos = textLeft.IndexOf(')', startScan);
                if (endPos > 0)
                {
                    foreach (Match m1 in m)
                    {
                        if (endPos >= m1.Index && endPos < (m1.Index + m1.Length))
                        {
                            startScan = endPos + 1;
                            endPos = -1;
                        }
                    }
                }
                else
                {
                    //error
                    break;
                }
            }

            //now that we know the end of the function, we scan for parameters, scan for ','
            int firstParamPos = 0;
            while (firstParamPos >= 0 && firstParamPos < endPos)
            {
                int endParamPos = -1;
                startScan = firstParamPos + 1;
                while (endParamPos < 0)
                {
                    endParamPos = textLeft.IndexOf(',', startScan);
                    if (endParamPos > 0)
                    {
                        foreach (Match m1 in m)
                        {
                            if (endParamPos >= m1.Index && endParamPos < (m1.Index + m1.Length))
                            {
                                startScan = endParamPos + 1;
                                endParamPos = -1;
                            }
                        }
                    }
                    else
                    {
                        //end of func
                        break;
                    }
                }
                string p;
                if (endParamPos > 0 && endParamPos < endPos)
                {
                    p = textLeft.Substring(firstParamPos, endParamPos - firstParamPos).Trim();
                    firstParamPos = endParamPos + 1;
                }
                else
                {
                    p = textLeft.Substring(firstParamPos, endPos - firstParamPos).Trim();
                    endParamPos = endPos + 1;
                    firstParamPos = endPos + 1;
                }
                if (p != "")
                {
                    if (p.StartsWith("\"") && p.EndsWith("\""))
                    {
                        if (p.Length == 2)
                        {
                            p = "";
                        }
                        else
                        {
                            p = p.Substring(1, p.Length - 2);
                            p = p.Replace("\\\"", "\"");
                        }
                    }
                    result.Add(p);
                }
            }

            //int endPos = textLeft.IndexOf(')');
            if (endPos < 0 || (endPos + 1) == textLeft.Length)
            {
                textLeft = "";
            }
            else
            {
                textLeft = textLeft.Substring(endPos + 1);
            }
            return result;
        }

        private bool checkVariableName(string name)
        {
            const string validChars = "qwertyuiopasdfghjklzxcvbnm";
            bool result = true;
            for (int i = 0; i < name.Length; i++)
            {
                result &= validChars.IndexOf(name[i]) >= 0;
            }
            return result;
        }

        private string getDouble(string d)
        {
            return double.Parse(d, CultureInfo.InvariantCulture).ToString().Replace(',', '.');
        }

        private string getGCComIDArray(PetaPoco.Database db, List<string> names)
        {
            return string.Join(", ", getGCComIDs(db, names));
        }

        private List<long> getGCComIDs(PetaPoco.Database db, List<string> names)
        {
            List<long> result;
            StringBuilder sb = new StringBuilder();
            foreach (string s in names)
            {
                for (int i=0; i<names.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.AppendFormat("'{0}'", names[i].Replace("'", "''").Replace("@", "@@"));
                }
            }
            result = db.Fetch<long>(string.Format("select distinct ID from GcComData.dbo.GCComUser where UserName in ({0})", sb.ToString()));
            if (result.Count == 0)
            {
                result.Add(0);
            }
            return result;
        }

    }
}