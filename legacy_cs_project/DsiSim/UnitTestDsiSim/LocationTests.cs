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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Model;

namespace UnitTestDsiSim
{
   [TestClass]
   public class UnitTestGpsLocation
   {
      [TestMethod]
      public void TestGpsLocationParsing()
      {
         var location = new AGpsCoordinate();
         // default coordinates are "".

         Assert.AreEqual(null, location.Latitude);
         Assert.AreEqual(null, location.Longitude);

         location.coordinates = ";";
         Assert.AreEqual(null, location.Latitude);
         Assert.AreEqual(null, location.Longitude);

         location.coordinates = "1;";
         Assert.AreEqual(null, location.Latitude);
         Assert.AreEqual(null, location.Longitude);

         location.coordinates = ";2";
         Assert.AreEqual(null, location.Latitude);
         Assert.AreEqual(null, location.Longitude);

         location.coordinates = "1;2";
         Assert.AreEqual("1", location.Latitude);
         Assert.AreEqual("2", location.Longitude);

         location.coordinates = "1.0;-2.0";
         Assert.AreEqual("1.0", location.Latitude);
         Assert.AreEqual("-2.0", location.Longitude);
      }

      [TestMethod]
      public void TestGpsNullCoordinates()
      {
         var location = new AGpsCoordinate();
         location.coordinates = null;

         Assert.AreEqual(null, location.Latitude);
         Assert.AreEqual(null, location.Longitude);
      }
   }
}

