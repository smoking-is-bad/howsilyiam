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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MockData;
using Model;
using TabletApp.Autofill;

namespace TableAppTest
{
   [TestClass]
   public class AutofillTests
   {
      [TestMethod]
      public void TestReadCompanyAutofillSource()
      {
         var source = new AAutofillSource();

         string path = "..\\..\\..\\TabletApp\\Autofill\\AutofillCompany.xml";
         Assert.IsTrue(File.Exists(path));
         source.Initialize(path, "");

         IList<ACompany> companies = source.GetCompanyList();
         Assert.IsNotNull(companies);
         Assert.IsTrue(companies.Count > 0);

         foreach (ACompany c in companies)
         {
            Debug.WriteLine("{0}", c.name);
            Assert.IsNotNull(c.name);

            var siteNames = source.GetCompanySites(c);
            Assert.IsNotNull(siteNames);
            Assert.IsTrue(siteNames.Count != 0);
            foreach (var name in siteNames)
            {
               Debug.WriteLine(name);
               Assert.IsNotNull(name);
            }
         }
      }

      [TestMethod]
      public void TestAddCompanyAutofillSource()
      {
         // Not calling Initialize and turning off immediate writes so there is no file
         // interactions.
         var source = new AAutofillSource();
         source.ImmediateWrites = false;

         var mock = new AMockData();

         // Adding the same company shouldn't return true.
         ACompany mockCompany = mock.Company();
         Assert.IsTrue(source.AddCompany(mockCompany));
         Assert.IsTrue(source.GetCompanyList().Count == 1);
         Assert.IsFalse(source.AddCompany(mockCompany));
         Assert.IsTrue(source.GetCompanyList().Count == 1);

         // Adding a new company should succeed.
         var mockCompany2 = new ACompany()
         {
            name = "A New Company"
         };

         Assert.IsTrue(source.AddCompany(mockCompany2));
         Assert.IsTrue(source.GetCompanyList().Count == 2);
         Assert.IsFalse(source.AddCompany(mockCompany2));
         Assert.IsTrue(source.GetCompanyList().Count == 2);
      }

      [TestMethod]
      public void TestAddPlacesAutofillSource()
      {
         // Not calling Initialize and turning off immediate writes so there is no file
         // interactions.
         var source = new AAutofillSource();
         source.ImmediateWrites = false;

         var mock = new AMockData();

         // Adding the same company shouldn't return true.
         ACompany mockCompany = mock.Company();
         Assert.IsTrue(source.AddCompany(mockCompany));

         String name =  "A New Place";

         this.AddPlacesTest(source, "A New Place", mockCompany);

         // Company lists are independent. Adding to one should not affect another.
         ACompany mockCompany2 = mock.Company();
         mockCompany2.id = null;
         mockCompany2.name = "MockTestCompany2";
         Assert.IsTrue(source.AddCompany(mockCompany2));

         name = "A New Place";
         this.AddPlacesTest(source, "A New Place", mockCompany2);

         Assert.IsTrue(source.GetCompanySites(mockCompany).Count == 1);
         Assert.IsTrue(source.GetCompanyPlants(mockCompany).Count == 1);
         Assert.IsTrue(source.GetCompanyAssets(mockCompany).Count == 1);

         // Adding a new place to each should work.
         name = "AnotherName";
         Assert.IsTrue(source.AddSite(name, mockCompany));
         Assert.IsTrue(source.AddPlant(name, mockCompany));
         Assert.IsTrue(source.AddAsset(name, mockCompany));

         Assert.IsTrue(source.GetCompanySites(mockCompany).Count == 2);
         Assert.IsTrue(source.GetCompanyPlants(mockCompany).Count == 2);
         Assert.IsTrue(source.GetCompanyAssets(mockCompany).Count == 2);

      }

      // Test adding to source one of each company's site, plant and asset with the given name.
      void AddPlacesTest(AAutofillSource source, String name, ACompany mockCompany)
      {
         // Names for sites, plants and assets are independent.
         // Adding one type should leave others as is.
         Assert.IsTrue(source.GetCompanySites(mockCompany).Count == 0);
         Assert.IsTrue(source.GetCompanyPlants(mockCompany).Count == 0);
         Assert.IsTrue(source.GetCompanyAssets(mockCompany).Count == 0);

         Assert.IsTrue(source.AddSite(name, mockCompany));
         Assert.IsFalse(source.AddSite(name, mockCompany));

         Assert.IsTrue(source.GetCompanySites(mockCompany).Count == 1);
         Assert.IsTrue(source.GetCompanyPlants(mockCompany).Count == 0);
         Assert.IsTrue(source.GetCompanyAssets(mockCompany).Count == 0);

         Assert.IsTrue(source.AddPlant(name, mockCompany));
         Assert.IsFalse(source.AddPlant(name, mockCompany));

         Assert.IsTrue(source.GetCompanySites(mockCompany).Count == 1);
         Assert.IsTrue(source.GetCompanyPlants(mockCompany).Count == 1);
         Assert.IsTrue(source.GetCompanyAssets(mockCompany).Count == 0);

         Assert.IsTrue(source.AddAsset(name, mockCompany));
         Assert.IsFalse(source.AddAsset(name, mockCompany));

         Assert.IsTrue(source.GetCompanySites(mockCompany).Count == 1);
         Assert.IsTrue(source.GetCompanyPlants(mockCompany).Count == 1);
         Assert.IsTrue(source.GetCompanyAssets(mockCompany).Count == 1);
      }

      [TestMethod]
      public void TestReadProbeAutofillSource()
      {
         var source = new AAutofillSource();

         string path = "..\\..\\..\\TabletApp\\Autofill\\AutofillProbe.xml";
         Assert.IsTrue(File.Exists(path));
         source.Initialize(companyPath: "", probePath: path);

         IList<AProbe> probes = source.GetProbeList();
         Assert.IsNotNull(probes);
         Assert.IsTrue(probes.Count > 0);
      }
   }
}

