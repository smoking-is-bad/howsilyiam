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
using System.Diagnostics;

namespace TabletApp.Content
{
   /// <summary>
   /// User control for a generic message screen.  Message text is passed in as a state
   /// paramater.
   /// </summary>
   public partial class Message : BaseContent
   {
      public string MessageText
      {
         get
         {
            return this.messageLabel.Text;
         }

         set
         {
            this.messageLabel.Text = value;
         }
      }

      public Label MessageLabel
      {
         get
         {
            return this.messageLabel;
         }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public Message(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         Debug.Assert(fParams.ContainsKey("message"));
         this.messageLabel.Text = (string)fParams["message"];
      }
   }
}

