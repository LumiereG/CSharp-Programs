using MiniTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestRunner
{
    public static class TestExecutor
    {
        // Runs a single test method
        public static bool RunTest(object instance, MethodInfo? beforeEach, MethodInfo testMethod, MethodInfo? afterEach)
        {
            string formatted = testMethod.Name.PadRight(83);

            try
            {
                beforeEach?.Invoke(instance, null);
                testMethod.Invoke(instance, null);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{formatted} : PASSED");
                return true;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is AssertionException assertionEx)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{formatted} : FAILED \n{assertionEx.Message}");
                    return false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{formatted} : FAILED \n{ex.InnerException?.Message ?? ex.Message} :");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{formatted} : FAILED \n{ex.InnerException?.Message ?? ex.Message} :");
                return false;
            }
            finally
            {
                Console.ResetColor();
                afterEach?.Invoke(instance, null);
            }
        }

        // Runs a parameterized test method
        public static bool RunParameterizedTest(object instance, MethodInfo? beforeEach, MethodInfo testMethod, MethodInfo? afterEach, DataRowAttribute dataRow)
        {
            var methodDescription = dataRow.Description;
            if (string.IsNullOrEmpty(methodDescription))
            {
                methodDescription = string.Join(", ", dataRow.Data.Select(p => p?.ToString() ?? "null"));
            }
            string formatted = methodDescription.PadRight(80);
            try
            {
                beforeEach?.Invoke(instance, null);
                testMethod.Invoke(instance, dataRow.Data);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" - {formatted} : PASSED");
                return true;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is AssertionException assertionEx)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" - {formatted} : FAILED \n{assertionEx.Message}");
                    return false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" - {formatted} : FAILED \n{ex.InnerException?.Message ?? ex.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" - {formatted} : FAILED \n{ex.InnerException?.Message}");
                return false;
            }
            finally
            {
                Console.ResetColor();
                afterEach?.Invoke(instance, null);
            }
        }
    }
}
