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
using System.Xml.Serialization;

using TabletApp.Api.DsiApi;

namespace Model
{
   [XmlRoot("collectionPoint"), StructLayout(LayoutKind.Sequential, Pack=2, CharSet = CharSet.Ansi)]
   public partial class ACollectionPoint
   {
      public ACollectionPoint()
      {
         name = "";
         description = "";
         location = new AGpsCoordinate();

      }

      public AUtf8String64 name;
      public AUtf8String128 description;

      [XmlIgnore]
      public AGpsCoordinate location;

      [XmlAttribute("latitude")]
      public string Latitude
      {
         get { return location.Latitude; }
         set { location.Latitude = value; }
      }

      [XmlAttribute("longitude")]
      public string Longitude
      {
         get { return location.Longitude; }
         set { location.Longitude = value; }
      }
   }
}
