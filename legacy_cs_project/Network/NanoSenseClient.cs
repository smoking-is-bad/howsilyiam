// Copyright (c) 2015 Sensor Networks, Inc.
// 
// All rights reserved. No part of this publication may be reproduced,
// distributed, or transmitted in any form or by any means, including
// photocopying, recording, or other electronic or mechanical methods, without
// the prior written permission of Sensor Networks, Inc., except in the case of
// brief quotations embodied in critical reviews and certain other noncommercial
// uses permitted by copyright law.
// 
// 
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

using DsiExtensions;
using Logging;
using Model;
using Newtonsoft.Json.Linq;

// TODO: Add success and failure completions. 

namespace Network
{
   /// <summary>
   /// Handle communication with the NanoSense Cloud server.
   /// </summary>
   class ANanoSenseClient
   {
      const string kDefaultBaseUriString = "https://smartpims.com";

      const bool kForceSsl = true;

      public ANanoSenseClient(string baseUri = kDefaultBaseUriString)
      {
         fHttpClientHandler = new HttpClientHandler() { AllowAutoRedirect = false };
         fHttpClientHandler.CookieContainer = new System.Net.CookieContainer();
         fProgressHandler = new ProgressMessageHandler(fHttpClientHandler);

         fHttpClient = new HttpClient(fProgressHandler);
         // This header is important for login. Without it we get no session id cookie.
         fHttpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
         fHttpClient.DefaultRequestHeaders.Add("Accept", "*/*");
         fHttpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8");
         fHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         // The http client must be initialized before we set the base URL.
         this.SetBaseUrl(baseUri);
      }

      public void SetBaseUrl(string baseUrl)
      {
         fBaseUri = baseUrl.AsUriForcingSecureHttp();

         fHttpClient.DefaultRequestHeaders.Remove("Referer");
         fHttpClient.DefaultRequestHeaders.Add("Referer", fBaseUri.AbsoluteUri);
      }

      public ProgressMessageHandler GetProgressHandler()
      {
         return fProgressHandler;
      }

      public async Task Login(string username, string password)
      {
         try
         {
            var loginUri = new Uri(fBaseUri, "/smartpims/login/");

            // Send user/pass as JSON
            var request = new HttpRequestMessage(HttpMethod.Post, loginUri);
            request.Content = new StringContent("{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}",
                                                Encoding.UTF8,
                                                "application/json");

            var response = await fHttpClient.SendAsync(request).ConfigureAwait(false);

            var csrfToken = this.CookieValueWithName("csrftoken");
            if (null == csrfToken)
            {
               ALog.Warning("Network", "Failed to find csrftoken in response message.");
            }

            response.EnsureSuccessStatusCode(); // throws

            var sessionId = this.CookieValueWithName("sessionid");
            if (null == sessionId)
            {
               ALog.Error("Network", "Failed to authenticate - no sessionid returned.");
               throw new HttpRequestException(DsiApi.ApiResources.ErrorLoginInvalidUserPass);
            }

            // FAILURE LOOKS LIKE THIS (yep, status:200):   
            // {'status': 200, 'result': False}
            // SUCCESS LOOKS LIKE THIS:   
            // {'status': 200, 'result': True}
            string body = await response.Content.ReadAsStringAsync();
            var jo = JObject.Parse(body);
            if (200 != (int)jo["status"] || !(bool)jo["result"])
            {
               throw new HttpRequestException(DsiApi.ApiResources.ErrorLoginInvalidUserPass);
            }

            ALog.Info("Network", "Login succeeded.");
         }
         catch (HttpRequestException ex)
         {
            ALog.Error("Network", "Login failed; {0}", ex.ToString());
            throw ex;
         }
      }


      public async Task Logout()
      {
         await fHttpClient.GetAsync(new Uri(fBaseUri, "/smartpims/logout/"),
          HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
         this.ClearCookies();
      }


      public async Task UploadXmlFile(string path, bool setTestId = false)
      {
         using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
         {
            var bytes = new byte[stream.Length];
            int toRead = bytes.Length;
            int numRead = 0;
            
            while (toRead > 0)
            {
               int n = stream.Read(bytes, numRead, toRead);
               if (0 == n)
               {
                  break;
               }
               numRead += n;
               toRead -= n;
            }

            await this.UploadXml(Encoding.UTF8.GetString(bytes), setTestId);
         }
      }

      /// TODO: Rewrite to handle stream for more efficient memory use?
      /// <summary>
      /// Upload the given string containing xml. Throws AServerException if the
      /// server returns an error response.
      /// </summary>
      /// <param name="xml"></param>
      /// <param name="setTestId">Test only, replace the XML's testid attribute with a new GUID.</param>
      /// <returns></returns>
      public async Task UploadXml(string xml, bool setTestId = false)
      {
         var uploader = new AUploader(fHttpClient, fBaseUri.ToString());

         // Testing hack so we can upload the same file repeatedly.
         if (setTestId)
         {
            xml = this.SetTestId(xml);
         }
         var token = this.CookieValueWithName("csrftoken");
         var payload = new AMultiPartMessage(xml, token, "resultsFile", "nanosense.xml", "text/xml");

         HttpResponseMessage response = await uploader.Upload("smartpims/uploadResults", payload).ConfigureAwait(false);
         string responseXml = null;
         if (response.Content != null)
         {
            responseXml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
         }

         try
         {
            response.EnsureSuccessStatusCode(); // Throws if not successful
            ALog.Info("Network", "Upload succeeded.");
         }
         catch (HttpRequestException)
         {
            ALog.Warning("Network", "Upload failed.");
            var responseObj = AServerResponse.MakeFromXml(responseXml);
            throw new AServerException(responseObj);
         }
      }


      public string CookieValueWithName(string name)
      {
         var cookieCollection = fHttpClientHandler.CookieContainer.GetCookies(fBaseUri);
         string value = null;
         foreach (Cookie c in cookieCollection)
         {
            if (c.Name == name)
            {
               value = c.Value;
               break;
            }
         }

         return value;
      }

      public void ClearCookies()
      {
         foreach (Cookie cookie in fHttpClientHandler.CookieContainer.GetCookies(fBaseUri))
         {
            cookie.Expired = true;
            cookie.Discard = true;
         }
      }

      private string GenerateXmlContent<T>(T data)
      {
         MemoryStream stream = new MemoryStream();
         var serializer = new XmlSerializer(typeof(T));
         serializer.Serialize(stream, data);
         stream.Seek(0, SeekOrigin.Begin);
         return new StreamReader(stream, Encoding.UTF8).ReadToEnd();
      }


      private string SetTestId(string xml)
      {
         using (var stream = new MemoryStream())
         {
            var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(xml);
            writer.Flush();
            stream.Position = 0;

            var xs = new XmlSerializer(typeof(ANanoSense));
            var root = (ANanoSense)xs.Deserialize(stream);
            root.testId = Guid.NewGuid().ToString();

            stream.SetLength(0);
            xs.Serialize(stream, root);
            stream.Position = 0;

            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
         }
      }


      private Uri fBaseUri;
      private HttpClient fHttpClient;
      private HttpClientHandler fHttpClientHandler;
      private ProgressMessageHandler fProgressHandler;
   }
}

