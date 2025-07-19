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

using System.Windows.Forms;

namespace TabletApp.Views
{
   /// <summary>
   /// User control containing an "up" button and a "down" button.
   /// </summary>
   public partial class UpDownControl : UserControl
   {
      public Button UpButton { get { return this.upButton; } }
      public Button DownButton { get { return this.downButton; } }
      public NumberPad NumberPad { get { return this.numberPad; } }

      public UpDownControl()
      {
         InitializeComponent();
      }
   }
}

