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

        static private void ListMunicipalities(string filter = "") {
            DataTable municipalities = conn.metadataMunicipality(filter);
            Console.WriteLine(string.Format("Number of rows returned {0}", municipalities.Rows.Count));
            foreach( DataRow row in municipalities.Rows ) {
                Console.WriteLine(string.Format("id: {0} - title: {1} - type: {2}", row["id"], row["title"], row["type"]));
            }
        }

        static private void ListKPI(string filter = "") {
            DataTable kpis = conn.metadataKPI(filter);
            Console.WriteLine(string.Format("Number of rows returned {0}", kpis.Rows.Count));
            
            foreach( DataRow row in kpis.Rows ) {
                Console.WriteLine(
                        string.Format("id: {0} - title: {1} - type: {2}", row["id"], row["title"], row["municipality_type"])
                );
            }
            
        }

        static private void ListOU(string municipality, string filter = "") {
            DataTable municipalities = conn.metadataOU(municipality, filter);
            Console.WriteLine(string.Format("Number of rows returned {0}", municipalities.Rows.Count));
            foreach( DataRow row in municipalities.Rows ) {
                Console.WriteLine(string.Format("id: {0} - title: {1}", row["id"], row["title"]));
            }
        }

        static private void ListDataPerMunicipality(string kpiId, string municipalityId) {
            DataTable data = conn.dataPerMunicipality(kpiId, municipalityId);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - municipality: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["municipality"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static private void ListDataPerYear(string kpiId, int year) {
            DataTable data = conn.dataPerYear(kpiId, year);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - municipality: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["municipality"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static private void ListDataExcact(string kpiId, string municipalityId, int year) {
            DataTable data = conn.dataExact(kpiId, municipalityId, year);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - municipality: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["municipality"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static private void ListDataExcact(string kpiId, string[] municipalities, int[] years) {
            DataTable data = conn.dataExact(kpiId, municipalities, years);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - municipality: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["municipality"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static private void ListDataExcact(string[] kpis, string[] municipalities, int[] years) {
            DataTable data = conn.dataExact(kpis, municipalities, years);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - municipality: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["municipality"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static private void ListDataOuPerOu(string kpiId, string ouId) {
            DataTable data = conn.dataOuPerOu(kpiId, ouId);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - ou: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["ou"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static private void ListDataOuPerYear(string kpiId, int year) {
            DataTable data = conn.dataOuPerYear(kpiId, year);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - ou: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["ou"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static private void ListDataOuExcact(string kpiId, string ouId, int year) {
            DataTable data = conn.dataOuExact(kpiId, ouId, year);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - ou: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["ou"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static private void ListDataOuExcact(string kpiId, string[] ous, int[] years) {
            DataTable data = conn.dataOuExact(kpiId, ous, years);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - ou: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["ou"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static private void ListDataOuExcact(string[] kpis, string[] ous, int[] years) {
            DataTable data = conn.dataOuExact(kpis, ous, years);
            Console.WriteLine(string.Format("Number of rows returned {0}", data.Rows.Count));
            foreach( DataRow row in data.Rows ) {
                Console.WriteLine(string.Format("kpi: {0} - ou: {1} - period: {2} - value: {3} - value_m: {4} - value_f: {5}", row["kpi"], row["ou"], row["period"], row["value"], row["value_m"], row["value_f"]));
            }
        }

        static void Main(string[] args)
        {
            conn = new KoladaClient.Connection();

            // All municipalities
            //ListMunicipalities();

            // All municipalities containing text Ale in title
            //ListMunicipalities("Ale");

            // All kpis
            //ListKPI();

            // All kpis containing text Antal invånare totalt in title
            //ListKPI("Antal invånare totalt");

            // id: N01951 - title: Invånare totalt, antal - type: K
            // id: 1440 - title: Ale - type: K

            // Values for antal invånare for municipality Ale
            //ListDataPerMunicipality("N01951", "1440");

            // Values for year 2004 and all municipalities
            //ListDataPerYear("N01951", 2004);

            // Values for antal invånare for municipality Ale and year 2007, 2008, 2010
            //ListDataExcact("N01951", "1440", 2007);

            // Values for antal invånare for municipality Ale and year 2007, 2008, 2010
            //ListDataExcact("N01951", new string[] { "1440" }, new int[] { 2007, 2008, 2010 });

            // Values for antal invånare for municipalities Kalix (2514), Övertorneå (2518), Pajala (2521) and year 2007, 2008, 2010
            //ListDataExcact("N01951", new string[] {"2514", "2518", "2521"}, new int[] {2007, 2008, 2010});

            // Values for antal invånare (N01951), Införandebidrag, kr/inv (2005-) (N00018)
            // for municipalities Kalix (2514), Övertorneå (2518), Pajala (2521) and year 2007, 2008, 2010
            //ListDataExcact(new string[] { "N01951", "N00018" }, new string[] { "2514", "2518", "2521" }, new int[] { 2007, 2008, 2010 });

            // List OUs for Upplands Väsby (0114)
            //ListOU("0114", "");

            // List OUs for Upplands Väsby (0114) with title containing Ru
            //ListOU("0114", "Ru");

            // Values for kpi Lärare med pedagogisk högskoleexamen i grundskola, totalt, (%) (N15030) and 
            // unit Runby Skola (V01011400101)
            //ListDataOuPerOu("N15030", "V01011400101");

            // Values for kpi Lärare med pedagogisk högskoleexamen i grundskola, totalt, (%) (N15030) and 
            // year 2011
            ListDataOuPerYear("N15030", 2011);

            // Values for kpi Lärare med pedagogisk högskoleexamen i grundskola, totalt, (%) (N15030) and 
            // unit Runby Skola (V01011400101) and year 2011
            //ListDataOuExcact("N15030", "V01011400101", 2011);

            // Values for kpi Lärare med pedagogisk högskoleexamen i grundskola, totalt, (%) (N15030) and 
            // unit Runby Skola (V01011400101), Väsby Skola (V01011400201) and year 2011
            //ListDataOuExcact("N15030", new string[] {"V01011400101", "V01011400201"}, new int[] {2010, 2011});
        }

    }
}
