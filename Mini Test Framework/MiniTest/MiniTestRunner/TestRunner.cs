using MiniTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestRunner
{
    public static class TestRunner
    {
        // Runs all test classes found in the provided assembly
        public static void RunTestsInAssembly(Assembly assembly)
        {
            var testClasses = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null);

            foreach (var testClass in testClasses)
            {
                RunTestsInClass(testClass);
            }
        }

        // Runs all test methods in a given test class
        public static void RunTestsInClass(Type testClass)
        {
            // Check if the test class has a parameterless constructor
            if (testClass.GetConstructor(Type.EmptyTypes) == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Test class '{testClass.Name}' does not have a parameterless constructor.");
                Console.ResetColor();
                return;
            }

            var instance = Activator.CreateInstance(testClass);
            var beforeEach = testClass.GetMethods().FirstOrDefault(m => m.GetCustomAttribute<BeforeEachAttribute>() != null);
            var afterEach = testClass.GetMethods().FirstOrDefault(m => m.GetCustomAttribute<AfterEachAttribute>() != null);
            var testMethods = testClass.GetMethods()
                .Where(m => m.GetCustomAttribute<TestMethodAttribute>() != null)
                .OrderBy(m => m.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0)
                .ThenBy(m => m.Name);
            var description = testClass.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (!string.IsNullOrEmpty(description))
            {
                Console.WriteLine($"Description: {description}\n");
            }
            Console.WriteLine($"Running tests in {testClass.FullName}...\n");

            int totalTests = 0, passedTests = 0, failedTests = 0;

            foreach (var method in testMethods)
            {
                var dataRows = method.GetCustomAttributes<DataRowAttribute>();

                var methodDescription = method.GetCustomAttribute<DescriptionAttribute>()?.Description;

                if (!dataRows.Any())
                {
                    totalTests++;
                    if (TestExecutor.RunTest(instance, beforeEach, method, afterEach)) passedTests++;
                    else failedTests++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(method.Name);
                    foreach (var dataRow in dataRows)
                    {
                        totalTests++;
                        if (TestExecutor.RunParameterizedTest(instance, beforeEach, method, afterEach, dataRow))
                            passedTests++;
                        else
                            failedTests++;
                    }
                }
                if (!string.IsNullOrEmpty(methodDescription)) Console.WriteLine(methodDescription);
            }

            Console.WriteLine(new String('*', 30));
            Console.WriteLine($"* Test passed:    {passedTests,2} / {totalTests,-6}*");
            Console.WriteLine($"* Failed:          {failedTests}         *");
            Console.WriteLine(new String('*', 30));
            Console.WriteLine(new String('#', 93));
        }
    }
}
