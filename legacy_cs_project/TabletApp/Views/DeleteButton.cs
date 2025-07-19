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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Properties;

namespace TabletApp.Views
{
   /// <summary>
   /// Custom button that shows an X representing a delete function
   /// </summary>
   class DeleteButton : Button
   {
      /// <summary>
      /// Constructor
      /// </summary>
      public DeleteButton()
      {
         this.Image = Resources.delete_file;
         this.Anchor = AnchorStyles.Bottom;
         this.FlatStyle = FlatStyle.Flat;
         this.FlatAppearance.BorderSize = 0;
         this.FlatAppearance.MouseDownBackColor = Color.Transparent;
         this.FlatAppearance.MouseOverBackColor = Color.Transparent;
         this.BackColor = Color.Transparent;
      }
   }
}

