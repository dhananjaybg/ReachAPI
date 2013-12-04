using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml.Linq;
using System.IO;

public partial class LC : System.Web.UI.Page
{
    private const string rawLinkFormat = "{0}{1}";
    private const string sessionAuthUrlParams = "?action=login&username=admin&password=";
        
    private const string urlParamsFormat = "?action=login&username=admin&password=admin&flag=sales&vert=prn&location=/Reach/web/pub.xql&model={0}&profile={1}&notoc={2}&docid={3}&lang={4}&pub={5}";
    //private const string httpRequestUrlFormat = "https://sdlpe.pe.local:8993/Reach/web/content.xql?flag=sales&vert=prn&model={0}&action=topic&profile={1}&docid={3}&lang={4}&pub={5}&jsessionid={6}";
    //private const string LCBaseUrl = "https://sdlpe.pe.local:8993/Reach/web/session.xql";
        
    
    private const string LCBaseUrl = "http://sdldemo2013:8080/Reach/web/session.xql";    
    private const string httpRequestUrlFormat = "http://sdldemo2013:8080/Reach/web/content.xql;jsessionid={6}?flag=sales&vert=prn&model={0}&action=topic&profile={1}&docid={3}&lang={4}&pub={5}";

    private const string LCDatasheetGUID = "GUID-C0B9E1AB-3F25-4569-8C42-F79CC6AD6544";
    private const string LCDatasheetPub = "AIO Datasheet-v1";
    
    
    
    
    protected void Page_Load(object sender, EventArgs e)
    {
        //Not needed since we DONOT Authenticate

        ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        
        //login in to  Reach  make sure that this login is 
        var sessionId = Authenticate();

        //get some topic content
        if (!string.IsNullOrEmpty(sessionId))
        {
            
            string httpRequestParameters = string.Format(httpRequestUrlFormat, "910", "cust", "no", LCDatasheetGUID, "en", LCDatasheetPub, sessionId);
            
            var url = HttpUtility.HtmlDecode(httpRequestParameters);            
            var req = (HttpWebRequest)WebRequest.Create(url);
            Response.Write("Session Id=" + sessionId);
            Response.Write("<br/>LC URL=" + url);
            req.Method = "GET";
            req.ContentType = "text/html";
            req.KeepAlive = true;
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            
            var resp = (HttpWebResponse)req.GetResponse();
            using (var sr = new StreamReader(resp.GetResponseStream()))
            {                
                Response.Write(sr.ReadToEnd().Trim());
            }
        }
    }

    private string Authenticate()
    {
        string sessionId = "";
        var url = HttpUtility.HtmlDecode(string.Format("{0}{1}", LCBaseUrl, sessionAuthUrlParams));

        var req = (HttpWebRequest)WebRequest.Create(url);
        req.Method = "GET";
        req.ContentType = "text/html";
        req.KeepAlive = true;        
        var resp = (HttpWebResponse)req.GetResponse();

        using (var sr = new StreamReader(resp.GetResponseStream()))
        {
            XElement xe = XElement.Parse(sr.ReadToEnd().Trim());
            if (xe != null)
            {
                sessionId = xe.Element("info").Attribute("value").Value;
            }
        }

        Response.Write( "<h4>Logged in as admin with the sessionid: " + sessionId + "</h4>");

        return sessionId;
    }

    public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}