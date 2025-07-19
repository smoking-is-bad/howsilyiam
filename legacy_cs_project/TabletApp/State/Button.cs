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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TabletApp.State
{
   /// <summary>
   /// A button definition for a given state, defining its name and
   /// transition state
   /// </summary>
   public class AButton : AStateObject
   {
      public AButton()
      {
         this.Force = false;
         this.TabStop = true;
      }

      [XmlAttribute("id")]
      public string Id { get; set; }
      [XmlAttribute("title")]
      public string Title { get; set; }
      [XmlAttribute("state")]
      public string State { get; set; }
      [XmlAttribute("action")]
      public string Action { get; set; }
      [XmlAttribute("position")]
      public int Position { get; set; }         // index of button, 1-based
      [XmlAttribute("image")]
      public string Image { get; set; }
      [XmlAttribute("force")]
      public bool Force { get; set; }           // should it force the state change, no matter what?
      [XmlAttribute("tabStop")]
      public bool TabStop { get; set; }         // override default tabStop setting
   }
}

