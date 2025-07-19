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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabletApp.State;
using System.Collections.Generic;

namespace TableAppTest
{
   [TestClass]
   public class StateTest
   {
      [TestInitialize]
      public void Initialize()
      {
         AStateController.Instance.Initialize();
      }

      [TestMethod]
      public void TestInstance()
      {
         Assert.IsNotNull(AStateController.Instance);
      }

      [TestMethod]
      public void TestStateDef()
      {
         Assert.IsNotNull(AStateController.Instance.StateDef);
      }

      [TestMethod]
      public void TestStates()
      {
         List<AState> states = AStateController.Instance.StateDef.States;
         bool hasHome = false;

         foreach (AState state in states)
         {
            if (state.Id == "home")
            {
               hasHome = true;
            }
            Assert.IsNotNull(state.Id);
            Assert.IsTrue(state.Id.Length > 0);
            //Assert.IsNotNull(state.Title);
            //Assert.IsTrue(state.Title.Length > 0);
            //Assert.IsNotNull(state.Content);
            //Assert.IsTrue(state.Content.Length > 0);
         }
         Assert.IsTrue(hasHome);
      }

      [TestMethod]
      public void TestStateButtons()
      {
         List<AState> states = AStateController.Instance.StateDef.States;

         foreach (AState state in states)
         {
            if (null != state.Buttons)
            {
               foreach (AButton button in state.Buttons)
               {
                  Assert.IsNotNull(button.Id);
                  Assert.IsTrue(button.Id.Length > 0);
                  Assert.IsNotNull(button.Title);
                  Assert.IsTrue(button.Title.Length > 0);
                  Assert.IsTrue(null != button.State || null != button.Action);
               }
            }
         }
      }

      [TestMethod]
      public void TestStateParams()
      {
         List<AState> states = AStateController.Instance.StateDef.States;

         foreach (AState state in states)
         {
            if (null != state.Params)
            {
               foreach (AParam param in state.Params)
               {
                  Assert.IsNotNull(param.Name);
                  Assert.IsTrue(param.Name.Length > 0);
                  Assert.IsNotNull(param.Value);
                  Assert.IsTrue(param.Value.Length > 0);
               }
            }
         }
      }

      [TestMethod]
      public void TestStateRetrieval()
      {
         AState state = AStateController.Instance.StateWithId("home");
         Assert.IsNotNull(state);
         Assert.IsTrue(state.Id == "home");
      }

      [TestMethod]
      public void TestStateChange()
      {
         AStateController.Instance.ChangeToState("review");
         AState state = AStateController.Instance.CurrentState;
         Assert.IsNotNull(state);
         Assert.IsTrue(state.Id == "review");
      }

      [TestMethod]
      public void TestStateChangeBack()
      {
         AStateController.Instance.ChangeToState("home");
         AStateController.Instance.ChangeToState("review");
         AStateController.Instance.ChangeToStateForControlId("back");
         AState state = AStateController.Instance.CurrentState;
         Assert.IsNotNull(state);
         Assert.IsTrue(state.Id == "home");
      }
   }
}

