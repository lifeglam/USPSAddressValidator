using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace USPSServices
{
    class USPSAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip5 { get; set; }
        public string Zip4 { get; set; }
    }
    class USPSService
    {
        private string APIKey;
        static string BaseUrl = "http://production.shippingapis.com/ShippingAPI.dll";
        public USPSService(string Key)
        {
            APIKey = Key;
        }

        private string CreateRequest(string Api, string Xml)
        {
            return String.Format("{0}?API={1}&XML={2}", BaseUrl, Api, Xml);
        }

        private XDocument MakeRequest(string RequestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(RequestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                XDocument doc = XDocument.Load(response.GetResponseStream());
                return (doc);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
                return null;
            }
        }

        public USPSAddress ValidateCityState(string Zip)
        {
            string xml = String.Format("<CityStateLookupRequest%20USERID='{0}'><ZipCode ID='0'><Zip5>{1}</Zip5></ZipCode></CityStateLookupRequest>", APIKey, Zip);
            string request = CreateRequest("CityStateLookup", xml);
            XDocument doc = MakeRequest(request);
            return doc.Root.Elements("ZipCode").Select(d => new USPSAddress { City = d.Element("City").Value, State = d.Element("State").Value, Zip5 = d.Element("Zip5").Value }).FirstOrDefault();
        }

        public USPSAddress ValidateCityState(int Zip)
        {
            return ValidateCityState(Zip.ToString());
        }

        public USPSAddress ValidateAddress(USPSAddress address)
        {
            string xml = String.Format("<AddressValidateRequest%20USERID='{0}'><Address><Address1></Address1><Address2>{1}</Address2><City>{2}</City><State>{3}</State><Zip5>{4}</Zip5><Zip4></Zip4></Address></AddressValidateRequest >", APIKey, address.Address2, address.City, address.State, address.Zip5);
            string request = CreateRequest("Verify", xml);
            XDocument doc = MakeRequest(request);
            return doc.Root.Elements("Address").Select(d => new USPSAddress { Zip4 = d.Element("Zip4").Value, Address2 = d.Element("Address2").Value, City = d.Element("City").Value, State = d.Element("State").Value, Zip5 = d.Element("Zip5").Value }).FirstOrDefault();
        }



    }
}
