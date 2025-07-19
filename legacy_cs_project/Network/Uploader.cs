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
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using DsiExtensions;


namespace Network
{
   /// <summary>
   /// Helper for posting files to NanoSense cloud server.
   /// </summary>
   public class AUploader
   {
      public AUploader(HttpClient client, string baseUrl)
      {
         Debug.Assert(null != baseUrl && baseUrl.Length != 0);
         fBaseUri = baseUrl.AsUriForcingSecureHttp();
         fHttpClient = client;
      }

      public AUploader(string baseUrl)
         : this(new HttpClient(), baseUrl)
      {
      }


      /// <returns>A task producing the server's response.</returns>
      public Task<HttpResponseMessage> Upload(string path, MultipartContent multiPart)
      {
         var uri = new Uri(fBaseUri, path);
         var request = new HttpRequestMessage(HttpMethod.Post, uri);
         request.Content = multiPart;
         fHttpClient.DefaultRequestHeaders.Remove("Referer");
         fHttpClient.DefaultRequestHeaders.Add("Referer", fBaseUri.AbsoluteUri);
         return fHttpClient.SendAsync(request);
      }

      private HttpClient fHttpClient;
      private Uri fBaseUri;
   }

   /// <summary>
   /// Simple mult-part message form-data content builder expecting only one value.
   /// (I've seen places in .Net that return multi-part data but haven't seen where to create some.)
   /// </summary>
   public class AMultiPartMessage : MultipartFormDataContent
   {
      public AMultiPartMessage(string content, string csrfToken, string name, string filename, string mimeType)
         : base(MakeBoundary())
      {
         fName = name;
         fFilename = filename;
         if (null != csrfToken)
         {
            this.Add(new StringContent(csrfToken, Encoding.UTF8), "csrfmiddlewaretoken");
         }
         this.Add(new StringContent(content, Encoding.UTF8, mimeType), name, filename);
      }

      public static string MakeBoundary()
      {
         var random = new Random().Next(1, 9999999).ToString();
         return "----SensNet" + AMultiPartMessage.Base64Encode(random);
      }

      private static string Base64Encode(string s)
      {
         var bytes = Encoding.UTF8.GetBytes(s);
         return Convert.ToBase64String(bytes);
      }

      private string fName;
      private string fFilename;
   }


}

