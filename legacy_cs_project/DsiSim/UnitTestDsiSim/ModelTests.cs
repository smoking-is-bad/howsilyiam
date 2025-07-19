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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DsiApi;
using MockData;
using Model;


namespace UnitTestDsiSim
{
   /// <summary>
   /// Summary description for ModelTests
   /// </summary>
   [TestClass]
   public class ModelTests
   {
      public ModelTests()
      {
         fRng = new Random();
         var mockData = new AMockData(fRng);

         fCompany = mockData.Company();
         fCollectionPoint = mockData.CollectionPoint();
         fDsiModel = mockData.DsiAndProbes(numProbes:2, numSetups:3)[0];
      }

      [TestMethod]
      public void TestXmlToModel()
      {
         // Make the XML
         var root = new ANanoSense();
         root.fVersion = ANanoSense.kMaxSupportedVersion;
         root.testId = Guid.NewGuid().ToString();
         root.testUser = "LoggedInUserName";

         root.company = fCompany;
         root.collectionPoint = fCollectionPoint;
         root.Dsi = fDsiModel;
         root.asset = new AAsset { name = "Our Asset" };
         root.site = new ASite { name = "Our Site" };
         root.plant = new APlant { name = "Our Plant" };

         var outStream = new MemoryStream();
         var serializer = new XmlSerializer(typeof(ANanoSense));
         serializer.Serialize(outStream, root);
         outStream.Seek(0, SeekOrigin.Begin);
        
         // Re-initialize to simulate conditions in the app.
         serializer = new XmlSerializer(typeof(ANanoSense));
         ANanoSense readBackNanoSense = (ANanoSense)serializer.Deserialize(outStream);
         
         // TODO: Test values read back.
      }

      [TestMethod]
      public void TestBasicModelToXml()
      {
         var root = new ANanoSense();
         root.fVersion = ANanoSense.kMaxSupportedVersion;
         root.testId = Guid.NewGuid().ToString();
         root.testUser = "LoggedInUserName";

         root.company = fCompany;
         root.collectionPoint = fCollectionPoint;
         root.Dsi = fDsiModel;
         // At least one of these is required by the server.
         root.asset = new AAsset { name = "Our Asset" };
         root.site = new ASite { name = "Our Site" };
         root.plant = new APlant { name = "Our Plant" };

         var stream = new MemoryStream();

#if false  // DataContractSerializer can't output XML attributes which we need in the nanosense element.
      
         var serializer = new DataContractSerializer(typeof(ANanosense));
         serializer.WriteObject(stream, root);

#else

         var serializer = new XmlSerializer(typeof(ANanoSense));
         serializer.Serialize(stream, root);

#endif

         stream.Seek(0, SeekOrigin.Begin);
         string content = new StreamReader(stream, Encoding.UTF8).ReadToEnd();

         Console.WriteLine(content);

         // TODO: Test that output is what we expect
      }


      [TestMethod]
      public void TestFullXmlWriteAndRead()
      {
         string kTestDsiDir = "XmlUnitTestDsiNet";
         Directory.CreateDirectory(kTestDsiDir);
         Console.WriteLine(Directory.GetCurrentDirectory());

         string xmlPath = "..\\..\\..\\..\\TabletApp\\Sim\\testdata.xml";

         // Use the DSINetwork's ability to write the XML to the device.
         var dsiNet = new ADsiNetwork(1, kTestDsiDir, 0, null, null, xmlPath);
         byte netAddress = 0x01;

         this.TestFullXmlWithDsiNetwork(dsiNet, xmlPath, netAddress);
      }


      [TestMethod]
      public void TestFullXmlWriteAndReadViaSerial()
      {
         string xmlPath = "..\\..\\..\\..\\TabletApp\\Sim\\testdata.xml";
         var dsiNet = new ADsiNetwork("COM3", 115200, null, null, xmlPath);
         byte netAddress = 0x02; // May change in future.

         this.TestFullXmlWithDsiNetwork(dsiNet, xmlPath, netAddress);
      }


      private void TestFullXmlWithDsiNetwork(ADsiNetwork dsiNet, string xmlPath, byte netAddress)
      {
         // But read in the XML for comparison.
         // !!!: Maybe we need a class to wrap the xml generation to set counts.
         var stream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read);
         var serializer = new XmlSerializer(typeof(ANanoSense));
         ANanoSense expected = (ANanoSense)serializer.Deserialize(stream);
         expected.Dsi.probeCount = (UInt16)expected.Dsi.probes.Length;
         foreach (AProbe probe in expected.Dsi.probes)
         {
            probe.numSetups = (UInt16)probe.setups.Length;
         }

         ANanoSense newRoot = XmlRoundTrip(dsiNet, netAddress);

         this.CompareResults(expected, newRoot);
      }


      private ANanoSense XmlRoundTrip(ADsiNetwork dsiNet, byte netAddress)
      {
         // ReadBack into nanosense obj.
         var newRoot = new ANanoSense();

         newRoot.company = dsiNet.ReadCompany(netAddress);
         newRoot.collectionPoint = dsiNet.ReadCollectionPoint(netAddress);
         newRoot.Dsi = new ADsiInfo(dsiNet.ReadDsiInfo(netAddress));
         newRoot.Dsi.probes = new AProbe[newRoot.Dsi.probeCount];
         List<AProbe> probes = dsiNet.ReadProbesAndSetups(netAddress, newRoot.Dsi.probeCount);
         int i = 0;
         foreach (AProbe probe in probes)
         {
            // Copy ref to the object tree.
            newRoot.Dsi.probes[i++] = probe;

            foreach (ASetup setup in probe.setups)
            {
               setup.gates = dsiNet.ReadGates(netAddress, (UInt16)(probe.num - 1),
                (UInt16)(setup.num - 1));
            }
         }

         return newRoot;
      }


      private void CompareResults(ANanoSense expected, ANanoSense actual)
      {
         Assert.AreEqual(expected.company, actual.company);
         Assert.AreEqual(expected.collectionPoint, actual.collectionPoint);
         Assert.AreEqual(expected.Dsi, actual.Dsi);
         IEnumerator actualProbes = actual.Dsi.probes.GetEnumerator();
         foreach (AProbe expectedProbe in expected.Dsi.probes)
         {
            // If this fails, there were different numbers of probes.
            Assert.IsTrue(actualProbes.MoveNext());

            AProbe actualProbe = (AProbe)actualProbes.Current;
            Assert.AreEqual(expectedProbe, actualProbe);

            IEnumerator actualSetups = actualProbe.setups.GetEnumerator();
            foreach (ASetup expectedSetup in expectedProbe.setups)
            {
               Assert.IsTrue(actualSetups.MoveNext());

               ASetup actualSetup = (ASetup)actualSetups.Current;
               Assert.AreEqual(expectedSetup, actualSetup);

               IEnumerator actualGates = actualSetup.gates.GetEnumerator();
               foreach (AGate expectedGate in expectedSetup.gates)
               {
                  Assert.IsTrue(actualGates.MoveNext());

                  AGate actualGate = (AGate)actualGates.Current;
                  Assert.AreEqual(expectedGate, actualGate);
               }
            }

         }
      }

      private Random fRng;
      private ACompany fCompany;
      private ACollectionPoint fCollectionPoint;
      private ADsiInfo fDsiModel;
   }
}

