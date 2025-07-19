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
using TabletApp.Persist;
using Model;
using MockData;
using System.Collections.Generic;
using System.IO;

namespace TableAppTest
{
   [TestClass]
   public class PersistTest
   {
      private Random fRng;
      private ACompany fCompany;
      private ACollectionPoint fCollectionPoint;
      private ADsiInfo fDsiModel;

      [TestInitialize]
      public void Initialize()
      {
         this.Load();

         fRng = new Random();
         var mockData = new AMockData(fRng);

         fCompany = mockData.Company();
         fCollectionPoint = mockData.CollectionPoint();
         fDsiModel = mockData.DsiAndProbes(numProbes: 2, numSetups: 3)[0];
      }

      [TestCleanup]
      public void Cleanup()
      {
         this.Delete();
      }

      private void Load()
      {
         AFileManager.Instance.Initialize(@"C:\nanotest");
      }

      private void Delete()
      {
         Directory.Delete(@"C:\nanotest", true);
      }

      private ANanoSense BaseNano()
      {
         var nano = new ANanoSense();
         nano.fVersion = ANanoSense.kMaxSupportedVersion;
         nano.testId = Guid.NewGuid().ToString();
         nano.testUser = "LoggedInUserName";

         nano.company = fCompany;
         nano.collectionPoint = fCollectionPoint;
         nano.Dsi = fDsiModel;

         return nano;
      }

      private ANanoSense C_S_CP()
      {
         var nano = this.BaseNano();

         nano.asset = new AAsset();
         nano.site = new ASite { name = "Test Site 1" };
         nano.plant = new APlant();

         return nano;
      }

      private ANanoSense C_P_A_CP()
      {
         var nano = this.BaseNano();

         nano.asset = new AAsset { name = "Test Asset 2" };
         nano.site = new ASite();
         nano.plant = new APlant { name = "Test Plant 2" };

         return nano;
      }

      private ANanoSense C_S_P_A_CP()
      {
         var nano = this.BaseNano();

         nano.asset = new AAsset { name = "Test Asset 3" };
         nano.site = new ASite { name = "Test Site 3" };
         nano.plant = new APlant { name = "Test Plant 3" };

         return nano;
      }

      private ANanoSense C_S_P_A_CP_Long()
      {
         var nano = this.BaseNano();

         nano.asset = new AAsset { name = "A23456789012345678901234567890" };
         nano.site = new ASite { name = "S23456789012345678901234567890" };
         nano.plant = new APlant { name = "P23456789012345678901234567890" };

         return nano;
      }

      [TestMethod]
      public void TestSave()
      {
         List<ANanoSense> nanos = new List<ANanoSense>();

         // save C1,S,CP
         ANanoSense one = this.C_S_CP();
         nanos.Add(one);

         // save C2,P,A,CP
         ANanoSense two = this.C_P_A_CP();
         two.company = new ACompany() { name = "Company 2" };
         nanos.Add(two);

         // save C3,S,P,A,CP
         ANanoSense three = this.C_S_P_A_CP();
         three.company = new ACompany() { name = "Company 3" };
         nanos.Add(three);

         // save C3,S2,P,A,CP
         ANanoSense four = this.C_S_P_A_CP();
         four.company = new ACompany() { name = "Company 3" };
         four.site = new ASite { name = "Test Site 4" };
         nanos.Add(four);

         string errorString;
         AFileManager.Instance.Save(nanos, out errorString);

         // check that the expected directory structure exists
         Assert.IsTrue(Directory.Exists(@"C:\nanotest\Boalsburg Gas Company\Test Site 1\123456"));
         Assert.IsTrue(Directory.Exists(@"C:\nanotest\Company 2\Test Plant 2\Test Asset 2\123456"));
         Assert.IsTrue(Directory.Exists(@"C:\nanotest\Company 3\Test Site 3\Test Plant 3\Test Asset 3\123456"));
         Assert.IsTrue(Directory.Exists(@"C:\nanotest\Company 3\Test Site 4\Test Plant 3\Test Asset 3\123456"));

         // check that the meta files exists and are correct
         Assert.IsTrue(File.Exists(@"C:\nanotest\Boalsburg Gas Company\.meta"));
         Assert.IsTrue(File.Exists(@"C:\nanotest\Boalsburg Gas Company\Test Site 1\.meta"));
         Assert.IsTrue(File.Exists(@"C:\nanotest\Boalsburg Gas Company\Test Site 1\123456\.meta"));
         Assert.IsTrue(File.Exists(@"C:\nanotest\Company 2\Test Plant 2\.meta"));
         Assert.IsTrue(File.Exists(@"C:\nanotest\Company 3\Test Site 3\Test Plant 3\Test Asset 3\123456\.meta"));

         string contents = File.ReadAllText(@"C:\nanotest\Boalsburg Gas Company\.meta");
         Assert.IsTrue(int.Parse(contents) == 1);
         contents = File.ReadAllText(@"C:\nanotest\Boalsburg Gas Company\Test Site 1\.meta");
         Assert.IsTrue(int.Parse(contents) == 2);
         contents = File.ReadAllText(@"C:\nanotest\Company 2\Test Plant 2\.meta");
         Assert.IsTrue(int.Parse(contents) == 3);
         contents = File.ReadAllText(@"C:\nanotest\Company 3\Test Site 3\Test Plant 3\Test Asset 3\123456\.meta");
         Assert.IsTrue(int.Parse(contents) == 5);

         // TODO: check that the timestamp directory exists

         // TODO: check that the xml file exists
      }

      [TestMethod]
      public void TestSaveLongNames()
      {
         List<ANanoSense> nanos = new List<ANanoSense>();

         // save C3,S,P,A,CP
         ANanoSense nano = this.C_S_P_A_CP_Long();
         nano.company = new ACompany() { name = "C23456789012345678901234567890123456789012345678901234567890" };
         nano.collectionPoint = new ACollectionPoint() { name = "CP3456789012345678901234567890" };
         nanos.Add(nano);

         string errorString;
         AFileManager.Instance.Save(nanos, out errorString);

         // check that the expected directory structure exists
         Assert.IsTrue(Directory.Exists(@"C:\nanotest\C23456789012345678901234567890123456789012345678\S23456789012345678901234\P23456789012345678901234\A23456789012345678901234\CP3456789012345678901234"));
      }

      [TestMethod]
      public void TestResave()
      {
         // save once to create the structure
         this.TestSave();

         this.Load();

         // save again to test saving with an existing structure
         this.TestSave();
      }

      [TestMethod]
      public void TestLoad()
      {
         this.TestSave();

         // re-load and check that the new structure exists
         this.Load();

         ADirNode rootNode = AFileManager.Instance.DirStructure.RootNode;
         Assert.IsTrue(3 == rootNode.Children.Count);
         Assert.IsTrue(rootNode.ChildExists(fCompany.name));
         ADirNode company3 = rootNode.Children[2];
         Assert.IsTrue("Company 3" == company3.Name);
         Assert.IsTrue(NodeLevel.Company == company3.Level);
         Assert.IsTrue(2 == company3.Children.Count);       // should have 2 sites
         ADirNode company3site1 = company3.Children[0];
         Assert.IsTrue("Test Site 3" == company3site1.Name);
         Assert.IsTrue(NodeLevel.Site == company3site1.Level);
         ADirNode company3site2 = company3.Children[1];
         Assert.IsTrue("Test Site 4" == company3site2.Name);
         Assert.IsTrue(NodeLevel.Site == company3site2.Level);
      }
   }
}

