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
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using TabletApp.Api.DsiApi;

namespace Model
{
   /// <summary>
   /// Handle XML output of the asset,site,plant name as XmlText rather than with a `name` subnode
   /// </summary>
   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public partial class APlaceName : IXmlSerializable
   {
      public void WriteXml(XmlWriter writer)
      {
         writer.WriteString((string)name);
      }
      public void ReadXml(XmlReader reader)
      {
         name = reader.ReadString();
         reader.Read(); // Move to next node or XmlSerializer gets confused.
      }
      public XmlSchema GetSchema()
      {
         return (null);
      }
  
      public AUtf8String64 name;
   }

   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public partial class AAsset : APlaceName
   {}

   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public partial class APlant : APlaceName
   {}

   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public partial class ASite : APlaceName
   {}
}

