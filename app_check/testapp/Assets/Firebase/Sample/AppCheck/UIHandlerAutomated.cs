// Copyright 2023 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Firebase.Sample.AppCheck {
  using Firebase;
  using Firebase.Extensions;
  using Firebase.AppCheck;
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;
  using UnityEngine;

  // An automated version of the UIHandler that runs tests on Firebase AppCheck.
  public class UIHandlerAutomated : UIHandler {
    // Delegate which validates a completed task.
    delegate Task TaskValidationDelegate(Task task);

    private Firebase.Sample.AutomatedTestRunner testRunner;

    protected override void Start() {
      // Set the list of tests to run, note this is done at Start since they are
      // non-static.
      Func<Task>[] tests = {
        // Add tests here
        TestDummyTest
      };

      testRunner = AutomatedTestRunner.CreateTestRunner(
        testsToRun: tests,
        logFunc: DebugLog
      );

      Debug.Log("NOTE: Some API calls report failures using UnityEngine.Debug.LogError which will " +
                "pause execution in the editor when 'Error Pause' in the console window is " +
                "enabled.  `Error Pause` should be disabled to execute this test.");

      UIEnabled = true;
      // Do not call base.Start(), as we don't want to initialize Firebase (and instead do it in the tests).
    }

    // Passes along the update call to automated test runner.
    protected override void Update() {
      base.Update();
      if (testRunner != null) {
        testRunner.Update();
      }
    }

    // Throw when condition is false.
    private void Assert(string message, bool condition) {
      if (!condition)
        throw new Exception(String.Format("Assertion failed ({0}): {1}",
                                          testRunner.CurrentTestDescription, message));
    }

    // Throw when value1 != value2.
    private void AssertEq<T>(string message, T value1, T value2) {
      if (!(object.Equals(value1, value2))) {
        throw new Exception(String.Format("Assertion failed ({0}): {1} != {2} ({3})",
                                          testRunner.CurrentTestDescription, value1, value2,
                                          message));
      }
    }

    // Placeholder dummy test
    Task TestDummyTest() {
      return Task.Run(() => {
        DebugLog("Ran the dummy test");
      });
    }
  }
}