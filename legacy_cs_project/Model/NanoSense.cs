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
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace Model
{
   /// <summary>
   /// Root object for describing test results and associated metadata for sending
   /// back to the server.
   /// </summary>
   [XmlRoot(ElementName = "nanosense", Namespace = "")]
   public class ANanoSense
   {
      public ACompany company;
      public ACollectionPoint collectionPoint;

      [XmlElement(IsNullable = true)]
      public AAsset asset;
      [XmlElement(IsNullable = true)]
      public APlant plant;
      [XmlElement(IsNullable = true)]
      public ASite site;

      /// <summary>
      /// Array of DSI information. There should usually be only one item in the array
      /// unless configuring multiple DSIs. See Dsi property.
      /// </summary>
      [XmlArray("dsis"), XmlArrayItem("dsi")]
      public ADsiInfo[] dsis;

      /// <summary>
      /// Wrapper for handling the common single DSI case. Always sets/returns the first DSI in the array.
      /// </summary>
      [XmlIgnore]
      public ADsiInfo Dsi
      {
         get { return this.dsis[0]; }

         set
         {
            if (null == this.dsis)
            {
               this.dsis = new ADsiInfo[1];
            }
            this.dsis[0] = value;
         }
      }

      [XmlAttribute("version")]
      public string fVersion = "1.0";

      // Identifier of this particular test
      [XmlAttribute("testId")]
      public string testId;

      // Who ran the test
      [XmlAttribute("testUser")]
      public string testUser;

      [XmlIgnore]
      public const string kMaxSupportedVersion = "1.0";

      // Path for XML file that represents this object
      [XmlIgnore]
      public string backingFilePath;

      // Path for CSV file that represents this object
      [XmlIgnore]
      public string csvFilePath;

      /// <summary>
      /// Performa a shallow copy on this object, meaning reference all the existing objects and give it
      /// a new array of probes.
      /// </summary>
      /// <returns>The ANanoSense copy</returns>
      public ANanoSense ShallowCopy()
      {
         ANanoSense newNanoSense = new ANanoSense();
         newNanoSense.company = this.company;
         newNanoSense.collectionPoint = this.collectionPoint;
         newNanoSense.asset = this.asset;
         newNanoSense.plant = this.plant;
         newNanoSense.site = this.site;
         newNanoSense.Dsi = new ADsiInfo(this.Dsi);
         newNanoSense.Dsi.Longitude = this.Dsi.Longitude;
         newNanoSense.Dsi.Latitude = this.Dsi.Latitude;
         newNanoSense.Dsi.probes = new AProbe[0];

         return newNanoSense;
      }

      public static ANanoSense MakeFromXmlFile(string xmlFilePath)
      {
         ANanoSense results = null;
         using (var xmlStream = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read))
         {
            var serializer = new XmlSerializer(typeof(ANanoSense));

            // Get the model and reset the address from whatever was stored.
            results = (ANanoSense)serializer.Deserialize(xmlStream);
         }
         return results;
      }
   }
}

