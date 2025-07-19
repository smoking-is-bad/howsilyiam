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
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using TabletApp.Api.DsiApi;

namespace Model
{
   [XmlRoot("company"), StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
   public partial class ACompany
   {
      public ACompany()
      {
         id = "";
         name = "";
         location = new APostalAddress();
         phone = "";
      }

      public AUtf8String32 id;
      public AUtf8String64 name;
      public APostalAddress location;

      [Phone, StringLength(32)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string phone;
   }
}

