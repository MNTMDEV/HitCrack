using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

/// <summary>
///HttpPost 的摘要说明
/// </summary>
public class HttpPost
{
    /// <summary>
    /// POST方法请求返回html
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="postDataStr">请求正文</param>
    /// <param name="cookies">Cookies</param>
    /// <returns></returns>
    public static string Post(string Url, string postDataStr,string cookies)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        request.Proxy = null;
        request.Method = "POST";
        request.Headers.Add("Cookie", cookies);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = postDataStr.Length;
        StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
        writer.Write(postDataStr);
        writer.Close();
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string encoding = response.ContentEncoding;
        if (encoding == null || encoding.Length < 1)
        {
            encoding = "UTF-8"; //默认编码  
        }
        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
        string retString = reader.ReadToEnd();
        return retString;
    }

    /// <summary>
    /// GET请求并返回html
    /// </summary>
    /// <param name="Url"></param>
    /// <returns></returns>
    public static string GetContent(string Url)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        request.Method = "GET";
        request.ContentType = "application/x-www-form-urlencoded";
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string encoding = response.ContentEncoding;
        if (encoding == null || encoding.Length < 1)
        {
            encoding = "UTF-8"; //默认编码  
        }
        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
        string retString = reader.ReadToEnd();
        return retString;
    }

    /// <summary>
    /// GET方法进行请求并返回Cookies
    /// </summary>
    /// <param name="Url"></param>
    /// <returns></returns>
    public static string Get(string Url)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        request.Method = "GET";
        request.ContentType = "application/x-www-form-urlencoded";
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string encoding = response.ContentEncoding;
        if (encoding == null || encoding.Length < 1)
        {
            encoding = "UTF-8"; //默认编码  
        }
        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
        string retString = reader.ReadToEnd();
        return response.GetResponseHeader("Set-Cookie");
    }

	public HttpPost()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}
}