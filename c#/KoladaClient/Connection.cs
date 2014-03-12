using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace KoladaClient
{
    class MetadataRS {
        public DataTable values { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public int count { get; set; }
    }

    public class Connection {
        const string KOLADA_API_SERVER = "api.kolada.se";
        const string KOLADA_API_VERSION = "v1";
        const string KOLADA_API_PREPEND_PATH = "";

        /// <summary>
        /// Internal function. Concatenates a list to comma-separated string
        /// </summary>
        /// <param name="list">Array</param>
        /// <returns>String</returns>
        private string concatList<T>(T[] list) {
            string ret = "";

            for( int i = 0; i < list.Length; i++ ) {
                if( i > 0 )
                    ret += ",";
                ret += list[i].ToString();
            }

            return ret;
        }

        /// <summary>
        /// Internal function. Creates a web request and retrives result as a Stream object.
        /// </summary>
        /// <param name="url">Kolada Webservice query URL</param>
        /// <returns>Stream object containing result</returns>
        private Stream getDataFromServer(string url) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }

        /// <summary>
        /// Internal function to convert JSON response from webserver to DataTable
        /// </summary>
        /// <param name="url">Kolada Webservice query URL</param>
        /// <returns>DataTable containing rows</returns>
        private DataTable getValuesTable(string url) {
            DataTable resultTable = null;
            MetadataRS rs = null;

            do {
                StreamReader reader = new StreamReader(this.getDataFromServer(url), Encoding.UTF8);
                rs = JsonConvert.DeserializeObject<MetadataRS>(reader.ReadToEnd());
                if( resultTable == null ) {
                    resultTable = rs.values;
                } else {
                    resultTable.Merge(rs.values);
                }

                url = rs.next;

            } while( url != null );

            return resultTable;
        }

        private string getQueryUri(string path) {
            UriBuilder u = new UriBuilder();

            u.Scheme = "http";
            u.Host = KOLADA_API_SERVER;
            u.Path = KOLADA_API_PREPEND_PATH + "/" + KOLADA_API_VERSION + "/" + path;
             
            return u.ToString();
        }

        /// <summary>
        /// Returns metadata information for KPIs 
        /// </summary>
        /// <param name="filter">String used to filter out KPIs on title. Empty string for no filtering</param>
        /// <returns>DataTable containing KPIs</returns>
        public DataTable metadataKPI(string filter = "") {
            string url = getQueryUri(string.Format("kpi/{0}", filter));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns metadata information for municipalities 
        /// </summary>
        /// <param name="filter">String used to filter out municipalities on title. Empty string for no filtering</param>
        /// <returns>DataTable containing municipalities</returns>
        public DataTable metadataMunicipality(string filter = "") {
            string url = getQueryUri(string.Format("municipality/{0}", filter));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns metadata information for organizational units within a municipality 
        /// </summary>
        /// <param name="municipality">String with id of municipality to return organizational units from</param>
        /// <param name="filter">String used to filter out organizational units on title. Empty string for no filtering</param>
        /// <returns>DataTable containing organizational units</returns>
        public DataTable metadataOU(string municipality, string filter = "") {
            string url = getQueryUri(string.Format("ou/{0}/{1}", municipality, filter));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for all years for given kpi and municipality.
        /// </summary>
        /// <param name="kpiId">String with id of KPI to select data for</param>
        /// <param name="municipalityId">String with id of municipality to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataPerMunicipality(string kpiId, string municipalityId) {
            string url = getQueryUri(string.Format("data/permunicipality/{0}/{1}", kpiId, municipalityId));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for all municipalities for given kpi and year.
        /// </summary>
        /// <param name="kpiId">String with id of KPI to select data for</param>
        /// <param name="year">Int with year to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataPerYear(string kpiId, int year) {
            string url = getQueryUri(string.Format("data/peryear/{0}/{1}", kpiId, year));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for selected year, kpi and municipality.
        /// </summary>
        /// <param name="kpiId">String with id of KPI to select data for</param>
        /// <param name="municipalityId">String with id of municipality to select data for</param>
        /// <param name="year">Integer with year to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataExact(string kpiId, string municipalityId, int year) {
            string url = getQueryUri(string.Format("data/exact/{0}/{1}/{2}", kpiId, municipalityId, year));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for selected years, municipalities and given kpi
        /// </summary>
        /// <param name="kpiId">String with id of KPI to select data for</param>
        /// <param name="municipalities">Array of strings with id of municipalities to select data for</param>
        /// <param name="years">Array of integers containing years to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataExact(string kpiId, string[] municipalities, int[] years) {
            string yearArg = this.concatList<int>(years);
            string munArg = this.concatList<string>(municipalities);

            string url = getQueryUri(string.Format("data/exact/{0}/{1}/{2}", kpiId, munArg, yearArg));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for selected years, municipalities and kpi:s
        /// </summary>
        /// <param name="kpis">Array of strings with id of KPIs to select data for</param>
        /// <param name="municipalities">Array of strings with id of municipalities to select data for</param>
        /// <param name="years">Array of integers containing years to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataExact(string[] kpis, string[] municipalities, int[] years) {
            string yearArg = this.concatList<int>(years);
            string munArg = this.concatList<string>(municipalities);
            string kpiArg = this.concatList<string>(kpis);

            string url = getQueryUri(string.Format("data/exact/{0}/{1}/{2}", kpiArg, munArg, yearArg));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for all years for given kpi and organizational unit.
        /// </summary>
        /// <param name="kpiId">String with id of KPI to select data for</param>
        /// <param name="ouId">String with id of organizational unit to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataOuPerOu(string kpiId, string ouId) {
            string url = getQueryUri(string.Format("ou/data/perou/{0}/{1}", kpiId, ouId));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for all organizational units for given kpi and year.
        /// </summary>
        /// <param name="kpiId">String with id of KPI to select data for</param>
        /// <param name="year">Int with year to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataOuPerYear(string kpiId, int year) {
            string url = getQueryUri(string.Format("ou/data/peryear/{0}/{1}", kpiId, year));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for selected year, kpi and organizational unit.
        /// </summary>
        /// <param name="kpiId">String with id of KPI to select data for</param>
        /// <param name="ouId">String with id of organizational unit to select data for</param>
        /// <param name="year">Integer with year to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataOuExact(string kpiId, string ouId, int year) {
            string url = getQueryUri(string.Format("ou/data/exact/{0}/{1}/{2}", kpiId, ouId, year));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for selected years, organizational units and given kpi
        /// </summary>
        /// <param name="kpiId">String with id of KPI to select data for</param>
        /// <param name="ous">Array of strings with id of organiztional units to select data for</param>
        /// <param name="years">Array of integers containing years to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataOuExact(string kpiId, string[] ous, int[] years) {
            string yearArg = this.concatList<int>(years);
            string ouArg = this.concatList<string>(ous);

            string url = getQueryUri(string.Format("ou/data/exact/{0}/{1}/{2}", kpiId, ouArg, yearArg));

            return this.getValuesTable(url);
        }

        /// <summary>
        /// Returns data values for selected years, organizational units and kpi:s
        /// </summary>
        /// <param name="kpis">Array of strings with id of KPIs to select data for</param>
        /// <param name="ous">Array of strings with id of organizational units to select data for</param>
        /// <param name="years">Array of integers containing years to select data for</param>
        /// <returns>DataTable containing data rows</returns>
        public DataTable dataOuExact(string[] kpis, string[] ous, int[] years) {
            string yearArg = this.concatList<int>(years);
            string ouArg = this.concatList<string>(ous);
            string kpiArg = this.concatList<string>(kpis);

            string url = getQueryUri(string.Format("ou/data/exact/{0}/{1}/{2}", kpiArg, ouArg, yearArg));

            return this.getValuesTable(url);
        }
    }
}
