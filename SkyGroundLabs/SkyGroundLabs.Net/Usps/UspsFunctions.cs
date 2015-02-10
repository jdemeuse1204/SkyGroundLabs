using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace SkyGroundLabs.Net.Usps
{
    public static class UspsFunctions
    {
        private static readonly string _key = "544NICKO0542";

        public static UspsAddressValidateResponse VerifyAddress(UspsAddress address)
        {
            var url = string.Format("http://production.shippingapis.com/ShippingAPITest.dll?API=Verify&XML=<AddressValidateRequest%20USERID=\"{0}\">" +
                "<Address>" +
                 "<Address1>{1}</Address1>" +
                 "<Address2>{2}</Address2>" +
                 "<City>{3}</City>" +
                 "<State>{4}</State> " +
                 "<Zip5>{5}</Zip5> " +
                 "<Zip4>{6}</Zip4> " +
                "</Address> " +
                "</AddressValidateRequest>",
                _key,
                address.Address1,
                address.Address2,
                address.City,
                address.State,
                address.Zip5,
                address.Zip4);

            var wc = new WebClient();
            var response = wc.DownloadData(url);
            var directions = Encoding.ASCII.GetString(response);
            var serializer = new XmlSerializer(typeof(UspsAddressValidateResponse));

            return serializer.Deserialize(new StringReader(directions)) as UspsAddressValidateResponse;
        }
    }
}
