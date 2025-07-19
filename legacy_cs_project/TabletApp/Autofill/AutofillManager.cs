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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabletApp.Properties;
using TabletApp.Utils;

namespace TabletApp.Autofill
{
   /// <summary>
   /// Singleton for handling interaction with the auto-fill source.  Manage a single auto-fill source for the whole app.
   /// </summary>
   public class AAutofillManager : ASingleton<AAutofillManager>
   {
      public AAutofillSource Source { get; private set; }

      /// <summary>
      /// Initialize the auto-fill source.
      /// </summary>
      public void Initialize()
      {
         this.Source = new AAutofillSource();
         string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Resources.RootDataFolderName);
         Directory.CreateDirectory(basePath);
         string companyPath = Path.Combine(basePath, Resources.AutoFillCompanyFilename);
         string probePath = Path.Combine(basePath, Resources.AutoFillProbeFilename);
         this.Source.Initialize(companyPath, probePath);
      }
   }
}

