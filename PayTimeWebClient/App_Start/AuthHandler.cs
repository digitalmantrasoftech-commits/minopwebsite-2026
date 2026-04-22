using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using JWT;
using System.Configuration;
using System.Text;
using System.Xml;
using System.Data;
using System.Web.Mvc;
namespace PayTimeWebClient
{
    public class AuthHandler : DelegatingHandler
    {
        MRespoe resp = new MRespoe();

        //protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        //            CancellationToken cancellationToken)
        //{
        //    HttpResponseMessage errorResponse = null;

        //    try
        //    {
        //        IEnumerable<string> authHeaderValues;
        //        request.Headers.TryGetValues("Authorization", out authHeaderValues);


        //        if (authHeaderValues == null)
        //            return base.SendAsync(request, cancellationToken); // cross fingers


        //        var bearerToken = authHeaderValues.ElementAt(0);
        //        var token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

        //        var secret = ConfigurationManager.AppSettings.Get("jwtKey");
        //        //var secret = "secretKey";

        //        Thread.CurrentPrincipal = ValidateToken(
        //            token,
        //            secret,
        //            true
        //            );

        //        if (HttpContext.Current != null)
        //        {
        //            HttpContext.Current.User = Thread.CurrentPrincipal;
        //        }
        //    }


        //    catch (SignatureVerificationException ex)
        //    {
        //        resp.MegSts = "error";
        //        resp.Meg = "Token VerificationException ! " + ex.Message;
        //        errorResponse = request.CreateResponse(HttpStatusCode.Unauthorized, resp);
        //    }
        //    catch (Exception ex)
        //    {
        //        resp.MegSts = "error";
        //        resp.Meg = "InternalServerError! " + ex.Message;
        //        errorResponse = request.CreateResponse(HttpStatusCode.InternalServerError, resp);
        //    }


        //    return errorResponse != null
        //        ? Task.FromResult(errorResponse)
        //        : base.SendAsync(request, cancellationToken);
        //}

        public static string getPayload(string token, string secret, bool checkExpiration)
        {

            var jsonSerializer = new JavaScriptSerializer();

            var payloadJson = JsonWebToken.Decode(token, secret);

            return payloadJson;
        }

        private static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        //public static SelectList GetCityByid(string state_id)
        //{
        //    System.Data.DataSet ds = new System.Data.DataSet();
        //    ds.ReadXml(HttpContext.Current.Server.MapPath("/App_Start/timezones.xml"));
        //    System.Data.DataTable dt = ds.Tables["cities"];
        //    List<Cities> lstcity = new List<Cities>();
        //    string expr = "state_id='" + state_id + "'";

        //    foreach (DataRow dr in dt.Select(expr))
        //    {
        //        lstcity.Add(new Cities { cityid = Convert.ToInt32(dr[0].ToString().Replace(" ", "")), cityname = dr[1].ToString() });
        //    }
        //    return new SelectList(lstcity, "cityid", "cityname");
        //}
    }

    public class MRespoe
    {
        public string MegSts { get; set; }
        public string Meg { get; set; }
    }

    //public class Cities
    //{
    //    public int cityid { get; set; }
    //    public string cityname { get; set; }
    //    public int stateid { get; set; }
    //}
    //public class states
    //{
    //    public int statesid { get; set; }
    //    public string statesname { get; set; }
    //    public int countryid { get; set; }
    //}
    //public class countries
    //{
    //    public int countriesid { get; set; }
    //    public string countriesname { get; set; }
    //    public int zoneid { get; set; }
    //}

}