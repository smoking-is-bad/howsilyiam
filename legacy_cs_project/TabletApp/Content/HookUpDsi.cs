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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.State;

namespace TabletApp.Content
{
   /// <summary>
   /// User control for the hookup screen
   /// </summary>
   public partial class HookUpDsi : BaseContent
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public HookUpDsi(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();
      }
   }
}

