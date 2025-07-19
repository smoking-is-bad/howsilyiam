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
using System.Security.Cryptography;


namespace DsiUtil
{
   public class Checksum
   {
      /// <summary>
      /// 16 bit cyclic redundancy checksum calculator. See Modbus over serial line v1.02.pdf
      /// appendix B.
      /// </summary>
      /// <param name="numBytes">The number of bytes in data to process</param>
      /// <returns>A 16-bit checksum</returns>
      /// 
#if false
      static public UInt16 Crc16(byte[] data, int numBytes)
      {
         Debug.Assert(numBytes > 0);
         const UInt16 kPoly = 0xA001;
         UInt16 crcRegister = 0xFFFF;

         foreach (byte b in data)
         {
            crcRegister ^= b;
            for (int i = 0; i < 8; ++i)
            {
               bool carryOver = (0x01 == (crcRegister & 0x01));
               crcRegister >>= 1;
               if (carryOver)
               {
                  crcRegister ^= kPoly;
               }
            }

            if (--numBytes <= 0)
            {
               break;
            }
         }
         return crcRegister;
      }

#else

      public static UInt16 Crc16(byte[] message, int length)
      {
         byte crcHi = 0xFF; /* high byte of CRC initialized */
         byte crcLo = 0xFF; /* low byte of CRC initialized */

         int i; /* will index into CRC lookup table */
         foreach (byte b in new ArraySegment<byte>(message, 0, length))
         {
            i = crcLo ^ b; /* calculate the CRC */
            crcLo = (byte)(crcHi ^ sCrcHiLookUp[i]);
            crcHi = sCrcLoLookUp[i];
         }

         return (UInt16)((crcHi << 8) | crcLo);
      }

      /* Table of CRC values for high */
      static byte[] sCrcHiLookUp = { 
       0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
       0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
       0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
       0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
       0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81,
       0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
       0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
       0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
       0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
       0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
       0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
       0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
       0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
       0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
       0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
       0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
       0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
       0x40 };

      static byte[] sCrcLoLookUp = { 
       0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4,
       0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
       0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD,
       0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
       0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7,
       0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
       0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE,
       0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
       0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2,
       0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
       0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB,
       0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
       0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91,
       0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
       0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88,
       0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
       0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80,
       0x40 };

#endif
   }

   public class AIdentity
   {
      public static string LoggedInUsername()
      {
         return System.Environment.GetEnvironmentVariable("USERNAME");
      }
   }

   public static class Tracing
   {
      public static void Print(this byte[] buffer, string prompt)
      {
         var bufferString = BitConverter.ToString(buffer, 0, Math.Min(16, buffer.Length));
         Console.Write("{0} |{1}|: {2}", prompt, buffer.Length, bufferString);
         if (buffer.Length > 16)
         {
            Console.WriteLine("...");
         }
         else
         {
            Console.WriteLine();
         }
      }
   }

   public static class ArrayUtil
   {
      public static string ToDelimitedString<T>(this IList<T> array, string separator)
      {
         int count = array.Count;
         var stringVals = new string[count];
         IEnumerator values = array.GetEnumerator();
         for (int i = 0; i < count && values.MoveNext(); ++i)
         {
            stringVals[i] = values.Current.ToString();
         }

         return String.Join(separator, stringVals);
      }


      public static void PopulateWithDelimitedString<T>(this IList<T> array, string value, string separator, Func<string, T> converter)
      {
         string[] stringVals = value.Split(new string[] { separator },
          StringSplitOptions.None);
         foreach (string stringVal in stringVals)
         {
            array.Add(converter(stringVal));
         }
      }
   }

   /// <summary>
   /// Code to encrypt and decrypt strings taken from MSDN AES documentation
   /// </summary>
   public static class ACrypto
   {
      public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] initVector)
      {
         // Check arguments. 
         if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
         if (key == null || key.Length <= 0)
            throw new ArgumentNullException("Key");
         if (initVector == null || initVector.Length <= 0)
            throw new ArgumentNullException("Key");

         byte[] encrypted;

         // Create an Aes object 
         // with the specified key and IV. 
         using (Aes aesAlg = Aes.Create())
         {
            aesAlg.Key = key;
            aesAlg.IV = initVector;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption. 
            using (MemoryStream msEncrypt = new MemoryStream())
            {
               using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
               {
                  using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                  {
                     //Write all data to the stream.
                     swEncrypt.Write(plainText);
                  }
                  encrypted = msEncrypt.ToArray();
               }
            }
         }

         // Return the encrypted bytes from the memory stream. 
         return encrypted;
      }

      public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] initVector)
      {
         // Check arguments. 
         if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
         if (key == null || key.Length <= 0)
            throw new ArgumentNullException("Key");
         if (initVector == null || initVector.Length <= 0)
            throw new ArgumentNullException("Key");

         // Declare the string used to hold 
         // the decrypted text. 
         string plaintext = null;

         // Create an Aes object 
         // with the specified key and IV. 
         using (Aes aesAlg = Aes.Create())
         {
            aesAlg.Key = key;
            aesAlg.IV = initVector;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption. 
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
               using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
               {
                  using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                  {

                     // Read the decrypted bytes from the decrypting stream
                     // and place them in a string.
                    plaintext = srDecrypt.ReadToEnd();
                  }
               }
            }

         }

         return plaintext;
      }

      /// <summary>
      /// Obviously simple SHA1 hash with fixed key and salt.
      /// </summary>
      public static string OneWayHash(string val)
      {
         string sSalt = "/\x05M\xa0\xda\xd0\x15\xab\xe4\r\x02h\xb8<\xdb\xa2";
         byte[] sKey = {
             59, 31, 113, 147, 31, 245, 22, 186,
             94, 71, 224, 13, 151, 78, 184, 10,
             166, 35, 240, 109, 119, 224, 87, 7,
             218, 158, 234, 174, 64, 186, 68, 170,
             218, 78, 161, 10, 93, 67, 232, 73,
             13, 82, 127, 230, 246, 111, 164, 119,
             148, 144, 112, 29, 241, 64, 199, 51,
             70, 249, 23, 33, 29, 63, 232, 133
            };
         var hasher = new HMACSHA1(sKey);
         var passwordBytes = System.Text.Encoding.UTF8.GetBytes(sSalt + val);
         var hash = System.Text.Encoding.UTF8.GetString(hasher.ComputeHash(passwordBytes));
         return hash;
      }
   }
}
