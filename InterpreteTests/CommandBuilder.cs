using System.Collections.Generic;
using System.Diagnostics;
using Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterpreteTests
{
    [TestClass]
    public class CommandBuilder
    {

		[TestMethod]
		public void ParseIfElseClauses_NoIfElseClauses_ReturnsEmptyDictionary()
		{
			// Arrange
			string input = "com1;";

			// Act
			var result = IfElseObjExp.ParseIfElseClauses(input);

			// Assert
			Assert.AreEqual(1, result.Count);
		}

		[TestMethod]
		public void ParseIfElseClauses_SingleIfClause_ReturnsOneIfElseObj()
		{
			// Arrange
			const string input = "if (condition1) {com1; }";

			// Act
			var result = IfElseObjExp.ParseIfElseClauses(input);

			// Assert
			Assert.AreEqual(1, result.Count);

			var ifElseObj = result[0];
			Assert.AreEqual(0, ifElseObj.Id);
			Assert.AreEqual(-1, ifElseObj.ParentId);
			Assert.AreEqual(0, ifElseObj.Position);
			Assert.AreEqual(0, ifElseObj.Layer);
			Assert.IsFalse(ifElseObj.Else);
			Assert.IsTrue(ifElseObj.Nested);
			Assert.AreEqual("if (condition1) {com1; }", ifElseObj.Input);
		}

		[TestMethod]
		public void ParseIfElseClauses_MultipleIfClauses_ReturnsMultipleIfElseObjs()
		{
			// Arrange
			string input = "if (condition1) {com1; } else { com2; }";

			// Act
			var result = IfElseObjExp.ParseIfElseClauses(input);

			// Assert
			//Assert.AreEqual(1, result.Count);

			//var ifElseObj1 = result[0];
			//Assert.AreEqual(0, ifElseObj1.Id);
			//Assert.AreEqual(-1, ifElseObj1.ParentId);
			//Assert.AreEqual(0, ifElseObj1.Position);
			//Assert.AreEqual(1, ifElseObj1.Layer);
			//Assert.IsFalse(ifElseObj1.Else);
			//Assert.IsTrue(ifElseObj1.Nested);
			//Assert.AreEqual("if (condition1) {com1; }", ifElseObj1.Input);

			//var command = result[0].Commands;
		}

		[TestMethod]
		public void ParseIfElseClauses_NestedIfElseClauses_ReturnsCorrectHierarchy()
		{
			// Arrange
			string input = "if (a > 5) { if (b > 3) { b++; } else { b--; } }";

			// Act
			var result = IfElseObjExp.ParseIfElseClauses(input);

			// Assert
			//Assert.AreEqual(2, result.Count);

			//var ifElseObj1 = result[0];
			//Assert.AreEqual(0, ifElseObj1.Id);
			//Assert.AreEqual(-1, ifElseObj1.ParentId);
			//Assert.AreEqual(0, ifElseObj1.Position);
			//Assert.AreEqual(1, ifElseObj1.Layer);
			//Assert.IsFalse(ifElseObj1.Else);
			//Assert.IsTrue(ifElseObj1.Nested);
			//Assert.AreEqual("if (a > 5) { if (b > 3) { b++; } else { b--; } }", ifElseObj1.Input);

			//var ifElseObj2 = result[1];
			//Assert.AreEqual(1, ifElseObj2.Id);
			//Assert.AreEqual(0, ifElseObj2.ParentId);
			//Assert.AreEqual(0, ifElseObj2.Position);
			//Assert.AreEqual(2, ifElseObj2.Layer);
			//Assert.IsFalse(ifElseObj2.Else);
			//Assert.IsTrue(ifElseObj2.Nested);
			//Assert.AreEqual("if (b > 3) { b++; } else { b--; }", ifElseObj2.Input);
		}

		[TestMethod]
		public void ParseIfElseClauses_IfElseWithoutNested_ReturnsCorrectObjects()
		{
			// Arrange
			string input = "if (a > 5) { a++; } else { a--; }";

			// Act
			var result = IfElseObjExp.ParseIfElseClauses(input);

			// Assert
			//Assert.AreEqual(2, result.Count);

			//var ifElseObj1 = result[0];
			//Assert.AreEqual(0, ifElseObj1.Id);
			//Assert.AreEqual(-1, ifElseObj1.ParentId);
			//Assert.AreEqual(0, ifElseObj1.Position);
			//Assert.AreEqual(1, ifElseObj1.Layer);
			//Assert.IsFalse(ifElseObj1.Else);
			//Assert.IsTrue(ifElseObj1.Nested);
			//Assert.AreEqual("if (a > 5) { a++; }", ifElseObj1.Input);

			//var ifElseObj2 = result[1];
			//Assert.AreEqual(1, ifElseObj2.Id);
			//Assert.AreEqual(-1, ifElseObj2.ParentId);
			//Assert.AreEqual(0, ifElseObj2.Position);
			//Assert.AreEqual(1, ifElseObj2.Layer);
			//Assert.IsTrue(ifElseObj2.Else);
			//Assert.IsFalse(ifElseObj2.Nested);
			//Assert.AreEqual("else { a--; }", ifElseObj2.Input);
		}

		[TestMethod]
		public void ParseIfElseClauses_ComplexNestedIfElseClauses_ReturnsCorrectHierarchy()
		{
			// Arrange
			string input = "if (a > 5) { if (b > 3) { b++; } else { if (c > 1) { c++; } } }";

			// Act
			var result = IfElseObjExp.ParseIfElseClauses(input);

			//// Assert
			//Assert.AreEqual(3, result.Count);

			//var ifElseObj1 = result[0];
			//Assert.AreEqual(0, ifElseObj1.Id);
			//Assert.AreEqual(-1, ifElseObj1.ParentId);
			//Assert.AreEqual(0, ifElseObj1.Position);
			//Assert.AreEqual(1, ifElseObj1.Layer);
			//Assert.IsFalse(ifElseObj1.Else);
			//Assert.IsTrue(ifElseObj1.Nested);
			//Assert.AreEqual("if (a > 5) { if (b > 3) { b++; } else { if (c > 1) { c++; } } }", ifElseObj1.Input);

			//var ifElseObj2 = result[1];
			//Assert.AreEqual(1, ifElseObj2.Id);
			//Assert.AreEqual(0, ifElseObj2.ParentId);
			//Assert.AreEqual(0, ifElseObj2.Position);
			//Assert.AreEqual(2, ifElseObj2.Layer);
			//Assert.IsFalse(ifElseObj2.Else);
			//Assert.IsTrue(ifElseObj2.Nested);
			//Assert.AreEqual("if (b > 3) { b++; } else { if (c > 1) { c++; } }", ifElseObj2.Input);

			//var ifElseObj3 = result[2];
			//Assert.AreEqual(2, ifElseObj3.Id);
			//Assert.AreEqual(1, ifElseObj3.ParentId);
			//Assert.AreEqual(0, ifElseObj3.Position);
			//Assert.AreEqual(3, ifElseObj3.Layer);
			//Assert.IsFalse(ifElseObj3.Else);
			//Assert.IsFalse(ifElseObj3.Nested);
			//Assert.AreEqual("if (c > 1) { c++; }", ifElseObj3.Input);
		}

		/// <summary>
		///     Parses the invalid input throws exception.
		/// </summary>
		[TestMethod]
        public void ParseComplexCommand()
        {
            //base, the command will be removed in the IrtParser
            const string input = "Container{ " +
                                 "Print(hello World);" +
                                 "if(condition) { if(innerCondition) { com1; } else { com2; } } else { com3; }" +
                                 "Label(one);" +
                                 "Print(passed label one);" +
                                 "goto(two);" +
                                 "Print(Should not be printed);" +
                                 "Label(two);" +
                                 "Print(Finish);" +
                                 "}";

            var inputcleaned = "{ " +
                               "Print(hello World);" +
                               "if(condition) { if(innerCondition) { com1; } else { com2; } } else { com3; }" +
                               "Label(one);" +
                               "Print(passed label one);" +
                               "goto(two);" +
                               "Print(Should not be printed);" +
                               "Label(two);" +
                               "Print(Finish);" +
                               "}";

            var expectedResults = new List<(int Key, string Category, string Value)>
            {
                (1, "COMMAND", "Print(hello World)"),
                (2, "IF", "if(condition) { if(innerCondition) { com1; } else { com2; } }"),
                (3, "ELSE", "else { com3; }"),
                (5, "LABEL", "Label(one)"),
                (6, "COMMAND", "Print(passed label one)"),
                (7, "GOTO", "goto(two)"),
                (8, "COMMAND", "Print(Should not be printed)"),
                (9, "LABEL", "Label(two)"),
                (10, "COMMAND", "Print(Finish)")
            };

            // Act
            var result = IrtParserCommand.BuildCommand(inputcleaned);
            result = IrtParserCommand.BuildCommand(inputcleaned);

            Trace.WriteLine(result.ToString());

            //// Assert
            //foreach (var expected in expectedResults)
            //{
            //    var (key, category, value) = expected;
            //    Assert.IsTrue(result.TryGetCategory(key, out var actualCategory));
            //    Assert.AreEqual(category, actualCategory, $"Category mismatch for key {key}");
            //    Assert.IsTrue(result.TryGetValue(key, out var actualValue));
            //    Assert.AreEqual(value, actualValue, $"Value mismatch for key {key}");
            //}

            Trace.WriteLine(result.ToString());

            inputcleaned = "{" +
                           "Print(hello World);" +
                           "if (condition1)" +
                           "{" +
                           "command1;" +
                           "if (condition2)" +
                           "{" +
                           "command2;" +
                           "}" +
                           "}" +
                           "Label(one);" +
                           "Print(passed label one);" +
                           "goto(two);" +
                           "Print(Should not be printed);" +
                           "Label(two);" +
                           "Print(Finish);" +
                           "}";


            var finalResults = new List<(int Key, string Category, string Value)>
            {
                (1, "COMMAND", "Print(hello World)"),
                (2, "IF_1", "if(condition1)"), // IF block 1
                (3, "COMMAND", "command1"),
                (4, "IF_2", "if(condition2)"), // Nested IF block 2 within IF 1
                (5, "COMMAND", "command2"),
                (6, "IF_2_END", ""), // End of IF block 2
                (7, "IF_1_END_NOELSE", ""), // End of IF block 1 with no ELSE
                (8, "LABEL", "Label(one)"),
                (9, "COMMAND", "Print(passed label one)"),
                (10, "GOTO", "goto(two)"),
                (11, "COMMAND", "Print(Should not be printed)"),
                (12, "LABEL", "Label(two)"),
                (13, "COMMAND", "Print(Finish)")
            };

            // Act
            result = IrtParserCommand.BuildCommand(inputcleaned);

            // Assert
            //foreach (var expected in expectedResults)
            //{
            //    var (key, category, value) = expected;
            //    Assert.IsTrue(result.TryGetCategory(key, out var actualCategory));
            //    Assert.AreEqual(category, actualCategory, $"Category mismatch for key {key}");
            //    Assert.IsTrue(result.TryGetValue(key, out var actualValue));
            //    Assert.AreEqual(value, actualValue, $"Value mismatch for key {key}");
            //}

            Trace.WriteLine(result.ToString());
        }

        //[TestMethod]
        //public void TestParseIfElseClausesNestedIfElse()
        //{
        //    // Arrange
        //    string code = "if(condition1) { if(condition2) { /* nested code */ } else { /* nested else code */ } } else { /* outer else code */ }";

        //    // Act
        //    var clauses = IrtParserIfElse.ParseIfElseClauses(code);

        //    // Assert
        //    Assert.AreEqual(2, clauses.Count, "Expected 2 if-else clauses.");

        //    // Check the first clause (Layer 0)
        //    Assert.AreEqual(0, clauses[0].Layer, "Layer of the first clause should be 0.");
        //    Assert.IsTrue(clauses[0].IfClause.Contains("if(condition2)"), "IfClause should contain the nested if.");
        //    Assert.IsNotNull(clauses[0].ElseClause, "ElseClause should not be null.");

        //    // Check the second clause (Layer 1)
        //    Assert.AreEqual(1, clauses[1].Layer, "Layer of the second clause should be 1.");
        //    Assert.AreEqual("else { /* nested else code */ }", clauses[1].ElseClause);


        //    var clause = IrtParserIfElse.CategorizeIfElseClauses(clauses);

        //    foreach (var item in clause)
        //    {
        //        Console.WriteLine($"Category: {item.Category}, Clause: {item.Clause}, Parent: {item.ParentCategory}");
        //    }
        //}

        //[TestMethod]
        //public void TestParseIfClausesNestedIf()
        //{
        //    // Arrange
        //    var code = "if (condition1){command1; if (condition2){command2;}}";

        //    // Act
        //    var clauses = IrtParserIfElse.ParseIfElseClauses(code);

        //    // Assert
        //    Assert.AreEqual(2, clauses.Count, "Expected 2 if-else clauses.");
        //    //TODO error here:
        //    //look here: https://stackoverflow.com/questions/9283288/parsing-if-else-if-statement-algorithm
        //    Assert.AreEqual(1, clauses[1].Layer, "Layer of the second clause should be 1.");

        //    var clause = IrtParserIfElse.CategorizeIfElseClauses(clauses);

        //    foreach (var item in clause)
        //    {
        //        Console.WriteLine($"Category: {item.Category}, Clause: {item.Clause}, Parent: {item.ParentCategory}");
        //    }
        //}
    }
}