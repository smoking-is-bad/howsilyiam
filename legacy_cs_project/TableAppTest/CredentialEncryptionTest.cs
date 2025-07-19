using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Model;

namespace TableAppTest
{
   /// <summary>
   /// Summary description for CredentialEncryptionTest
   /// </summary>
   [TestClass]
   public class ACredentialEncryptionTest
   {
      public ACredentialEncryptionTest()
      {
         
      }


      [TestMethod]
      public void TestTwoWayEncryption()
      {
         ADsiInfo dsiInfo = new ADsiInfo();

         string url = "http://www.example.com/nanosensecloud";
         string username = "joecoder";
         string password = "agreatpasswordthatcanneverbesniffed";

         dsiInfo.cloudAppUrl = url;
         dsiInfo.cloudAppUserName = username;
         dsiInfo.cloudAppPassword = password;

         string encrypted = dsiInfo.Account;
         Assert.IsNotNull(encrypted);
         Assert.IsTrue(encrypted.Length > 0);

         // Zero the encrypted fields to test decryption.
         dsiInfo.cloudAppUrl = "";
         dsiInfo.cloudAppUserName = "";
         dsiInfo.cloudAppPassword = "";

         // Test round-trip
         dsiInfo.Account = encrypted;
         Assert.AreEqual(dsiInfo.cloudAppUrl, url);
         Assert.AreEqual(dsiInfo.cloudAppUserName, username);
         Assert.AreEqual(dsiInfo.cloudAppPassword, password);
      }
   }
}
