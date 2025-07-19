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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using TabletApp.Utils;

using Logging;

namespace TabletApp.State
{
   public delegate void StateChange(AState newState);
   public delegate void StateWillChange(AState newState, string actionName, CancelEventArgs args);
   public delegate void PerformAction(string actionName, object data);

 
   /// <summary>
   /// Controls the application flow via state definitions.
   /// </summary>
   public class AStateController : ASingleton<AStateController>
   {
      // maintain our current and previous state
      private AState fCurrentState;
      public AState CurrentState { get { return fCurrentState; } }

      // event for getting state change notifications
      public event StateChange StateChangeEvent;
      public event StateWillChange StateWillChangeEvent;

      // event for performing an action within the current state
      public event PerformAction PerformActionEvent;

      // the state definition object
      public AStateDef StateDef { get; set; }

      /// <summary>
      /// Data left behind by the previous state.  This is stored on a per state basis, so is persistent once set
      /// by that state.
      /// </summary>
      public object PreviousStateData
      {
         get
         {
            object data = null;
            AState prevState = this.StateWithId(fCurrentState.PreviousStateId);

            if (null != prevState)
            {
               data = prevState.Data;
            }

            return data;
         }
      }

      /// <summary>
      /// Global state data available to all states once set.
      /// </summary>
      public Dictionary<string, object> GlobalData { get; set; }

      /// <summary>
      /// Initialize by reading in the state definition from the default state XML file
      /// </summary>
      public void Initialize()
      {
         try
         {
            XmlSerializer deserializer = new XmlSerializer(typeof(AStateDef));
            TextReader reader = new StreamReader(@"State\StateDef.xml");
            this.StateDef = (AStateDef)deserializer.Deserialize(reader);
            reader.Close();
         }
         catch (Exception e)
         {
            ALog.Warning("State", "Exception while reading in state definition; {0}.", e.Message);
            throw e;
         }

         this.GlobalData = new Dictionary<string, object>();
      }

      /// <summary>
      /// Post event about a potential state change, giving receivers a chance to cancel it.
      /// </summary>
      /// <param name="newState">The new state</param>
      /// <param name="actionName">The action associated with the state change, if any</param>
      /// <returns>true if listener canceled</returns>
      private bool PostStateWillChangeEvent(AState newState, string actionName)
      {
         bool cancel = false;

         if (null != this.StateWillChangeEvent)
         {
            CancelEventArgs cancelArgs = new CancelEventArgs();
            this.StateWillChangeEvent(newState, actionName, cancelArgs);
            cancel = cancelArgs.Cancel;
         }

         return cancel;
      }

      /// <summary>
      /// Send out the state change event to any listening clients.
      /// </summary>
      /// <param name="newState">The new AState object</param>
      private void PostStateChangeEvent(AState newState)
      {
         if (null != this.StateChangeEvent)
         {
            this.StateChangeEvent(newState);
         }
      }

      /// <summary>
      /// Read in and assign the global vars from the state
      /// </summary>
      /// <param name="state"></param>
      private void StoreGlobals(AState state)
      {
         foreach (AParam global in state.Globals)
         {
            this.GlobalData[global.Name] = global.ExpandedValue("Value");
         }
      }

      /// <summary>
      /// Send out the perform action event to any listening clients.
      /// </summary>
      /// <param name="actionName">The name of the action</param>
      /// <param name="data">Data for the event</param>
      public void PostPerformActionEvent(string actionName, object data)
      {
         if (null != this.PerformActionEvent)
         {
            this.PerformActionEvent(actionName, data);
         }
      }

      /// <summary>
      /// Get the AState object with the given id.
      /// </summary>
      /// <param name="stateId">The state id</param>
      /// <returns>AState object</returns>
      public AState StateWithId(string stateId)
      {
         AState theState = null;

         foreach (AState state in this.StateDef.States)
         {
            if (state.Id == stateId)
            {
               theState = state;
            }
         }

         return theState;
      }

      /// <summary>
      /// Change to the previous state (eg back button).
      /// </summary>
      public void ChangeToPreviousState()
      {
         if (null != fCurrentState.PreviousStateId)
         {
            this.ChangeToState(fCurrentState.PreviousStateId, true);
         }
      }

      /// <summary>
      /// Change to the state with the given id.
      /// </summary>
      /// <param name="stateId">The state id</param>
      /// <param name="isPrevious">Are we going back to the previous state?</param>
      /// <param name="sendWillChange">Should we send the "will change" event?</param>
      /// <returns>True if the state actually changed</returns>
      public bool ChangeToState(string stateId, bool isPrevious=false, bool sendWillChange=true, bool forceChange=false)
      {
         AState newState = this.StateWithId(stateId);
         if (null != newState)
         {
            if (!forceChange && null != fCurrentState && newState.Id == fCurrentState.Id)
            {
               // already at that state
               return false;
            }
            if (sendWillChange)
            {
               if (this.PostStateWillChangeEvent(newState, null))
               {
                  // state change was canceled - return
                  return false;
               }
            }
            string prevStateId = null;
            if (null != fCurrentState)
            {
               prevStateId = fCurrentState.Id;
            }
            fCurrentState = newState;
            if (!isPrevious)
            {
               fCurrentState.PreviousStateId = prevStateId;
            }
            this.StoreGlobals(newState);
            this.PostStateChangeEvent(newState);

            // check if this is a "faceless" state, for which the "nextState" attribute will be set
            // and we continue immediately to that state
            if (null == newState.Content && null != newState.NextState)
            {
               this.ChangeToState(newState.NextState);
            }
         }

         return true;
      }

      /// <summary>
      /// Change to the state associated with the given control id.
      /// </summary>
      /// <param name="controlId">The id of the control (button) in the current state</param>
      public void ChangeToStateForControlId(string controlId)
      {
         string newStateId = null;
         string actionId = null;
         AButton theButton = null;

         // find the button definition within the current state
         foreach (AButton button in fCurrentState.Buttons)
         {
            if (button.Id == controlId)
            {
               if (null != button.State)
               {
                  newStateId = button.ExpandedValue("State");
               }
               if (null != button.Action)
               {
                  actionId = button.Action;
               }
               theButton = button;
            }
         }

         // if found, perform the action
         if (null != actionId)
         {
            this.PerformActionEvent(actionId, null);
         }
         if (null != newStateId)
         {
            // special-case the back button
            if ("back" == newStateId)
            {
               this.ChangeToPreviousState();
            }
            // give listeners a chance to cancel out of the state change
            else if (!this.PostStateWillChangeEvent(this.StateWithId(newStateId), actionId))
            {
               // if found, navigate to that corresponding state
               this.ChangeToState(newStateId, false, false, theButton.Force);
            }
         }
      }
   }
}

