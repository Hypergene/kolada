using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using KoladaClient;

namespace KoladaClientTest
{
    class Program
    {
        static private KoladaClient.Connection conn;

        static private void printRows(DataTable table, string[] columns) {
           string fmt = "";

           Console.WriteLine(string.Format("Number of rows returned {0}", table.Rows.Count));

           foreach( DataRow row in table.Rows ) {
              int count = 0;
              foreach( string col in columns ) {
                 fmt = String.Format("{0}: {{0}}{1}", col, (count == (columns.Length - 1) ? "" : " - "));
                 count++;
                 Console.Write(string.Format(fmt, row[col]));
              }
              Console.WriteLine();
           }
        }

        static void Main(string[] args)
        {
            conn = new KoladaClient.Connection();
            DataTable result;

            // All municipalities
            result = conn.metadataMunicipality("");
            printRows(result, new string[] {"id", "title", "type"});

            // All municipalities containing text Ale in title
            result = conn.metadataMunicipality("Ale");
            printRows(result, new string[] { "id", "title", "type" });

            // All kpis
            result = conn.metadataKPI("");
            printRows(result, new string[] { "id", "title", "municipality_type" });

            // All kpis containing text Antal invånare totalt in title
            result = conn.metadataKPI("Antal invånare totalt");
            printRows(result, new string[] { "id", "title", "municipality_type" });

            // id: N01951 - title: Invånare totalt, antal - type: K
            // id: 1440 - title: Ale - type: K

            // Values for antal invånare for municipality Ale
            result = conn.dataPerMunicipality("N01951", "1440");
            printRows(result, new string[] { "kpi", "municipality", "period", "value", "value_m", "value_f"});

            // Values for year 2004 and all municipalities
            result = conn.dataPerYear("N01951", 2004);
            printRows(result, new string[] { "kpi", "municipality", "period", "value", "value_m", "value_f" });

            // Values for antal invånare for municipality Ale and year 2007, 2008, 2010
            result = conn.dataExact("N01951", "1440", 2007);
            printRows(result, new string[] { "kpi", "municipality", "period", "value", "value_m", "value_f" });

            // Values for antal invånare for municipality Ale and year 2007, 2008, 2010
            result = conn.dataExact("N01951", new string[] { "1440" }, new int[] { 2007, 2008, 2010 });
            printRows(result, new string[] { "kpi", "municipality", "period", "value", "value_m", "value_f" });

            // Values for antal invånare for municipalities Kalix (2514), Övertorneå (2518), Pajala (2521) and year 2007, 2008, 2010
            result = conn.dataExact("N01951", new string[] { "2514", "2518", "2521" }, new int[] { 2007, 2008, 2010 });
            printRows(result, new string[] { "kpi", "municipality", "period", "value", "value_m", "value_f" });

            // Values for antal invånare (N01951), Införandebidrag, kr/inv (2005-) (N00018)
            // for municipalities Kalix (2514), Övertorneå (2518), Pajala (2521) and year 2007, 2008, 2010
            result = conn.dataExact(new string[] { "N01951", "N00018" }, new string[] { "2514", "2518", "2521" }, new int[] { 2007, 2008, 2010 });
            printRows(result, new string[] { "kpi", "municipality", "period", "value", "value_m", "value_f" });

            // List OUs for Upplands Väsby (0114)
            result = conn.metadataOU("0114", "");
            printRows(result, new string[] { "id", "title" });

            // List OUs for Upplands Väsby (0114) with title containing Ru
            result = conn.metadataOU("0114", "Ru");
            printRows(result, new string[] { "id", "title" });

            // Values for kpi Lärare med pedagogisk högskoleexamen i grundskola, totalt, (%) (N15030) and 
            // unit Runby Skola (V01011400101)
            result = conn.dataOuPerOu("N15030", "V01011400101");
            printRows(result, new string[] { "kpi", "ou", "period", "value", "value_m", "value_f" });

            // Values for kpi Lärare med pedagogisk högskoleexamen i grundskola, totalt, (%) (N15030) and 
            // year 2011
            result = conn.dataOuPerYear("N15030", 2011);
            printRows(result, new string[] { "kpi", "ou", "period", "value", "value_m", "value_f" });

            // Values for kpi Lärare med pedagogisk högskoleexamen i grundskola, totalt, (%) (N15030) and 
            // unit Runby Skola (V01011400101) and year 2011
            result = conn.dataOuExact("N15030", "V01011400101", 2011);
            printRows(result, new string[] { "kpi", "ou", "period", "value", "value_m", "value_f" });

            // Values for kpi Lärare med pedagogisk högskoleexamen i grundskola, totalt, (%) (N15030) and 
            // unit Runby Skola (V01011400101), Väsby Skola (V01011400201) and year 2011
            result = conn.dataOuExact("N15030", new string[] { "V01011400101", "V01011400201" }, new int[] { 2010, 2011 });
            printRows(result, new string[] { "kpi", "ou", "period", "value", "value_m", "value_f" });
        }
    }
}
