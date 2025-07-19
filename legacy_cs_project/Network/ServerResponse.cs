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
using System.Text;
using System.Xml.Serialization;

using Logging;

namespace Network
{
   /// <summary>
   /// An object with data populated by a server error response.
   /// </summary>
   [XmlRoot("response")]
   public class AServerResponse
   {
      public static AServerResponse MakeFromXml(string xmlResponse)
      {
         AServerResponse response = null;
         try
         {
            var stream = new StringReader(xmlResponse);
            response = (AServerResponse)sSerializer.Deserialize(stream);
         }
         catch (Exception ex)
         {
            ALog.Error("Network", "Failed to parse server response.\r\n{0}", ex.Message);

            if (null == response)
            {
               response = new AServerResponse();
               response.code = "Unknown Error";
               response.description = "An unknown error response was received.";
               response.technicalDetails = xmlResponse;
            }
         }
         return response;
      }

      [XmlAttribute]
      public string success;

      [XmlAttribute]
      public string code;

      public string description;

      [XmlElement("technical")]
      public string technicalDetails;

      private static XmlSerializer sSerializer = new XmlSerializer(typeof(AServerResponse));
   }


   /// <summary>
   /// Thrown by ANanoSenseClient when the server returns a response with success == "no"
   /// </summary>
   public class AServerException : Exception
   {
      public AServerException(AServerResponse response)
      : base(String.Format("{0} - {1}", response.code, response.description))
      {
         fResponse = response;
      }

      public AServerResponse Response { get { return fResponse; } }

      private AServerResponse fResponse;
   }
}
