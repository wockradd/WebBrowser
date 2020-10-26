using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;


public class Requester{
    public struct Response{
        public string res {get;set;}
        public int status {get;set;}
        public string title {get;set;}
    } 

    static private Regex urlRegex = new Regex(@"^https?\:\/\/(www\.)?[a-zA-Z0-9@:%._\+~#=-]+\.[a-zA-Z0-9@:%_\+.~#?&/=-]+$");
    
    static public bool matchesUrl(string test){
        return urlRegex.IsMatch(test);
    }

    static public string getTitle(string html){
        int start = html.IndexOf("<title>");
        int end = html.IndexOf("</title>");
        return html.Substring(start+7,end-start-7);
    }

    static public async Task<Response> asyncRequest(string url){
        WebRequest webReq;
        WebResponse webRes;
        Stream resStream;
        StreamReader streamReader;
        Response response = new Response();
        bool fatalError = false;
        
        url = url.Trim();
        if(!urlRegex.IsMatch(url)){
            response.res = "Invalid url";
            response.status = -1;
        }else{
            webReq = WebRequest.Create(url);

            try{//everything goes well
                webRes = await webReq.GetResponseAsync();
                resStream = webRes.GetResponseStream();
                streamReader= new StreamReader(resStream);
                response.res = streamReader.ReadToEnd();
                response.title = getTitle(response.res);

            }catch(WebException we){//valid url but error
                response.res = we.Message;
                if(we.Status == WebExceptionStatus.ProtocolError){//404,403,etc
                    webRes = we.Response;
                }else{                                            //name resolution error, etc                     
                    response.status = -1;
                    webRes = null;
                    fatalError = true;
                } 
            }

            if(!fatalError){
                response.status = (int)((HttpWebResponse)webRes).StatusCode;
                webRes.Close();
            }
            
        }
        return response;
    }
}