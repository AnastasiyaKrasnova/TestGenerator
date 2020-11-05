﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestGeneratorDll
{
    public class TestGenerator
    {
        private AttributeSyntax TestSetupAttr = SyntaxFactory.Attribute(SyntaxFactory.ParseName("ClassInitialize"));
        private AttributeSyntax TestMethodAttr = SyntaxFactory.Attribute(SyntaxFactory.ParseName("TestMethod"));
        private AttributeSyntax TestClassAttr = SyntaxFactory.Attribute(SyntaxFactory.ParseName("TestClass"));

        public List<CompilationUnitSyntax> Generate(string sourceCode)
        {
            try
            {
                CompilationUnitSyntax sourceRoot = CSharpSyntaxTree.ParseText(sourceCode).GetCompilationUnitRoot();
                if (sourceRoot.Members.Count == 0)
                {
                    return null;
                }

                List<ClassDeclarationSyntax> classDeclarations = sourceRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
                List<CompilationUnitSyntax> compilationUnits = new List<CompilationUnitSyntax>();
                foreach (var classDeclaration in classDeclarations)
                {
                    CompilationUnitSyntax result = GenerateCompilationUnit(sourceRoot,classDeclaration);
                    compilationUnits.Add(result);
                }
                return compilationUnits;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public CompilationUnitSyntax GenerateCompilationUnit(CompilationUnitSyntax root,ClassDeclarationSyntax classDeclaration)
        {
            var diagnostics = classDeclaration.GetDiagnostics().Where(n => n.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error);
            if (diagnostics.Count()>0)
            {
                return null;
            }
            var FileName = classDeclaration.Identifier.ValueText;
            if (!(classDeclaration.Parent is NamespaceDeclarationSyntax))
            {
                return null;
            }
            string namespaceOfSourceClass = (classDeclaration.Parent as NamespaceDeclarationSyntax).Name.ToString();
            NamespaceDeclarationSyntax NamespaceDecl = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceOfSourceClass+"Tests"));
            var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(method => method.Modifiers.Any(SyntaxKind.PublicKeyword));
            var unit = SyntaxFactory.CompilationUnit()
                .AddUsings(GenerateUsings(root))
                .AddMembers(NamespaceDecl
                .AddMembers(GenerateClass(FileName)
                .AddMembers(GenerateMethods(methods)
            )));

            return unit.NormalizeWhitespace();
        }

        private ClassDeclarationSyntax GenerateClass(string FileName)
        {
            ClassDeclarationSyntax ClassDecl = SyntaxFactory.ClassDeclaration($"{FileName}Test").
                AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword)).
                AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.AttributeList().Attributes.Add(TestClassAttr)));
            return ClassDecl;
        }

        private UsingDirectiveSyntax[] GenerateUsings(CompilationUnitSyntax root)
        {
            return root.DescendantNodes().OfType<UsingDirectiveSyntax>()
                .Append(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.VisualStudio.TestTools.UnitTesting")))
                .ToArray();
        }

        private MethodDeclarationSyntax[] GenerateMethods(IEnumerable<MethodDeclarationSyntax> methods)
        {
            var ResultMethods = new List<MethodDeclarationSyntax>();
            ResultMethods.Add(GenerateMethod(TestSetupAttr, "Initialize"));
            foreach (MethodDeclarationSyntax item in methods)
                ResultMethods.Add(GenerateMethod(TestMethodAttr, $"Test{item.Identifier.ValueText}"));

            return ResultMethods.ToArray();
        }

        private MethodDeclarationSyntax GenerateMethod(AttributeSyntax attribute, string MethodName)
        {
            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), MethodName).
                AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword)).
                AddBodyStatements(GenerateAsserts(MethodName)).
                AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.AttributeList().Attributes.Add(attribute)));
        }

        private StatementSyntax[] GenerateAsserts(string MethodName)
        {
            var statements = new List<StatementSyntax>();
            if (MethodName != "Initialize")
                statements.Add(SyntaxFactory.ParseStatement("Assert.Fail(\"autogenerated\");"));
            return statements.ToArray();
        }

    }
}