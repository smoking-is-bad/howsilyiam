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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TabletApp.State
{
   /// <summary>
   /// Represents an application state, defining the content for the main form,
   /// any parameters for that content, and any wizard buttons that need displayed.
   /// </summary>
   public class AState : AStateObject
   {
      [XmlAttribute("id")]
      public string Id { get; set; }
      [XmlAttribute("title")]
      public string Title { get; set; }
      [XmlAttribute("background")]
      public string Background { get; set; }
      [XmlAttribute("content")]
      public string Content { get; set; }
      [XmlAttribute("align")]
      public string Align { get; set; }
      [XmlElement("param")]
      public List<AParam> Params { get; set; }
      [XmlElement("button")]
      public List<AButton> Buttons { get; set; }
      [XmlElement("global")]
      public List<AParam> Globals { get; set; }
      [XmlAttribute("nextState")]
      public string NextState { get; set; }

      /// <summary>
      /// Get the key/value parameters as a Dictionary object
      /// </summary>
      [XmlIgnore]
      public Dictionary<string, string> ParamsAsDict
      {
         get 
         {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (AParam param in this.Params)
            {
               dict.Add(param.Name, param.ExpandedValue("Value"));
            }

            return dict;
         }
      }

      /// <summary>
      /// Runtime designation of this state's previous state for handling
      /// "back" logic.
      /// </summary>
      [XmlIgnore]
      public string PreviousStateId { get; set; }

      /// <summary>
      /// 
      /// </summary>
      [XmlIgnore]
      public object Data { get; set; }
   }
}

