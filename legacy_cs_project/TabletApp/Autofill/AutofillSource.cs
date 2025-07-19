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
using System.Linq;
using System.Xml.Serialization;

using Model;

namespace TabletApp.Autofill
{
   /**
    * Manages autofill information for company and probe data. 
    * 
    * Usage:
    *     var source = new AAutofillSource();
    *     source.Initialize(companyPath, probePath);
    * 
    * Company and probe data is stored in two separate XML files. Neither company nor probe data
    * must be intialized though Initialize(), if, for example, you want to start new autofill
    * files or just need a source of autofill data for one or the other. However, it is assumed 
    * that probe data will not change.
    * 
    * Changes via AddNNNN(...) are immediately saved unless the ImmediateWrites property is false.
    */
   public class AAutofillSource
   {
      public AAutofillSource()
      {
         this.ImmediateWrites = true;

         fCompanySource = new ACompanyAutofillSource();
         fProbeSource = new AProbeAutofillSource();
      }


      public void Initialize(String companyPath, String probePath)
      {
         this.ReadCompanyData(companyPath);
         this.ReadProbeData(probePath);
      }

      /** When true, any adds are written immediately. Defaults to true. */
      public bool ImmediateWrites { get; set; }


      public void WriteCompanyData(String companyPath)
      {
         using (var xmlStream = new FileStream(companyPath, FileMode.OpenOrCreate, FileAccess.Write))
         {
            var serializer = new XmlSerializer(typeof(ACompanyAutofillSource));
            serializer.Serialize(xmlStream, fCompanySource);
         }
      }


      public IList<AProbe> GetProbeList()
      {
         return fProbeSource.fProbes;
      }


      public IList<ACompany> GetCompanyList()
      {
         return fCompanySource.fCompanies;
      }


      public IList<string> GetCompanySites(ACompany company)
      {
         return this.FilterPlaces(company, AAutofillSource.kSiteType);
      }


      public IList<string> GetCompanyPlants(ACompany company)
      {
         return this.FilterPlaces(company, AAutofillSource.kPlantType);
      }


      public IList<string> GetCompanyAssets(ACompany company)
      {
         return this.FilterPlaces(company, AAutofillSource.kAssetType);
      }


      public bool AddCompany(ACompany company)
      {
         bool add = !fCompanySource.fCompanies.Exists((c) => c.name == company.name);
         if (add)
         {
            fCompanySource.fCompanies.Add(company);
            var cp = new AAutofillCompanyPlaces();
            cp.companyName = company.name;
            cp.companyId = company.id;

            fCompanySource.fPlaces.Add(cp);

            if (this.ImmediateWrites)
            {
               this.WriteCompanyData(fCompanyPath);
            }
         }
         return add;
      }


      public bool AddAsset(string assetName, ACompany company)
      {
         return this.AddPlace(assetName, AAutofillSource.kAssetType, company);
      }


      public bool AddPlant(string plantName, ACompany company)
      {
         return this.AddPlace(plantName, AAutofillSource.kPlantType, company);
      }


      public bool AddSite(string siteName, ACompany company)
      {
         return this.AddPlace(siteName, AAutofillSource.kSiteType, company);
      }


      private bool AddPlace(string name, string type, ACompany company)
      {
         var companyPlaces = fCompanySource.fPlaces.Find((cp) => cp.IsInCompany(company));
         if (null == companyPlaces)
         {
            // Company does not exist, add it
            companyPlaces = new AAutofillCompanyPlaces() { companyId = company.id, companyName = company.name };
            fCompanySource.fPlaces.Add(companyPlaces);
         }

         bool add = !companyPlaces.fPlaces.Exists((p) => ((p.type == type) && (p.name == name)));

         if (add)
         {
            companyPlaces.fPlaces.Add(new AAutofillPlace() { name = name, type = type });

            if (this.ImmediateWrites)
            {
               this.WriteCompanyData(fCompanyPath);
            }
         }

         return add;
      }


      private void ReadProbeData(String probePath)
      {
         if (File.Exists(probePath))
         {
            using (var xmlStream = new FileStream(probePath, FileMode.Open, FileAccess.Read))
            {
               var serializer = new XmlSerializer(typeof(AProbeAutofillSource));

               fProbeSource = (AProbeAutofillSource)serializer.Deserialize(xmlStream);
            }
         }
         fProbePath = probePath;
      }


      private void ReadCompanyData(String companyPath)
      {
         if (File.Exists(companyPath))
         {
            using (var xmlStream = new FileStream(companyPath, FileMode.Open, FileAccess.Read))
            {
               var serializer = new XmlSerializer(typeof(ACompanyAutofillSource));

               fCompanySource = (ACompanyAutofillSource)serializer.Deserialize(xmlStream);
            }
         }
         fCompanyPath = companyPath;
      }


      private IList<string> FilterPlaces(ACompany company, string type)
      {
         var companyPlaces = fCompanySource.fPlaces.Where(cp => cp.IsInCompany(company));

         if (companyPlaces.Count() > 0)
         {
            // There should only be one list for each company.
            Debug.Assert(1 == companyPlaces.Count());

            var names = companyPlaces.Last().fPlaces.Where(p => (p.type == type)).
             Select(p => p.name);

            return names.ToList();
         }

         return new List<string>();
      }


      private ACompanyAutofillSource fCompanySource;
      private AProbeAutofillSource fProbeSource;

      private String fCompanyPath;
      private String fProbePath;

      private const String kSiteType = "site";
      private const String kPlantType = "plant";
      private const String kAssetType = "asset";
   };


   [XmlRoot("autofill")]
   public class ACompanyAutofillSource
   {
      public ACompanyAutofillSource()
      {
         fCompanies = new List<ACompany>();
         fPlaces = new List<AAutofillCompanyPlaces>();
      }

      [XmlArray("companies"), XmlArrayItem("company")]
      public List<ACompany> fCompanies;

      [XmlArray("autofillplaces"), XmlArrayItem("companyPlaces")]
      public List<AAutofillCompanyPlaces> fPlaces;
   }


   [XmlRoot("companyPlaces")]
   public class AAutofillCompanyPlaces
   {
      public AAutofillCompanyPlaces()
      {
         fPlaces = new List<AAutofillPlace>();
      }


      public bool IsInCompany(ACompany company)
      {
         return ((company.name == companyName) || (null != company.id && (company.id == companyId)));
      }


      [XmlAttribute]
      public string companyId;
      [XmlAttribute]
      public string companyName;

      [XmlArray("places"), XmlArrayItem("place")]
      public List<AAutofillPlace> fPlaces; // sites, assets and plants
   }


   [XmlRoot("place")]
   public class AAutofillPlace
   {
      [XmlAttribute]
      public string type;

      [XmlText]
      public string name;
   }


   [XmlRoot("autofill")]
   public class AProbeAutofillSource
   {
      [XmlArray("probes"), XmlArrayItem("probe")]
      public AProbe[] fProbes;
   }

}

