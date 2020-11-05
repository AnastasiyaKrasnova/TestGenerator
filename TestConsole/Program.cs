using System;
using TestGeneratorDll;
namespace TestConsole
{
    class Program
    {
        public static string sourceCode = @"using System;
							using System.Collections.Generic;
							using System.Reflection;
							using System.Linq;

							namespace Faker
							{
								class FakeCreator : ICreator
								{
									private object CreateInstance(Type type)
									{
									}

									public object TestConstructor(ConstructorInfo constructor)
									{
									}

									public ConstructorInfo[] ChooseConstructor(ConstructorInfo[] constr)
									{
									}

									public void SetPropertiesOutConstructor(object obj, Type type)
									{
									}
			
									public override void SetFieldsOutConstructor(object obj, Type type)
									{
									}

									public static object GetDefaultValue(Type t)
									{
									}
								
								}
								class Huilo : ICreator
								{
									private object CreateHuilo(Type type)
									{
									}

									public object TestHuilo(ConstructorInfo constructor)
									{
									}

									public object ChooseHuilo(ConstructorInfo[] constr)
									{
									}

									public void SetPropertiesOutHuilo(object obj, Type type)
									{
									}
			
									public override void SetFieldsOutHuilo(object obj, Type type)
									{
									}

									public static object GetDefaultHuilo(Type t)
									{
									}
									
								}
							}";
        static void Main(string[] args)
        {
            TestGenerator testGenerator = new TestGenerator();
            var units=testGenerator.Generate(sourceCode);
			if (units != null)
            {
				foreach (var unit in units)
				{
					if (unit != null)
					{
						Console.WriteLine(unit.ToFullString());
					}
					Console.WriteLine("-----------------------------------");
				}
			}
			
        }
    }
}
